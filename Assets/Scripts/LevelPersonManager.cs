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
        p.healthCode = CalculateHealthCode(p);

        return p;
    }

    HealthCode CalculateHealthCode(PersonData p)
    {
        if (p.nucleic == NucleicResult.Positive || p.temperature >= 38f)
            return HealthCode.Red;

        if (p.temperature >= 37.3f)
            return HealthCode.Yellow;

        return HealthCode.Green;
    }

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
        isPersonMoving = false;

        thermometer.SetHumanArea(currentNPC.humanArea);
        cottonTest.SetHumanArea(currentNPC.humanArea);

        cottonTest.ResetForNextPerson(
            currentNPC.humanArea,
            currentNPC.GetComponent<Animator>()
        );
    }


    public void OnPassButton()
    {
        if (currentNPC == null) return;
        if (isPersonMoving) return;

        isPersonMoving = true;

        passCount++;

        //判定惩罚
        if (currentNPC.data.healthCode == HealthCode.Red)
        {
            wrongCount++;
            Debug.Log("放走红码人");
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
        if (currentNPC.data.healthCode == HealthCode.Green)
        {
            wrongCount++;
            Debug.Log("误伤绿码人");
        }

        currentNPC.MoveTo(
            ((RectTransform)failPoint).anchoredPosition,
            OnPersonLeave
        );
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
}