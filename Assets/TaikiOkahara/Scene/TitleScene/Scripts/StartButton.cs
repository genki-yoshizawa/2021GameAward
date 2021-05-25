using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Input.GetKeyDown("joystick button 0"))
            OnStartButtonClick();

    }
    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
