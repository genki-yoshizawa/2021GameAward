using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCreator : MonoBehaviour
{

    [SerializeField,Range(-1, 1)]
    private float _DirectionRangeX;

    [SerializeField,Range(-1, 1)]
    private float _DirectionRangeY;

    [SerializeField]
    private float _TranslateSpeed;

    [SerializeField]
    private GameObject _Target;


    private int _CreateTimer = 0;

    void Start()
    {
        
    }

    void Update()
    {


        TargetCreate();

    }

    void TargetCreate()
    {
        _CreateTimer++;

        if (_CreateTimer < 120)
            return;

        GameObject obj;
        obj = Instantiate(_Target, this.transform.position, Quaternion.identity);

        //ƒ‰ƒ“ƒ_ƒ€‚ÉˆÚ“®•ûŒü‚ðŒˆ‚ß‚é
        Vector3 dir;
        dir.x = Random.Range(-_DirectionRangeX, _DirectionRangeX);
        dir.y = Random.Range(-_DirectionRangeY, _DirectionRangeY);
        dir.z = -_TranslateSpeed;

        obj.gameObject.GetComponent<Cube>().Init(dir);


        _CreateTimer = 0;
    }
}
