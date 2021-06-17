using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWorkScript : MonoBehaviour
{
    private GameObject _GameManager = null;
    private Camera _CameraObject = null;
    private GameObject _TopViewObject = null;
    private GameObject _PlayerObject = null;

    private bool _isFront = true;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");

        _PlayerObject = _GameManager.GetComponent<GameManagerScript>().GetPlayer();

        // ビューポイントが増えた時に対応しやすくするためにfor文
        for(int i = 0; i < transform.childCount; ++i)
        {
            if(transform.GetChild(i).CompareTag("MainCamera"))
            {
                _CameraObject = transform.GetChild(i).GetComponent<Camera>();
            }
            else
            {
                _TopViewObject = transform.GetChild(i).gameObject;
            }
        }

        _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ExchangeTopToFollowPlayer()
    {

    }

    private void FollowPlayer() { }

    private void FreeCamera() { }

    private bool CheckIsDisplayPlayer()
    {
        return true;
    }

    private void SetCameraIsFront(bool front) { _isFront = front; }
    private bool GetIsFront() { return _isFront; }
}
