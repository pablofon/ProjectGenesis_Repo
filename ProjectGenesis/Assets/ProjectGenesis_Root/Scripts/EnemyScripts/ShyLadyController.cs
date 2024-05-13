using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

public class ShyLadyController : MonoBehaviour
{
    [SerializeField] Transform target;

    NavMeshAgent agent;
    Animator anim;

    [SerializeField] float detectDistance;
    [SerializeField] float inRange;
    bool startAttack;
    bool isFacingRight;

    [SerializeField] float castTime;
    [SerializeField] float repeatTime;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        anim = GetComponentInChildren<Animator>();

        target = GameObject.FindGameObjectWithTag("Player").transform;
        anim.SetBool("Walk", false);

        isFacingRight = false;

    }

    private void Update()
    {

        if (Vector2.Distance(transform.position, target.position) < detectDistance)
        {
            startAttack = true;
        }

        if (startAttack)
        {
            anim.SetBool("Walk", true);

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
                Attack();
            }
        }

        FlipUpdater();

    }

    void Follow()
    {
        agent.SetDestination(target.position);
    }

    void StopFollow()
    {
        agent.SetDestination(transform.position);
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

    void Attack()
    {
        anim.SetTrigger("Attack");
    }
}
