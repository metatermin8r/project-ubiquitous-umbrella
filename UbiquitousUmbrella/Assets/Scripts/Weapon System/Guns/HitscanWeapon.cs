using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    //PhotonView PV;

    private void Awake()
    {
        //PV = GetComponent<PhotonView>(); for Photon

        playerController = GetComponentInParent<PlayerMovement>();
        playerAction = GetComponentInParent<PlayerAction>();

        //fpsCam = playerController.transform.GetChild(0).GetChild(0).GetComponent<Camera>(); //GameObject.Find("WeaponCamera").GetComponent<Camera>();

        weaponCam = fpsCam; //fpsCam = weaponCam;

        //if (weaponType == WeaponType.AssaultRifle)
        //{
        //    weaponHandIK = GameObject.Find("ARIKRig").GetComponent<Rig>();
        //    fpWeaponHandIK = GameObject.Find("ARFPIKRig").GetComponent<Rig>();
        //}
        //else if (weaponType == WeaponType.Pistol)
        //{
        //    weaponHandIK = GameObject.Find("PistolIKRig").GetComponent<Rig>();
        //    fpWeaponHandIK = GameObject.Find("PistolFPIKRig").GetComponent<Rig>();
        //}
        //else if (weaponType == WeaponType.BattleRifle)
        //{
        //    weaponHandIK = GameObject.Find("BRIKRig").GetComponent<Rig>();
        //    fpWeaponHandIK = GameObject.Find("BRFPIKRig").GetComponent<Rig>();
        //}

        bulletsLeft = magazineSize;
        if (hudAmmoCounter == null)
            hudAmmoCounter = GameObject.Find("Player_Hud/HUD_Canvas/AmmoCounter/HudAmmoCounter").GetComponent<TextMeshProUGUI>();
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
        //PlaySound(2); Need an audio manager
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

        //if (!playerController.isZoomed)
        //{
        //    weaponAnimator.SetBool("Zoom", false);
        //}
    }

    public override void Use()
    {
        //shooting = Input.GetKey(KeyCode.Mouse0);

        if (readyToShoot && !playerController.isSprinting && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();

            if (weaponAnimator != null)
                weaponAnimator.SetBool("Firing", true);
            //playerAnimator.SetBool("WeaponFiring", true);
        }
        //Shoot();
        //Debug.Log("Firing gun " + itemInfo.itemName);
    }

    public override void Reload()
    {
        if (bulletsLeft < magazineSize && !reloading && !playerController.isSprinting) //&& !playerController.isZoomed)
            WeaponReload();
    }
    public override void Zoom()
    {
        //if (playerController.isZoomed)
        //{
        //    weaponAnimator.SetBool("Zoom", true);
        //}
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
    public override void Super()
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
        if(Physics.Raycast(weaponCam.transform.position, direction, out RaycastHit hit, range))
        {
            //Possible damage falloff implementation, we'll see.
            //if (hit.distance > effectiveRange)
            //{
            //    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage / 2);
            //    PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            //}
            //else
            //{
            //    hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            //    PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            //}

            //TODO: Update bullet trail system with object pooling from old system and better trail accuracy.
            //Good starting point: https://forum.unity.com/threads/need-advice-on-making-high-speed-bullet-trails-with-raycasting.1211583/
            TrailRenderer trail = Instantiate(bulletTrail, particalEffect.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));

            //For variable curve damage system
            float distance = Vector3.Distance(particalEffect.transform.position, hit.point);

            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).GetDamage(distance)); //.damage);
            RPC_Shoot(hit.point, hit.normal); //PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
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
        else
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
        hudAmmoCounter.SetText(bulletsLeft + "                            " + maxAmmo);
        //hudAmmoCounter.SetText(bulletsLeft / bulletsPerTap + "                            " + maxAmmo / bulletsPerTap);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = hit.point;

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
