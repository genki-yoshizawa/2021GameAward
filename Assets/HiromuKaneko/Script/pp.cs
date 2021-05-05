using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            this.transform.position += new Vector3(0.0f, 0.0f, 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.transform.position += new Vector3(0.0f, 0.0f, -1.0f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        }
    }
}
