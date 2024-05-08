using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyShooter : MonoBehaviour
{
    [Header("EnemyStats")]
    [SerializeField] float life;
    [SerializeField] float maxlife;


    [Header("Chase")]
    public Transform playerTransform;
    [SerializeField] float speed;
    [SerializeField] bool isChasing;
    [SerializeField] float chaseDistance;
    [SerializeField] float tooFar;
    Vector2 chaseAim;
    bool needsChase = false;

    [Header("FOV")]
    [SerializeField] float fovAngle = 90f;
    [SerializeField] Transform fovPoint;
    [SerializeField] float range;

    [SerializeField] GameObject bullet;
    Quaternion rot;
    bool detected;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (needsChase)
        {
            Attacking();
        }

        Vector2 dir = playerTransform.position - transform.position;
        float angle = Vector3.Angle(dir, fovPoint.localEulerAngles);
        RaycastHit2D hit = Physics2D.Raycast(fovPoint.position, dir, range);
        rot = new Quaternion();

        if (angle < fovAngle / 2)
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
                }
            }

            if (Vector2.Distance(transform.position, playerTransform.position) > chaseDistance && chaseDistance < tooFar)
            {
                needsChase = true;
            }
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
}
