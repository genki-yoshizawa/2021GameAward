using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIconScript : MonoBehaviour
{
    [Header("縮小を始める距離")]
    [SerializeField] private float _MinDistance = 0.0f;
    [Header("サイズが0になる距離(これを遠くに設定することで大きさの変位量も調整できる)")]
    [SerializeField] private float _MaxDistance = 20.0f;
    [Header("トップビューの時のスケール")]
    [SerializeField] private float _WhenTopViewScale = 0.5f;
    [Header("エネミーアイコンアニメーションの周期(秒)")]
    [SerializeField] private float _EnemyIconAnimationCycle = 1.0f;

    private GameObject _CharacterIconObject1 = null;
    private GameObject _CharacterIconObject2 = null;
    private GameObject _CharacterArrowObject = null;

    private Image _CharacterIconImage1 = null;
    private Image _CharacterIconImage2 = null;
    private Image _CharacterArrowImage = null;

    private GameObject _GameManager = null;
    private List<GameObject> _EnemysObject = null;
    private GameObject _CameraObject = null;
    
    // 画面縁からどれだけ離れるか
    private float _FrameDistance = 0f;

    // 最初のローカルスケールの値
    private Vector3 _DefaultLocalScale;
    
    [Header("Assetから適用する表エネミー(enemyA)アイコンテクスチャを入れてください")]
    [SerializeField] private Sprite _FrontEnemyIconSprite;
    [Header("Assetから適用する表エネミー矢印(enemyA)テクスチャを入れてください")]
    [SerializeField] private Sprite _FrontEnemyArrowSprite;

    [Header("Assetから適用する裏エネミールーペアイコン(enemyB)テクスチャを入れてください")]
    [SerializeField] private Sprite _BackEnemyLoupeIconSprite;
    [Header("Assetから適用する裏エネミー矢印ルーペアイコン(enemyB)テクスチャを入れてください")]
    [SerializeField] private Sprite _BackEnemyLoupeArrowSprite;

    [Header("Assetから適用する1つ目の裏エネミーピンアイコン(enemyC1)テクスチャを入れてください")]
    [SerializeField] private Sprite _BackEnemyPinIconSprite1;
    [Header("Assetから適用する裏エネミー矢印ピンアイコン(enemyC)テクスチャを入れてください")]
    [SerializeField] private Sprite _BackEnemyPinArrowSprite;

    // Start is called before the first frame update
    void Start()
    {
        float canvasWidth = transform.parent.parent.GetComponent<RectTransform>().sizeDelta.x;

        float modifyScale = canvasWidth / Screen.width;

        _CharacterIconObject1 = transform.GetChild(0).gameObject;
        _CharacterIconObject2 = transform.GetChild(1).gameObject;
        _CharacterArrowObject = transform.GetChild(2).gameObject;

        _CharacterIconImage1 = _CharacterIconObject1.GetComponent<Image>();
        _CharacterIconImage2 = _CharacterIconObject2.GetComponent<Image>();
        _CharacterArrowImage = _CharacterArrowObject.GetComponent<Image>();

        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _EnemysObject = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
        _CameraObject = _GameManager.GetComponent<GameManagerScript>().GetCamera();

        _DefaultLocalScale = transform.GetComponent<RectTransform>().localScale;

        // アイコンのPivot位置を調整する
        RectTransform characterRect = _CharacterIconObject1.GetComponent<RectTransform>();
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
        // エネミーとカメラが同じ面のときはエネミー位置を参照し
        // 逆面のときはエネミーの乗るパネル位置を参照する

        // エネミーが複数になった場合アイコン表示を複数用意してないため上手く行かないことになっている
        foreach (GameObject enemyObject in _EnemysObject)
        {
            if (enemyObject == null)
                return;

            // 同じ面かどうかのbool
            bool isSameSide = enemyObject.GetComponent<EnemyControl>().GetIsFront() == _CameraObject.GetComponent<MainCameraScript>().GetIsFront();

            Vector3 objectPosition = new Vector3();

            if (isSameSide)
            {
                objectPosition = enemyObject.transform.position;
                if (CheckPositionInDisplay(objectPosition))
                    EnemyInDisplay();
                else
                    EnemyOutDisplay(enemyObject, objectPosition);
            }
            else
            {
                objectPosition = _GameManager.GetComponent<GameManagerScript>().GetBlock(enemyObject.GetComponent<EnemyControl>().GetLocalPosition()).transform.position;
                if (CheckPositionInDisplay(objectPosition))
                    PanelOnEnemyInDisplay(enemyObject, objectPosition);
                else
                    PanelOnEnemyOutDisplay(enemyObject, objectPosition);
            }
        }
    }
    // エネミーが画面内にいる時に呼び出す関数
    private void EnemyInDisplay()
    {
        _CharacterIconImage1.enabled = false;
        _CharacterIconImage2.enabled = false;
        _CharacterArrowImage.enabled = false;
    }

    // エネミーが画面外にいる時に呼び出す関数
    private void EnemyOutDisplay(GameObject enemy, Vector3 position)
    {
        PointToPositionLoupe(enemy, position);
    }

    // エネミーの乗るパネルが画面内にいる時に呼び出す関数
    private void PanelOnEnemyInDisplay(GameObject enemy, Vector3 position)
    {
        _CharacterIconImage1.enabled = true;
        _CharacterArrowImage.enabled = true;

        if (enemy.GetComponent<EnemyControl>().GetIsFront())
        {
            _CharacterIconImage1.sprite = _FrontEnemyIconSprite;
            _CharacterArrowImage.sprite = _FrontEnemyArrowSprite;
        }
        else
        {
            _CharacterIconImage2.enabled = true;

            _CharacterIconImage1.sprite = _BackEnemyPinIconSprite1;
            _CharacterArrowImage.sprite = _BackEnemyPinArrowSprite;
            StartCoroutine(EnemyPinIconAnimation(enemy));
        }

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
                _CharacterIconImage1.enabled = false;
                _CharacterIconImage2.enabled = false;
                _CharacterArrowImage.enabled = false;
                return;
            }
        }
        newLocalScale = _DefaultLocalScale * scale;
        transform.GetComponent<RectTransform>().localScale = newLocalScale;

        // 回転をデフォルトに設定
        _CharacterArrowObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0f, 0f, 0f);
        // iconPositionを指すように位置を調整
        float newPositionY = iconPosition.y + (_CharacterArrowObject.GetComponent<RectTransform>().sizeDelta.y + _CharacterIconObject1.GetComponent<RectTransform>().sizeDelta.y * 0.5f) * modifyScale * newLocalScale.y;
        transform.GetComponent<RectTransform>().position = new Vector3(iconPosition.x, newPositionY, 0.0f);
    }

    // エネミーの乗るパネルが画面外にいる時に呼び出す関数
    private void PanelOnEnemyOutDisplay(GameObject enemy, Vector3 position)
    {
        PointToPositionLoupe(enemy, position);
    }

    // エネミーのいる位置をルーペで指す関数
    private void PointToPositionLoupe(GameObject enemy, Vector3 position)
    {
        _CharacterIconImage1.enabled = true;
        _CharacterArrowImage.enabled = true;

        if (enemy.GetComponent<EnemyControl>().GetIsFront())
        {
            _CharacterIconImage1.sprite = _FrontEnemyIconSprite;
            _CharacterArrowImage.sprite = _FrontEnemyArrowSprite;
        }
        else
        {
            _CharacterIconImage1.sprite = _BackEnemyLoupeIconSprite;
            _CharacterArrowImage.sprite = _BackEnemyLoupeArrowSprite;
        }

        // 大きさを元に戻す
        transform.GetComponent<RectTransform>().localScale = _DefaultLocalScale;

        // スクリーン座標を取得する
        Vector3 iconPosition = ConvertPositionToScreenSpaceOverlayAndDepth(position);
        // スクリーン座標中心からのベクトルを作る
        Vector2 screenCenter = new Vector2(Screen.width, Screen.height);
        screenCenter *= 0.5f;
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

        if (angle >= -135.0f && angle < -45.0f)
        {//左
            angle += 135.0f;
            // 90度が境目なのでマジックナンバー
            newPosition.y = Screen.height - _FrameDistance - (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = _FrameDistance;

        }
        else if (angle >= -45.0f && angle < 45.0f)
        {//下
            angle += 45f;
            // 90度が境目なのでマジックナンバー
            newPosition.x = _FrameDistance + (angle / 90.0f) * (Screen.width - 2 * _FrameDistance);
            newPosition.y = _FrameDistance;
        }
        else if (angle >= 45.0f && angle < 135.0f)
        {//右
            angle -= 45.0f;
            // 90度が境目なのでマジックナンバー
            newPosition.y = _FrameDistance + (angle / 90.0f) * (Screen.height - 2 * _FrameDistance);
            newPosition.x = Screen.width - _FrameDistance;
        }
        else
        {//上
            if (angle < 0f)
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

    // エネミーピンアイコンのアニメーションコルーチン
    IEnumerator EnemyPinIconAnimation(GameObject enemy)
    {
        float passedTime = 0.0f;

        while (true)
        {
            Vector3 objectPosition = _GameManager.GetComponent<GameManagerScript>().GetBlock(enemy.GetComponent<EnemyControl>().GetLocalPosition()).transform.position;

            // エネミーが表、もしくはエネミーとカメラが同じ面、もしくはエネミーの乗るパネルがカメラ外に来たらアニメーションコルーチン終了
            // CheckPositionInDisplayをここに置くのは処理的に無駄？
            if (enemy.GetComponent<EnemyControl>().GetIsFront() ||
                !_CameraObject.GetComponent<MainCameraScript>().GetIsFront() ||
                !CheckPositionInDisplay(objectPosition))
            {
                _CharacterIconImage2.enabled = false;
                break;
            }
            // コルーチン開始フレームから何秒経過したか
            passedTime += Time.deltaTime;

            if (passedTime > _EnemyIconAnimationCycle)
                passedTime -= _EnemyIconAnimationCycle;

            // 指定された時間に対して経過した時間の割合
            float ratio = passedTime / _EnemyIconAnimationCycle;

            Color newColor1 = _CharacterIconImage1.color;
            Color newColor2 = _CharacterIconImage2.color;

            newColor1.a = Mathf.Sin(Mathf.Abs(0.5f - ratio) * Mathf.PI);
            newColor2.a = Mathf.Sin(ratio * Mathf.PI);

            _CharacterIconImage1.color = newColor1;
            _CharacterIconImage2.color = newColor2;

            yield return null;
        }
    }
}
