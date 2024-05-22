using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI.Table;

public class ShyLadyController : MonoBehaviour
{
    [SerializeField] Transform target;
    //[SerializeField] float life;

    NavMeshAgent agent;
    Animator anim;

    [SerializeField] float detectDistance;
    [SerializeField] float inRange;
    bool startAttack;
    bool isFacingRight;
    //[SerializeField] bool stoped;


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
                Attack();
            }

            if (Vector2.Distance(transform.position, target.position) < inRange)
            {
               // Attack();
            }
        }

        FlipUpdater();

        /*if (life <= 0)
        {
            EnemyDeath();
        }*/
    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerHit"))
        {
            life -= 1;
        }
    }*/

    void Follow()
    {
        agent.SetDestination(target.position);
        //stoped = false;
    }

    void StopFollow()
    {
        agent.SetDestination(transform.position);
        //stoped = true;
    }

    public void EnemyDeath()
    {
        anim.SetTrigger("Death");
        gameObject.SetActive(false);
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
