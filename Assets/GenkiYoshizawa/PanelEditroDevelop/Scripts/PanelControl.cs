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
}
