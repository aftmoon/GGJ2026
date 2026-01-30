using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactItemUI : MonoBehaviour
{
    public TMP_Text remarkText;
    public GameObject redDot;

    private ChatContact contact;
    private PhoneChatManager manager;

    public void Init(ChatContact data, PhoneChatManager mgr)
    {
        contact = data;
        manager = mgr;

        remarkText.text = data.remark;
        redDot.SetActive(data.hasUnread);
    }

    public void OnClick()
    {
        manager.OpenChat(contact);
    }
}
