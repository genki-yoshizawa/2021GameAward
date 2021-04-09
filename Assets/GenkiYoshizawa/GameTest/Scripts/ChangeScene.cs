using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �Q�[�����I�����锻����s�����̂�GameSceneController�I�u�W�F�N�g���擾���Ă��̊֐����Ăяo��
    public void ChangeSceneResult()
    {
        // �C�x���g�ɓo�^
        SceneManager.sceneLoaded += InputScore;

        // �V�[���؂�ւ�
        SceneManager.LoadScene("SubSceneTest");

    }

    private void InputScore(Scene next, LoadSceneMode mode)
    {
        // �V�[���؂�ւ���̃X�N���v�g���擾
        SubSceneManager subSceneManagerScript = GameObject.FindWithTag("Manager").GetComponent<SubSceneManager>();

        // �f�[�^��n������

        // �C�x���g����폜
        SceneManager.sceneLoaded -= InputScore;
    }
}
