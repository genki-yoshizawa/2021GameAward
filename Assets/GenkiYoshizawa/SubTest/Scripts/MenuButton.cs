using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    [Header("HierarchyのSubSceneManagerを入れてください")]
    [SerializeField] private GameObject _SubSceneManagerObject;
    private SubSceneManager _SubSceneManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        _SubSceneManagerScript = _SubSceneManagerObject.GetComponent<SubSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // ボタンが押された場合、今回呼び出される関数
    public void OnClick()
    {
        _SubSceneManagerScript.UninitMenuScene();
    }
}
