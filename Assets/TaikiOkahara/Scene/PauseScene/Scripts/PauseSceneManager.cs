using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�ŏI�I�ɂ̓Q�[�����C���V�[���ɑg�ݍ���


public class PauseSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pose();
        }
    }


    void Pose()
    {

    }

}
