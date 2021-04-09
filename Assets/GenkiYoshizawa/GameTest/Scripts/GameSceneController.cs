using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    // 最初はプレイモードで始まるためキューブ選択モード識別フラグ
    private bool _isCubeSelectMode;

    private GameObject _DesktopMaskObject;
    private Animator _DesktopMaskAnimator;
    private GameObject _LifeWindowObject;
    private Animator _LifeWindowAnimator;
    private GameObject _ScoreWindowObject;
    private Animator _ScoreWindowAnimator;
    private GameObject _DescriptWindowObject;
    private Animator _DescriptWindowAnimator;
    private GameObject _PlayWindowObject;
    private Animator _PlayWindowAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _isCubeSelectMode = false;

        // find関数は重いので初期化時に行う
        _DesktopMaskObject = GameObject.Find("DesktopMask");
        _DesktopMaskAnimator = _DesktopMaskObject.GetComponent<Animator>();
        _LifeWindowObject = GameObject.Find("LifeWindow");
        _LifeWindowAnimator = _LifeWindowObject.GetComponent<Animator>();
        _ScoreWindowObject = GameObject.Find("ScoreWindow");
        _ScoreWindowAnimator = _ScoreWindowObject.GetComponent<Animator>();
        _DescriptWindowObject = GameObject.Find("DescriptWindow");
        _DescriptWindowAnimator = _DescriptWindowObject.GetComponent<Animator>();
        _PlayWindowObject = GameObject.Find("PlayWindow");
        _PlayWindowAnimator = _PlayWindowObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // キューブ選択モードへの変更
    public void ChangeCubeSelectMode()
    {
        _isCubeSelectMode = true;
        _DesktopMaskAnimator.SetBool("isCubeSelectMode", true);
        _LifeWindowAnimator.SetBool("isCubeSelectMode", true);
        _ScoreWindowAnimator.SetBool("isCubeSelectMode", true);
        _DescriptWindowAnimator.SetBool("isCubeSelectMode", true);
        _PlayWindowAnimator.SetBool("isCubeSelectMode", true);
    }

    // プレイモードへの変更
    public void ChangePlayMode()
    {
        _isCubeSelectMode = false;
        _DesktopMaskAnimator.SetBool("isCubeSelectMode", false);
        _LifeWindowAnimator.SetBool("isCubeSelectMode", false);
        _ScoreWindowAnimator.SetBool("isCubeSelectMode", false);
        _DescriptWindowAnimator.SetBool("isCubeSelectMode", false);
        _PlayWindowAnimator.SetBool("isCubeSelectMode", false);
    }

    public bool GetisCubeSelectMode()
    {
        return _isCubeSelectMode;
    }
}
