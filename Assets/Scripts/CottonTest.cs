using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CottonTest : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Cotton UI Prefab")]
    public GameObject cottonSwabPrefab;

    private Canvas canvas;

    [Header("Human UI Image")]
    public RectTransform humanArea;
    public Animator humanAnimator;

    [Header("Test Kit UI Image")]
    public RectTransform testKitArea;
    public Image testKitImage;
    public Sprite testKitBeforeSprite;
    public Sprite testKitAfterSprite;

    [Header("试管液体")]
    public Image liquidImage;
    public Color negativeColor = Color.clear;   // 阴性：不变 / 透明
    public Color positiveColor = new Color(0.6f, 0f, 0.8f); // 紫色

    private PersonData currentPerson;

    private RectTransform currentSwab;
    private bool isDragging = false;

    private enum CottonState
    {
        None,
        Checking,   // 已在 Human 检测
        Finished    // 已完成
    }

    private CottonState state = CottonState.None;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentSwab != null || state == CottonState.Finished) return;

        GameObject swabObj = Instantiate(cottonSwabPrefab, canvas.transform);
        currentSwab = swabObj.GetComponent<RectTransform>();

        SetSwabPosition(eventData);

        isDragging = true;
        currentSwab.SetAsLastSibling();
        state = CottonState.None;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || currentSwab == null) return;

        SetSwabPosition(eventData);

        //拖到 Human 上，且还没检测过
        if (state == CottonState.None && IsOverlapping(currentSwab, humanArea))
        {
            StartChecking();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging || currentSwab == null) return;

        isDragging = false;

        //只有检测完成后，才能放入试剂盒
        if (state == CottonState.Checking && IsOverlapping(currentSwab, testKitArea))
        {
            FinishTest();
        }
        if(state != CottonState.Finished && !isDragging)
        {
            Destroy(currentSwab.gameObject);
        }
    }

    // ========================= 核心逻辑 =========================

    private void StartChecking()
    {
        state = CottonState.Checking;
        Debug.Log("正在检测");

        if (humanAnimator != null)
        {
            humanAnimator.SetTrigger("Check");
        }
    }

    private void FinishTest()
    {
        state = CottonState.Finished;

        if (testKitImage != null && testKitAfterSprite != null)
        {
            testKitImage.sprite = testKitAfterSprite;
        }

        Destroy(currentSwab.gameObject);
        currentSwab = null;

        //开始显示核酸结果
        StartCoroutine(ShowNucleicResult());

        Debug.Log("核酸检测完成，等待反应结果...");
    }

    private IEnumerator ShowNucleicResult()
    {
        float waitTime = Random.Range(5f, 10f);
        yield return new WaitForSeconds(waitTime);

        if (currentPerson == null || liquidImage == null)
            yield break;

        // 阴性：不变色
        if (currentPerson.nucleic == NucleicResult.Negative)
        {
            Debug.Log("核酸结果：阴性");
            yield break;
        }

        // 阳性：逐渐变紫
        Debug.Log("核酸结果：阳性，试管变色");

        float duration = 1.5f;
        float t = 0f;

        Color startColor = liquidImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            liquidImage.color = Color.Lerp(
                startColor,
                positiveColor,
                t / duration
            );
            yield return null;
        }

        liquidImage.color = positiveColor;
    }

    // ========================= 工具方法 =========================

    private void SetSwabPosition(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPos
        );

        currentSwab.localPosition = localPos;
    }

    private bool IsOverlapping(RectTransform a, RectTransform b)
    {
        Rect rectA = GetWorldRect(a);
        Rect rectB = GetWorldRect(b);
        return rectA.Overlaps(rectB);
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector2 size = corners[2] - corners[0];
        return new Rect(corners[0], size);
    }

    public void SetHumanArea(RectTransform area)
    {
        humanArea = area;
    }

    public void ResetForNextPerson(
    RectTransform newHumanArea,
    Animator newAnimator,
    PersonData personData
)
    {
        state = CottonState.None;
        isDragging = false;

        if (currentSwab != null)
        {
            Destroy(currentSwab.gameObject);
            currentSwab = null;
        }

        if (testKitImage != null && testKitBeforeSprite != null)
        {
            testKitImage.sprite = testKitBeforeSprite;
        }

        // 重置液体颜色
        if (liquidImage != null)
        {
            liquidImage.color = negativeColor;
        }

        humanArea = newHumanArea;
        humanAnimator = newAnimator;
        currentPerson = personData;
    }

    //public void ResetForNextPerson(RectTransform newHumanArea, Animator newAnimator)
    //{
    //    // 重置状态
    //    state = CottonState.None;
    //    isDragging = false;

    //    // 清理残留棉签
    //    if (currentSwab != null)
    //    {
    //        Destroy(currentSwab.gameObject);
    //        currentSwab = null;
    //    }

    //    if (testKitImage != null && testKitBeforeSprite != null)
    //    {
    //        testKitImage.sprite = testKitBeforeSprite;
    //    }

    //    // 绑定新的人
    //    humanArea = newHumanArea;
    //    humanAnimator = newAnimator;

    //    Debug.Log("棉签检测已重置，准备检测下一个人");
    //}
}