using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float _MaxSize;

    [SerializeField]
    private float _ScaleSpeed;

   
    


    private Vector3 _Rotation;

    private Vector3 _Direction;


    void Start()
    {
        this.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);

        Vector3 rotate;
        rotate.x = Random.Range(-0.1f, 0.1f);
        rotate.y = Random.Range(-0.1f, 0.1f);
        rotate.z = Random.Range(-0.1f, 0.1f);

        _Rotation = rotate;



    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.z <= -3.0f)
            Destroy(this.gameObject);


        if(transform.localScale.x <= _MaxSize)
            transform.localScale += new Vector3(_ScaleSpeed, _ScaleSpeed, _ScaleSpeed);

        transform.Rotate(_Rotation);

        transform.position += _Direction;
    }

    public void Init(Vector3 direction)
    {
        _Direction = direction;
        return;
    }

}
