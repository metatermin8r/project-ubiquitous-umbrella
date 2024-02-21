using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimaryWeaponUI : MonoBehaviour
{
    public GameObject primaryUI;

    public void SwapWeaponAnimation()
    {
        LeanTween.moveY(primaryUI, 50, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleX(primaryUI, .6f, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleY(primaryUI, .6f, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.moveX(primaryUI, 200, .5f).setEase(LeanTweenType.easeInQuad);

    }

    public void SwapBackWeaponAnimation()
    {
        LeanTween.moveY(primaryUI, 100, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleX(primaryUI, 1, .5f).setEase(LeanTweenType.easeInQuad);
        LeanTween.scaleY(primaryUI, 1, .5f).setEase(LeanTweenType.easeInQuad);
    }
}
