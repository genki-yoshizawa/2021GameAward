using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// パネルの操作を行うクラス
public class PanelControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    //このパネルにつくギミック（子オブジェクト）をまとめて処理する
    public void TurnEndUpdate()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<GimmicControl>().TurnEndUpdate();
        }
    }

    public int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        int breakResult = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            breakResult = transform.GetChild(i).GetComponent<GimmicControl>().BreakWall(objectPosition, panelPosition, direction, lv);
            if (breakResult != 0) // 判定できる壁が合った
                break;
        }
        return breakResult;
    }
}
