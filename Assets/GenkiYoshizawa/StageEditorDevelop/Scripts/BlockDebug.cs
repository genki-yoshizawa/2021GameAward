using UnityEngine;

// Block(表裏合わせたパネルを便宜的にBlockとする)のデバッグ表示を行うクラス
// オブジェクトのGetもここから取得する
public class BlockDebug : MonoBehaviour
{
    [Header("ワイヤーカラーの設定")]
    [SerializeField] private Color _WireColor = new Color(1f, 0, 0, 0.5f);

    private void OnDrawGizmos()
    {
        Gizmos.color = _WireColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

}
