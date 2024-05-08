using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float speed;
    [SerializeField] Transform playerPosition;
    [SerializeField] float chaseDistance;
    [SerializeField] bool playerDetected;
    [SerializeField] float jumpForce;
    [SerializeField] float life;
    Rigidbody2D enemyRb;
    //float horInput;

    [Header("GroundCheck")]
    [SerializeField] bool isGrounded;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (life <= 0)
        {
            EnemyDeath();
        }

        //Detección de la capa ground para no hacer saltos infinitos
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        Detect();
        Chase();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            life -= 1;
        }
    }

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
            if (playerPosition.position.x - transform.position.x < 0.02f)
            {
                enemyRb.velocity = new Vector2( -1f * speed, enemyRb.velocity.y);
            }
            if (playerPosition.position.y - transform.position.y > 1.5f)
            {
                if (isGrounded)
                {
                    enemyRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void EnemyDeath()
    {
        gameObject.SetActive(false);
    }
}
