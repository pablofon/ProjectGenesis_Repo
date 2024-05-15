using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChaser : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float speed;
    [SerializeField] Transform playerPosition;
    [SerializeField] float chaseDistance;
    [SerializeField] bool playerDetected;
    [SerializeField] float jumpForce;
    [SerializeField] float attackJumpForce;
    Rigidbody2D enemyRb;
    //float horInput;
    Animator anim;

    [Header("GroundCheck")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    bool isFacingRight;
    bool attacking;

    //bool attackRange;

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRb = GetComponent<Rigidbody2D>();

        //attackRange = false;
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("Walk", false);
    }

    // Update is called once per frame
    void Update()
    {
        

        //Detección de la capa ground para no hacer saltos infinitos
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Detect();
        Chase();

        //Attack();

        

        FlipUpdater();

        if (playerDetected)
        {
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

    }

    /*private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("JumpZone"))
        {
            Jump();
        }
    }*/



    void Detect()
    {
        if (Vector2.Distance(transform.position, playerPosition.position) < chaseDistance)
        {
            playerDetected = true;
        }
    }

    void Chase()
    {
        //horInput = Input.GetAxis("Horizontal");
        if (playerDetected)
        {

            if (playerPosition.position.x - transform.position.x > 0.02f)
            {
                enemyRb.velocity = new Vector2( speed, enemyRb.velocity.y);
            }
            if (playerPosition.position.x - transform.position.x < -0.02f)
            {
                enemyRb.velocity = new Vector2( -1f * speed, enemyRb.velocity.y);
            }
            if (playerPosition.position.y - transform.position.y > 1.5f)
            {
                if (isGrounded)
                {
                    enemyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    anim.SetBool("Jump", true);
                }
            }
            else
            {
                anim.SetBool("Jump", false);
            }
            if (Vector2.Distance(playerPosition.position, transform.position) < 2f)
            {
                attacking = true;
                anim.SetTrigger("Attack");
                //StartCoroutine(Attack());
            }
            if(!attacking)
            {
               // StopCoroutine(Attack());
            }

        }
    }

    /*void Jump()
    {
        if (isGrounded)
        {
            enemyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            Debug.Log("JumpZone");
        }
    }*/

    /*void Attack()
    {
        if (playerDetected && isGrounded)
        {
            if (playerPosition.position.x - transform.position.x < 0.2f)
            {
                attackRange = true;
            }
            else
            {
                attackRange = false;
            }
        }
    }*/

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
        if (playerPosition.position.x - transform.position.x > 0.02f)
        {
            if (!isFacingRight)
            {
                Flip();
            }
        }
        if (playerPosition.position.x - transform.position.x < -0.02f)
        {
            if (isFacingRight)
            {
                Flip();
            }
        }
    }

    /*IEnumerator Attack()
    {
        Vector2 dir = playerPosition.position - transform.position;
        yield return new WaitForSeconds(1f);
        //playerDetected = false;
        yield return new WaitForSeconds(1f);
        anim.SetTrigger("Attack");
        enemyRb.AddForce(dir * jumpForce, ForceMode2D.Impulse);
        attacking = false;
        yield return null;
    }*/
}
