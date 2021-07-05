using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconScrip : MonoBehaviour
{
    [Header("縮小を始める距離")]
    [SerializeField] private float _MinDistance = 0.0f;
    [Header("サイズが0になる距離(これを遠くに設定することで大きさの変位量も調整できる)")]
    [SerializeField] private float _MaxDistance = 20.0f;
    [Header("トップビューの時のスケール")]
    [SerializeField] private float _WhenTopViewScale = 0.5f;

    private GameObject _CharacterIconObject = null;
    private GameObject _CharacterArrowObject = null;

    private GameObject _GameManager = null;
    private GameObject _PlayerObject = null;
    private GameObject _CameraObject = null;

    // 画面縁からどれだけ離れるか
    private float _FrameDistance = 0f;

    // 最初のローカルスケールの値
    private Vector3 _DefaultLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = canvasWidth / Screen.width;

        _CharacterIconObject = transform.GetChild(0).gameObject;
        _CharacterArrowObject = transform.GetChild(1).gameObject;

        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _PlayerObject = _GameManager.GetComponent<GameManagerScript>().GetPlayer();
        _CameraObject = _GameManager.GetComponent<GameManagerScript>().GetCamera();

        _DefaultLocalScale = transform.GetComponent<RectTransform>().localScale;

        // アイコンのPivot位置を調整する
        RectTransform characterRect = _CharacterIconObject.GetComponent<RectTransform>();
        RectTransform arrowRect = _CharacterArrowObject.GetComponent<RectTransform>();

        // 画面縁からの距離を計算し決定する
        // 矢印のheight+キャラクターアイコンと矢印が離れている(重なっている)距離
        arrowRect = SetPivotKeepPosition(arrowRect, new Vector2(0.5f, 0.5f));
        //画面縁から離す距離 = 矢印アイコンheight　+ キャラアイコンの半分 + (アイコンの離れている(重なっている)距離  = 中心位置の距離 - それぞれの半分のサイズの合計)
        //_FrameDistance = arrowRect.sizeDelta.y * _DefaultLocalScale.y / modifyScale + characterRect.sizeDelta.y * _DefaultLocalScale.y / modifyScale + (characterRect.position.y - arrowRect.position.y) / modifyScale - (arrowRect.sizeDelta.y * 0.5f / modifyScale + characterRect.sizeDelta.y * 0.5f / modifyScale) * _DefaultLocalScale.y;
        _FrameDistance = ((arrowRect.sizeDelta.y + characterRect.sizeDelta.y * 0.5f) * _DefaultLocalScale.y * 0.5f + (characterRect.position.y - arrowRect.position.y)) / modifyScale;
       
        // 新しいPivot位置(矢印のPivotはキャラクターアイコンの中心)を計算する
        arrowRect = SetPivotKeepPosition(arrowRect, new Vector2(0f, 0f));

        Vector3 offsetArrowToCharacter = (characterRect.position - arrowRect.position) * modifyScale;

        arrowRect = SetPivotKeepPosition(arrowRect, new Vector2(offsetArrowToCharacter.x / arrowRect.sizeDelta.x, offsetArrowToCharacter.y / arrowRect.sizeDelta.y));
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーとカメラが同じ面のときはプレイヤー位置を参照し
        // 逆面のときはプレイヤーの乗るパネル位置を参照する

        // 同じ面かどうかのbool
        bool isSameSide = _PlayerObject.GetComponent<PlayerControl>().GetIsFront() == _CameraObject.GetComponent<MainCameraScript>().GetIsFront();

        Vector3 objectPosition = new Vector3();

        if (isSameSide)
        {

            objectPosition = _PlayerObject.transform.position;
            if (CheckPositionInDisplay(objectPosition))
                PlayerInDisplay();
            else
                PlayerOutDisplay(objectPosition);

        }
        else
        {
            objectPosition = _GameManager.GetComponent<GameManagerScript>().GetBlock(_PlayerObject.GetComponent<PlayerControl>().GetLocalPosition()).transform.position;
            if (CheckPositionInDisplay(objectPosition))
                PanelOnPlayerInDisplay(objectPosition);
            else
                PanelOnPlayerOutDisplay(objectPosition);
        }
    }

    // プレイヤーが画面内にいる時に呼び出す関数
    private void PlayerInDisplay()
    {
        _CharacterIconObject.GetComponent<Image>().enabled = false;
        _CharacterArrowObject.GetComponent<Image>().enabled = false;
    }

    // プレイヤーが画面外にいる時に呼び出す関数
    private void PlayerOutDisplay(Vector3 position)
    {
        PointToPositionLoupe(position);
    }

    // プレイヤーの乗るパネルが画面内にいる時に呼び出す関数
    private void PanelOnPlayerInDisplay(Vector3 position)
    {
        _CharacterIconObject.GetComponent<Image>().enabled = true;
        _CharacterArrowObject.GetComponent<Image>().enabled = true;

        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = Screen.width / canvasWidth;

        // スクリーン座標を取得する
        Vector3 iconPosition = ConvertPositionToScreenSpaceOverlayAndDepth(position);

        Vector3 newLocalScale = new Vector3();
        float scale = 1.0f;

        if (_CameraObject.GetComponent<MainCameraScript>().GetIsTop()) //トップビューの時
            scale = _WhenTopViewScale;
        else
        {
            // depthに応じてスケール変換
            // 0~1の値
            if (iconPosition.z > _MinDistance)
                scale = (iconPosition.z - _MaxDistance) / (_MinDistance - _MaxDistance);
            else
                scale = 1.0f;
            if (scale <= 0f)
            {
                _CharacterIconObject.GetComponent<Image>().enabled = false;
                _CharacterArrowObject.GetComponent<Image>().enabled = false;
                return;
            }
        }
        newLocalScale = _DefaultLocalScale * scale;
        transform.GetComponent<RectTransform>().localScale = newLocalScale;

        // 回転をデフォルトに設定
        _CharacterArrowObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
        // iconPositionを指すように位置を調整
        float newPositionY = iconPosition.y + (_CharacterArrowObject.GetComponent<RectTransform>().sizeDelta.y + _CharacterIconObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f) * modifyScale * newLocalScale.y;
        transform.GetComponent<RectTransform>().position = new Vector3(iconPosition.x, newPositionY, 0.0f);
    }

    // プレイヤーの乗るパネルが画面外にいる時に呼び出す関数
    private void PanelOnPlayerOutDisplay(Vector3 position)
    {
        PointToPositionLoupe(position);
    }

    // プレイヤーのいる位置をルーペで指す関数
    private void PointToPositionLoupe(Vector3 position)
    {
        _CharacterIconObject.GetComponent<Image>().enabled = true;
        _CharacterArrowObject.GetComponent<Image>().enabled = true;

        // 大きさを元に戻す
        transform.GetComponent<RectTransform>().localScale = _DefaultLocalScale;

        // スクリーン座標を取得する
        Vector3 iconPosition = ConvertPositionToScreenSpaceOverlayAndDepth(position);
        // スクリーン座標中心からのベクトルを作る
        Vector2 screenCenter = new Vector2(Screen.width, Screen.height);
        screenCenter*= 0.5f;
        iconPosition = iconPosition - new Vector3(screenCenter.x, screenCenter.y, 0.0f);

        // 下向きベクトルがデフォルトなので、そのベクトルとの角度を調べる
        Vector2 down = new Vector2(0.0f, -1.0f);
        float angle = Vector2.SignedAngle(down, iconPosition);

        // カメラの正面方向の逆にいた場合角度が狂うので反転させる
        // カメラの正面ベクトルとpositionへのベクトルの内積が負なら逆にいる
        if (Vector3.Dot(_CameraObject.transform.forward, position - _CameraObject.transform.position) < 0f)
            angle += 180 * (angle < 0 ? 1 : -1);

        // 角度を適用し、矢印をプレイヤーのいる位置に向ける
        _CharacterArrowObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, angle);

        Vector3 newPosition = new Vector3();

        if(angle >= -135.0f && angle < -45.0f)
        {//左
            angle += 135.0f;
            // 90度が境目なのでマジックナンバー
            newPosition.y = Screen.height - _FrameDistance - (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = _FrameDistance;

        }
        else if(angle >= -45.0f && angle < 45.0f)
        {//下
            angle += 45f;
            // 90度が境目なのでマジックナンバー
            newPosition.x = _FrameDistance + (angle / 90.0f) * (Screen.width - 2 * _FrameDistance);
            newPosition.y = _FrameDistance;
        }
        else if(angle >= 45.0f && angle < 135.0f)
        {//右
            angle -= 45.0f;
            // 90度が境目なのでマジックナンバー
            newPosition.y = _FrameDistance + (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = Screen.width - _FrameDistance;
        }
        else
        {//上
            if(angle < 0f)            
                angle = 360f - Mathf.Abs(angle);
            
            angle -= 135f;
            // 90度が境目なのでマジックナンバー
            newPosition.x = Screen.width - _FrameDistance - (angle / 90.0f) * (Screen.width - 2 * _FrameDistance);
            newPosition.y = Screen.height - _FrameDistance;
        }

        newPosition.z = 0f;
        transform.GetComponent<RectTransform>().position = newPosition;
    }

    // 引数で与えられた座標が描画範囲か調べる関数
    private bool CheckPositionInDisplay(Vector3 position)
    {
        Plane[] plane = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        for (int i = 0; i < 6; i++)
        {
            if (!plane[i].GetSide(position))
                return false;
        }

        return true;
    }

    // スクリーン座標にDepth込みで変換する
    private Vector3 ConvertPositionToScreenSpaceOverlayAndDepth(Vector3 position)
    {
        Vector2 uiPos;
        uiPos = RectTransformUtility.WorldToScreenPoint(Camera.main, position);

        float depth = Vector3.Distance(_CameraObject.transform.position, position);

        Vector3 pos = new Vector3(uiPos.x, uiPos.y, depth);

        return pos;
    }

    // 見た目的な座標を変えずPivotを移動させる関数
    RectTransform SetPivotKeepPosition(RectTransform rect, Vector2 newPivot)
    {
        float width = rect.sizeDelta.x;
        float height = rect.sizeDelta.y;

        Vector3 oldPos = rect.position;
        Vector2 oldPivot = rect.pivot;

        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = Screen.width / canvasWidth;

        Vector2 pivotOffset = newPivot - oldPivot;
        Vector3 positionOffset = new Vector3(pivotOffset.x * width, pivotOffset.y * height, 0.0f) * modifyScale;

        Vector3 newPosition = oldPos + positionOffset;

        RectTransform newRect = rect;
        newRect.pivot = newPivot;
        newRect.position = newPosition;

        return newRect;

    }
}
