using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussCamera : MonoBehaviour
{
    // Start is called before the first frame update

    private Camera _MainCamera;
    void Start()
    {
        _MainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = _MainCamera.transform.position;
        this.transform.rotation = _MainCamera.transform.rotation;

        this.GetComponent<Camera>().fieldOfView = _MainCamera.fieldOfView;

    }
}
