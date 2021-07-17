using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaussShaderGraphScript : MonoBehaviour
{
    private RawImage _Image;       //オブジェクトにアタッチされるImageコンポーネント
    private float _PassedTime;  //Start()が実行されてからの経過時間

    private bool _GaussPlay = false;

    private float _GaussTime = 0.5f;

    


    // Start is called before the first frame update
    void Start()
    {
        // イメージコンポーネントの取得
        _Image = GetComponent<RawImage>();

        // 経過時間の初期化
        _PassedTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GaussPlay();
    }

    void GaussPlay()
    {
        if (!_GaussPlay) return;


        // 経過時間を増やす
        _PassedTime += Time.deltaTime;

        // ShaderGraph内で使われているぼかしの強さを決める変数に、経過時間を代入
        _Image.material.SetFloat("ScaleValue", _PassedTime);

        if (_PassedTime >= _GaussTime)
            _GaussPlay = false;

        return;
    }

    public void GaussStart()
    {
        _GaussPlay = true;
    }

    public void GaussReset()
    {
        _GaussPlay = false;
        _Image.material.SetFloat("ScaleValue", 0.0f);
    }
}
