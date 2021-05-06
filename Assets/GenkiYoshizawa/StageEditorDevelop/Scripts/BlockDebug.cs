using UnityEngine;

// Block(�\�����킹���p�l����֋X�I��Block�Ƃ���)�̃f�o�b�O�\�����s���N���X
// �I�u�W�F�N�g��Get����������擾����
public class BlockDebug : MonoBehaviour
{
    [Header("���C���[�J���[�̐ݒ�")]
    [SerializeField] private Color _WireColor = new Color(1f, 0, 0, 0.5f);

    private void OnDrawGizmos()
    {
        Gizmos.color = _WireColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

}
