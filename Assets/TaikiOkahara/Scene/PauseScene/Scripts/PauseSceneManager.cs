using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//最終的にはゲームメインシーンに組み込む


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
