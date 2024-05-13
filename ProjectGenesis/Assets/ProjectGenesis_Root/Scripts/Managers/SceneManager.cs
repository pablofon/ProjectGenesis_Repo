using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void SceneLoader(int sceneToLoad)
    {
        //SceneManager.LoadScene(sceneToLoad); //Preguntar a Jorge
    }

    public void ExitGame()
    {
        Debug.Log("saliendo de Juego");
        Application.Quit();
    }


}
