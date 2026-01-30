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
    public Sprite testKitAfterSprite;

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
        if (currentSwab != null) return;

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

        // 改变试剂盒图片
        if (testKitImage != null && testKitAfterSprite != null)
        {
            testKitImage.sprite = testKitAfterSprite;
        }

        // 销毁棉签
        Destroy(currentSwab.gameObject);
        currentSwab = null;

        Debug.Log("核算检测完成。");
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
}