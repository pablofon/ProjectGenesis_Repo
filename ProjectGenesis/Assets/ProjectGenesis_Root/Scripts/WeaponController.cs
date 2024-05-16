using BF_SubclassList_Examples;
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
    [SerializeField] GameObject projectile;
    [SerializeField] public float dmg;
    [Range(1, 60)]
    [Tooltip("Attacks per second")]
    [SerializeField] public float fireRate;
    [SerializeField] public float ammo;
    [SerializeField] public float reloadTime;
    [SerializeField] public float velocity;
    [SerializeField] public float bloom;
    [SerializeField] public float knockback;

    [Header("MultiShot")]
    [SerializeField] public float shotDispersion;
    [SerializeField] public float multiShotCount;

    [Header("BurstShot")]
    [SerializeField] public int burstCount;
    [SerializeField] public float burstRate;


    // Make Shoot a virtual method instead of abstract
    public virtual void Shoot(InputAction.CallbackContext context) { Debug.Log("Sad :("); }
}

[System.Serializable]
public class Pistol : Weapon
{
    public override void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Pistoll :DDDDDDDDD");
        }
    }
}

[System.Serializable]
public class Shotgun : Weapon
{
    public override void Shoot(InputAction.CallbackContext context)
    {
        Debug.Log("Shotgun :DDDD");
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

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = weapons.weapons[currentWeaponIndex].sprite;
    }

    public void Atk(InputAction.CallbackContext context)
    {
        weapons.weapons[currentWeaponIndex].Shoot(context);
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