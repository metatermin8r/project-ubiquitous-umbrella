using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Animations.Rigging;

[CreateAssetMenu(menuName = "Multiplayer FPS/New Gun Info")]
public class GunInfo : ItemInfo
{
    //public float damage;

    public MinMaxCurve DamageCurve;
    //public GameObject fpWeaponRig;
    private void Reset()
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value));
    }
}
