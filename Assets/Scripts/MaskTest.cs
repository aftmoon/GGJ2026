using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class MaskTest : MonoBehaviour, IPointerClickHandler
{
    private RectTransform rectTransform;

    public bool HasBeenTested {  get; private set; }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HasBeenTested = true;
        Debug.Log("ø⁄’÷ºÏ≤‚ÕÍ±œ°£");
    }
}
