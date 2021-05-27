using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLoadManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = GameObject.Find("StageManager");
        if (obj == null)
        {
            // �v���n�u���擾
            GameObject prefab = (GameObject)Resources.Load("StageManager/StageManager");
            // �v���n�u����C���X�^���X�𐶐�
            Instantiate(prefab, new Vector3(0,0,1), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
