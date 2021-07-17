using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaussShaderGraphScript : MonoBehaviour
{
    private Image _Image;       //�I�u�W�F�N�g�ɃA�^�b�`�����Image�R���|�[�l���g
    private float _PassedTime;  //Start()�����s����Ă���̌o�ߎ���

    // Start is called before the first frame update
    void Start()
    {
        // �C���[�W�R���|�[�l���g�̎擾
        _Image = GetComponent<Image>();

        // �o�ߎ��Ԃ̏�����
        _PassedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // �o�ߎ��Ԃ𑝂₷
        _PassedTime += Time.deltaTime;

        // ShaderGraph���Ŏg���Ă���ڂ����̋��������߂�ϐ��ɁA�o�ߎ��Ԃ���
        _Image.material.SetFloat("ScaleValue", _PassedTime);
    }
}
