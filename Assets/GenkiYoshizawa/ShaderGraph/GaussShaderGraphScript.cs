using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaussShaderGraphScript : MonoBehaviour
{
    private Image _Image;       //オブジェクトにアタッチされるImageコンポーネント
    private float _PassedTime;  //Start()が実行されてからの経過時間

    // Start is called before the first frame update
    void Start()
    {
        // イメージコンポーネントの取得
        _Image = GetComponent<Image>();

        // 経過時間の初期化
        _PassedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間を増やす
        _PassedTime += Time.deltaTime;

        // ShaderGraph内で使われているぼかしの強さを決める変数に、経過時間を代入
        _Image.material.SetFloat("ScaleValue", _PassedTime);
    }
}
