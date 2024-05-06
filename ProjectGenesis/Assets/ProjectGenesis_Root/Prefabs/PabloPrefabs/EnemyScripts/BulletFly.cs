using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BulletFly : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 10f;
    private GameObject player;
    Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dir = player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * bulletSpeed * Time.deltaTime);
    }
}
