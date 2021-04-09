using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultButton : MonoBehaviour
{
    [Header("Hierarchy��SubSceneManager�����Ă�������")]
    [SerializeField] private GameObject _SubSceneManagerObject;
    private SubSceneManager _SubSceneManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        _SubSceneManagerScript = _SubSceneManagerObject.GetComponent<SubSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // �{�^���������ꂽ�ꍇ�A����Ăяo�����֐�
    public void OnClick()
    {
        _SubSceneManagerScript.UninitResultScene();
    }
}
