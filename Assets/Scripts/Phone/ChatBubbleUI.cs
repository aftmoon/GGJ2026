using UnityEngine;
using TMPro;

public class ChatBubbleUI : MonoBehaviour
{
    public TMP_Text messageText;

    public void Set(string content)
    {
        messageText.text = content;
    }
}