using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.Return))
            OnStartButtonClick();

    }
    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
