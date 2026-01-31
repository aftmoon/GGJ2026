using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Rendering;

public class ButtonHoldFear : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [Header("References")]
    public Image targetImage;          // alpha 变化 + 变黑
    public RectTransform buttonRect;   // Button 自身
    public Animator buttonAnimator;    // 完成时播放动画（可选）
    public GameObject startPanel;    // 完成时播放动画（可选）

    [Header("Hold Settings")]
    public float holdDuration = 3f;
    public string finishTrigger = "Finish";

    [Header("Fear Shake Settings")]
    public float minShake = 2f;
    public float maxShake = 10f;

    Coroutine holdCo;
    Coroutine shakeCo;

    Vector2 originPos;
    bool finished;
    float holdProgress; // 0~1

    void Awake()
    {
        originPos = buttonRect.anchoredPosition;
        SetAlpha(0f);
    }

    // ================= Pointer =================

    public void OnPointerDown(PointerEventData eventData)
    {
        if (finished) return;

        holdCo = StartCoroutine(HoldRoutine());
        shakeCo = StartCoroutine(FearShakeRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Cancel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cancel();
    }

    // ================= Core =================

    IEnumerator HoldRoutine()
    {
        float t = 0f;

        while (t < holdDuration)
        {
            t += Time.deltaTime;
            holdProgress = Mathf.Clamp01(t / holdDuration);
            SetAlpha(holdProgress);
            yield return null;
        }

        // 完成
        finished = true;
        SetAlpha(1f);
        StopShake();

        if (buttonAnimator != null)
            buttonAnimator.SetTrigger(finishTrigger);

        // 等动画（假设 1 秒）
        yield return new WaitForSeconds(3f);

        //targetImage.color = Color.black;
        Destroy(startPanel);
    }

    IEnumerator FearShakeRoutine()
    {
        while (true)
        {
            // 晃动强度随进度增强
            float strength = Mathf.Lerp(minShake, maxShake, holdProgress);

            // 完全随机、不规则 → 恐惧感
            Vector2 offset = 3*Random.insideUnitCircle * strength;
            buttonRect.anchoredPosition = originPos + offset;

            // 不规则间隔（关键！）
            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
        }
    }

    // ================= Helpers =================

    void Cancel()
    {
        if (finished) return;

        if (holdCo != null) StopCoroutine(holdCo);
        StopShake();
        holdProgress = 0f;
        SetAlpha(0f);
    }

    void StopShake()
    {
        if (shakeCo != null) StopCoroutine(shakeCo);
        buttonRect.anchoredPosition = originPos;
    }

    void SetAlpha(float a)
    {
        Color c = targetImage.color;
        c.a = a;
        targetImage.color = c;
    }
}
