using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowCommon : MonoBehaviour
{
    [SerializeField] private EventSystem _EventSystem;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_EventSystem.currentSelectedGameObject == this.gameObject)
        {
            // 処理したい内容
            Debug.Log(gameObject.name + "上にマウスがあるよ");
        }
    }
}
