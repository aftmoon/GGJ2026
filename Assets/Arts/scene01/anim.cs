using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class anim : MonoBehaviour
{
    public GameObject targetGameObject;  // 这是你要销毁的目标对象

    // 销毁目标对象
    public void DestroyTarget()
    {
        if (targetGameObject != null)
        {
            Destroy(targetGameObject);
        }
        else
        {
            Debug.LogWarning("目标对象为空！");
        }
    }
}
