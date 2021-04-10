using UnityEngine;

[SelectionBase]
public class ObjectSnap : MonoBehaviour
{
    // GameConfig.GridSize を1目盛りとした場合の、グリッド座標
    [SerializeField] public Vector2Int gridPos = Vector2Int.zero;

    /// <summary>
    /// グリッド移動量を指定して移動する
    /// </summary>
    public void Move(Vector2Int vec2)
    {
        gridPos += vec2;
        transform.position = GetGlobalPosition(gridPos);
    }

    /// <summary>
    /// グリッド座標をGlobal座標に変換する
    /// </summary>
    public static Vector3 GetGlobalPosition(Vector2Int gridPos)
    {
        float gridSize = Resources.Load<GameConfig>("ScriptableObject/GameConfig").GridSize;
        return new Vector3(
            gridPos.x * gridSize,
            gridPos.y * gridSize,
            0);
    }
}
