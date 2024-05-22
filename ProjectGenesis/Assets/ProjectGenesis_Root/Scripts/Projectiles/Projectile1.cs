using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile1 : MonoBehaviour
{
    private Rigidbody2D rb;
    private TrailRenderer trail;

    public bool started;

    [Header("Stats")]
    public float velocity;
    public float dmg;
    public float knockback;

    [Header("Velocity Dependant Damage")]
    [SerializeField] public float lethalVelCutoff;
    [SerializeField] private float mult;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public LayerMask collisionLayers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
    }

    private void Update()
    {
        if (started) Trail();
    }

    private void Trail()
    {
        float bulletVelocity = rb.velocity.magnitude;

        if (bulletVelocity > lethalVelCutoff)
        {
            mult = bulletVelocity / velocity;
            trail.enabled = true;
        }
        else
        {
            mult = 0;
            trail.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (started)
        {
            if (collision.gameObject.layer == collisionLayers && mult != 0)
            {
                Health healthScript = collision.gameObject.GetComponent<Health>();
                healthScript.Damage(dmg);

                Vector3 vel = rb.velocity.normalized;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(vel * knockback, ForceMode2D.Impulse);
            }
        }
    }
}
