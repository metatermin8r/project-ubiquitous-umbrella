using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Trail Config", menuName = "Weapon System/New Bullet Trail Configuration", order = 4)]
public class BulletTrailConfiguration : ScriptableObject
{
    public Material material;
    public AnimationCurve widthCurve;
    public float duration = 0.5f;
    public float minVertexDistance = 0.1f;
    public Gradient color;

    public float missDistance = 100f;
    public float simulationSpeed = 100f;
}

