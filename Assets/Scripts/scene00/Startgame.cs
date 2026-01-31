using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonHoldEffect : MonoBehaviour
{
    public Button button;              // 需要监测的按钮
    public Image targetImage;          // 需要改变 alpha 的图片
    public AnimationClip shakeAnimClip; // 按钮的颤抖动画
    public float holdTime = 3f;        // 长按时长，3秒
    private float currentHoldTime = 0f;
    private bool isHolding = false;
    private bool hasPlayedAnimation = false;

    private Animator buttonAnimator;    // 按钮的 Animator，用于播放颤抖动画

    private void Start()
    {
        buttonAnimator = button.GetComponent<Animator>();
        if (buttonAnimator == null)
        {
            Debug.LogError("Button does not have an Animator component!");
        }

        // 初始时将目标图片 alpha 设置为 0
        if (targetImage != null)
        {
            Color tempColor = targetImage.color;
            tempColor.a = 0f;
            targetImage.color = tempColor;
        }
        
        // 按钮的点击事件
        button.onClick.AddListener(OnButtonClick);
    }

    private void Update()
    {
        // 监测鼠标是否按住按钮
        if (isHolding)
        {
            currentHoldTime += Time.deltaTime;

            // 目标图片的 alpha 从 0 到 1 渐变
            if (targetImage != null)
            {
                float alpha = Mathf.Clamp01(currentHoldTime / holdTime);
                Color tempColor = targetImage.color;
                tempColor.a = alpha;
                targetImage.color = tempColor;
            }

            // 如果已达到 3 秒，播放按钮动画并且将图片变黑
            if (currentHoldTime >= holdTime && !hasPlayedAnimation)
            {
                PlayButtonAnimation();
                SetImageBlack();
                hasPlayedAnimation = true; // 防止多次播放动画
            }
        }

        // 处理鼠标是否按下（开始长按）
        if (Input.GetMouseButtonDown(0) && button.GetComponent<RectTransform>().rect.Contains(Input.mousePosition))
        {
            isHolding = true;
            currentHoldTime = 0f; // 重置计时
            StartButtonShake();
        }

        // 鼠标松开时停止
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            StopButtonShake();
            ResetImageAlpha();
        }
    }

    private void StartButtonShake()
    {
        // 使用 DoTween 实现按钮的颤抖动画
        if (button != null)
        {
            button.transform.DOShakePosition(0.3f, 10f, 20, 90, false, true);
        }
    }

    private void StopButtonShake()
    {
        // 停止按钮的颤抖动画
        if (button != null)
        {
            button.transform.DOKill(); // 停止当前所有的 DoTween 动画
        }
    }

    private void PlayButtonAnimation()
    {
        // 播放按钮动画
        if (buttonAnimator != null)
        {
            buttonAnimator.Play(shakeAnimClip.name);
        }
    }

    private void SetImageBlack()
    {
        // 使用 DoTween 修改图片颜色变黑
        if (targetImage != null)
        {
            targetImage.DOColor(Color.black, 1f); // 在 1 秒内将图片变为黑色
        }
    }

    private void ResetImageAlpha()
    {
        // 使用 DoTween 恢复图片透明度
        if (targetImage != null)
        {
            targetImage.DOFade(0f, 0.3f); // 在 0.3 秒内将图片的透明度恢复为 0
        }
    }

    private void OnButtonClick()
    {
        // 如果点击事件中有其他处理，可以在这里处理
        Debug.Log("Button clicked");
    }
}
