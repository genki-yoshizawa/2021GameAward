using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FukidasiAnimationUI : MonoBehaviour
{
    //���W�Ƃ��ăv���C���[�Ǐ]�p �q�N���X�ɂ��Ă��悢�H
    private GameObject _Player;

    private Animator _FukidasiAnimator;

    [Header("Act�i�[�p"), SerializeField] private Sprite[] _ActSprite;

    //�ǂ̌��̂ӂ������𐶐����邩
    private int _FukidasiCount = -1;

    //RectTransform
    private RectTransform _RectTransform;

    public void Start()
    {
        //����FindWithTag
        var GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Player = GameManagerScript.GetPlayer();

        _FukidasiAnimator = GetComponent<Animator>();
        _RectTransform = GetComponent<RectTransform>();


    }

    public void Update()
    {
        //�r���{�[�h����
        //Vector3 p = _Camera.transform.position;
        //p.x = transform.position.x;
        //p.y = transform.position.y;
        //p.z = transform.position.z;
        //this.transform.LookAt(p);

        _RectTransform.position = new Vector3
            (_Player.transform.position.x + 1.0f, _Player.transform.position.y + 1.0f, _Player.transform.position.z);
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

    public void SetActPattern(List<int> panelList, int select = -1)
    {
        Image actImage;

        if (select == -1)
            select = panelList.Count;

        for (int i = 0; i < panelList.Count; i++)
        {
            actImage = transform.GetChild(i).GetComponent<Image>();
            if(i == select - 1)
                actImage.sprite = _ActSprite[panelList[i] * 2];
            else
                actImage.sprite = _ActSprite[panelList[i] * 2 + 1];
        }

    }


}

