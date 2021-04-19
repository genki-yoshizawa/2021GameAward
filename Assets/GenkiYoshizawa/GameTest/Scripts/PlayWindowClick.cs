using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayWindowClick : MonoBehaviour, IPointerClickHandler
{
    [Header("HierarchyのEventSystemを入れてください")]
    [SerializeField] private EventSystem _EventSystem;
    [Header("ダブルクリックの猶予時間(秒)")]
    [SerializeField] private float _DoubleClickTime = 0.2f;

    private bool _isClick;

    private GameObject _GameSceneControllerObject;
    private GameSceneController _GameSceneControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        _isClick = false;

        // find関数は重いので初期化時に行う
        _GameSceneControllerObject = GameObject.Find("GameSceneController");
        _GameSceneControllerScript = _GameSceneControllerObject.GetComponent<GameSceneController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // クリックされた時の関数
    public void OnPointerClick(PointerEventData pointerData)
    {
        Debug.Log(gameObject.name + " がクリックされた!");
        if (!_isClick)
        {
            _isClick = true;
            StartCoroutine(MeasureTime());
        }
        else
            DoubleClick();
    }

    // ダブルクリック用タイムカウント関数(コルーチン)
    IEnumerator MeasureTime()
    {
        // 測定タイムの初期化
        float times = 0f;
        while (_isClick)
        {
            times += Time.deltaTime;
            if (times < _DoubleClickTime)
                yield return null;
            else
            {
                // 0.2fの間にクリックしないとクリック判定を落とす
                _isClick = false;
                yield break;
            }
        }
    }

    // ダブルクリック時の関数
    public void DoubleClick()
    {
        _isClick = false;
        if (_GameSceneControllerScript.GetisCubeSelectMode())
            _GameSceneControllerScript.ChangePlayMode();
    }
}
