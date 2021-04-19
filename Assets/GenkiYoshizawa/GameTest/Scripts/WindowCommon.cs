using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ウィンドウで共通の処理を行うクラス
// なければ消す
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
        Debug.Log(gameObject.name + " がクリックされた!");
    }
}
