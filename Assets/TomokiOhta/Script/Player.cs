using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            this.transform.Translate(0.01f,0.0f,0.0f);
            AudioManager.Instance.PlaySE(audioClip);
        }
    }
}
