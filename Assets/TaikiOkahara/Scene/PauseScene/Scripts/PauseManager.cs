using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class PauseManager : MonoBehaviour
{

    [SerializeField]
    private float _Power = 0.6f;//透明度


    [SerializeField]
    private float _FadeInTime = 0.5f;//フェード時間
    [SerializeField]
    private float _FadeOutTime = 1.0f;//フェード時間

    private float _FadeCount = 0;


    [SerializeField] private GameObject _Image;
    [SerializeField] private Animator _StageModelAnimator;
    [SerializeField] private Animator _DetailUIAnimator;
    [SerializeField] private GameObject _StageNumberUI;
    [SerializeField] private Animator _UIAnimator;
    [SerializeField] Button _StartSetButton;//ポーズ画面に入ったとき選択中のボタン
    [SerializeField] private GameObject _StageModel;
    [SerializeField] private GameObject _PauseOut;
    [SerializeField] private GameObject _Restart;
    [SerializeField] private GameObject _Menu;
    [SerializeField] private AudioClip _PauseStartSound;
    [SerializeField] private AudioClip _Selectound;
    [SerializeField] private AudioClip _DecisionSound;

    private enum FadeType
    {
        NONE,
        IN,
        OUT,
        DO//ポーズ実行中
    }

    private FadeType _FadeType;

    void Start()
    {
        _FadeType = FadeType.NONE;
        // 画面サイズに変更する
        var targetSize = new Vector2(Screen.width, Screen.height);
        _Image.GetComponent<RectTransform>().sizeDelta = targetSize;


        string name = "WorldModel/" + StageManager._StageModelName;
        GameObject obj = (GameObject)Resources.Load(name);

        if (obj == null)
            return;

        _StageModel.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial = obj.transform.GetComponent<Renderer>().sharedMaterial;
        _StageNumberUI.GetComponent<PauseStageNumber>().SetScore(StageManager._ChoiceStageNumber);
    }

    void Update()
    {
        Pose();
    }

    public void Pose()
    {

        FadeIn();
        FadeOut();


    


        if ((_Restart.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Selected") && (/*Input.GetKeyDown("joystick button 1")*/Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return))))
        {
            OnClickRestart();
        }
        else if (_Menu.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Selected") && (/*Input.GetKeyDown("joystick button 1")*/Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return)))
        {
            OnClickMenu();
           
        }

        if (Input.GetKeyDown(KeyCode.Escape) ||(Input.GetKeyDown("joystick button 2"))
            || (_PauseOut.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Selected") && (/*Input.GetKeyDown("joystick button 1")*/Input.GetButtonDown("Controller_B") || Input.GetKeyDown(KeyCode.Return))))
        {
            switch(_FadeType)
            {
                case FadeType.NONE:
                    _FadeType = FadeType.IN;
                    PoseInAnimation();
                    GameObject obj;
                    obj = GameObject.FindGameObjectWithTag("Manager");
                    obj.GetComponent<GameManagerScript>().SetPause();

                    AudioManager.Instance.SetSEVol(0.125f);
                    AudioManager.Instance.SetBGMVol(0.05f);
                    AudioManager.Instance.PlaySE(_PauseStartSound);
                    AudioManager.Instance.ResetSEVol();
                    break;

                case FadeType.DO:
                    _FadeType = FadeType.OUT;
                    PoseOutAnimation();

                    AudioManager.Instance.ResetBGMVol();
                    AudioManager.Instance.PlaySE(_PauseStartSound);

                    break;

                default:
                    break;
            }

        }
    }

    void FadeIn()
    {
        if (_FadeType != FadeType.IN) return;

        _FadeCount += Time.deltaTime;

        if(_FadeCount <= _FadeInTime)
        {
            _Image.GetComponent<Image>().color = new Color(0, 0, 0, _Power * (_FadeCount / _FadeInTime));

            return;
        }

        _FadeCount = 0;
        _FadeType = FadeType.DO;
       
        _StartSetButton.Select();

        return;
    }

    void FadeOut()
    {
        if (_FadeType != FadeType.OUT) return;


        _FadeCount += Time.deltaTime;


        if (_FadeCount <= _FadeOutTime)
        {
            _Image.GetComponent<Image>().color = new Color(0, 0, 0, _Power * ((_FadeOutTime - _FadeCount) / _FadeOutTime));
            return;
        }


        _Image.GetComponent<Image>().color = new Color(0, 0, 0,0);
        _FadeCount = 0;
        _FadeType = FadeType.NONE;
        GameObject obj;
        obj = GameObject.FindGameObjectWithTag("Manager");
        obj.GetComponent<GameManagerScript>().SetUnPause();

        return;
    }


    void PoseInAnimation()
    {
        _StageModelAnimator.SetBool("Pose", true);
        _DetailUIAnimator.SetBool("Pose", true);
        _StageNumberUI.GetComponent<Animator>().SetBool("Pose", true);
        _UIAnimator.SetBool("Pose", true);
    }

    void PoseOutAnimation()
    {
        _StageModelAnimator.SetBool("Pose", false);
        _DetailUIAnimator.SetBool("Pose", false);
        _StageNumberUI.GetComponent<Animator>().SetBool("Pose", false);
        _UIAnimator.SetBool("Pose", false);

    }

    public void OnClickPauseOut()
    {
        if (_FadeType != FadeType.DO)
            return;

        _FadeType = FadeType.OUT;
        PoseOutAnimation();
    }

    public void OnClickRestart()
    {
        AudioManager.Instance.PlaySE(_DecisionSound);
        string restartStageName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(restartStageName);
    }

    public void OnClickMenu()
    {
        AudioManager.Instance.StopBGM();
        AudioManager.Instance.PlaySE(_DecisionSound);
        SceneManager.LoadScene("MenuScene");
        StageManager.Instance.UpdateUI();
    }
}
