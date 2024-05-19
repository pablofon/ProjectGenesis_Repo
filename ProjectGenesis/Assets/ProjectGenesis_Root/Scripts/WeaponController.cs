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

    [Header("Arm Recoil")]
    [SerializeField] public float hcKbDuration;
    [Range(0, 1)]
    [SerializeField] public float hcKbDist;
    [SerializeField] public float hcKbRotation;

    [Header("Weapon Stats")]
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

    [SerializeField] public LayerMask collisionLayers;

    [Header("BurstShot")]
    [SerializeField] public int burstCount;
    [SerializeField] public float burstRate;

    public virtual void Shoot(InputAction.CallbackContext context, Vector2 aimDir) { Debug.Log("Sad :("); }

    public void AtkTimer() { if (atkCd > 0) atkCd--; }
}

[System.Serializable]
public class Pistol : Weapon
{
    
    public override void Shoot(InputAction.CallbackContext context, Vector2 aimDir)
    {
        if (context.started)
        {
            Debug.Log("Pistoll :DDDDDDDDD");
            atkCd = fireRate;
            GameObject newBullet = Object.Instantiate(projectile, firePoint.position, Quaternion.Euler(0, 0, Vector2.Angle(aimDir, Vector2.right)));
            Hitscan1 script = newBullet.GetComponent<Hitscan1>();

            if (script != null) script.StartCoroutine(script.Hit(aimDir, range, collisionLayers,pierce, dmg, knockback));
        }
    }
}

[System.Serializable]
public class Shotgun : Weapon
{
    [Header("MultiShot")]
    [SerializeField] public float shotDispersion;
    [SerializeField] public float multiShotCount;

    public override void Shoot(InputAction.CallbackContext context, Vector2 aimDir)
    {
        if (context.started)
        {
            Debug.Log("Shotgun :DDDD");
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

    private void Awake()
    {
        spriteRenderer.sprite = weapons.weapons[currentWeaponIndex].sprite;
    }

    private void FixedUpdate()
    {
        foreach (Weapon weapon in weapons.weapons) { weapon.AtkTimer(); }
    }

    public void Atk(InputAction.CallbackContext context, Vector2 aimDir)
    {
        weapons.weapons[currentWeaponIndex].Shoot(context, aimDir);
    }

    public void SwitchToWeapon()
    {
        
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