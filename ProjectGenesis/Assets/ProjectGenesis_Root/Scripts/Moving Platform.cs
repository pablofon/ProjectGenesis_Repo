using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movin : MonoBehaviour
{
    [SerializeField] float speed;//Velocidad de la plataforma.
    [SerializeField] int startingPoint;//N�mero para determinar el �ndice del punto de inicio de la plataforma.
    [SerializeField] Transform[] points;//Array de puntos de posici�n hacia los que la paltaforma se mover�.
    private int i;//�ndice del Array.
 
    // Start is called before the first frame update
    void Start()
    {
        //Setear la posici�n de la plataforma a uno de los puntos, asignado a startin point un valor num�rico.
        transform.position = points[startingPoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position)<0.02f)
        {
            i++;//Aumenta el �ndice, cambia de objetivo.
            if(i==points.Length)//Chequea si la plataforma ha llegado al �ltimo punto.   
            {
                i = 0;//Reseta el �ndice para que vuelva a empezar.
            }
        }
        //Mueve la plataforma a la posici�n del punto guardado en el Array en el espacio con valor igual a "i"
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
}
