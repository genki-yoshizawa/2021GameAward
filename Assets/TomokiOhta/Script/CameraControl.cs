using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private Vector3 nowMousePos;

    private bool mouseWheelPress;
    private bool mouseRightPress;


    void Start()
    {
        mouseWheelPress = false;
    }

    void Update()
    {

        float scroll = scroll = Input.GetAxis("Mouse ScrollWheel");
        this.transform.position += transform.forward * scroll * 3.0f;

        //ホイール、pos変更
        if (Input.GetMouseButtonDown(2))
        {
            nowMousePos = Input.mousePosition;
            mouseWheelPress = true;
        }
        if (Input.GetMouseButtonUp(2))
        {
            mouseWheelPress = false;
        }

        //右、rotate変更
        if (Input.GetMouseButtonDown(1))
        {
            nowMousePos = Input.mousePosition;
            mouseRightPress = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            mouseRightPress = false;
        }




        if (mouseWheelPress)
        {
            var mousePos = Input.mousePosition;
            var moveDirection = nowMousePos - mousePos;

            moveDirection *= 0.01f;

            this.transform.Translate(moveDirection);

            nowMousePos = mousePos;
        }

        if (mouseRightPress)
        {
            var mousePos = Input.mousePosition;
            var moveDirection = nowMousePos - mousePos;

            moveDirection *= 0.25f;

            this.transform.RotateAround(transform.position, transform.right, moveDirection.y);
            this.transform.RotateAround(transform.position, Vector3.up, -moveDirection.x);

            nowMousePos = mousePos;
        }

    }
}
