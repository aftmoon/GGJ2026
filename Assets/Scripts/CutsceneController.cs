using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    public Transform mid;
    public string enterStateName = "Enter";
    public float fadeDuration = 0.5f;  // 渐变时间

    private Coroutine fadeCoroutine;
    [SerializeField]private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowCutsceneByLevel(int levelIndex)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        //if (gameObject.activeSelf && levelIndex > 0) fadeCoroutine = StartCoroutine(FadeInAfterAnimation(levelIndex - 1));
        if (levelIndex > 0)
        {
            StartCoroutine(FadeInAfterAnimation((levelIndex - 1) * 3 ));
            StartCoroutine(FadeInAfterAnimation((levelIndex - 1) * 3 + 1));
            StartCoroutine(FadeInAfterAnimation((levelIndex - 1) * 3 + 2));
        }

    }

    private IEnumerator FadeInAfterAnimation(int targetIndex)
    {
        if (targetIndex < 0 || targetIndex >= mid.childCount)
            yield break;

        GameObject to = mid.GetChild(targetIndex).gameObject;

        //CanvasGroup toGroup = to.GetComponent<CanvasGroup>();
        //if (toGroup == null)
        //    toGroup = to.AddComponent<CanvasGroup>();

        to.SetActive(true);
        Image img = to.GetComponent<Image>();
        if (img == null)
            yield break;

        // 确保初始是完全不透明
        Color c = img.color;
        c.a = 0f;
        img.color = c;
       
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeDuration);
            img.color = c;
            yield return null;
        }

        c.a = 1f;
        img.color = c;
    }

    public void AnimatorSetBool()
    {
        if(animator != null) 
            animator.SetBool("GameEnd", true);
    }

    public void OnGameEndAnimationFinished()
    {
        LevelManager.Instance.QuitGame();
    }
}