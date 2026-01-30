using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public class TemTest : MonoBehaviour,
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Canvas canvas;

    [Header("Human Area")]
    public RectTransform humanArea;

    [Header("Temperature")]
    public float humanTemperature = 36.8f;
    public float feverThreshold = 38f;

    [Header("Thermometer UI")]
    public TMP_Text temperatureText;
    public Image screenImage;            // 显示屏 Image
    public Color normalColor = Color.green;
    public Color feverColor = Color.red;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool isDragging = false;
    private bool isChecking = false;

    private int originalSiblingIndex; //记录原始层级

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = rectTransform.GetSiblingIndex();
        ResetDisplay();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        rectTransform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // 拖到人身上时检测一次
        if (!isChecking && IsOverlapping(rectTransform, humanArea))
        {
            CheckTemperature();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        isChecking = false;

        // 松手回到原位
        rectTransform.anchoredPosition = originalPosition;
        // 恢复原来的 UI 层级
        rectTransform.SetSiblingIndex(originalSiblingIndex);
    }

    // ================= 核心逻辑 =================

    private void CheckTemperature()
    {
        isChecking = true;

        float temp = humanTemperature;
        temperatureText.text = temp.ToString("F1") + "°C";

        if (temp >= feverThreshold)
        {
            screenImage.color = feverColor;
        }
        else
        {
            screenImage.color = normalColor;
        }
    }

    private void ResetDisplay()
    {
        temperatureText.text = "--.-°C";
        screenImage.color = normalColor;
    }

    // ================= 工具方法 =================

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
