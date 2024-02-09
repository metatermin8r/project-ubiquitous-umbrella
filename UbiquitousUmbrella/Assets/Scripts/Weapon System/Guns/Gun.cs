using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [Header("Weapon Type")]
    public WeaponType weaponType;

    //TODO: All of these need tooltips and comments, jfc I barely understand it anymore
    [Header("Weapon Stats")]
    public float timeBetweenShooting;
    public float spread, range, speed, effectiveRange, reloadTime, timeBetweenShots; //aimSpread,
    public int bulletsPerTap, magazineSize, maxAmmo;
    public bool allowButtonHold;
    public bool multiBarrelWeapon; //Investigate reimplementing this with optional values that show only if true
    public bool hasBouncingBullets = false;
    public float bounceDistance = 10f;
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
    public LayerMask excludeFromRaycast;

    [Header("Graphics")]
    public GameObject bulletImpactPrefab; //Change this from BulletHoleGraphic in old code
    //GameObject bulletHole;
    public ParticleSystem muzzleFlash;
    //public TextMeshProUGUI hudAmmoCounter; Main ammo counter component
    //public TextMeshProUGUI gunAmmoCounter;
    //public bool hasAmmoBar = true; Here in case we want both in the future
    public Image ammobarImage;
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
