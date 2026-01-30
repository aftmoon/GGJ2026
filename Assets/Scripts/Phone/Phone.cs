using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Phone : MonoBehaviour, IPointerClickHandler
{
    [Header("Phone UI")]
    public GameObject phoneMask;   // 全屏遮罩
    public GameObject phonePanel;  // 手机界面
    public GameObject ContactListPanel; //消息列表

    // 点击手机
    public void OnPointerClick(PointerEventData eventData)
    {
        OpenPhone();
    }

    public void OpenPhone()
    {
        phoneMask.SetActive(true);
        phonePanel.SetActive(true);
        ContactListPanel.SetActive(true);
    }

    // 点击遮罩时调用
    public void ClosePhone()
    {
        phoneMask.SetActive(false);
        phonePanel.SetActive(false);
        ContactListPanel.SetActive(false);
    }
}
