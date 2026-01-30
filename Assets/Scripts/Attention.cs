using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Attention : MonoBehaviour,IPointerClickHandler
{
    public GameObject attention;
    public GameObject attentionPanel;
    public GameObject attentionMask;

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenAttention();
    }

    public void OpenAttention()
    {
        attention.SetActive(true);
        attentionPanel.SetActive(true);
        attentionMask.SetActive(true);
    }

    public void CloseAttention()
    {
        attention.SetActive(false);
        attentionPanel.SetActive(false);
        attentionMask.SetActive(false);
    }
}
