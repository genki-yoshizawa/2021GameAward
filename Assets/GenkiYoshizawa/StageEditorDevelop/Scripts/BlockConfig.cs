using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(表裏合わせたパネルを便宜的にBlockとする)の値を扱うクラス
// オブジェクトのGetもここから取得する
public class BlockConfig : MonoBehaviour
{
    [Header("表パネルの位置(ローカル座標)(プランナーは弄らなくてもいい)")]
    [SerializeField] private Vector3 _FrontPanelLocalPosition = new Vector3(0f, 0f, 0f);
    [Header("裏パネルの位置(ローカル座標)(プランナーは弄らなくてもいい)")]
    [SerializeField] private Vector3 _BackPanelLocalPosition = new Vector3(0f, 0f, 0f);

    private Vector2Int _BlockLocalPosition;

    //[Header("表裏で共通のギミックをプレハブから設定してください")]
    //List<GameObject> _Gimmic;

    private GameObject _GameManager = null;

    private void Awake()
    {
        // 子オブジェクトが2つセットされているかチェック
        if (gameObject.transform.childCount < 2)
        {
            Debug.LogError(gameObject.name + ":子オブジェクトに設定するパネルを追加してください。(表裏の2つ)");
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // 2つ以上の子オブジェクトがセットされていれば消す
            for (int i = 2; i < transform.childCount; ++i)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).localPosition = _FrontPanelLocalPosition;
        transform.GetChild(1).localPosition = _BackPanelLocalPosition;

        _GameManager = GameObject.FindGameObjectWithTag("Manager");
    }


    // パネルが回転パネルかのチェック関数
    public bool CheckPanelRotate(bool isFront)
    {
        return transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetCanRotate();
    }
    // パネルがひっくり返しパネルかのチェック関数
    public bool CheckPanelTurnOver(bool isFront)
    {
        return transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetCanTurnOver();
    }
    // パネルが入れ替えパネルかのチェック関数
    public bool CheckPanelSwap(bool isFront)
    {
        // canSwapがfalseもしくはIndexが0のときfalse
        if (!transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetCanSwap() || transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() == 0)
            return false;

        // 全ブロックの取得
        GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
        foreach (GameObject[] blockXLine in blocks)
        {
            foreach (GameObject blockZLine in blockXLine)
            {
                if (blockZLine == gameObject || blockZLine == null)
                    continue;

                if (blockZLine.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() == transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex())
                    return true;
            }
        }
        
        return false;
    }
    // パネルが移動可能パネルかのチェック関数
    public bool CheckPanelMove(bool isFront, Vector2Int objectPosition, Vector2 direction)
    {
        GameObject objectBlock = null;
        if (objectPosition != _BlockLocalPosition)
        {// 調べるブロックにオブジェクトがいなければ
            // オブジェクトのいるブロックの取得
            objectBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlock(objectPosition);
        }

        // オブジェクトのいないブロックを調べて通ることができればオブジェクトのいるブロックを調べる
        if (transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckEnter(objectPosition, _BlockLocalPosition, direction))
        {
            if (objectBlock != null)
                return objectBlock.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckEnter(objectPosition, objectPosition, direction);
            else return true;
        }
        
        return false;
    }

    //壁のレベルをチェックする関数(2枚重なっている場合は自分の乗るパネルのみ(2枚重なっていることの判定はできない))(0は壁なし)
    public int CheckWallLevel(bool isFront, Vector2Int objectPosition, Vector2 direction)
    {
        int wallLevel = 0;

        GameObject objectBlock = null;
        if (objectPosition != _BlockLocalPosition)
        {// 調べるブロックにオブジェクトがいなければ
            // オブジェクトのいるブロックの取得
            objectBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlock(objectPosition);
        }

        if (objectBlock != null)
        {
            wallLevel = objectBlock.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckWallLevel(objectPosition, objectPosition, direction);
        }

        if (wallLevel == 0)
        {
            wallLevel = transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckWallLevel(objectPosition, _BlockLocalPosition, direction);
        }

        return wallLevel;
    }

    public void PanelAttention(bool isFront) { transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().AttentionPanel(true); }
    public void PanelRemoveAttention(bool isFront) { transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().AttentionPanel(false); }


    public Vector2Int GetBlockLocalPosition() { return _BlockLocalPosition; }
    public void SetBlockLocalPosition(Vector2Int localPos) { _BlockLocalPosition = localPos; }

    //public GameObject GetFrontPanel() { return transform.GetChild(0); }
    //public GameObject GetBackPanel() { return transform.GetChild(1); }
}