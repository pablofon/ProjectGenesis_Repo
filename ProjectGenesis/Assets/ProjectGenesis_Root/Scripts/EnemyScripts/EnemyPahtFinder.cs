using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;

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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        target = GameObject.FindGameObjectWithTag("Player").transform;
        startAttack = false;


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
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.DrawRay(fovPoint.position, dir, Color.green);
                        detected = true;
                        timer += Time.deltaTime;
                        rot = new Quaternion(0, 0, angle, angle);
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
                }
            }
        }
    }

    void Follow()
    {
        agent.SetDestination(target.position);
    }

    void StopFollow()
    {
        agent.SetDestination(transform.position);
    }
}
