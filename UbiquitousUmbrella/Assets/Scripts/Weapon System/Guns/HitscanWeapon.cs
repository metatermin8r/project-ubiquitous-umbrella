using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Photon.Pun; Anything relating to Photon is for netcode, will need to be replaced later. Kept for reference.
using TMPro;
using UnityEngine.Animations.Rigging;

public class HitscanWeapon : Gun
{
    [Header("Weapon Camera")]
    Camera weaponCam;

    [Header("Player Movement and Action")]
    [SerializeField] PlayerAction playerAction;
    [SerializeField] PlayerMovement playerController;
    [SerializeField] AdvancedCamRecoil recoil;
    [SerializeField] FirstPersonWeaponMovement weaponSway;

    //PhotonView PV;

    private void Awake()
    {
        //PV = GetComponent<PhotonView>(); for Photon

        playerController = GetComponentInParent<PlayerMovement>();
        playerAction = GetComponentInParent<PlayerAction>();
        recoil = GetComponentInParent<AdvancedCamRecoil>();
        weaponSway = GetComponentInParent<FirstPersonWeaponMovement>();

        //fpsCam = playerController.transform.GetChild(0).GetChild(0).GetComponent<Camera>(); //GameObject.Find("WeaponCamera").GetComponent<Camera>();

        weaponCam = fpsCam; //fpsCam = weaponCam;

        if (weaponType == WeaponType.AssaultRifle)
        {
            //weaponHandIK = GameObject.Find("ARIKRig").GetComponent<Rig>();
            fpWeaponHandIK = GameObject.Find("HK416RigLayer").GetComponent<Rig>(); //((GunInfo)itemInfo).fpWeaponRig.GetComponent<Rig>(); 
        }
        //else if (weaponType == WeaponType.Pistol)
        //{
        //    weaponHandIK = GameObject.Find("PistolIKRig").GetComponent<Rig>();
        //    fpWeaponHandIK = GameObject.Find("PistolFPIKRig").GetComponent<Rig>();
        //}
        else if (weaponType == WeaponType.BattleRifle)
        {
        //    weaponHandIK = GameObject.Find("BRIKRig").GetComponent<Rig>();
            fpWeaponHandIK = GameObject.Find("AR15RigLayer").GetComponent<Rig>();
        }

        bulletsLeft = magazineSize;

        //This is better, but still needs sure-fire hard coding.
        weaponChildIndex = transform.GetSiblingIndex();

        if (ammobarImage == null)
        {
            switch (weaponChildIndex)
            {
                case 0:
                    ammobarImage = GameObject.Find("Player_Hud/HUD_Canvas/AmmoCounter (Primary)/border/Image").GetComponent<Image>();
                    break;

                case 1:
                    ammobarImage = GameObject.Find("Player_Hud/HUD_Canvas/AmmoCounter (Secondary)/border/Image").GetComponent<Image>();
                    break;
                //Other cases can be added here if we have more weapon slots
            }
        }
        //Old code just in case
        //if (ammobarImage == null && weaponChildIndex == 0)
        //    ammobarImage = GameObject.Find("Player_Hud/HUD_Canvas/AmmoCounter (Primary)/border/Image").GetComponent<Image>();
        //else if(ammobarImage == null && weaponChildIndex == 1)
        //    ammobarImage = GameObject.Find("Player_Hud/HUD_Canvas/AmmoCounter (Secondary)/border/Image").GetComponent<Image>();

        //weaponAnimator.SetBool("Reloading", false);
        readyToShoot = true;
        defaultSpread = spread;
        if (TryGetComponent<Animator>(out var animator))
            weaponAnimator = animator;

        //Changes the weapon's render layer to default if it is not the player's weapon, fixing some visual bugs
        //int LayerDefault = LayerMask.NameToLayer("Default");
        //if (!playerController.PV.IsMine)
        //{
        //    gameObject.layer = LayerDefault;
        //    SetLayerAllChildren(transform, LayerDefault);
        //}
    }

    public void OnEnable()
    {
        //PlaySound(2); Needs an audio manager
        //weaponChildIndex = transform.GetSiblingIndex();
    }

    public void Update()
    {
        UpdateAmmoHud();

        shooting = Input.GetKey(KeyCode.Mouse0);

        if (!shooting || playerController.isSprinting)
        {
            if (weaponAnimator != null)
                weaponAnimator.SetBool("Firing", false);
            //playerAnimator.SetBool("WeaponFiring", false);
        }

        if (!playerController.isSprinting)
        {
            if (weaponAnimator != null)
                weaponAnimator.SetBool("Sprinting", false);
        }

        if (!playerAction.isZoomed)
        {
            weaponAnimator.SetBool("Aiming", false);
        }
    }

