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
            // �������������e
            Debug.Log(gameObject.name + "��Ƀ}�E�X�������");
        }
    }
}
