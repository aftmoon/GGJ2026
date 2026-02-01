using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Phone : MonoBehaviour, IPointerClickHandler
{
    [Header("Phone UI")]
    public GameObject phoneScreen;
    public GameObject phoneMask;   // 全屏遮罩
    public GameObject phonePanel;  // 手机界面
    public GameObject ContactListPanel; //消息列表
    public GameObject ChatPanel;

    public Image healthCodeBlock;
    public Color greenCode = Color.green;
    public Color yellowCode = Color.yellow;
    public Color redCode = Color.red;

    private PersonData currentNPCData;  // 当前 NPC 的数据


    // 点击手机
    public void OnPointerClick(PointerEventData eventData)
    {
        
        OpenPhone();
    }

    public void OpenPhone()
    {
        if (ChatPanel != null) ChatPanel.SetActive(false);
        phoneScreen.SetActive(true);
        phoneMask.SetActive(true);
        if(phonePanel != null) phonePanel.SetActive(true);
        if (ContactListPanel != null) ContactListPanel.SetActive(true);
    }

    // 点击遮罩时调用
    public void ClosePhone()
    {
        phoneScreen.SetActive(false);
        phoneMask.SetActive(false);
        if (phonePanel != null) phonePanel.SetActive(false);
        if (ContactListPanel != null) ContactListPanel.SetActive(false);
        if (ChatPanel != null) ChatPanel.SetActive(false);
    }

    // 设置当前 NPC 的数据，并更新健康码显示
    public void SetNpcData(PersonData npcData)
    {
        currentNPCData = npcData;
        UpdateHealthCodeColor();
    }

    // 更新健康码颜色
    private void UpdateHealthCodeColor()
    {
        if (healthCodeBlock == null || currentNPCData == null)
            return;

        // 根据 HealthCode 来设置颜色
        switch (currentNPCData.healthCode)
        {
            case HealthCode.Green:
                healthCodeBlock.color = greenCode;
                break;
            case HealthCode.Yellow:
                healthCodeBlock.color = yellowCode;
                break;
            case HealthCode.Red:
                healthCodeBlock.color = redCode;
                break;
            default:
                healthCodeBlock.color = Color.white; // 默认颜色
                break;
        }
    }


}
