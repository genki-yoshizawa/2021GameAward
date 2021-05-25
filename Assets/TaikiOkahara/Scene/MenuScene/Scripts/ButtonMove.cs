using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMove : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float _Length;

    [SerializeField]
    private float _Speed;


    [SerializeField]
    private float _Direction;//�����E��

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
