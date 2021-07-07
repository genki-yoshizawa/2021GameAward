using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
    [Header("act格納用"), SerializeField]
    private Sprite[] _CommandSprite;

    private Animator _Animator;

    //上下コマンドの文字の状態
    private int _TopState = 0;
    private int _UnderState = 3;

    //移動コマンドの状態確認
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
        //コマンドを選ぶときに呼び出す
        //移動後にカーソルが上ならtrue
        Image actImage;

        //上と下で+-を反対にする
        actImage = transform.GetChild(0).GetComponent<Image>();
        actImage.sprite = _CommandSprite[top ? _TopState - 1 : _TopState];

        actImage = transform.GetChild(1).GetComponent<Image>();
        actImage.sprite = _CommandSprite[top ? _UnderState : _UnderState - 1];
    }

    public void SetActPattern(BlockConfig script, bool isEnemy, bool isFront, bool canMove)
    {
        Image actImage;
        _CanMove = canMove;

        //敵がいたらそれ用の画像の描画をする
        if (isEnemy)
        {
            actImage = transform.GetChild(0).GetComponent<Image>();
            actImage.sprite = _CommandSprite[9];
            return;
        }

        //移動の文字設定
        _TopState = canMove ? 1 : 10;

        actImage = transform.GetChild(0).GetComponent<Image>();
        actImage.sprite = _CommandSprite[canMove ? 1 : 10];

        //下の文字を描画しないならreturn
        if (!IsUnder())
            return;

        //下の文字設定
        if (script.CheckPanelRotate(isFront))
            _UnderState = 3;
        else if (script.CheckPanelTurnOver(isFront))
            _UnderState = 5;
        else if (script.CheckPanelSwap(isFront))
            _UnderState = 7;

        //1番の子要素のspriteを変更する
        actImage = transform.GetChild(1).GetComponent<Image>();
        actImage.sprite = _CommandSprite[_UnderState];

    }

    public void ActiveSelectCommand(bool top, bool isEnemy)
    {
        //コマンドを選べるようにする

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
        //コマンドを選べないようにする

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
