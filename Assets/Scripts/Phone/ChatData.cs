using System.Collections.Generic;

[System.Serializable]
public class ChatMessage
{
    public string content;
    public bool isSelf; // true = 自己（右边），false = 对方（左边）
}

[System.Serializable]
public class ChatContact
{
    public string remark;                 // 备注名
    public bool hasUnread;                // 是否未回复（红点）
    public List<ChatMessage> messages;    // 聊天记录
}