using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FukidasiAnimationUI : MonoBehaviour
{
    //座標としてプレイヤー追従用 子クラスにしてもよい？
    private GameObject _Player;

    private Animator _FukidasiAnimator;

    [Header("コマンド入れる")]
    [SerializeField] private List<GameObject> _PanelList;
    private List<Animator> _PanelAnimList = new List<Animator>();


    [Header("選択アイコン入れる")]
    [SerializeField] private GameObject _Icon;
    private Animator _IconAnim;

    //どの個数のふきだしを生成するか
    private int _FukidasiCount = 0;

    public void Start()
    {
        //初手FindWithTag
        var GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Player = GameManagerScript.GetPlayer();

        _FukidasiAnimator = GetComponent<Animator>();

        //パネルの初期化
        foreach (var panel in _PanelList)
        {
            _PanelAnimList.Add(panel.GetComponent<Animator>());
        }
        //とりあえず見えなくしておく
        foreach (var panelAnim in _PanelAnimList)
        {
            panelAnim.SetInteger("_ActType", -1);
        }
        _IconAnim = _Icon.GetComponent<Animator>();
    }

    public void Update()
    {
        //ビルボード処理
        //Vector3 p = Camera.main.transform.position;
        //p.x = transform.position.x;
        //p.y = transform.position.y;
        //p.z = transform.position.z;
        //transform.LookAt(p);
    }

    public void SetCount(int num)
    {
        _FukidasiCount = num;
        _FukidasiAnimator.SetInteger("_ActionCount", num);
    }

    public void ResetCount()
    {
        _FukidasiCount = -1;
        _FukidasiAnimator.SetInteger("_ActionCount", _FukidasiCount);
    }

    public int GetCount() { return _FukidasiCount; }

    public void SetPanel(List<int> panelList)
    {
        //listの要素をそのまま_PanelAnimListのSetIntしてあげる

        foreach (var panel in panelList)
        {

        }
        _IconAnim.SetInteger("_Select", 1);
    }

    public void ResetPanel()
    {
        foreach (var panelAnim in _PanelAnimList)
        {
            panelAnim.SetInteger("_ActType", -1);
        }
        _IconAnim.SetInteger("_Select", 0);
    }
}

