using System.Collections;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public Transform mid;
    public string enterStateName = "Enter";
    public float fadeDuration = 0.5f;  // 渐变时间

    private Coroutine fadeCoroutine;

    public void ShowCutsceneByLevel(int levelIndex)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInAfterAnimation(levelIndex - 1));
    }

    private IEnumerator FadeInAfterAnimation(int targetIndex)
    {
        // 新 cutscene
        GameObject to = mid.GetChild(targetIndex).gameObject;
        CanvasGroup toGroup = to.GetComponent<CanvasGroup>();
        if (toGroup == null)
            toGroup = to.AddComponent<CanvasGroup>();

        // 激活并初始化透明度
        to.SetActive(true);
        toGroup.alpha = 0f;
        toGroup.interactable = false;
        toGroup.blocksRaycasts = false;

        // 播放 Animator 并等待动画结束
        Animator animator = to.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play(enterStateName, 0, 0f);

            // 获取动画长度
            float clipLength = 0f;
            if (animator.runtimeAnimatorController != null && animator.runtimeAnimatorController.animationClips.Length > 0)
            {
                foreach (var clip in animator.runtimeAnimatorController.animationClips)
                {
                    if (clip.name == enterStateName)
                    {
                        clipLength = clip.length;
                        break;
                    }
                }
            }

            // 等待动画播放完
            yield return new WaitForSeconds(clipLength);
        }

        // 动画播完后开始渐变 alpha 0 → 1
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = t / fadeDuration;
            toGroup.alpha = p;
            yield return null;
        }

        // 渐变结束，保持 alpha = 1
        toGroup.alpha = 1f;
        toGroup.interactable = true;
        toGroup.blocksRaycasts = true;

        fadeCoroutine = null;
    }
}