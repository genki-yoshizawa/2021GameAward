using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallConfig : MonoBehaviour
{
    [Header("�ǃ��x���̐ݒ�")]
    [SerializeField] private int _WallLevel = 1;
    [Header("�f�t�H���g�̕ǂ̈ʒu(�Е��͕K��0�A����0�̓_��)")]
    [SerializeField, TooltipAttribute("X���̈ړ�"), Range(-1, 1)] private int _XAxis = 0;
    [SerializeField, TooltipAttribute("Z���̈ړ�"), Range(-1, 1)] private int _ZAxis = 0;
    [Header("�p�l�����S����ǂꂾ�����炷��(�v�����i�[�͘M��Ȃ��Ă�����)")]
    [SerializeField] private float _LocalOffset = 0.45f;

    // �ǂ̌���
    private Vector2 _Direction = new Vector3(1f, 0f);
    // �ǂ̔j��t���O
    private bool _isBreak = true;

    // Start is called before the first frame update
    void Start()
    {
        if (_XAxis != 0 && _ZAxis != 0 || _XAxis == 0 && _ZAxis == 0)
        {
            Debug.LogError(gameObject.name + ":�f�t�H���g�̕ǂ̈ʒu���s���ł��B");
            //UnityEditor.EditorApplication.isPlaying = false;
        }

        _isBreak = true;

        transform.localPosition = new Vector3(_XAxis * _LocalOffset, 0f, _ZAxis * _LocalOffset);

        Vector3 direction = new Vector3(1f, 0f, 0f);

        float angle = 0f;

        if(_XAxis == 0)
        {
            if (_ZAxis > 0) angle = -90;
            if (_ZAxis < 0) angle = 90;
        }
        else
        {
            if (_XAxis < 0) angle = 180; 
        }

        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        _Direction = new Vector2(direction.x, direction.z);

        transform.Rotate(Vector3.up, angle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public Vector2 GetDirection() { return _Direction; }
    public void SetDirection(Vector2 directiron) { _Direction = directiron; }
}