    public override void Use()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (readyToShoot && !playerController.isSprinting && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();

            recoil.Fire();

            if (weaponAnimator != null)
                weaponAnimator.SetBool("Firing", true);
            //playerAnimator.SetBool("WeaponFiring", true);
        }
        //Debug.Log("Firing " + itemInfo.itemName);
    }

    public override void Reload()
    {
        if (bulletsLeft < magazineSize && !reloading && !playerController.isSprinting) //&& !playerController.isZoomed)
            WeaponReload();
    }
    public override void Zoom()
    {
        if (playerAction.isZoomed)
        {
            weaponAnimator.SetBool("Aiming", true);
        }
    }

    public override void Sprint()
    {
        if (weaponAnimator != null)
            weaponAnimator.SetBool("Sprinting", true);
    }
    public override void Pickup()
    {
        throw new System.NotImplementedException();
    }
    public override void Melee()
    {
        throw new System.NotImplementedException();
    }
    public override void Grenade()
    {
        throw new System.NotImplementedException();
    }

    void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        Ray ray = weaponCam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = weaponCam.transform.position;

        if (Physics.Raycast(weaponCam.transform.position, direction, out RaycastHit hit, range, ~excludeFromRaycast))
        {
            //TODO: Update bullet trail system with object pooling from old system.
            TrailRenderer trail = Instantiate(bulletTrail, particalEffect.transform.position, Quaternion.identity);

            //Determines if the collider being hit is of a certain layer, and allows/doesn't allow bouncing based on that
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, bounceDistance, true, false));
            }
            else
            {
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, bounceDistance, true, true));
            }

            //For variable curve damage system
            float distance = Vector3.Distance(particalEffect.transform.position, hit.point);

            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).GetDamage(distance)); //.damage);
            RPC_Shoot(hit.point, hit.normal); //PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
        else
        {
            TrailRenderer trail = Instantiate(bulletTrail, particalEffect.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, transform.position + direction * range, Vector3.zero, bounceDistance, false, true));
        }

        //PlaySound(0);

        muzzleFlash.Play();

        roundsFired++;
        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

        if (bulletsLeft <= 0)
            Reload();
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    //Loops through all transforms in children and changes render layer
    //void SetLayerAllChildren (Transform root, int layer)
    //{
    //    var children = root.GetComponentsInChildren<Transform>(includeInactive: true);
    //    foreach (var child in children)
    //    {
    //        child.gameObject.layer = layer;
    //    }
    //}

    //[PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
            if (bulletImpactPrefab != null)
            {
                GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition, Quaternion.LookRotation(hitNormal, Vector3.up));
                Destroy(bulletImpactObj, 7f);
                bulletImpactObj.transform.SetParent(colliders[0].transform);
            }
            //Debug.Log(hitPosition);
        }
    }

    private void WeaponReload()
    {
        if (maxAmmo > 0)
        {
            reloading = true;
            if (weaponAnimator != null)
            {
                weaponAnimator.SetBool("Firing", false);
                weaponAnimator.SetBool("Reloading", true);
            }
            Invoke("ReloadFinished", reloadTime);
            //PlaySound(1); for sound manager
        }
        else
        {
            return;
        }
    }

    private void ReloadFinished()
    {
        if (maxAmmo <= 0)
        {
            return;
        }
        else if (!isEquiped)
        {
            reloading = false;
        }
        else if (isEquiped)
        {
            maxAmmo = maxAmmo - roundsFired;
            roundsFired = 0;
            bulletsLeft = magazineSize;
            reloading = false;
            //playerController.isReloading = false;
            if (weaponAnimator != null)
                weaponAnimator.SetBool("Reloading", false);
        }
    }

    public void UpdateAmmoHud()
    {
        ammobarImage.fillAmount = (float)bulletsLeft / magazineSize;
        //hudAmmoCounter.SetText(bulletsLeft + "                            " + maxAmmo);
        //hudAmmoCounter.SetText(bulletsLeft / bulletsPerTap + "                            " + maxAmmo / bulletsPerTap);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint, Vector3 hitNormal, float bounceDistance, bool madeImpact, bool canBounce)
    {
        Debug.Log("SpawnTrail started!");
        Vector3 startPosition = trail.transform.position;
        Vector3 direction = (hitPoint - trail.transform.position).normalized;

        float distance = Vector3.Distance(trail.transform.position, hitPoint);
        float startingDistance = distance;

        while (distance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (distance / startingDistance));
            distance -= Time.deltaTime * speed; // trail.time;

            yield return null;
        }

        trail.transform.position = hitPoint;

        if (madeImpact && canBounce)
        {
            //TODO: Play a particle system, like an impact or something, here when ready. Use Object Pooling.
            //Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(hitNormal));

            //TODO: Realistically, this system should have bullets bouncing X% of the time, but only on certain surfaces.
            //This kinda works with layers now, but future Surface Manager implementation will do wonders.

            //VERY simple RNG code
            float rngValue = Random.Range(0, 100);

            if (hasBouncingBullets && bounceDistance > 0 && rngValue <= 25)
            {
                Vector3 bounceDirection = Vector3.Reflect(direction, hitNormal);

                if(Physics.Raycast(hitPoint, bounceDirection, out RaycastHit hit, bounceDistance, ~excludeFromRaycast))
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        yield return StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, 
                            bounceDistance - Vector3.Distance(hit.point, hitPoint), true, false));

                        hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).GetDamage(distance));
                    }
                    else
                    {
                        yield return StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, 
                            bounceDistance - Vector3.Distance(hit.point, hitPoint), true, true));
                    }
                    
                }
                else
                {
                    yield return StartCoroutine(SpawnTrail(trail, bounceDirection * bounceDistance, Vector3.zero, 0, true, true));
                }
            }
        }

        Destroy(trail.gameObject, trail.time);
    }

    //All photon sound code
    //[PunRPC]
    //public void PlaySound(int index)
    //{
    //    if (PV.IsMine == true)
    //    {
    //        PV.RPC("PlaySound", RpcTarget.Others, index);
    //    }
    //    weaponSoundEmitter.clip = gunSounds[index];
    //    GetComponent<AudioSource>().Play();
    //}
}
