using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FukidasiAnimationUI : MonoBehaviour
{
    //座標としてプレイヤー追従用 子クラスにしてもよい？
    private GameObject _Player;

    private Animator _FukidasiAnimator;

    [Header("Act格納用"), SerializeField] private Sprite[] _ActSprite;

    //どの個数のふきだしを生成するか
    private int _FukidasiCount = -1;

    public void Start()
    {
        //初手FindWithTag
        var GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Player = GameManagerScript.GetPlayer();

        _FukidasiAnimator = GetComponent<Animator>();

    }

    public void Update()
    {
        //ビルボード処理
        //Vector3 p = _Camera.transform.position;
        //p.x = transform.position.x;
        //p.y = transform.position.y;
        //p.z = transform.position.z;
        //this.transform.LookAt(p);
    }

    public void SetAnimPattern(int num)
    {
        _FukidasiCount = num;
        _FukidasiAnimator.SetInteger("_ActionCount", num);
    }

    public void ResetAnimPattern()
    {
        _FukidasiCount = -1;
        _FukidasiAnimator.SetInteger("_ActionCount", _FukidasiCount);
    }

    public int GetAnimPattern() { return _FukidasiCount; }

    public void SetActPattern(List<int> panelList)
    {
        //listの要素をそのまま_PanelAnimListのSetIntしてあげる

        Image actImage;

        for (int i = 0; i < panelList.Count; i++)
        {
            actImage = transform.GetChild(i).GetComponent<Image>();
            if(i == panelList.Count - 1)
                actImage.sprite = _ActSprite[panelList[i] * 2];
            else
                actImage.sprite = _ActSprite[panelList[i] * 2 + 1];
        }

    }


}

