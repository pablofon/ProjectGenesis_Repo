using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private float health;

    public void ChangeHealth(float dmg)
    {
        health += dmg;
    }
}
