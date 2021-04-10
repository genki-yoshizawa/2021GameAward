using UnityEditor;
using UnityEngine;

public class ExampleClass
{
    [InitializeOnLoadMethod]
    static void Example()
    {
        SceneView.duringSceneGui += OnGUI;
    }

    static void OnGUI(SceneView sceneView)
    {
        var rect = new Rect(8, 24, 80, 0);

        GUI.WindowFunction func = id =>
        {
            if (GUILayout.Button("編集を開始する（未実装）"))
            {
                // TODO
            }
            else if (GUILayout.Button("編集開始前に戻す（未実装）"))
            {
                // TODO
            }
        };
        GUILayout.Window(1, rect, func, string.Empty);
    }
}