using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneController : MonoBehaviour
{
    public Transform mid;

    public void ShowCutsceneByLevel(int levelIndex)
    {
        for (int i = 1; i < mid.childCount; i++)
        {
            mid.GetChild(i).gameObject.SetActive(i == levelIndex);
        }
    }
}
