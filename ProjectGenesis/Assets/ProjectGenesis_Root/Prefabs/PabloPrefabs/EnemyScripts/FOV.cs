using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FOV : MonoBehaviour
{
    [Header("FOV")]
    [SerializeField] float fovAngle = 90f;
    [SerializeField] Transform fovPoint;
    [SerializeField] float range;

    [SerializeField] Transform target;
    [SerializeField] GameObject bullet;
    Quaternion rot;
    bool detected;
    float timer;

    [Header("patrulla")]
    private int i; //indice del array de puntos para que el enemigo detecte un punto a seguir.
    [Header("Point & Movement Configuration")]
    [SerializeField] Transform[] points; //Array de puntos de posición hacia los que el enemigo se moverá.
    [SerializeField] int startingPoint; //Número mpara determinar el punto de inicio del enemigo.
    [SerializeField] float speed; //Velocidad de la plataforma

    [Header("Stats")]
    [SerializeField] float life = 2;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        

        //Vector3 currentAngle = transform.localEulerAngles;
        //currentAngle = rot * currentAngle;
        //transform.localScale = currentAngle;
    }

    private void Update()
    {
        if (life <= 0)
        {
            EnemyDeath();
        }

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
        if (life > 0)
        {
            if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
            {
                transform.localScale = new Vector3(1, 1, 1);
                i++; //Aumenta el indice, cambia de objetivo hacia el que moverse.
                if (i == points.Length) //Chequea si el enemigo ha llegado al ultimo punto del array.
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    i = 0;//Resetea el índice para volver a empezar, el enemigo va hacia el punto 0.
                }
            }


            //Mueve el enemigo a la posición del punto guardado en el array...
            //... que corresponda al espacio del array con valor igual a la variable "i"
            transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            life -= 1;
        }
    }

    public void EnemyDeath()
    {
        gameObject.SetActive(false);
    }
}
    
