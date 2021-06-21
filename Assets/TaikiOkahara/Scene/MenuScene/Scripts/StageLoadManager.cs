using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoadManager : MonoBehaviour
{
    void Start()
    {
        GameObject obj = GameObject.Find("StageManager");
        if (obj == null)
        {
            // プレハブを取得
            GameObject prefab = (GameObject)Resources.Load("StageManager/StageManager");
            // プレハブからインスタンスを生成
            Instantiate(prefab, new Vector3(0,0,1), Quaternion.identity);
        }
    }

    void Update()
    {
        
    }
}
