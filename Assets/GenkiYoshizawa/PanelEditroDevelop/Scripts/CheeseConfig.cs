using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseConfig : MonoBehaviour
{
    [Header("チーズの影響範囲")]
    [SerializeField] private int _CheeseRange = 0;

    [Header("デバッグカラーの設定")]
    [SerializeField] private Color _DebugColor = new Color(1f, 0, 0, 0.5f);

    private Vector2Int _LocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        _LocalPosition = transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _DebugColor;
        Vector3 size = new Vector3(1.0f, 0.01f, 1.0f);

        for (int x = 0; x < 2 * _CheeseRange + 1; ++x)
        {
            for (int z = 0; z < 2 * _CheeseRange + 1; ++z)
            {
                Vector2 pos = new Vector2(x - _CheeseRange, z - _CheeseRange);
                Vector3 position = new Vector3(pos.x, 0.0f, pos.y);
                Gizmos.DrawCube(transform.position + position, size);
            }
        }
    }

    public Vector2Int GetCheeseLocalPosition() { return _LocalPosition; }
    public int GetRange() { return _CheeseRange; }
}
