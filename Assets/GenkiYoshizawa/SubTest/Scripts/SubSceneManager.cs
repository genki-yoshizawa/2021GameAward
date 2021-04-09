using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneManager : MonoBehaviour
{
    [Header("Hierarchy��TitleScene�����Ă�������")]
    [SerializeField] private GameObject _TitleSceneObject;
    [Header("Hierarchy��MenuScene�����Ă�������")]
    [SerializeField] private GameObject _MenuSceneObject;
    [Header("Hierarchy��ResultScene�����Ă�������")]
    [SerializeField] private GameObject _ResultSceneObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ����̓t�F�[�h�C���E�A�E�g�ɂ��V�[���J�ڂ��Ȃ�����
    // �֋X�I�ȃV�[���J�ڂ��s��

    // �^�C�g���V�[���̏�����
    public void InitTitleScene()
    {

    }

    // �^�C�g���V�[���̏I��
    public void UninitTitleScene()
    {
        _TitleSceneObject.SetActive(false);

        InitMenuScene();
    }

    // ���j���[�V�[���̏�����
    public void InitMenuScene()
    {
        _MenuSceneObject.SetActive(true);
    }

    // ���j���[�V�[���̏I��
    public void UninitMenuScene()
    {
        //_MenuSceneObject.SetActive(false);

        // �C�x���g�ɓo�^
        SceneManager.sceneLoaded += StageSelect;

        // �V�[���؂�ւ�
        SceneManager.LoadScene("GameTestScene");

    }

    // ���U���g�V�[���̏�����
    public void InitResultScene()
    {
        _ResultSceneObject.SetActive(true);
    }

    // ���U���g�V�[���̏I��
    public void UninitResultScene()
    {
        _ResultSceneObject.SetActive(false);

        InitMenuScene();
    }


    private void StageSelect(Scene next, LoadSceneMode mode)
    {
        // �V�[���؂�ւ���̃X�N���v�g���擾
        GameSceneController gameSceneControllerScript = GameObject.FindWithTag("Manager").GetComponent<GameSceneController>();

        // �f�[�^��n������

        // �C�x���g����폜
        SceneManager.sceneLoaded -= StageSelect;
    }
}
