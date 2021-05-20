using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FukidasiAnimationUI : MonoBehaviour
{
    //���W�Ƃ��ăv���C���[�Ǐ]�p �q�N���X�ɂ��Ă��悢�H
    private GameObject _Player;

    private Animator _FukidasiAnimator;

    [Header("�R�}���h�����")]
    [SerializeField] private List<GameObject> _PanelList;
    private List<Animator> _PanelAnimList = new List<Animator>();


    [Header("�I���A�C�R�������")]
    [SerializeField] private GameObject _Icon;
    private Animator _IconAnim;

    [SerializeField] private Camera _Camera;

    //�ǂ̌��̂ӂ������𐶐����邩
    private int _FukidasiCount = 0;

    public void Start()
    {
        //����FindWithTag
        var GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _Player = GameManagerScript.GetPlayer();

        _FukidasiAnimator = GetComponent<Animator>();

        //�p�l���̏�����
        foreach (var panel in _PanelList)
        {
            _PanelAnimList.Add(panel.GetComponent<Animator>());
        }
        //�Ƃ肠���������Ȃ����Ă���
        foreach (var panelAnim in _PanelAnimList)
        {
            panelAnim.SetInteger("_ActType", -1);
        }
        Debug.Log("�A�C�R���ǂݍ���");
        _IconAnim = _Icon.GetComponent<Animator>();
        Debug.Log("�A�C�R���ǂݍ��݊���");
    }

    public void Update()
    {
        //�r���{�[�h����
       // Vector3 p = _Camera.transform.position;
        //p.x = transform.position.x;
        //p.y = transform.position.y;
        //p.z = transform.position.z;
      //  this.transform.LookAt(p);


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
        //list�̗v�f�����̂܂�_PanelAnimList��SetInt���Ă�����

        for (int i = 0; i < _PanelAnimList.Count; i++)
        {
            if (i != panelList[i])
                return;

            _PanelAnimList[i].SetInteger("_ActType", i);
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

