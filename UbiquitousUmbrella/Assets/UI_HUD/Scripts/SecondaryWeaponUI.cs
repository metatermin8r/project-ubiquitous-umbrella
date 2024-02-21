using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondaryWeaponUI : MonoBehaviour
{
    public GameObject secondaryUI;

    public void SwapWeaponAnimation()
    {
        LeanTween.moveY(secondaryUI, 100, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleX(secondaryUI, 1f, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleY(secondaryUI, 1f, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.moveX(secondaryUI, 200, .5f).setEase(LeanTweenType.easeInQuad);
    }

    public void SwapBackWeaponAnimation()
    {
        LeanTween.moveY(secondaryUI, 50, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleX(secondaryUI, .6f, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleY(secondaryUI, .6f, .5f).setEase(LeanTweenType.easeInQuad);
    }
}
