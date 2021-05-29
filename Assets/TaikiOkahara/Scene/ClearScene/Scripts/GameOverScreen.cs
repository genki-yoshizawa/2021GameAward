using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameOverScreen : MonoBehaviour
{

    [SerializeField]
    Gauss _Gauss;

    [SerializeField]
    GameObject _Pause = null;

    

    [SerializeField]
    private Animator _GameOverScreenAnimator;

   

    bool _PauseFlag = false;
    float _Intencity = 0;

    float _GaussTime = 1.0f;


   

    float _GameOverTextStartCount = 0;
    float _GameOverTextStartTime = 0.5f;

  

    [SerializeField]
    float _ChangeMenuSceneCount = 0;
    float _ChangeMenuSceneTime = 1.0f;

    void Start()
    {
        //DisplayGameOverScreen();
    }

    void Update()
    {


        GaussFilter();
        GameOverScreenAnim();
        ChangeMenuScene();
    }

  

    void GaussFilter()
    {
        if (!_PauseFlag) return;

        if (_Intencity > _GaussTime)
        {
            //_PauseFlag = false;

            return;
        }

        _Intencity += Time.deltaTime;

        _Gauss.Resolution = (int)((_Intencity / _GaussTime) * 20);
    }

    void GameOverScreenAnim()
    {
        if (!_PauseFlag) return;

        if (_GameOverTextStartCount < _GameOverTextStartTime)
        {
            _GameOverTextStartCount += Time.deltaTime;
            return;
        }

        _GameOverScreenAnimator.SetBool("Display", true);
        //_StarParticle.SetBool("StarParticle", true);

        return;
    }

   

    void ChangeMenuScene()
    {
        if (!_PauseFlag) return;

        if (_ChangeMenuSceneCount < _ChangeMenuSceneTime)
        {
            _ChangeMenuSceneCount += Time.deltaTime;
            return;
        }


        if (Input.GetKeyDown("joystick button 1") || Input.GetKeyDown(KeyCode.Return))
        {
            string restartStageName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(restartStageName);
        }


    }

    public void DisplayGameOverScreen()
    {

        AudioManager.Instance.SetBGMVol(0.05f);
        GameObject obj = GameObject.FindGameObjectWithTag("Manager");
        obj.GetComponent<GameManagerScript>().SetPause();
        _PauseFlag = true;
        if (_Pause != null)
            _Pause.SetActive(false);

        transform.parent.GetChild(0).gameObject.SetActive(true);
    }
}