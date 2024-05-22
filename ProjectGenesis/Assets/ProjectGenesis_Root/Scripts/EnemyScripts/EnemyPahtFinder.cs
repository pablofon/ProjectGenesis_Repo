using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPahtFinder : MonoBehaviour
{
    [SerializeField] Transform target;

    NavMeshAgent agent;

    bool startAttack;
    [SerializeField] float inRange;
    [SerializeField] float detectDistance;

    [Header("FOV")]
    [SerializeField] float fovAngle = 90f;
    [SerializeField] Transform fovPoint;
    [SerializeField] float range;
    [SerializeField] GameObject bullet;
    Quaternion rot;
    bool detected;
    float timer;
    bool isFacingRight;

    Animator anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        startAttack = false;

        isFacingRight = false;

        anim = GetComponentInChildren<Animator>();
        anim.SetBool("Walk", false);
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, target.position) < detectDistance)
        {
            startAttack = true;
        }

        if (startAttack)
        {
            

            if (Vector2.Distance(transform.position, target.position) > inRange)
            {
                Follow();
            }
            else
            {
                StopFollow();
            }
            
            if (Vector2.Distance(transform.position, target.position) < inRange)
            {
                Vector2 dir = target.position - transform.position;
                float angle = Vector3.Angle(dir, fovPoint.localEulerAngles);
                RaycastHit2D hit = Physics2D.Raycast(fovPoint.position, dir, range);
                rot = new Quaternion();

                if (angle < fovAngle / 2)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.CompareTag("Player"))
                        {
                            Debug.DrawRay(fovPoint.position, dir, Color.green);
                            detected = true;
                            timer += Time.deltaTime;
                        }
                        else
                        {
                            detected = false;
                            timer = 1.5f;
                        }

                        if (detected)
                        {
                            if (timer >= 1.5f)
                            {
                                Instantiate(bullet, gameObject.transform.position, rot);
                                timer = 0f; //Para que no salgan balas infinitamente y parezca un rayo
                                anim.SetTrigger("Attack");
                            }
                        }
                    }
                }
            }
        }

        FlipUpdater();
    }

    void Follow()
    {
        agent.SetDestination(target.position);
        anim.SetBool("Walk", true);
    }

    void StopFollow()
    {
        agent.SetDestination(transform.position);
        anim.SetBool("Walk", false);
    }

    public void EnemyDeath()
    {
        anim.SetTrigger("Death");
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
        isFacingRight = !isFacingRight;
    }

    void FlipUpdater()
    {
        if (target.position.x - transform.position.x > 0.02f)
        {
            if (!isFacingRight)
            {
                Flip();
            }
        }
        if (target.position.x - transform.position.x < -0.02f)
        {
            if (isFacingRight)
            {
                Flip();
            }
        }
    }
}
