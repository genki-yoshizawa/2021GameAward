using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MainCameraScript : MonoBehaviour
{

    [Header("入力のデッドゾーン")]
    [SerializeField, Range(0.0f, 1.0f)] float _InputDeadZone = 0.3f;

    [Header("プレイヤーとの離れる距離（設定した値が最初の距離になる）")]
    [SerializeField] private float _ToPlayerOffset = 1.0f;
    [Header("プレイヤーとの離れる距離の最大値")]
    [SerializeField] private float _ToPlayerOffsetMax = 10.0f;
    [Header("プレイヤーとの離れる距離の最小値")]
    [SerializeField] private float _ToPlayerOffsetMin = 0.5f;
    [Header("横方向のカメラ移動最大速度")]
    [SerializeField] private float _CameraVelocityHorizontal = 0.5f;
    [Header("前後方向のカメラ移動最大速度")]
    [SerializeField] private float _CameraVelocityVertical = 0.5f;

    private GameObject _GameManager = null;
    private GameObject _TopViewObject = null;
    private GameObject _PlayerObject = null;

    // 表か
    private bool _isFront = true;
    // トップビューか
    private bool _isTop = false;

    // カメラの移動制限の判定に使う左右上下のパネルの限界位置
    private float _UpLimitPanelPosition;
    private float _DownLimitPanelPosition;
    private float _RightLimitPanelPosition;
    private float _LeftLimitPanelPosition;

    // ズームの現在速度
    private float _ZoomVelocity = 0.0f;

    // カメラの現在注視点
    private Vector3 _CameraLookAt;

    // 正面ベクトルの保存(編集時にも使う)
    private Vector3 _SaveForward;

    [Header("トップビューのカメラ視点(編集用のbool)")]
    [SerializeField] private bool _EditTopView;
    private bool _CurEditTopView;
    // 編集用プレイヤーとの距離を保存する
    private float _EditSaveOffset;

    // Start is called before the first frame update
    void Start()
    {
        // ExecuteAlwaysを使っているため、ゲーム中かで判別を行う
        if (!Application.IsPlaying(gameObject))
        {
            // エディタ(編集)中の処理
            return;
        }

        _GameManager = GameObject.FindGameObjectWithTag("Manager");

        _PlayerObject = _GameManager.GetComponent<GameManagerScript>().GetPlayer();

        // ビューポイントが増えた時に対応しやすくするためにfor文
        for (int i = 0; i < transform.parent.childCount; ++i)
        {
            if (!transform.parent.GetChild(i).CompareTag("MainCamera"))
            {
                _TopViewObject = transform.parent.GetChild(i).gameObject;
            }
        }

        _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();

        GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();

        Vector2 minPos = new Vector2(0.0f, 0.0f);
        Vector2 maxPos = new Vector2(0.0f, 0.0f);

        bool breakFlg = false;

        foreach (GameObject[] block in blocks)
        {
            foreach (GameObject b in block)
            {
                if (b == null)
                    continue;
                minPos = new Vector2(b.transform.position.x, b.transform.position.z);
                maxPos = new Vector2(b.transform.position.x, b.transform.position.z);
                breakFlg = true;
                break;
            }
            if (breakFlg)
                break;
        }

        foreach (GameObject[] block in blocks)
        {
            foreach (GameObject b in block)
            {
                if (b == null)
                    continue;

                if (minPos.x > b.transform.position.x)
                    minPos.x = b.transform.position.x;
                if (minPos.y > b.transform.position.z)
                    minPos.y = b.transform.position.z;

                if (maxPos.x < b.transform.position.x)
                    maxPos.x = b.transform.position.x;
                if (maxPos.y < b.transform.position.z)
                    maxPos.y = b.transform.position.z;
            }
        }
        _UpLimitPanelPosition = maxPos.y;
        _DownLimitPanelPosition = minPos.y;
        _RightLimitPanelPosition = maxPos.x;
        _LeftLimitPanelPosition = minPos.x;

        _ZoomVelocity = 0.0f;
        _CameraLookAt = _PlayerObject.transform.position;

        // 本当はトップビューからのアニメーションを行う
        // 正面ベクトルも事前に設定していないといけないのはちょい悪い
        //_CameraObject.transform.LookAt(_CameraLookAt);
        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;

    }

    // Update is called once per frame
    void Update()
    {
        // ExecuteAlwaysを使っているため、ゲーム中かで判別を行う
        if (!Application.IsPlaying(gameObject))
        {
            // エディタ(編集)中の処理
            if (_CurEditTopView && !_EditTopView)
            {
                transform.forward = _SaveForward;
                if (CheckPlayerObjectWhenEdit())
                {
                    transform.position = _PlayerObject.transform.position - transform.forward * _EditSaveOffset;
                    _ToPlayerOffset = _EditSaveOffset;
                }
            }

            if (CheckPlayerObjectWhenEdit())
            {
                transform.LookAt(_PlayerObject.transform.position, new Vector3(0.0f, 1.0f, 0.0f));

                _ToPlayerOffset = Vector3.Distance(_PlayerObject.transform.position, transform.position);
            }

            if (!_CurEditTopView && _EditTopView)
            {
                _SaveForward = transform.forward;
                _EditSaveOffset = _ToPlayerOffset;
            }

            _CurEditTopView = _EditTopView;

            if (!_EditTopView)
                return;

            // ビューポイントが増えた時に対応しやすくするためにfor文
            for (int i = 0; i < transform.parent.childCount; ++i)
            {
                if (!transform.parent.GetChild(i).CompareTag("MainCamera"))
                {
                    _TopViewObject = transform.parent.GetChild(i).gameObject;
                }
            }

            if (_TopViewObject == null)
                return;
            transform.position = _TopViewObject.transform.position;
            transform.rotation = _TopViewObject.transform.transform.rotation;

            return;
        }

        float trigger = Input.GetAxis("Controller_L_R_Trigger");
        float rightStickVertical = Input.GetAxis("Controller_R_Stick_Vertical");
        float rightStickHorizontal = Input.GetAxis("Controller_R_Stick_Horizontal");

        if (Input.GetButtonDown("Controller_Y"))
        {
            ResetCamera();
        }

        if (Input.GetButtonDown("Controller_RB"))
        {
            ExchangeTopToFollowPlayer();
        }

        if (Input.GetButtonDown("Controller_LB"))
        {
            ReturnCamera();
        }

        if (_isTop)
            return;
        // これ以降の処理はトップビューの時行わない

        if (trigger < -_InputDeadZone)
            ZoomInOut(/*_isZoomIn = */ false);
        else if (trigger > _InputDeadZone)
            ZoomInOut(/*_isZoomIn = */ true);
        else
            _ZoomVelocity = 0.0f;

        if ((rightStickVertical < -_InputDeadZone || rightStickVertical > _InputDeadZone) || (rightStickHorizontal < -_InputDeadZone || rightStickHorizontal > _InputDeadZone))
        {
            FreeCamera(new Vector2(rightStickHorizontal, rightStickVertical));
            Debug.Log(new Vector2(rightStickHorizontal, rightStickVertical));
        }
    }


    private void FollowPlayer() { }

    private void FreeCamera(Vector2 stick)
    {
        // トップビューなら動かさない
        if (_isTop)
            return;

        // カメラの注視点を移動させる
        _CameraLookAt += new Vector3(-stick.x * _CameraVelocityHorizontal, 0.0f, stick.y * _CameraVelocityVertical * (_isFront ? 1 : -1));

        //位置の補正
        // パネルの位置がすべてy=0前提
        // カメラの座標から視線方向に線を引き、y=0のx,z座標を求める
        float t = /*_CameraLookAt.y - 0.0f*/_CameraLookAt.y / transform.forward.y; //媒介変数
        Vector3 hitPanelPos = _CameraLookAt + transform.forward * t;
        if (hitPanelPos.x < _LeftLimitPanelPosition)
        {
            hitPanelPos.x = _LeftLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }
        else if (hitPanelPos.x > _RightLimitPanelPosition)
        {
            hitPanelPos.x = _RightLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }

        if (hitPanelPos.z < _DownLimitPanelPosition)
        {
            hitPanelPos.z = _DownLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }
        else if (hitPanelPos.z > _UpLimitPanelPosition)
        {
            hitPanelPos.z = _UpLimitPanelPosition;
            _CameraLookAt = hitPanelPos - transform.forward * t;
        }

        // カメラの注視点からカメラのポジションを決める
        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
    }


    // ズームイン・アウトの関数
    private void ZoomInOut(bool isZoomIn)
    {
        // トップビューなら動かさない
        if (_isTop)
            return;

        // 加速度の値設定
        float maxDistance = _ToPlayerOffsetMax - _ToPlayerOffsetMin;
        float accel = (2 * maxDistance) / (90.0f * 90.0f);// 90フレーム

        if ((isZoomIn && _ZoomVelocity < 0.0f) || (!isZoomIn && _ZoomVelocity > 0.0f))
        {
            _ZoomVelocity = 0.0f;
            return;
        }

        _ZoomVelocity = _ZoomVelocity + accel * (isZoomIn ? 1.0f : -1.0f);
        _ToPlayerOffset += _ZoomVelocity;


        // 値の補正
        if (_ToPlayerOffset > _ToPlayerOffsetMax)
        {
            _ToPlayerOffset = _ToPlayerOffsetMax;
            _ZoomVelocity = 0.0f;
        }

        if (_ToPlayerOffset < _ToPlayerOffsetMin)
        {
            _ToPlayerOffset = _ToPlayerOffsetMin;
            _ZoomVelocity = 0.0f;
        }

        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
    }

    private void ExchangeTopToFollowPlayer()
    {
        // 通常カメラ→トップビュー
        if (!_isTop)
        {
            _SaveForward = transform.forward;
            transform.rotation = _TopViewObject.transform.transform.rotation;

            if (_isFront)
                transform.position = _TopViewObject.transform.position;
            else
            {
                // 裏面の場合はTopViewをxz平面に鏡面反射し、z軸回転
                transform.position = new Vector3(_TopViewObject.transform.position.x, -_TopViewObject.transform.position.y, -_TopViewObject.transform.position.z);
                Vector3 newForward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);
                transform.LookAt(transform.position + newForward, -Vector3.up);

            }
        }
        else
        {
            // トップビュー→通常カメラ
            _CameraLookAt = _PlayerObject.transform.position;
            // 表裏でyの位置を変える
            _CameraLookAt.y = _CameraLookAt.y * (_isFront ? 1 : -1);

            transform.position = _CameraLookAt - _SaveForward * _ToPlayerOffset;
            transform.LookAt(_CameraLookAt, Vector3.up * (_isFront ? 1 : -1));
        }
        _isTop = !_isTop;
    }

    private void ReturnCamera()
    {
        _CameraLookAt = new Vector3(_CameraLookAt.x, -_CameraLookAt.y, _CameraLookAt.z);
        Vector3 newForward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);


        if (_isTop)
        {
            // トップビュー
            transform.position = new Vector3(transform.position.x, -transform.position.y, -transform.position.z);
            _SaveForward = new Vector3(_SaveForward.x, -_SaveForward.y, -_SaveForward.z);
        }
        else
        {
            // 非トップビュー
            transform.position = _CameraLookAt - newForward * _ToPlayerOffset;
        }

        _isFront = !_isFront;
        // 設定によっては(トップビューで)反転した時ここで左右反転するのでなにか起きたらここを見る
        transform.LookAt(transform.position + newForward, Vector3.up * (_isFront ? 1.0f : -1.0f));
    }

    private void ResetCamera()
    {
        if (_isTop)
        {
            _CameraLookAt = _PlayerObject.transform.position;

            if (_isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {
                _SaveForward = new Vector3(_SaveForward.x, -_SaveForward.y, -_SaveForward.z);
                _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();
            }

            transform.position = _CameraLookAt - _SaveForward * _ToPlayerOffset;
            transform.LookAt(_CameraLookAt);
            _isTop = false;
        }
        else
        {

            // 見る位置をプレイヤーオブジェクトの位置にする
            // オフセットから位置を決める
            // カメラをLookAtに向かせる
            _CameraLookAt = _PlayerObject.transform.position;

            if (_isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {
                transform.forward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);
                _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();
            }
            transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
            //transform.LookAt(_CameraLookAt);
        }
    }

    private bool CheckIsDisplayPlayer()
    {
        return true;
    }


    private void SetCameraIsFront(bool front) { _isFront = front; }
    private bool GetIsFront() { return _isFront; }

    // このスクリプト内部の値が変更された時に自動で呼び出される
    private void OnValidate()
    {
        // プレイヤーとのOffsetが変更された時にtransform.positionも反映する

        if (CheckPlayerObjectWhenEdit())
            transform.position = _PlayerObject.transform.position - transform.forward * _ToPlayerOffset;
    }

    // 編集時に呼ばれる関数。プレイヤーオブジェクトが見つかればtrueを返す
    private bool CheckPlayerObjectWhenEdit()
    {
        if (_PlayerObject == null)
            _PlayerObject = GameObject.FindGameObjectWithTag("Player");

        if (_PlayerObject != null)
            return true;

        return false;
    }
}
