using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOptionScript : MonoBehaviour
{
    private GameObject _GameManager = null;
    private Animator _CameraOptionAnimator = null;
    private GameObject _CameraObject = null;

    private float _InputDeadZone;
    private float _PassedTime;

    // Start is called before the first frame update
    void Start()
    {
        _CameraOptionAnimator = GetComponent<Animator>();
        _GameManager = GameObject.FindGameObjectWithTag("Manager");

        _CameraObject = _GameManager.GetComponent<GameManagerScript>().GetCamera();
        _InputDeadZone = _CameraObject.GetComponent<MainCameraScript>().GetInputDeadZone();

    }

    // Update is called once per frame
    void Update()
    {
        _PassedTime += Time.deltaTime;

        float trigger = Input.GetAxis("Controller_L_R_Trigger");
        float rightStickVertical = Input.GetAxis("Controller_R_Stick_Vertical");
        float rightStickHorizontal = Input.GetAxis("Controller_R_Stick_Horizontal");
        
        if (_CameraObject.GetComponent<MainCameraScript>().GetIsTop())
        {
            _CameraOptionAnimator.SetFloat("TopViewTime", _PassedTime);
            _CameraOptionAnimator.SetBool("InputKey", false);
        }
        else
        {
            _CameraOptionAnimator.SetFloat("UsuallyTime", _PassedTime);
            _CameraOptionAnimator.SetBool("InputKey", false);
        }

        if ( // カメラ入力
            Input.GetButton("Controller_Y") || Input.GetKey(KeyCode.Y) ||
            Input.GetButton("Controller_RB") || Input.GetKey(KeyCode.T) ||
            Input.GetButton("Controller_LB") || Input.GetKey(KeyCode.R) ||
            trigger < -_InputDeadZone || Input.GetKey(KeyCode.Keypad9) ||
            trigger > _InputDeadZone || Input.GetKey(KeyCode.Keypad3) ||
            Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.Keypad8) ||
            (rightStickVertical < -_InputDeadZone || rightStickVertical > _InputDeadZone) || (rightStickHorizontal < -_InputDeadZone || rightStickHorizontal > _InputDeadZone) ||
            // プレイヤー入力
            Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Controller_L_Stick_Horizontal") > 0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") > 0.5f ||
            Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Controller_L_Stick_Horizontal") < -0.5f || Input.GetAxis("Controller_D_Pad_Horizontal") < -0.5f ||
            Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("Controller_L_Stick_Vertical") > 0.5f || Input.GetAxis("Controller_D_Pad_Vertical") > 0.5f ||
            Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("Controller_L_Stick_Vertical") < -0.5f || Input.GetAxis("Controller_D_Pad_Vertical") < -0.5f ||
            Input.GetButton("Controller_B") || Input.GetKey(KeyCode.Return)
            ) 
        {
            _CameraOptionAnimator.SetBool("InputKey",true);
            _PassedTime = 0.0f;
            _CameraOptionAnimator.SetFloat("TopViewTime", _PassedTime);
            _CameraOptionAnimator.SetFloat("UsuallyTime", _PassedTime);
        }
    }
}
