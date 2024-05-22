using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using BF_SubclassList;

[System.Serializable]
public class Weapon
{
    public Sprite sprite;
    public Transform firePoint;
    public bool isUnlocked;

    [System.NonSerialized] public WeaponController weaponController;
    [System.NonSerialized] public PlayerController playerController;

    [Header("Arm Recoil")]
    [SerializeField] public float hcKbDuration;
    [Range(0, 1)]
    [SerializeField] public float hcKbDist;
    [Range(0, 90)]
    [SerializeField] public float hcKbRotation;

    [Header("Normal Shot")]
    [SerializeField] public GameObject projectile;
    [SerializeField] public float dmg;
    [Range(1, 60)]
    [Tooltip("Attacks per second")]
    [SerializeField] public int fireRate;
    [SerializeField] public int atkCd;
    [SerializeField] public float ammo;
    [SerializeField] public float reloadTime;
    [SerializeField] public float range;
    [SerializeField] public int pierce;
    [SerializeField] public float velocity;
    [SerializeField] public float bloom;
    [SerializeField] public float knockback;

    [Header("Special Shot")]
    [SerializeField] public GameObject sProjectile;
    [SerializeField] public float sDmg;
    [Range(1, 60)]
    [Tooltip("Attacks per second")]
    [SerializeField] public int sFireRate;
    [SerializeField] public int sAtkCd;
    [SerializeField] public float sAmmo;
    [SerializeField] public float sReloadTime;
    [SerializeField] public float sRange;
    [SerializeField] public int sPierce;
    [SerializeField] public float sVelocity;
    [SerializeField] public float sBloom;
    [SerializeField] public float sKnockback;

    [SerializeField] public LayerMask collisionLayers;

    [Header("BurstShot")]
    [SerializeField] public int burstCount;
    [SerializeField] public float burstRate;

    public virtual void Shoot(Vector2 aimDir) { }

    public void AtkTimer() { if (atkCd > 0) atkCd--; if (sAtkCd > 0) sAtkCd--; }
}

[System.Serializable]
public class Pistol : Weapon
{

    public override void Shoot(Vector2 aimDir)
    {
        if (weaponController.shootOnce && atkCd <= 0)
        {
            weaponController.shootOnce = false;

            playerController.PlayerHandRecoil(hcKbDuration, hcKbDist, hcKbRotation);
            atkCd = fireRate;
            GameObject newBullet = Object.Instantiate(projectile, firePoint.position, Quaternion.Euler(0, 0, Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg));
            Hitscan1 script = newBullet.GetComponent<Hitscan1>();

            if (script != null) script.StartCoroutine(script.Hit(aimDir, range, collisionLayers, pierce, dmg, knockback));


        }
    }
}

[System.Serializable]
public class Shotgun : Weapon
{
    [Header("MultiShot")]
    [SerializeField] public float shotDispersion;
    [SerializeField] public float multiShotCount;

    public override void Shoot(Vector2 aimDir)
    {
        if (weaponController.shootOnce)
        {
            weaponController.shootOnce = false;
        }
    }
}

[System.Serializable]
public class Weapon_Container
{
    [SerializeReference] public List<Weapon> weapons;
}

public class WeaponController : MonoBehaviour
{
    [SubclassList(typeof(Weapon)), SerializeField] private Weapon_Container weapons;
    [SerializeField] private int currentWeaponIndex = 0;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private PlayerController playerController;

    [Header("Normal Input")]
    [SerializeField] public bool shootOnce;
    [SerializeField] public bool shooting;
    [SerializeField] private int shotQueue;
    [SerializeField] private int shotQueueTimer;

    [Header("Special Input")]
    [SerializeField] public bool sShootOnce;
    [SerializeField] public bool sShooting;
    [SerializeField] private int sShotQueue;
    [SerializeField] private int sShotQueueTimer;

    private void Awake()
    {
        foreach (Weapon weapon in weapons.weapons)
        {
            weapon.weaponController = this;
            weapon.playerController = playerController;
        }

        spriteRenderer.sprite = weapons.weapons[currentWeaponIndex].sprite;
    }

    private void Update()
    {
        if (playerController.armLayer.active)
        {
            weapons.weapons[currentWeaponIndex].Shoot(playerController.aimDir);
        }
    }

    private void FixedUpdate()
    {
        foreach (Weapon weapon in weapons.weapons) { weapon.AtkTimer(); }

        if (shotQueueTimer > 0) shotQueueTimer--;
        else shootOnce = false;

        if (sShotQueueTimer > 0) sShotQueueTimer--;
        else sShootOnce = false;
    }

    public void Input(bool input)
    {
        if (input)
        {
            shooting = true; 
            shootOnce = true;
            shotQueueTimer = shotQueue;
        }
        else shooting = false;
    }

    public void SpecialInput(bool input)
    {
        if (input)
        {
            sShooting = true;
            sShootOnce = true;
            sShotQueueTimer = sShotQueue;
        }
        else sShooting = false;
    }

    public void ScrollThroughWeapons(int scroll)
    {
        int tempWeaponIndex = currentWeaponIndex;

        do
        {
            tempWeaponIndex = (tempWeaponIndex + scroll + weapons.weapons.Count) % weapons.weapons.Count;
        }
        while (!weapons.weapons[tempWeaponIndex].isUnlocked);

        currentWeaponIndex = tempWeaponIndex;
        spriteRenderer.sprite = weapons.weapons[currentWeaponIndex].sprite;
    }
}