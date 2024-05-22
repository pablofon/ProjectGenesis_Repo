using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MyUnityEvent : UnityEvent { }

public class Health : MonoBehaviour
{
    [SerializeField] private float health;

    public MyUnityEvent damage;
    public MyUnityEvent death;

    public void Damage(float dmg)
    {
        if (health - dmg > 0)
        {
            health -= dmg;
            damage.Invoke();
        }
        else death.Invoke();
    }

    public void Heal(float healing) { health += healing; }
}
