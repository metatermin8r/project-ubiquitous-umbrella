using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CH_BR_Anim_left : MonoBehaviour
{
    public GameObject leftCH;

    public void Shoot()
    {
        LeanTween.moveLocal(leftCH, new Vector3(-20f, 0f, 0f), .1f);
    }

    public void Idle()
    {
        LeanTween.moveLocal(leftCH, new Vector3(-10f, 0f, 0f), .1f);
    }
}
