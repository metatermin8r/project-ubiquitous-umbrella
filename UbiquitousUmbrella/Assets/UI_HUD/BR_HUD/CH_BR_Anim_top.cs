using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CH_BR_Anim_top : MonoBehaviour
{
    public GameObject topCH;

    public void Shoot()
    {
        LeanTween.moveLocal(topCH, new Vector3(0f, 20f, 0f), .1f);
    }

    public void Idle()
    {
        LeanTween.moveLocal(topCH, new Vector3(0f, 10f, 0f), .1f);
    }
}
