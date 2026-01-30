using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContactItemUI : MonoBehaviour, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("´ò¿ªÁÄÌì£º" + contact.remark);
        manager.OpenChat(contact);
    }
}