using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void BoatTrip()
    {
        SceneManager.LoadScene(1);
    }

    public void Flying()
    {
        
    }

    public void CarScene()
    {
        SceneManager.LoadScene(4);
    }

    public void RollerCoaster()
    {
        SceneManager.LoadScene(2);
    }

    public void Everest()
    {
        SceneManager.LoadScene(7);
    }

    public void Horror()
    {
        SceneManager.LoadScene(6);
    }

    public void CookieMan()
    {
        SceneManager.LoadScene(5);
    }

    public void Skydive()
    {
        SceneManager.LoadScene(3);
    }
}
