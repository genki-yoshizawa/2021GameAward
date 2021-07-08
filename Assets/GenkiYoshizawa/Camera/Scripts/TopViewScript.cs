using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewScript : MonoBehaviour
{
    [Header("部分俯瞰の判定を設置位置からどれだけずらすか")]
    [SerializeField] private Vector2 _TopViewColliderCenerOffset = new Vector2(0f, 0f);
    [Header("部分俯瞰の判定の範囲（この範囲内なら(重なっていなければ)確実にこの位置に来る）(すべての範囲外、もしくは範囲が重なっているなら一番近いところ行く)")]
    [SerializeField] private float _TopViewColliderRange = 1f;
    [Header("デバッグカラー")]
    [SerializeField] private Color _DebugColor = new Color(1f, 0f, 0f, 0.5f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _DebugColor;

        Gizmos.DrawCube(transform.position + new Vector3(_TopViewColliderCenerOffset.x, -5f, _TopViewColliderCenerOffset.y), new Vector3(2 * _TopViewColliderRange, 10f, 2 * _TopViewColliderRange));

    }

    public Vector3 GetTopViewColliderCenter()
    {
        return transform.position + new Vector3(_TopViewColliderCenerOffset.x, 0.0f, _TopViewColliderCenerOffset.y);
    }

    public float GetTopViewColliderRange()
    {
        return _TopViewColliderRange;
    }
}
