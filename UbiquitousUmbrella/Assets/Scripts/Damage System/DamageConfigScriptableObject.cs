using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName = "Damage Config", menuName = "Weapon System/Guns/Damage Config", order = 1)]
public class DamageConfigScriptableObject : ScriptableObject
{
    public MinMaxCurve damageCurve;

    private void Reset()
    {
        damageCurve.mode = ParticleSystemCurveMode.Curve;
    }

    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(damageCurve.Evaluate(Distance, Random.value));
    }
}
