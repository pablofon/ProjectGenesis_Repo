using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movin : MonoBehaviour
{
    [SerializeField] float speed;//Velocidad de la plataforma.
    [SerializeField] int startingPoint;//Número para determinar el índice del punto de inicio de la plataforma.
    [SerializeField] Transform[] points;//Array de puntos de posición hacia los que la paltaforma se moverá.
    private int i;//Índice del Array.
 
    // Start is called before the first frame update
    void Start()
    {
        //Setear la posición de la plataforma a uno de los puntos, asignado a startin point un valor numérico.
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position)<0.02f)
        {
            i++;//Aumenta el índice, cambia de objetivo.
            if(i==points.Length)//Chequea si la plataforma ha llegado al último punto.   
            {
                i = 0;//Reseta el índice para que vuelva a empezar.
            }
        }
        //Mueve la plataforma a la posición del punto guardado en el Array en el espacio con valor igual a "i"
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
}
