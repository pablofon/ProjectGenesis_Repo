using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float health;



    public void Damage(float dmg) { health -= dmg; }

    public void Heal(float healing) { health += healing; }
}
