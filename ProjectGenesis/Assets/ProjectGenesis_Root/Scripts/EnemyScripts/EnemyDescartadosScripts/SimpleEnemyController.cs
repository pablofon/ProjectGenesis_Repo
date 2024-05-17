using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] float speed;
    [SerializeField] Transform playerPosition;
    [SerializeField] float chaseDistance;
    [SerializeField] bool playerDetected;
    [SerializeField] float enemyLife;
    Rigidbody2D enemyRb;
    //float horInput;

   
    // Start is called before the first frame update
    void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        enemyRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyLife <= 0)
        {
            EnemyDeath();
        }

        Detect();
        Chase();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            enemyLife -= 1;
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
                enemyRb.velocity = new Vector2(speed, enemyRb.velocity.y);
            }
            if (playerPosition.position.x - transform.position.x < 0.02f)
            {
                enemyRb.velocity = new Vector2(-1f * speed, enemyRb.velocity.y);
            }
            
        }
    }

    public void EnemyDeath()
    {
        gameObject.SetActive(false);
    }
}
