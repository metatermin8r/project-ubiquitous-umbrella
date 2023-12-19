using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Weapon Config", menuName = "Weapon System/New Weapon Configuration", order = 2)]
public class WeaponConfiguration : ScriptableObject
{
    public LayerMask hitMask;
    public float fireRate = 0.25f;
    public float recoilRecoverySpeed = 1f;
    public float maxSpreadTime = 1f;
    public BulletSpreadType spreadType = BulletSpreadType.Simple;
    [Header("Simple Spread options")]
    public Vector3 weaponSpread = new Vector3(0.1f, 0.1f, 0.1f);
    [Header("Simple Spread options")]
    [Range(0.001f, 5f)]
    public float spreadMultiplier = 0.1f;
    public Texture2D spreadTexture;

    public Vector3 GetSpread(float shootTime = 0)
    {
        Vector3 spread = Vector3.zero;

        if (spreadType == BulletSpreadType.Simple)
        {
            spread = Vector3.Lerp(Vector3.zero, new Vector3(
                Random.Range(-weaponSpread.x, weaponSpread.x),
                Random.Range(-weaponSpread.y, weaponSpread.y),
                Random.Range(-weaponSpread.z, weaponSpread.z)),
                Mathf.Clamp01(shootTime / maxSpreadTime)
                );
        }
        else if (spreadType == BulletSpreadType.TextureBased)
        {
            spread = GetTextureDirection(shootTime);
            spread *= spreadMultiplier;
        }

        return spread;
    }

    //Arcane code for the texture based recoil system. Creates dynamically increasing spread patter based on greyscale image.
    //I don't understand a lot of this, and likely don't need to should we choose to use this. Can be cut if unused.
    private Vector3 GetTextureDirection(float shootTime)
    {
        Vector2 halfSize = new Vector2(spreadTexture.width / 2f, spreadTexture.height / 2f);
        int halfSquareExtents = Mathf.CeilToInt(Mathf.Lerp(0.01f, halfSize.x, Mathf.Clamp01(shootTime / maxSpreadTime)));

        int minX = Mathf.FloorToInt(halfSize.x) - halfSquareExtents;
        int minY = Mathf.FloorToInt(halfSize.y) - halfSquareExtents;

        Color[] sampleColors = spreadTexture.GetPixels(minX, minY, halfSquareExtents * 2, halfSquareExtents * 2);

        float[] colorsAsGrey = System.Array.ConvertAll(sampleColors, (color) => color.grayscale);
        float totalGreyValue = colorsAsGrey.Sum();

        float grey = Random.Range(0, totalGreyValue);
        int i = 0;
        for (; i < colorsAsGrey.Length; i++)
        {
            grey -= colorsAsGrey[i];
            if (grey <= 0)
            {
                break;
            }
        }

        int x = minX + i % (halfSquareExtents * 2);
        int y = minY + i / (halfSquareExtents * 2);

        Vector2 targetPosition = new Vector2(x, y);
        Vector2 direction = (targetPosition - halfSize) / halfSize.x;

        return direction;
    }
}
