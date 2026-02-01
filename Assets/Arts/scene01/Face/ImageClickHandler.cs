using UnityEngine;
using UnityEngine.UI;  // 引用 UI 模块
using UnityEngine.EventSystems;  // 引用 EventSystem 事件

public class ImageClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetGameObject;  // 目标 GameObject
    public bool setActive = true;        // 设置目标 GameObject 为 true 或 false

    // 当点击这个 Image 时触发
    public void OnPointerClick(PointerEventData eventData)
    {
        // 设置 targetGameObject 的激活状态
        if (targetGameObject != null)
        {
            targetGameObject.SetActive(setActive);
        }
        else
        {
            Debug.LogWarning("目标 GameObject 为空!");
        }
    }
}