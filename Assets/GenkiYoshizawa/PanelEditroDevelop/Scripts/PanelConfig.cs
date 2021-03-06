using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// パネルの値を扱うクラス
// オブジェクトのGetもここから取得する
public class PanelConfig : MonoBehaviour
{
    [Header("回転パネルかどうか")]
    [SerializeField] private bool _canRotate = false;
    [Header("ひっくり返しパネルかどうか")]
    [SerializeField] private bool _canTurnOver = false;
    [Header("入れ替えパネルかどうか")]
    [SerializeField] private bool _canSwap = false;
    [Header("入れ替えパネルの通し番号(数字が大きくなる方に入れ替えをする)")]
    [SerializeField] private int _SwapIndex = 0;
    [Header("パネル同士を対応させる番号(0だと無対応)")]
    [SerializeField] private int _PanelIndex;

    private GameObject _GameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    public bool GetCanRotate() { return _canRotate; }
    public bool GetCanTurnOver() { return _canTurnOver; }
    public bool GetCanSwap() { return _canSwap; }
    public int GetSwapIndex() { return _SwapIndex; }
    public int GetPanelIndex() { return _PanelIndex; }
    
    public bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition,Vector2 direction)
    {
        GameManagerScript gmScript = _GameManager.GetComponent<GameManagerScript>(); 

        for(int i = 0; i < transform.childCount; ++i)
        {
            // エネミーと一致したら次の子オブジェクトに移る
            List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
            bool isThrow = false;
            foreach (GameObject enemy in enemys)
                if (transform.GetChild(i).gameObject == enemy)
                {
                    isThrow = true;
                    break;
                }
            if (isThrow)
                continue;

            // プレイヤーでもなくギミックのチェックエンターも通ったら
            if (gmScript.GetPlayer() != transform.GetChild(i).gameObject &&
                !transform.GetChild(i).GetComponent<GimmicControl>().CheckEnter(objectPosition, panelPosition, direction))
                return false;
        }

        return true;
    }

    public int CheckWallLevel(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        int wallLevel = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            wallLevel = transform.GetChild(i).GetComponent<GimmicControl>().CheckWallLevel(objectPosition, panelPosition, direction);
            if (wallLevel != 0)
                break;
        }
        return wallLevel;
    }

    //上からn番目のギミック(子オブジェクト)を取得する
    //public GameObject GetGimmic(int n) { return transform.GetChild(n); }
}
