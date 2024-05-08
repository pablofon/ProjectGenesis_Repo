using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShy : MonoBehaviour
{
    [Header("EnemyStats")]
    [SerializeField] float life;
    [SerializeField] float maxlife;


    [Header("Chase")]
    public Transform playerTransform;
    [SerializeField] float speed;
    [SerializeField] bool isChasing;
    [SerializeField] float chaseDistance;
    Vector2 chaseAim;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (life <= 0)
        {
            EnemyDeath();
        }

        if (life != maxlife)
        {
            Attacking();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Attack"))
        {
            life -= 1;
        }
    }

    private IEnumerator PlayerDetected()
    {
        yield return new WaitForSeconds(0.2f);
        isChasing = true;
        yield return new WaitForSeconds(1f);
        isChasing = false;
        yield return null;
    }

    private IEnumerator Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, chaseAim, speed * Time.deltaTime);
        yield return new WaitForSeconds(2f);
        //isChasing = false;
        yield return null;
    }

    void Attacking()
    {
        if (isChasing)
        {
            StopCoroutine(PlayerDetected());
            StartCoroutine(Chase());
        }
        else
        {
            chaseAim = playerTransform.position;
            StartCoroutine(PlayerDetected());
        }
    }

    public void EnemyDeath()
    {
        gameObject.SetActive(false);
    }
}
