using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ゲームを終了する判定を行うものがGameSceneControllerオブジェクトを取得してこの関数を呼び出す
    public void ChangeSceneResult()
    {
        // イベントに登録
        SceneManager.sceneLoaded += InputScore;

        // シーン切り替え
        SceneManager.LoadScene("SubSceneTest");

    }

    private void InputScore(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        SubSceneManager subSceneManagerScript = GameObject.FindWithTag("Manager").GetComponent<SubSceneManager>();

        // データを渡す処理

        // イベントから削除
        SceneManager.sceneLoaded -= InputScore;
    }
}
