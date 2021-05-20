using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButtonChoice : MonoBehaviour
{
    [SerializeField]
    Button _Button;


    void Start()
    {
        _Button = GameObject.Find("Canvas/PauseOut").GetComponent<Button>();

        _Button.Select();
    }

    void Update()
    {
        
    }
}
