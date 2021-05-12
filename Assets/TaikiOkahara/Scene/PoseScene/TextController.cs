using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

    [SerializeField]
    private GameObject _TextParent;

    private GameObject[] _TextList;

    [SerializeField]
    private Vector2 _StartPosition;


    [SerializeField]
    private Vector2 _StopPosition;


    private int _ChildCount;

    private int _FontSize;


    void Start()
    {
        _ChildCount = _TextParent.transform.childCount;
        _TextList = new GameObject[_ChildCount];

        _FontSize = 38;
        for(int i = 0;i < _ChildCount; i++)
        {
            _TextList[i] = _TextParent.transform.GetChild(i).gameObject;
            _TextList[i].GetComponent<RectTransform>().position = _StartPosition;
        }
    }

    void Update()
    {
        //TextIn();
    }

    void TextIn()
    {

        for(int i = 0;i< _ChildCount;i++)
        {
            RectTransform rectTrans = _TextList[i].GetComponent<RectTransform>();
            Vector3 pos = rectTrans.position;
            pos.x += 1.0f;
            rectTrans.position = pos;
        }
    }

}