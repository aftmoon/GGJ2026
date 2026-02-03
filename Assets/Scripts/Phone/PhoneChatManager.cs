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
    public GameObject ContactListPanel;
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
        ContactListPanel.SetActive(false);

        RefreshChat();
        RefreshContactList();
    }

    // ================= 聊天刷新 =================

    void RefreshChat()
    {
        Debug.Log("RefreshChat called");

        foreach (Transform t in chatContent)
            Destroy(t.gameObject);

        Debug.Log("消息数量: " + currentContact.messages.Count);

        foreach (var msg in currentContact.messages)
        {
            Debug.Log("生成气泡: " + msg.content + " isSelf=" + msg.isSelf);

            var prefab = msg.isSelf ? rightBubblePrefab : leftBubblePrefab;
            var bubble = Instantiate(prefab, chatContent);
            bubble.GetComponent<ChatBubbleUI>().Set(msg.content);
        }
    }

    // ================= 发送“收到” =================

    public void SendReply()
    {
        if (currentContact == null) return;

        if (currentContact.isReplied)
        {
            Debug.Log("已回复。");
            return;
        }

        currentContact.messages.Add(new ChatMessage
        {
            content = "Got it",
            isSelf = true
        });
        currentContact.isReplied = true;
        Debug.Log("回复成功");

        RefreshChat();
    }

    public void Back()
    {
        chatPanel.SetActive(false);
        ContactListPanel.SetActive(true);
    }

    public int GetUnrepliedCount()
    {
        int count = 0;
        foreach (var c in contacts)
        {
            if (c.isReplied == false)
                count++;
        }
        return count;
    }
}