using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : MonoBehaviour
{
    private float _Length = 7.0f;

    private float _Speed = 4.0f;


    [SerializeField]
    private float _Direction;//ç∂Ç©âEÇ©

    private RectTransform _Pos;
    private Vector3 _StartPos;

    void Start()
    {
        _Pos = GetComponent<RectTransform>();
        _StartPos = _Pos.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float sin = Mathf.Sin((Time.time) * _Speed * _Direction) * _Length;
        Vector3 nexPos = _StartPos + new Vector3(sin, 0.0f, 0.0f);
        _Pos.localPosition = nexPos;
    }
}
