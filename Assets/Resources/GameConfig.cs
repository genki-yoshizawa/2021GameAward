using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameConfig : ScriptableObject
{
    
    public float  GridSize = 1.0f;


    //public bool UseLocalEffectResource = false;
    //public GameObject EffectResource;
    //[Range(0, 100.0f)] public float DelayTime = 0.0f;
    //public string EffectName = "";
    [MenuItem("Assets/Create/GameConfig")]

    static void CreateGameConfig()
    {
        GameConfig gameConfig = CreateInstance<GameConfig>();
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/ScriptableObject/GameConfig.asset");
        AssetDatabase.CreateAsset(gameConfig, path);
        AssetDatabase.Refresh();
    }
}
