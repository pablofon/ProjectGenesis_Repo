using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyA : MonoBehaviour
{
    [Header("EnemyStats")]
    [SerializeField] float life;
    [SerializeField] float tooFar;

    [SerializeField] bool test;
    AIPath pathFinder;

    [Header("FOV")]
    [SerializeField] float fovAngle = 90f;
    [SerializeField] Transform fovPoint;
    [SerializeField] float range;

    [SerializeField] GameObject bullet;
    Quaternion rot;
    bool detected;
    float timer;
    public Transform playerTransform;

    private void Awake()
    {
        pathFinder = GetComponent<AIPath>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        pathFinder.canMove = test;

        Vector2 dir = playerTransform.position - transform.position;
        float angle = Vector3.Angle(dir, fovPoint.localEulerAngles);
        RaycastHit2D hit = Physics2D.Raycast(fovPoint.position, dir, range);
        rot = new Quaternion();

        if (angle < fovAngle / 2 && !test)
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(fovPoint.position, dir, Color.green);
                detected = true;
                timer += Time.deltaTime;
                //test = false;
            }
            else
            {
                detected = false;
                timer = 1.5f;
                //test = true;
            }

            if (detected)
            {
                if (timer >= 1.5f)
                {
                    Instantiate(bullet, gameObject.transform.position, rot);
                    timer = 0f; //Para que no salgan balas infinitamente 
                }
            }


        }
        

        if (Vector2.Distance(transform.position, playerTransform.position) > tooFar)
        {
            test = true;
        }
        else
        {
            test = false;
        }
    }
}
