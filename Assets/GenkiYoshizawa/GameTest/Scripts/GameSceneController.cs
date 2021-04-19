using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    // 最初はプレイモードで始まるためキューブ選択モード識別フラグ
    private bool _isCubeSelectMode;

    [Header("HierarchyのCanvasを入れてください")]
    [SerializeField] private GameObject _CanvasObject;
    [Header("HierarchyのDesktopMaskを入れてください")]
    [SerializeField] private GameObject _DesktopMaskObject;
    private Animator _DesktopMaskAnimator;
    [Header("HierarchyのLifeWindowを入れてください")]
    [SerializeField] private GameObject _LifeWindowObject;
    private Animator _LifeWindowAnimator;
    [Header("HierarchyのScoreWindowを入れてください")]
    [SerializeField] private GameObject _ScoreWindowObject;
    private Animator _ScoreWindowAnimator;
    [Header("HierarchyのDescriptWindowを入れてください")]
    [SerializeField] private GameObject _DescriptWindowObject;
    private Animator _DescriptWindowAnimator;
    [Header("HierarchyのPlayWindowを入れてください")]
    [SerializeField] private GameObject _PlayWindowObject;
    private Animator _PlayWindowAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _isCubeSelectMode = false;
        
        _DesktopMaskAnimator = _DesktopMaskObject.GetComponent<Animator>();
        _LifeWindowAnimator = _LifeWindowObject.GetComponent<Animator>();
        _ScoreWindowAnimator = _ScoreWindowObject.GetComponent<Animator>();
        _DescriptWindowAnimator = _DescriptWindowObject.GetComponent<Animator>();
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

        // 親子関係の再構築
        _PlayWindowObject.transform.parent = _DesktopMaskObject.transform;
        // ヒエラルキー順番の再設定
        _PlayWindowObject.transform.SetSiblingIndex(5);
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
        
        // 親子関係の再構築
        _PlayWindowObject.transform.parent = _CanvasObject.transform;
        // ヒエラルキー順番の再設定
        _PlayWindowObject.transform.SetSiblingIndex(1);
    }

    public bool GetisCubeSelectMode()
    {
        return _isCubeSelectMode;
    }
}
