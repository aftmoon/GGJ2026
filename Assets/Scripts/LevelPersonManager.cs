using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelPersonManager : MonoBehaviour
{
    [Header("结算统计")]
    public int passCount;
    public int failCount;
    public int wrongCount;

    [Header("结算界面")]
    public GameObject resultPanel;
    public TMP_Text passText;
    public TMP_Text failText;
    public TMP_Text wrongText;
    public TMP_Text missMessageText;

    public PhoneChatManager phoneChatManager;

    [Header("钟表")]
    public RectTransform clockHand;   // 指针
    public float startAngle = 0f;   // 9:00
    public float endAngle = -270f;    // 17:00

    [Header("生成设置")]
    public int personCount = 10;

    [Header("本关卡来的人")]
    public List<PersonData> persons = new List<PersonData>();

    [Header("场景引用")]
    public Transform spawnPoint;
    public Transform passPoint;
    public Transform failPoint;
    public GameObject personPrefab;

    [Header("温度计")]
    public TemTest thermometer;
    public CottonTest cottonTest;

    [Header("健康码")]
    public Phone NPCPhone;

    int currentIndex = 0;
    PersonController currentNPC;

    bool isPersonMoving = false;

    private void Start()
    {
        GeneratePersons();
        ShowNextPerson();
    }

    public void GeneratePersons()
    {
        persons.Clear();

        for (int i = 0; i < personCount; i++)
        {
            persons.Add(GenerateRandomPerson(i + 1));
        }

        Debug.Log($"生成了 {persons.Count} 个人");
    }

    PersonData GenerateRandomPerson(int id)
    {
        PersonData p = new PersonData();

        p.id = id;

        // 体温：36.0 ~ 39.5
        p.temperature = Random.Range(36.0f, 40f);

        // 是否戴口罩（80% 戴）
        p.hasMask = Random.value < 0.8f;

        // 核酸（90% 阴性）
        p.nucleic = Random.value < 0.8f
            ? NucleicResult.Negative
            : NucleicResult.Positive;

        // 健康码（根据核酸 & 体温决定）
        p.healthCode = Random.value < 0.7f
            ? HealthCode.Green   // 70% 概率为 Green
            : (Random.value < 0.285f  // 剩下的 30% 里，20% 可能是 Yellow，10% 可能是 Red
                ? HealthCode.Yellow  // 20% 概率为 Yellow
                : HealthCode.Red);   // 10% 概率为 Red

        return p;
    }

    //HealthCode CalculateHealthCode(PersonData p)
    //{
    //    if (p.nucleic == NucleicResult.Positive || p.temperature >= 38f)
    //        return HealthCode.Red;

    //    if (p.temperature >= 37.3f)
    //        return HealthCode.Yellow;

    //    return HealthCode.Green;
    //}

    void ShowNextPerson()
    {
        if (currentIndex >= persons.Count)
        {
            Debug.Log("本关结束");
            ShowResult();
            return;
        }

        UpdateClock();

        GameObject go = Instantiate(personPrefab, spawnPoint.parent);
        go.transform.SetAsLastSibling();

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = ((RectTransform)spawnPoint).anchoredPosition;

        currentNPC = go.GetComponent<PersonController>();
        currentNPC.Init(persons[currentIndex]);

        // 获取到 NpcPhone 并设置当前 NPC 数据
        if (NPCPhone != null)
        {
            NPCPhone.SetNpcData(currentNPC.data); // 设置 NPC 数据，自动更新健康码方块的颜色
        }

        isPersonMoving = false;

        thermometer.SetHumanArea(currentNPC.humanArea);
        cottonTest.SetHumanArea(currentNPC.humanArea);


        cottonTest.ResetForNextPerson(
            currentNPC.humanArea,
            currentNPC.GetComponent<Animator>(),
            currentNPC.data
        );
    }


    public void OnPassButton()
    {
        if (currentNPC == null) return;
        if (isPersonMoving) return;

        isPersonMoving = true;

        passCount++;

        //判定惩罚
        if (IsPersonAllowed(currentNPC.data) == false)
        {
            wrongCount++;
            Debug.Log("放走异常人");
        }

        currentNPC.MoveTo(
            ((RectTransform)passPoint).anchoredPosition,
            OnPersonLeave
        );
    }

    public void OnFailButton()
    {
        if (currentNPC == null) return;

        if (isPersonMoving) return;

        isPersonMoving = true;

        failCount++;

        //判定误伤
        if (IsPersonAllowed(currentNPC.data))
        {
            wrongCount++;
            Debug.Log("误伤健康人");
        }

        currentNPC.MoveTo(
            ((RectTransform)failPoint).anchoredPosition,
            OnPersonLeave
        );
    }

    bool IsPersonAllowed(PersonData p)
    {
        LevelConfig config = LevelManager.Instance.levels[
            LevelManager.Instance.currentLevelIndex
        ];
        if(!p.hasMask) return false;
        foreach (var item in config.enabledChecks)
        {
            switch (item)
            {
                case CheckItem.Mask:
                    if (!p.hasMask) return false;
                    break;

                case CheckItem.Temperature:
                    if (p.temperature >= 38f) return false;
                    break;

                case CheckItem.Nucleic:
                    if (p.nucleic == NucleicResult.Positive) return false;
                    break;

                case CheckItem.HealthCode:
                    if (p.healthCode == HealthCode.Red || p.healthCode == HealthCode.Yellow) return false;
                    break;
            }
        }

        return true;
    }

    void OnPersonLeave()
    {
        currentNPC = null;
        isPersonMoving = false;
        currentIndex++;
        ShowNextPerson();
    }

    void ShowResult()
    {
        int missMessageCount = phoneChatManager.GetUnrepliedCount();

        resultPanel.SetActive(true);

        passText.text = $"allowed:{passCount}";
        failText.text = $"interceptions:{failCount}";
        wrongText.text = $"incorrect judgments:{wrongCount}";
        missMessageText.text = $"Missed message:{missMessageCount}";
    }

    void UpdateClock()
    {
        if (clockHand == null) return;

        float progress = (float)currentIndex / personCount;
        float angle = Mathf.Lerp(startAngle, endAngle, progress);

        clockHand.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void ResetForNewLevel()
    {
        resultPanel.SetActive(false);

        // ===== 1. 清理流程状态 =====
        currentIndex = 0;
        currentNPC = null;
        isPersonMoving = false;

        // ===== 2. 清理统计 =====
        passCount = 0;
        failCount = 0;
        wrongCount = 0;

        // ===== 3. 清理 UI =====
        if (resultPanel != null)
            resultPanel.SetActive(false);

        // 重置时钟
        if (clockHand != null)
            clockHand.localRotation = Quaternion.Euler(0, 0, startAngle);


        // ===== 4. 清理场景中残留 NPC =====
        foreach (Transform child in spawnPoint.parent)
        {
            if (child.GetComponent<PersonController>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // ===== 5. 重新生成本关数据 =====
        persons.Clear();
        GeneratePersons();

        // ===== 6. 开始本关 =====
        ShowNextPerson();

        Debug.Log("关卡已重置，开始新一关");
    }
}