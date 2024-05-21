using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    public void SceneLoader(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        GameManager.Instance.level += 1;
    }

    public void ExitGame()
    {
        Debug.Log("saliendo de Juego");
        Application.Quit();
    }

    private void Update()
    {
        if (GameManager.Instance.levelFinished == true)
        {
            SceneLoader(GameManager.Instance.level);
            GameManager.Instance.levelFinished = false;
        }
        if (GameManager.Instance.playerFalled == true)
        {
            SceneLoader(5);
            GameManager.Instance.levelFinished = false;
        }
    }
}
