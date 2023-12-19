using UnityEngine;
using UnityEngine.Pool;
using System.Collections;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon System/Guns/New Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    //public ImpactType impactType; For impact effect system, not implemented yet
    public WeaponType type;
    public string name;
    public GameObject modelPrefab;
    public Vector3 spawnPoint;
    public Vector3 spawnRotation;

    public WeaponConfiguration weaponConfig;
    public BulletTrailConfiguration trailConfig;

    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private float lastShootTime;
    private float initalClickTime;
    private float stopShootingTime;
    private bool lastFrameWantedToShoot;

    public Camera weaponCamera;
    private ParticleSystem weaponParticleSystem;
    private ObjectPool<TrailRenderer> trailPool;

    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.activeMonoBehaviour = ActiveMonoBehaviour;
        lastShootTime = 0; //Fixes annoying bugs in editor play mode
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);

        model = Instantiate(modelPrefab);
        model.transform.SetParent(Parent, false);
        model.transform.localPosition = spawnPoint;
        model.transform.localRotation = Quaternion.Euler(spawnRotation);

        weaponParticleSystem = model.GetComponentInChildren<ParticleSystem>();

    }

    public void Shoot()
    {
        if (Time.time - lastShootTime - weaponConfig.fireRate > Time.deltaTime)
        {
            float lastDuration = Mathf.Clamp((stopShootingTime - initalClickTime), 0, weaponConfig.maxSpreadTime);

            float lerpTime = (weaponConfig.recoilRecoverySpeed - (Time.time - stopShootingTime)) 
                / weaponConfig.recoilRecoverySpeed;

            initalClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        if (Time.time > weaponConfig.fireRate + lastShootTime)
        {
            lastShootTime = Time.time;

            weaponParticleSystem.Play();

            Vector3 spreadAmount = weaponConfig.GetSpread(Time.time - initalClickTime);

            model.transform.forward += model.transform.TransformDirection(spreadAmount);

            Vector3 shootDirection = weaponCamera.transform.forward + spreadAmount;

            if (Physics.Raycast(weaponParticleSystem.transform.position, shootDirection,
                out RaycastHit hit, float.MaxValue, weaponConfig.hitMask))
            {
                activeMonoBehaviour.StartCoroutine(PlayTrail(weaponParticleSystem.transform.position, hit.point, hit));
            }
            else
            {
                activeMonoBehaviour.StartCoroutine(PlayTrail(weaponParticleSystem.transform.position, 
                    weaponParticleSystem.transform.position + (shootDirection * trailConfig.missDistance), 
                    new RaycastHit()));
            }
        }
    }

    public void Tick(bool wantsToShoot)
    {
        model.transform.localRotation = Quaternion.Lerp(model.transform.localRotation, Quaternion.Euler(spawnRotation),
            Time.deltaTime * weaponConfig.recoilRecoverySpeed);

        if (wantsToShoot)
        {
            lastFrameWantedToShoot = true;
            Shoot();
        }
        else if (!wantsToShoot && lastFrameWantedToShoot)
        {
            stopShootingTime = Time.time;
            lastFrameWantedToShoot = false;
        }
    }

    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;
        yield return null; //Avoids residual trail rendering carrying over and looking weird

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;
        while(remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= trailConfig.simulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = endPoint;

        //For impact manager system, not implemented (yet)
        //if (hit.collider != null)
        //{
            //SurfaceManager.Instance.HandleImpact(hit.transform.gameObject, endPoint, hit.normal, impactType, 0);
        //}

        yield return new WaitForSeconds(trailConfig.duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = trailConfig.color;
        trail.material = trailConfig.material;
        trail.widthCurve = trailConfig.widthCurve;
        trail.time = trailConfig.duration;
        trail.minVertexDistance = trailConfig.minVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}
