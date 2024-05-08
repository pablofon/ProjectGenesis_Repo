using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    bool detected;
    Quaternion rot;
    float timer;

    RaycastHit2D rchit;

    [Header("raycast")]
    [SerializeField] float distance;
    [SerializeField] GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        detected = false;
        rot = new Quaternion(0, 0, 90, 90);
    }

    // Update is called once per frame
    void Update()
    {
        rchit = Physics2D.Raycast(gameObject.transform.position, Vector2.left, distance); //Raycast
        
        //Si el raycast detecta algo saca un rayo verde, si no uno rojo
        if (rchit.collider != null)
        {
            Debug.DrawRay(gameObject.transform.position, Vector2.left * distance, Color.green);
            detected = true;
            timer += Time.deltaTime;
        }
        if (rchit.collider == null)
        {
            Debug.DrawRay(gameObject.transform.position, Vector2.left * distance, Color.red);
            detected = false;
            timer = 1.5f;
        }
        //disparo de la bala
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
