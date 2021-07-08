using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    [Header("act�i�[�p"), SerializeField]
    private Sprite[] _CommandSprite;

    private Animator _Animator;

    //�㉺�R�}���h�̕����̏��
    private int _TopState = 0;
    private int _UnderState = 3;

    //�ړ��R�}���h�̏�Ԋm�F
    private bool _CanMove = false;

    void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    void Update()
    {
    }

    public void CommandSelect(bool top)
    {
        //�R�}���h��I�ԂƂ��ɌĂяo��
        //�ړ���ɃJ�[�\������Ȃ�true
        Image actImage;

        //��Ɖ���+-�𔽑΂ɂ���
        actImage = transform.GetChild(0).GetComponent<Image>();
        actImage.sprite = _CommandSprite[top ? _TopState - 1 : _TopState];

        actImage = transform.GetChild(1).GetComponent<Image>();
        actImage.sprite = _CommandSprite[top ? _UnderState : _UnderState - 1];
    }

    public void SetActPattern(BlockConfig script, bool isEnemy, bool isFront, bool canMove)
    {
        Image actImage;
        _CanMove = canMove;

        //�G�������炻��p�̉摜�̕`�������
        if (isEnemy)
        {
            actImage = transform.GetChild(0).GetComponent<Image>();
            actImage.sprite = _CommandSprite[9];
            return;
        }

        //�ړ��̕����ݒ�
        _TopState = canMove ? 1 : 10;

        actImage = transform.GetChild(0).GetComponent<Image>();
        actImage.sprite = _CommandSprite[canMove ? 1 : 10];

        //���̕�����`�悵�Ȃ��Ȃ�return
        if (!IsUnder())
            return;

        //���̕����ݒ�
        if (script.CheckPanelRotate(isFront))
            _UnderState = 3;
        else if (script.CheckPanelTurnOver(isFront))
            _UnderState = 5;
        else if (script.CheckPanelSwap(isFront))
            _UnderState = 7;

        //1�Ԃ̎q�v�f��sprite��ύX����
        actImage = transform.GetChild(1).GetComponent<Image>();
        actImage.sprite = _CommandSprite[_UnderState];

    }

    public void ActiveSelectCommand(bool top, bool isEnemy)
    {
        //�R�}���h��I�ׂ�悤�ɂ���

        Image actImage;

        if (top)
        {
            actImage = transform.GetChild(0).GetComponent<Image>();

            if(isEnemy)
                actImage.sprite = _CommandSprite[8];
            else
                actImage.sprite = _CommandSprite[0];
        }
        else
        {
            actImage = transform.GetChild(1).GetComponent<Image>();
            actImage.sprite = _CommandSprite[_UnderState - 1];
        }

    }

    public void UnactiveCommand(bool top, bool isEnemy)
    {
        //�R�}���h��I�ׂȂ��悤�ɂ���

        Image actImage;

        if (top)
        {
            actImage = transform.GetChild(0).GetComponent<Image>();
            //actImage.sprite = _CommandSprite[1];

            if(isEnemy)
                actImage.sprite = _CommandSprite[9];
            else
                actImage.sprite = _CommandSprite[1];

        }
        else
        {
            actImage = transform.GetChild(1).GetComponent<Image>();
            actImage.sprite = _CommandSprite[_UnderState];
        }
    }

    public void SetDraw(bool flag) { _Animator.SetBool("Draw", flag); }

    public void SetUnder(bool flag) { _Animator.SetBool("Under", flag); }

    public bool IsDraw(){ return _Animator.GetBool("Draw"); }

    public bool IsUnder(){ return _Animator.GetBool("Under"); }
}
