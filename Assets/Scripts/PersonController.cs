using UnityEngine;
using System.Collections;

public class PersonController : MonoBehaviour
{
    public PersonData data;
    public float moveSpeed = 1200f; // UI 用像素速度

    RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void Init(PersonData personData)
    {
        data = personData;
        // TODO：更新人物头顶UI / 颜色 / 状态
    }

    public void MoveTo(Vector2 target, System.Action onArrived)
    {
        StartCoroutine(MoveCoroutine(target, onArrived));
    }

    IEnumerator MoveCoroutine(Vector2 target, System.Action onArrived)
    {
        while (Vector2.Distance(rt.anchoredPosition, target) > 5f)
        {
            rt.anchoredPosition = Vector2.MoveTowards(
                rt.anchoredPosition,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        onArrived?.Invoke();
        Destroy(gameObject);
    }
}