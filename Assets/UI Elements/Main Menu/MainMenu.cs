using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1;
        Resources.UnloadUnusedAssets();
    }
    public void OnExit()
    {
        Application.Quit();
    }

    public void OnPlay()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
