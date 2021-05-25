using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraWork : MonoBehaviour
{
    // 設定する各カメラ座標用GameObject
    [SerializeField] private GameObject[] _TopViewCameraPoint;           // 0:表側の俯瞰視点座標　1:裏側の俯瞰視点座標
    [SerializeField] private GameObject[] _TurnOverPoint;                     // 0:表側のギリギリ　1:ちょうどステージの真ん中　2:裏側のギリギリ

    // 各カメラワークの時間指定
    [SerializeField] private float _TopViewToPlayerView_MoveTime;         // 「俯瞰視点→プレイヤー」のカメラワークの移動時間
    [SerializeField] private float _TopViewToPlayerView_RotateTime;       // 「俯瞰視点→プレイヤー」のカメラワークの回転時間
    [SerializeField] private float _TurnOverCameraWork_MoveTime;        // 「ひっくり返し」のカメラワークの移動時間
    [SerializeField] private float _TurnOverCameraWork_RotateTime;      // 「ひっくり返し」のカメラワークの回転時間


    // ゲームマネージャー取得
    private GameManagerScript _GameManagerScript;

    // プレイヤー系
    private GameObject _PlayerObject;                                               // プレイヤーオブジェクト
    private Transform _PlayerOldTransform;                                        // プレイヤーの前トランスフォーム
    private Transform _PlayerCurTransform;                                        // プレイヤーの動いた直後のトランスフォーム
    [SerializeField] private Vector3 _PlayerViewPosOffset;                   // プレイヤー追従カメラで離れる距離(x, y, z)
    [SerializeField] private Vector3 _PlayerViewRotOffset;                   // プレイヤー追従カメラの向き(x, y, z)
    [SerializeField] private Vector3 _rPlayerViewRotOffset;                  // 裏面のプレイヤー追従カメラの向き(x, y, z)
    private Vector3 rPVposOffset;                                                       // 裏面用プレイヤーオフセット

    // TopView系
    private List<Transform> _TopViewTransform = new List<Transform>();                                        // 俯瞰視点のトランスフォーム、0:表側の俯瞰視点座標　1:裏側の俯瞰視点座標

    // カメラ反転用
    private List<Transform> _TurnOverTransform = new List<Transform>();                                       // 反転用のトランスフォーム、0:表側のギリギリ　1:ちょうどステージの真ん中　2:裏側のギリギリ

    private float CameraRotateOffset;                                                                           // レベルデザインをミスった報い

    // カメラ裏表判定フラグ
    [SerializeField] private bool _IsFront;                                                                    // カメラが今表ならtrue

    // カメラ俯瞰視点判定フラグ
    private bool _IsTopView;                                                                    // カメラが今俯瞰視点ならtrue

    // 入れ替え・ひっくり返し
    [SerializeField] private float _PlayerTurnSwapCameraWorkTime = 1.0f;                    // カメラワーク時間
    private bool _PlayerIsSwap;                 // 「入れ替え中」フラグ



    //------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        // 初手FindWithTag
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        // PlayerObjectを取得
        _PlayerObject = _GameManagerScript.GetPlayer();
        

        // プレイヤーの初期トランスフォームを設定
        _PlayerOldTransform = _PlayerObject.transform.transform;
        _PlayerCurTransform = _PlayerObject.transform.transform;
        // TopViewのトランスフォーム設定
        for(int i = 0; i < _TopViewCameraPoint.Length; i++)
        {
            _TopViewTransform.Add(_TopViewCameraPoint[i].transform.transform);
        }
        // TurnOverのトランスフォーム設定
        for (int i = 0; i < _TurnOverPoint.Length; i++)
        {
            _TurnOverTransform.Add(_TurnOverPoint[i].transform.transform);
        }

        // 初期視点を俯瞰視点に設定
        _IsTopView = true;

        CameraRotateOffset = 90.0f;

    }

    // Update is called once per frame
    void Update()
    {
        //// テスト用 ////
        // →を押したら「プレイヤー⇔俯瞰視点」
        if(Input.GetKeyDown(KeyCode.T))
        {
            TopViewToPlayerViewCameraWork();
        }
        // ←を押したら「プレイヤー操作時」
        if (Input.GetKeyDown(KeyCode.F))
        {
            //PlayerMoveCameraWork();     // 「移動」後
            PlayerSwapCameraWork();     // 「入れ替え」後
            //PlayerTurnCameraWork();     // 「反転」後
        }
        // ↑を押したら「ゲームスタート」
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameStartCameraWork();
        }
        // ↓を押したら「裏表反転」
        if (Input.GetKeyDown(KeyCode.R))
        {
            TurnOverCameraWork();
        }
        //// テスト用 ////
        



        // 入れ替えでプレイヤーに追従する処理
        if(_PlayerIsSwap)
        {
            // 座標だけプレイヤーに追従
            if (_IsFront)
            {
                this.gameObject.transform.position = _GameManagerScript.GetPlayer().transform.position + _PlayerViewPosOffset;
            }
            else
            {
                this.gameObject.transform.position = _GameManagerScript.GetPlayer().transform.position + rPVposOffset;
            }
        }


    }

    //------------------------------------------------------------------------------------------------------------
    // 「俯瞰視点⇔プレイヤー」のカメラワーク
    public void TopViewToPlayerViewCameraWork()
    {

        if(_IsTopView)
        {   // 「俯瞰視点→プレイヤー」のカメラワーク

            if (_IsFront && _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {   // カメラが表の時の処理
                // 俯瞰視点にカメラをセット
                this.gameObject.transform.transform.position = _TopViewTransform[0].position;
                this.gameObject.transform.transform.rotation = _TopViewTransform[0].rotation;

                // プレイヤー追従視点にカメラワーク
                iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + _PlayerViewPosOffset), _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
                
                _IsTopView = false;

            }
            else if (!_IsFront && !_PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {   // カメラが裏の時の処理
                // 俯瞰視点にカメラをセット
                this.gameObject.transform.transform.position = _TopViewTransform[1].position;
                this.gameObject.transform.transform.rotation = _TopViewTransform[1].rotation;

                // プレイヤー追従視点にカメラワーク
                // 裏面用にプレイヤー追従カメラのオフセットを設定
                rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
                iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + rPVposOffset), _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _rPlayerViewRotOffset.x, "y", _rPlayerViewRotOffset.y, "z", _rPlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));

                _IsTopView = false;

            }

        }
        else
        {   // 「プレイヤー→俯瞰視点」のカメラワーク
            if (_IsFront)
            {   // カメラが表の時の処理
                // プレイヤー追従視点にカメラをセット
                this.gameObject.transform.transform.position = (_PlayerOldTransform.position + _PlayerViewPosOffset);
                this.gameObject.transform.rotation = Quaternion.Euler(_PlayerViewRotOffset.x, _PlayerViewRotOffset.y, _PlayerViewRotOffset.z);

                // 俯瞰視点にカメラワーク
                iTween.MoveTo(this.gameObject, _TopViewTransform[0].position, _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].localEulerAngles.x, "y", (_TopViewTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[0].localEulerAngles.z, "time", _TopViewToPlayerView_RotateTime));
            }
            else
            {   // カメラが裏の時の処理
                // プレイヤー追従視点にカメラをセット
                rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
                this.gameObject.transform.transform.position = (_PlayerOldTransform.position + rPVposOffset);
                this.gameObject.transform.rotation = Quaternion.Euler(_rPlayerViewRotOffset.x, _rPlayerViewRotOffset.y, _rPlayerViewRotOffset.z);

                // 俯瞰視点にカメラワーク
                iTween.MoveTo(this.gameObject, _TopViewTransform[1].position, _TopViewToPlayerView_MoveTime);
                iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TopViewToPlayerView_RotateTime));
            }

            _IsTopView = true;

        }
        
    }


    // 「プレイヤー移動選択時のカメラワーク(引数：int 移動先のブロック配列の要素数)-----------------------------------------------------------------------
    public void PlayerMoveCameraWork(Vector2Int BlockNum)
    {
        // プレイヤーの現在トランスフォームを更新
        _PlayerObject = _GameManagerScript.GetPlayer();
        _PlayerCurTransform = _PlayerObject.transform.transform;
        _PlayerOldTransform = _PlayerCurTransform;
        
        // 次の移動先ブロックのTransformを取得
        Transform NextMoveToBlock_transform;
        NextMoveToBlock_transform = _GameManagerScript.GetBlock(BlockNum).transform;
        
        if (_IsFront)
        {   // カメラが表の時の処理

            // 次の移動先ブロック座標にカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", NextMoveToBlock_transform.position.x + _PlayerViewPosOffset.x, "y", _PlayerCurTransform.position.y + _PlayerViewPosOffset.y, "z", NextMoveToBlock_transform.position.z + _PlayerViewPosOffset.z, "time", _TopViewToPlayerView_MoveTime));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
            
        }
        else
        {   // カメラが裏の時の処理

            // 次の移動先ブロック座標にカメラワーク
            rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);     // 裏面用オフセット
            iTween.MoveTo(this.gameObject, iTween.Hash("x", NextMoveToBlock_transform.position.x + rPVposOffset.x, "y", _PlayerCurTransform.position.y + rPVposOffset.y, "z", NextMoveToBlock_transform.position.z + rPVposOffset.z, "time", _TopViewToPlayerView_MoveTime));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));
            
        }

        _IsTopView = false;

    }


    // プレイヤー入れ替え用カメラワーク------------------------------------------------------------------------------------------------------
    public void PlayerSwapCameraWork()
    {
        // 入れ替えフラグON
        _PlayerIsSwap = true;

        // カメラのrotationを合わせる
        if(_IsFront)
        {
            this.gameObject.transform.rotation = Quaternion.Euler(_PlayerViewRotOffset.x, _PlayerViewRotOffset.y, _PlayerViewRotOffset.z);
        }
        else
        {
            this.gameObject.transform.rotation = Quaternion.Euler(_rPlayerViewRotOffset.x, _rPlayerViewRotOffset.y, _rPlayerViewRotOffset.z);

            // 裏側のみオフセットを更新
            rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
        }
        
        // 指定時間後に関数を呼び出す
        Invoke(nameof(PlayerSwap_EndFunc), _PlayerTurnSwapCameraWorkTime);
    }
    void PlayerSwap_EndFunc()
    {
        // 入れ替えフラグOFF
        _PlayerIsSwap = false;

        _IsTopView = false;
    }


    // プレイヤーひっくり返し用カメラワーク----------------------------------------------------------------------------------------------
    public void PlayerTurnCameraWork()
    {
        // プレイヤーの現在のトランスフォームを取得してこのカメラをプレイヤーの子供にする
        _PlayerObject = _GameManagerScript.GetPlayer();

        if(_IsFront)
        {
            this.gameObject.transform.position = _PlayerObject.transform.position + _PlayerViewPosOffset;
            // カメラのrotationを合わせる
            this.gameObject.transform.rotation = Quaternion.Euler(_PlayerViewRotOffset.x, _PlayerViewRotOffset.y, _PlayerViewRotOffset.z);
        }
        else
        {
            // 裏側のみオフセットを更新
            rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
            this.gameObject.transform.position = _PlayerObject.transform.position + rPVposOffset;
            // カメラのrotationを合わせる
            this.gameObject.transform.rotation = Quaternion.Euler(_rPlayerViewRotOffset.x, _rPlayerViewRotOffset.y, _rPlayerViewRotOffset.z);
        }
        
        this.gameObject.transform.parent = _PlayerObject.transform;

        // 指定時間後に関数を呼び出す
        Invoke(nameof(PlayerTurn_EndFunc), _PlayerTurnSwapCameraWorkTime);
    }
    void PlayerTurn_EndFunc()
    {

        this.gameObject.transform.parent = null;

        _IsFront = !_IsFront;
        _IsTopView = false;

    }


    // 「裏表切り替え」のカメラワーク------------------------------------------------------------------------------------------------------------
    public void TurnOverCameraWork()
    {
        if(!_IsTopView)     // 俯瞰視点じゃない場合俯瞰視点になるカメラワーク
        {
            if (_IsFront)
            {   // カメラが表の時の処理
                // 俯瞰視点にカメラワーク
                iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].position.x, "y", _TopViewTransform[0].position.y, "z", _TopViewTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
                
                iTween.RotateTo(this.gameObject
                    ,iTween.Hash("x", _TopViewTransform[0].localEulerAngles.x, "y", (_TopViewTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_MoveTime
                    , "oncomplete", "TurnOverCameraWork2"
                    , "oncompletetarget", this.gameObject));
            }
            else
            {   // カメラが裏の時の処理
                // 俯瞰視点にカメラワーク
                iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].position.x, "y", _TopViewTransform[1].position.y, "z", _TopViewTransform[1].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
                
                iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TurnOverCameraWork_MoveTime
                    , "oncomplete", "TurnOverCameraWork2"
                    , "oncompletetarget", this.gameObject));
            }
        }
        else
        {
            this.TurnOverCameraWork2();
        }
    }
    void TurnOverCameraWork2()
    {
        if (_IsFront)
        {   // カメラが表の時の処理
            // 表側のギリギリにカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[0].position.x, "y", _TurnOverTransform[0].position.y, "z", _TurnOverTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[0].localEulerAngles.x, "y", (_TurnOverTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork3"
                    , "oncompletetarget", this.gameObject));
        }
        else
        {   // カメラが裏の時の処理
            // 裏側のギリギリにカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[2].position.x, "y", _TurnOverTransform[2].position.y, "z", _TurnOverTransform[2].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[2].localEulerAngles.x, "y", (_TurnOverTransform[2].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[2].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork3"
                    , "oncompletetarget", this.gameObject));
        }
    }
    void TurnOverCameraWork3()
    {
        // ちょうどステージの真ん中にカメラワーク
        iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[1].position.x, "y", _TurnOverTransform[1].position.y, "z", _TurnOverTransform[1].position.z, "time", _TurnOverCameraWork_MoveTime
            , "easeType", iTween.EaseType.linear));
        iTween.RotateTo(this.gameObject
                , iTween.Hash("x", _TurnOverTransform[1].localEulerAngles.x, "y", (_TurnOverTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[1].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                , "easeType", iTween.EaseType.linear
                , "oncomplete", "TurnOverCameraWork4"
                , "oncompletetarget", this.gameObject));
    }
    void TurnOverCameraWork4()
    {
        if (_IsFront)
        {   // カメラが表の時の処理
            // 裏側のギリギリにカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[2].position.x, "y", _TurnOverTransform[2].position.y, "z", _TurnOverTransform[2].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[2].localEulerAngles.x, "y", (_TurnOverTransform[2].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[2].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork5"
                    , "oncompletetarget", this.gameObject));
        }
        else
        {   // カメラが裏の時の処理
            // 表側のギリギリにカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TurnOverTransform[0].position.x, "y", _TurnOverTransform[0].position.y, "z", _TurnOverTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject
                    , iTween.Hash("x", _TurnOverTransform[0].localEulerAngles.x, "y", (_TurnOverTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TurnOverTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                    , "easeType", iTween.EaseType.linear
                    , "oncomplete", "TurnOverCameraWork5"
                    , "oncompletetarget", this.gameObject));
        }
    }
    void TurnOverCameraWork5()
    {
        if (_IsFront)
        {   // カメラが表の時の処理
            // 裏側の俯瞰視点にカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].position.x, "y", _TopViewTransform[1].position.y, "z", _TopViewTransform[1].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[1].localEulerAngles.x, "y", (_TopViewTransform[1].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[1].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                , "easeType", iTween.EaseType.linear));
        }
        else
        {   // カメラが裏の時の処理
            // 表側の俯瞰視点にカメラワーク
            iTween.MoveTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].position.x, "y", _TopViewTransform[0].position.y, "z", _TopViewTransform[0].position.z, "time", _TurnOverCameraWork_MoveTime
                , "easeType", iTween.EaseType.linear));
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _TopViewTransform[0].localEulerAngles.x, "y", (_TopViewTransform[0].localEulerAngles.y + CameraRotateOffset), "z", _TopViewTransform[0].localEulerAngles.z, "time", _TurnOverCameraWork_RotateTime
                , "easeType", iTween.EaseType.linear));
        }

        _IsFront = !_IsFront;
        _IsTopView = true;

    }


    // 「ゲームスタート時」のカメラワーク------------------------------------------------------------------------------------------------------------------------
    public void GameStartCameraWork()
    {
        // 暫定的に「俯瞰視点→プレイヤー」のカメラワークを設定

        if (_PlayerObject.GetComponent<PlayerControl>().GetIsFront())
        {   // プレイヤーが表スタートの時の処理
            // 俯瞰視点にカメラをセット
            this.gameObject.transform.transform.position = _TopViewTransform[0].position;
            this.gameObject.transform.transform.rotation = _TopViewTransform[0].rotation;

            // プレイヤー追従視点にカメラをセット 
            iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + _PlayerViewPosOffset), _TopViewToPlayerView_MoveTime);
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _PlayerViewRotOffset.x, "y", _PlayerViewRotOffset.y, "z", _PlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));

            _IsTopView = false;

        }
        else if (!_PlayerObject.GetComponent<PlayerControl>().GetIsFront())
        {   // プレイヤーが裏スタートの時の処理
            // 俯瞰視点にカメラをセット
            this.gameObject.transform.transform.position = _TopViewTransform[1].position;
            this.gameObject.transform.transform.rotation = _TopViewTransform[1].rotation;

            // プレイヤー追従視点にカメラをセット 
            // 裏面用にプレイヤー追従カメラのオフセットを設定
            Vector3 rPVposOffset = new Vector3(_PlayerViewPosOffset.x, -(_PlayerViewPosOffset.y), _PlayerViewPosOffset.z);
            iTween.MoveTo(this.gameObject, (_PlayerOldTransform.position + rPVposOffset), _TopViewToPlayerView_MoveTime);
            iTween.RotateTo(this.gameObject, iTween.Hash("x", _rPlayerViewRotOffset.x, "y", _rPlayerViewRotOffset.y, "z", _rPlayerViewRotOffset.z, "time", _TopViewToPlayerView_RotateTime));

            _IsTopView = false;

        }

    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------
    // IsFrontを取得(カメラが表を映しているか否か)
    public bool GetCameraWorkIsFront()
    {
        return _IsFront;
    }

    // IsFrontを設定(パネル操作時に裏面に行った時に使う)
    public void SetCameraWorkIsFront(bool FrontOrBack)
    {
        _IsFront = FrontOrBack;
    }

    // カメラ俯瞰視点判定フラグ
    public bool GetIsTopView()
    {
        return _IsTopView;
    }
    
}
