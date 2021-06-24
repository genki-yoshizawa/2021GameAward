using UnityEngine;

public class CameraPointDebug : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
        Gizmos.DrawLine(transform.position, transform.position + (gameObject.transform.forward * 0.2f));

        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Gizmos.DrawLine(transform.position, transform.position - (gameObject.transform.right * 0.2f));

        Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
        Gizmos.DrawLine(transform.position, transform.position - (gameObject.transform.up * 0.2f));

        Gizmos.color = new Color(1.0f, 0.5f, 0.0f);
        Gizmos.DrawWireCube(transform.position, transform.localScale * 0.05f);
    }
}
