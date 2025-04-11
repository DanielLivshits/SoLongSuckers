using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuScript : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnResume()
    {
        player.GetComponent<CharacterMovement>().PauseMenuThing();
    }

    public void OnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    public void OnRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Gameplay");
    }

    public void OnCheckpoint()
    {
        Time.timeScale = 1;

        player.GetComponent<CharacterMovement>().GoCheckpoint();

        player.GetComponent<CharacterMovement>().PauseMenuThing();
    }
}
