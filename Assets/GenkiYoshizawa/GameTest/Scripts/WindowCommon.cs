using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �E�B���h�E�ŋ��ʂ̏������s���N���X
// �Ȃ���Ώ���
public class WindowCommon : MonoBehaviour, IPointerClickHandler
{

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerClick(PointerEventData pointerData)
    {
        Debug.Log(gameObject.name + " ���N���b�N���ꂽ!");
    }
}
