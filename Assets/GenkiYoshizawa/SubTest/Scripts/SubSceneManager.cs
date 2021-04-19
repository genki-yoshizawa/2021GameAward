using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubSceneManager : MonoBehaviour
{
    [Header("HierarchyのTitleSceneを入れてください")]
    [SerializeField] private GameObject _TitleSceneObject;
    [Header("HierarchyのMenuSceneを入れてください")]
    [SerializeField] private GameObject _MenuSceneObject;
    [Header("HierarchyのResultSceneを入れてください")]
    [SerializeField] private GameObject _ResultSceneObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 今回はフェードイン・アウトによるシーン遷移がないため
    // 便宜的なシーン遷移を行う

    // タイトルシーンの初期化
    public void InitTitleScene()
    {

    }

    // タイトルシーンの終了
    public void UninitTitleScene()
    {
        _TitleSceneObject.SetActive(false);

        InitMenuScene();
    }

    // メニューシーンの初期化
    public void InitMenuScene()
    {
        _MenuSceneObject.SetActive(true);
    }

    // メニューシーンの終了
    public void UninitMenuScene()
    {
        //_MenuSceneObject.SetActive(false);

        // イベントに登録
        SceneManager.sceneLoaded += StageSelect;

        // シーン切り替え
        SceneManager.LoadScene("GameTestScene");

    }

    // リザルトシーンの初期化
    public void InitResultScene()
    {
        _ResultSceneObject.SetActive(true);
    }

    // リザルトシーンの終了
    public void UninitResultScene()
    {
        _ResultSceneObject.SetActive(false);

        InitMenuScene();
    }


    private void StageSelect(Scene next, LoadSceneMode mode)
    {
        // シーン切り替え後のスクリプトを取得
        GameSceneController gameSceneControllerScript = GameObject.FindWithTag("Manager").GetComponent<GameSceneController>();

        // データを渡す処理

        // イベントから削除
        SceneManager.sceneLoaded -= StageSelect;
    }
}
