using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PhoneChatManager : MonoBehaviour
{
    [Header("Contacts")]
    public List<ChatContact> contacts;

    [Header("List UI")]
    public Transform contactListContent;
    public GameObject contactItemPrefab;

    [Header("Chat UI")]
    public GameObject chatPanel;
    public TMP_Text remarkTitle;
    public Transform chatContent;
    public GameObject leftBubblePrefab;
    public GameObject rightBubblePrefab;

    private ChatContact currentContact;

    private void Start()
    {
        RefreshContactList();
    }

    // ================= 联系人列表 =================

    public void RefreshContactList()
    {
        foreach (Transform t in contactListContent)
            Destroy(t.gameObject);

        foreach (var contact in contacts)
        {
            var item = Instantiate(contactItemPrefab, contactListContent);
            item.GetComponent<ContactItemUI>().Init(contact, this);
        }
    }

    // ================= 打开聊天 =================

    public void OpenChat(ChatContact contact)
    {
        currentContact = contact;
        contact.hasUnread = false;

        remarkTitle.text = contact.remark;
        chatPanel.SetActive(true);

        RefreshChat();
        RefreshContactList();
    }

    // ================= 聊天刷新 =================

    void RefreshChat()
    {
        foreach (Transform t in chatContent)
            Destroy(t.gameObject);

        foreach (var msg in currentContact.messages)
        {
            var prefab = msg.isSelf ? rightBubblePrefab : leftBubblePrefab;
            var bubble = Instantiate(prefab, chatContent);
            bubble.GetComponent<ChatBubbleUI>().Set(msg.content);
        }
    }

    // ================= 发送“收到” =================

    public void SendReply()
    {
        if (currentContact == null) return;

        currentContact.messages.Add(new ChatMessage
        {
            content = "收到",
            isSelf = true
        });

        RefreshChat();
    }
}