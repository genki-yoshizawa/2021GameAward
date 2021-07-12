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
    [Header("ゲームスタート時のカメラワークに費やす秒数")]
    [SerializeField] private float _GameStartCameraWorkTime = 1.0f;

    private GameObject _GameManager = null;
    private List<GameObject> _TopViewObject = null;
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

    [Header("トップビューのカメラ視点の番号(編集用の整数)")]
    [SerializeField] private int _EditTopViewNumber;
    [Header("トップビューのカメラ視点(編集用のbool)")]
    [SerializeField] private bool _EditTopView;
    private bool _CurEditTopView;
    // 編集用プレイヤーとの距離を保存する
    private float _EditSaveOffset;

    private bool _isPlayerMove;
    private bool _isPlayerSwap;
    private bool _isPlayerTurnOver;
    private Vector3 _SaveLookAtToPlayerDirection;
    private float _SaveLookAtToPlayerOffset;

    private bool _isGameStartCameraWork; //スタート時のカメラワーク

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

        _TopViewObject = new List<GameObject>();

        // ビューポイントが増えた時に対応しやすくするためにfor文
        for (int i = 0; i < transform.parent.childCount; ++i)
        {
            if (!transform.parent.GetChild(i).CompareTag("MainCamera"))
            {
                _TopViewObject.Add(transform.parent.GetChild(i).gameObject);
            }
        }

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
        //transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
        _isPlayerMove = false;
        _isPlayerSwap = false;
        _isPlayerTurnOver = false;
        _isGameStartCameraWork = true;
        StartCoroutine(GameStartCameraWork());
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

            int topViewObjectCount = 0;

            _TopViewObject = new List<GameObject>();

            // ビューポイントが増えた時に対応しやすくするためにfor文
            for (int i = 0; i < transform.parent.childCount; ++i)
            {
                if (!transform.parent.GetChild(i).CompareTag("MainCamera"))
                {
                    if (_TopViewObject.Count < i)
                    {
                        _TopViewObject.Add(transform.parent.GetChild(i).gameObject);
                    }
                    else
                    {
                        _TopViewObject[topViewObjectCount] = transform.parent.GetChild(i).gameObject;
                    }
                    topViewObjectCount++;
                }
            }

            if (_TopViewObject.Count <= 0)
                return;
            transform.position = _TopViewObject[_EditTopViewNumber].transform.position;
            transform.rotation = _TopViewObject[_EditTopViewNumber].transform.transform.rotation;

            return;
        }

        // スタートカメラワーク時はreturn
        if (_isGameStartCameraWork)
            return;

        // プレイヤーが動いている時は入力処理を行わずreturn
        if (_isPlayerMove)
            return;

        // プレイヤーがパネル入れ替えで動いている時は入力処理を行わずreturn
        if (_isPlayerSwap)
            return;

        // プレイヤーがひっくり返しパネルで動いているときは入力処理を行わずreturn
        if (_isPlayerTurnOver)
        {
            return;
        }

        float trigger = Input.GetAxis("Controller_L_R_Trigger");
        float rightStickVertical = Input.GetAxis("Controller_R_Stick_Vertical");
        float rightStickHorizontal = Input.GetAxis("Controller_R_Stick_Horizontal");

        if (Input.GetButtonDown("Controller_Y") || Input.GetKeyDown(KeyCode.Y)) //暫定的なキーボード入力
        {
            ResetCamera();
        }

        if (Input.GetButtonDown("Controller_RB") || Input.GetKeyDown(KeyCode.T))//暫定的なキーボード入力
        {
            ExchangeTopToFollowPlayer();
        }

        if (Input.GetButtonDown("Controller_LB") || Input.GetKeyDown(KeyCode.R))//暫定的なキーボード入力
        {
            ReturnCamera();
        }

        if (_isTop)
            return;
        // これ以降の処理はトップビューの時行わない

        if (trigger < -_InputDeadZone || Input.GetKey(KeyCode.U))
            ZoomInOut(/*_isZoomIn = */ false);
        else if (trigger > _InputDeadZone || Input.GetKey(KeyCode.J))
            ZoomInOut(/*_isZoomIn = */ true);
        else
            _ZoomVelocity = 0.0f;

        //暫定的なキーボード入力
        float horizontalMove = 0.0f;
        float verticalMove = 0.0f;
        if (Input.GetKey(KeyCode.DownArrow))
            verticalMove = -1.0f;
        if (Input.GetKey(KeyCode.LeftArrow))
            horizontalMove = 1.0f;
        if (Input.GetKey(KeyCode.RightArrow))
            horizontalMove = -1.0f;
        if (Input.GetKey(KeyCode.UpArrow))
            verticalMove = 1.0f;

        if ((rightStickVertical < -_InputDeadZone || rightStickVertical > _InputDeadZone) || (rightStickHorizontal < -_InputDeadZone || rightStickHorizontal > _InputDeadZone))
            FreeCamera(new Vector2(rightStickHorizontal, rightStickVertical));
        else if (verticalMove != 0.0f || horizontalMove != 0.0f)
            FreeCamera(new Vector2(horizontalMove, verticalMove));


    }



    private void FreeCamera(Vector2 stick)
    {
        // トップビューなら動かさない
        if (_isTop)
            return;

        // カメラの注視点を移動させる
        _CameraLookAt += new Vector3(-stick.x * _CameraVelocityHorizontal, 0.0f, stick.y * _CameraVelocityVertical * (_isFront ? 1 : -1));

        //位置の補正
        CameraLookAtModifyPanelRange();

        // カメラの注視点からカメラのポジションを決める
        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
    }


    private void FollowPlayer()
    {
        _CameraLookAt = _PlayerObject.transform.position - _SaveLookAtToPlayerDirection * _SaveLookAtToPlayerOffset;

        //位置の補正
        CameraLookAtModifyPanelRange();

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
            int topViewObjectNumber = TargetTopViewObject();
            GameObject targetTopViewObject = _TopViewObject[topViewObjectNumber];

            transform.rotation = targetTopViewObject.transform.transform.rotation;

            if (_isFront)
                transform.position = targetTopViewObject.transform.position;
            else
            {
                // 裏面の場合はTopViewをxz平面に鏡面反射し、z軸回転
                transform.position = new Vector3(targetTopViewObject.transform.position.x, -targetTopViewObject.transform.position.y, targetTopViewObject.transform.position.z);
                Vector3 newForward = new Vector3(transform.forward.x, -transform.forward.y, transform.forward.z);
                transform.LookAt(transform.position + newForward, -Vector3.up);

            }
        }
        else
        {
            // トップビュー→通常カメラ
            //_CameraLookAt = _PlayerObject.transform.position;
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
            transform.position = new Vector3(transform.position.x, -transform.position.y, transform.position.z);
            _SaveForward = new Vector3(_SaveForward.x, -_SaveForward.y, _SaveForward.z);
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
        _CameraLookAt = _PlayerObject.transform.position;

        if (_isTop)
        {
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
            if (_isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront())
            {
                transform.forward = new Vector3(transform.forward.x, -transform.forward.y, -transform.forward.z);
                _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();
            }
            transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;
            //transform.LookAt(_CameraLookAt);
        }
    }


    private bool CheckPlayerInDisplay()
    {
        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(gameObject.GetComponent<Camera>());

        for (int i = 0; i < 6; i++)
        {
            if (!plane[i].GetSide(_PlayerObject.transform.position))
                return false;
        }

        return true;
    }

    private int TargetTopViewObject()
    {
        List<int> targetTopViewNumberList = new List<int>();

        for (int i = 0; i < _TopViewObject.Count; ++i)
        {
            if (CheckCameraInTopViewRange(_TopViewObject[i]))
                targetTopViewNumberList.Add(i);
        }

        int number = 0;

        switch (targetTopViewNumberList.Count)
        {
            case 0: // どの範囲にも入っていなかった場合すべてのTopViewの中で一番近いものを選ぶ
                number = NearestTopViewNumber(null);
                break;
            case 1: // 1つの場合それを選ぶ
                number = targetTopViewNumberList[0];
                break;
            default: // 複数ある場合はその選択肢の中で一番近いもの選ぶ
                number = NearestTopViewNumber(targetTopViewNumberList);
                break;
        }

        return number;
    }

    private bool CheckCameraInTopViewRange(GameObject topView)
    {
        TopViewScript script = topView.GetComponent<TopViewScript>();
        Vector3 position = script.GetTopViewColliderCenter();
        float range = script.GetTopViewColliderRange();

        if (position.x - range < _CameraLookAt.x && position.x + range > _CameraLookAt.x &&
            position.z - range < _CameraLookAt.z && position.z + range > _CameraLookAt.z)// xz平面において範囲内
            return true;

        return false;
    }

    private int NearestTopViewNumber(List<int> targetNumberList)
    {
        // nullが入ってきた場合すべてをチェック
        if (targetNumberList == null)
        {
            targetNumberList = new List<int>();
            for (int i = 0; i < _TopViewObject.Count; ++i)
            {
                targetNumberList.Add(i);
            }
        }

        // ターゲットのy座標は0にする
        Vector3 targetPosition = _TopViewObject[0].transform.position;
        targetPosition.y = 0f;

        float nearestDistance = Vector3.Distance(_CameraLookAt, targetPosition);
        int nearestNumber = 0;

        for(int i = 1; i < targetNumberList.Count; ++i)
        {
            targetPosition = _TopViewObject[i].transform.position;
            targetPosition.y = 0f;

            float dist = Vector3.Distance(_CameraLookAt, targetPosition);

            if (nearestDistance > dist)
            {
                nearestDistance = dist;
                nearestNumber = i;
            }
        }

        return nearestNumber;
    }

    private void SetCameraIsFront(bool front) { _isFront = front; }
    public bool GetIsFront() { return _isFront; }
    public bool GetIsTop() { return _isTop; }

    public float GetInputDeadZone() { return _InputDeadZone; }

    public void SetIsPlayerMove(bool move)
    {
        _isPlayerMove = move;

        if (!_isPlayerMove)
            return;

        if (_isTop || _isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront() || !CheckPlayerInDisplay())
            ResetCamera();

        _SaveLookAtToPlayerDirection = _PlayerObject.transform.position - _CameraLookAt;
        _SaveLookAtToPlayerOffset = _SaveLookAtToPlayerDirection.magnitude;   
        _SaveLookAtToPlayerDirection = Vector3.Normalize(_SaveLookAtToPlayerDirection);

        StartCoroutine(PlayerIsMoveCameraWork());

    }

    public void SetIsPlayerSwap()
    {
        _isPlayerSwap = true;

        //トップビューかカメラとプレイヤーが逆位置の時、もしくはプレイヤーがカメラに写っていないはリセットする
        if (_isTop || _isFront != _PlayerObject.GetComponent<PlayerControl>().GetIsFront() || CheckPlayerInDisplay()) 
            ResetCamera();

        StartCoroutine(PlayerIsSwapCameraWork());

    }

    public void SetIsPlayerTurnOver()
    {
        _isPlayerTurnOver = true;
        ResetCamera();

        StartCoroutine(PlayerIsTurnOverCameraWork());
    }


    // 注視点位置をパネルから決定される範囲内に補正する関数
    private void CameraLookAtModifyPanelRange()
    {        
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
    }


    private IEnumerator GameStartCameraWork()
    {
        // 全トップビューから一番プレイヤーに近いものをスタートカメラワークの最初の位置にする
        int topViewObjectNumber = NearestTopViewNumber(null);

        Vector3 currentPosition = _TopViewObject[topViewObjectNumber].transform.position;
        Quaternion currentRotation = _TopViewObject[topViewObjectNumber].transform.rotation;

        yield return null;
        _CameraLookAt = _PlayerObject.transform.position;
        transform.position = _CameraLookAt - transform.forward * _ToPlayerOffset;

        Vector3 targetPosition = transform.position;
        Quaternion targetRotation = transform.rotation;

        float passedTime = 0f;

        while (_isGameStartCameraWork)
        {
            //ゲームスタート時のカメラワーク処理
            // コルーチン開始フレームから何秒経過したか
            passedTime += Time.deltaTime;
            // 指定された時間に対して経過した時間の割合
            float ratio = passedTime / _GameStartCameraWorkTime;

            // Lerpは割合を示す引数は0 ~ 1の間に補正されるので1より大きくても問題なし
            transform.SetPositionAndRotation(Vector3.Lerp(currentPosition, targetPosition, ratio), Quaternion.Lerp(currentRotation, targetRotation, ratio));

            if (ratio > 1.0f)
            {
                _isGameStartCameraWork = false;
                break;
            }
            yield return null;//こうすることで次のフレームにこれ以降の処理（この場合は先頭）を行うようになる
        }

        _isFront = _PlayerObject.GetComponent<PlayerControl>().GetIsFront();

        // マネージャーのスタートエネミームービーを呼ぶ
        // この書き方はエネミーが2体以上の時に対応できない
        foreach (GameObject enemy in _GameManager.GetComponent<GameManagerScript>().GetEnemys())
            _GameManager.gameObject.GetComponent<GameManagerScript>().StartEnemyMovie(enemy.GetComponent<EnemyControl>().GetIsFront());
    }
    
    private IEnumerator PlayerIsMoveCameraWork()
    {
        while (_isPlayerMove)
        {
            FollowPlayer();
            yield return null;
        }
    }

    private IEnumerator PlayerIsSwapCameraWork()
    {
        GameObject targetBlock = _GameManager.GetComponent<GameManagerScript>().GetBlock(_PlayerObject.GetComponent<PlayerControl>().GetLocalPosition());

        while (_isPlayerSwap)
        {
            FollowPlayer();

            if (!targetBlock.GetComponent<BlockControl>().GetisSwapAnim())
                _isPlayerSwap = false;

            yield return null;
        }
    }

    private IEnumerator PlayerIsTurnOverCameraWork()
    {
        GameObject targetBlock = _GameManager.GetComponent<GameManagerScript>().GetBlock(_PlayerObject.GetComponent<PlayerControl>().GetLocalPosition());
        float animTime = _GameManager.GetComponent<GameManagerScript>().GetBlock(_PlayerObject.GetComponent<PlayerControl>().GetLocalPosition()).GetComponent<BlockControl>().GetTuenOverAnimTime(); ;

        // 最初の位置
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        // 1つ目の経由位置
        Vector3 passingPosition1 = _CameraLookAt - transform.forward * _ToPlayerOffsetMin;

        // ルックアットの位置を反転
        _CameraLookAt = new Vector3(_CameraLookAt.x, -_CameraLookAt.y, _CameraLookAt.z);

        // 2つ目の経由位置
        Vector3 passingPosition2 = _CameraLookAt - transform.forward * _ToPlayerOffsetMin;

        // 最終的な位置
        Vector3 targetPosition = new Vector3(transform.position.x, -transform.position.y, -transform.position.z);
        transform.position = targetPosition; //位置とLookAtを変更した値を回転角度に入れる
        transform.LookAt(_CameraLookAt);
        Quaternion targetRotation = transform.rotation;

        // 変更した角度を戻す
        transform.rotation = startRotation;

        while (_isPlayerTurnOver)
        {
            float passedTime = targetBlock.GetComponent<BlockControl>().GetAnimPassedTime();

            float ratio = passedTime / animTime;

            // 半分の時間で条件を分ける
            if (ratio < 0.5f)
            {
                ratio *= 2;

                Vector3.Lerp(startPosition, passingPosition1, ratio);

                // elseの処理に合わせて書くためにコメントアウト
                //transform.rotation = startRotation;
            }
            else
            {
                ratio = (1 - ratio) * 2;

                Vector3.Lerp(passingPosition2, targetPosition, ratio);

                // 最初の一回だけとかにしたい
                transform.rotation = targetRotation;
            }

            yield return null;
        }

    }

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

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f, 0.4f);

        Gizmos.DrawSphere(_CameraLookAt, 0.2f);
    }
}
