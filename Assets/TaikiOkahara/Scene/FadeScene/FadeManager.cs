using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _Score;

    [SerializeField]
    private string _NextSceneName;

    void Start()
    {
        int stageNumber = StageManager._ChoiceStageNumber;
        _Score.GetComponent<FadeStageNumber>().SetScore(stageNumber);
        _NextSceneName = StageManager._StageName;
        StartCoroutine(Display());


        //// �v���n�u���擾
        //GameObject prefab = (GameObject)Resources.Load("WorldModel/StageModel1");
        //// �v���n�u����C���X�^���X�𐶐�
        //Instantiate(prefab, new Vector3(0,0,0), Quaternion.identity);
    }

    void Update()
    {
        
    }

    private IEnumerator Display()
    {
        yield return new WaitForSeconds(4.0f);
        
        SceneManager.LoadScene(_NextSceneName);
    }
}
