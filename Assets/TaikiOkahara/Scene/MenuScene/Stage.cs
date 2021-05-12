using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private float _Height;

    [SerializeField]
    private float _Speed;


    private float _StartPos;

    void Start()
    {
        _StartPos = Random.Range(-_Height, _Height);
    }

    // Update is called once per frame
    void Update()
    {

        float sin = Mathf.Sin((Time.time + _StartPos)* _Speed) * _Height;
        Vector3 pos = transform.position;
        pos.y = sin;
        transform.position = pos;


        transform.Rotate(0.01f, 0.01f, 0.0f);
    }
}
