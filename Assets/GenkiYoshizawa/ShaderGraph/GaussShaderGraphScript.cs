using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaussShaderGraphScript : MonoBehaviour
{
    private RawImage _Image;       //�I�u�W�F�N�g�ɃA�^�b�`�����Image�R���|�[�l���g
    private float _PassedTime;  //Start()�����s����Ă���̌o�ߎ���

    private bool _GaussPlay = false;

    private float _GaussTime = 0.5f;

    


    // Start is called before the first frame update
    void Start()
    {
        // �C���[�W�R���|�[�l���g�̎擾
        _Image = GetComponent<RawImage>();

        // �o�ߎ��Ԃ̏�����
        _PassedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GaussPlay();
    }

    void GaussPlay()
    {
        if (!_GaussPlay) return;


        // �o�ߎ��Ԃ𑝂₷
        _PassedTime += Time.deltaTime;

        // ShaderGraph���Ŏg���Ă���ڂ����̋��������߂�ϐ��ɁA�o�ߎ��Ԃ���
        _Image.material.SetFloat("ScaleValue", _PassedTime);

        if (_PassedTime >= _GaussTime)
            _GaussPlay = false;

        return;
    }

    public void GaussStart()
    {
        _GaussPlay = true;
    }

    public void GaussReset()
    {
        _GaussPlay = false;
        _Image.material.SetFloat("ScaleValue", 0.0f);
    }
}
