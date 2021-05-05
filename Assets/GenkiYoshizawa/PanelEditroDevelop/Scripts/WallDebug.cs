using UnityEngine;

public class WallDebug : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        WallConfig config = transform.GetComponent<WallConfig>();
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Vector3 direction = new Vector3(config.GetDirection().x, 0.0f, config.GetDirection().y);
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}
