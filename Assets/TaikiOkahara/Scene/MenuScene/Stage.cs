using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float _Range;

    [SerializeField]
    private float _Speed;


    private float _StartPos;

    private float _StartY;

    void Start()
    {
        _StartPos = Random.Range(-_Range, _Range);
        _StartY = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        float sin = Mathf.Sin((Time.time + _StartPos)* _Speed) * _Range;
        Vector3 pos = transform.position;
        pos.y = sin + _StartY;
        transform.position = pos;


        transform.Rotate(0.01f, 0.01f, 0.0f);
    }
}
