using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Animations.Rigging;

public abstract class Gun : Item
{
    public abstract override void Use();
    public abstract override void Reload();
    public abstract override void Zoom();
    public abstract override void Sprint();
    public abstract override void Pickup();
    public abstract override void Melee();
    public abstract override void Grenade();
    public abstract override void Super();

    [Header("Weapon Type")]
    public WeaponType weaponType;

    [Header("Weapon Stats")]
    public float timeBetweenShooting;
    public float spread, range, effectiveRange, reloadTime, timeBetweenShots; //aimSpread,
    public int bulletsPerTap, magazineSize, maxAmmo;
    public bool allowButtonHold;
    public bool multiBarrelWeapon;
    //public bool divideAmmo = false;

    [Header("Interal Weapon int/float values")]
    public int bulletsLeft;
    public int bulletsShot;
    public int roundsFired;
    public float defaultSpread;

    [Header("Weapon State Bools")]
    public bool shooting;
    public bool readyToShoot;
    public bool reloading;
    public bool zoomed;

    [Header("References")]
    public Camera fpsCam;
    public Transform particalEffect;
    public RaycastHit rayHit;
    //public LayerMask whatIsEnemy;

    [Header("Graphics")]
    public GameObject bulletImpactPrefab; //Change this from BulletHoleGraphic in old code
    //GameObject bulletHole;
    public ParticleSystem muzzleFlash;
    public TextMeshProUGUI hudAmmoCounter;
    //public TextMeshProUGUI gunAmmoCounter;
    //public int killFeedHowImageIndex;
    public TrailRenderer bulletTrail;

    //[Header("Animation")]
    //public UnityEngine.Animations.Rigging.Rig weaponHandIk;
    public Animator weaponAnimator;
    //public Animator playerAnimator; I don't remember why this is here, disabling for now
    //public Rig weaponHandIK;
    //public Rig fpWeaponHandIK;
    //public bool isSprinting;

    //[Header("Audio")]
    //public AudioSource weaponSoundEmitter;
    //public AudioClip[] gunSounds;

    public enum WeaponType { AssaultRifle, Pistol, BattleRifle, Shotgun, SniperRifle }
}
