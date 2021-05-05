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
    private bool CheckPanelRotate(bool isFront)
    {
        if(isFront)
            return transform.GetChild(0).GetComponent<PanelConfig>().GetCanRotate();
        else
            return transform.GetChild(1).GetComponent<PanelConfig>().GetCanRotate();
    }
    // パネルがひっくり返しパネルかのチェック関数
    public bool CheckPanelTurnOver(bool isFront)
    {
        if (isFront)
            return transform.GetChild(0).GetComponent<PanelConfig>().GetCanTurnOver();
        else
            return transform.GetChild(1).GetComponent<PanelConfig>().GetCanTurnOver();

    }
    // パネルが入れ替えパネルかのチェック関数
    public bool CheckPanelSwap(bool isFront)
    {
        if (isFront)
        {
            // canSwapがfalseもしくはIndexが0のときfalse
            if (!transform.GetChild(0).GetComponent<PanelConfig>().GetCanSwap() || transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex() == 0)
                return false;

            // 全ブロックの取得
            GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
            foreach (GameObject[] blockXLine in blocks)
            {
                foreach (GameObject blockZLine in blockXLine)
                {
                    if(blockZLine.transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex() == transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex())
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            // canSwapがfalseもしくはIndexが0のときfalse
            if (!transform.GetChild(1).GetComponent<PanelConfig>().GetCanSwap() || transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex() == 0)
                return false;

            // 全ブロックの取得
            GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
            foreach (GameObject[] blockXLine in blocks)
            {
                foreach (GameObject blockZLine in blockXLine)
                {
                    if (blockZLine.transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex() == transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    // パネルが移動可能パネルかのチェック関数
    public bool CheckPanelMove(/*bool isFront, Vector2Int objectPosition, Vector2 direction, int lv = 0*/)
    {
        //GameObject playerBlock = null;
        //if (objectPosition != _BlockLocalPosition)
        //{// 調べるブロックにプレイヤーがいなければ
        //    // プレイヤーのいるブロックの取得
        //    playerBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlocks()[objectPosition.x][objectPosition.y];
        //}

        //// プレイヤーのいないブロックを調べて通ることができればプレイヤーのいるブロックを調べる
        //// 可読性悪い？
        //if (isFront)
        //{
        //    if (transform.GetChild(0).GetComponent<PanelConfig>().CheckEnter(objectPosition, _BlockLocalPosition, direction, lv))
        //    {
        //        if (playerBlock == null)
        //            return playerBlock.transform.GetChild(0).GetComponent<PanelConfig>().CheckEnter(objectPosition, objectPosition, direction, lv);
        //        else return true;
        //    }
        //}
        //else
        //{
        //    if (transform.GetChild(1).GetComponent<PanelConfig>().CheckEnter(objectPosition, _BlockLocalPosition, direction, lv))
        //    {
        //        if (playerBlock == null)
        //            return playerBlock.transform.GetChild(1).GetComponent<PanelConfig>().CheckEnter(objectPosition, objectPosition, direction, lv);
        //        else return true;
        //    }
        //}

        //return false;
        return true;
    }

    public Vector2Int GetBlockLocalPosition() { return _BlockLocalPosition; }
    public void SetBlockLocalPosition(Vector2Int localPos) { _BlockLocalPosition = localPos; }

    //public GameObject GetFrontPanel() { return transform.GetChild(0); }
    //public GameObject GetBackPanel() { return transform.GetChild(1); }
}