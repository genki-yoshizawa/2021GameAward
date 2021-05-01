using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(�\�����킹���p�l����֋X�I��Block�Ƃ���)�̒l�������N���X
// �I�u�W�F�N�g��Get����������擾����
public class BlockConfig : MonoBehaviour
{
    [Header("�\�p�l���̈ʒu(���[�J�����W)")]
    [SerializeField] private Vector3 _FrontPanelLocalPosition = new Vector3(0f, 0f, 0f);
    [Header("���p�l���̈ʒu(���[�J�����W)")]
    [SerializeField] private Vector3 _BackPanelLocalPosition = new Vector3(0f, 0f, 0f);

    private Vector2Int _BlockLocalPosition;

    //[Header("�\���ŋ��ʂ̃M�~�b�N���v���n�u����ݒ肵�Ă�������")]
    //List<GameObject> _Gimmic;

    private GameObject _GameManager = null;

    private void Awake()
    {
        // �q�I�u�W�F�N�g��2�Z�b�g����Ă��邩�`�F�b�N
        if (gameObject.transform.childCount < 2)
        {
            Debug.LogError(gameObject.name + ":�q�I�u�W�F�N�g�ɐݒ肷��p�l����ǉ����Ă��������B(�\����2��)");
            //UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            // 2�ȏ�̎q�I�u�W�F�N�g���Z�b�g����Ă���Ώ���
            for (int i = 2; i < transform.childCount; ++i)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).localPosition = _FrontPanelLocalPosition;
        transform.GetChild(1).localPosition = _BackPanelLocalPosition;

        _GameManager = GameObject.FindGameObjectWithTag("Manager");
    }


    // �p�l������]�p�l�����̃`�F�b�N�֐�
    private bool CheckPanelRotate(bool isFront)
    {
        if(isFront)
            return transform.GetChild(0).GetComponent<PanelConfig>().GetCanRotate();
        else
            return transform.GetChild(1).GetComponent<PanelConfig>().GetCanRotate();
    }
    // �p�l�����Ђ�����Ԃ��p�l�����̃`�F�b�N�֐�
    public bool CheckPanelTurnOver(bool isFront)
    {
        if (isFront)
            return transform.GetChild(0).GetComponent<PanelConfig>().GetCanTurnOver();
        else
            return transform.GetChild(1).GetComponent<PanelConfig>().GetCanTurnOver();

    }
    // �p�l��������ւ��p�l�����̃`�F�b�N�֐�
    public bool CheckPanelSwap(bool isFront)
    {
        if (isFront)
        {
            // canSwap��false��������Index��0�̂Ƃ�false
            if (!transform.GetChild(0).GetComponent<PanelConfig>().GetCanSwap() || transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex() == 0)
                return false;

            // �S�u���b�N�̎擾
            GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
            foreach (GameObject[] blockXLine in blocks)
            {
                foreach (GameObject blockZLine in blockXLine)
                {
                    if(blockZLine.transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex() == transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex())
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            // canSwap��false��������Index��0�̂Ƃ�false
            if (!transform.GetChild(1).GetComponent<PanelConfig>().GetCanSwap() || transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex() == 0)
                return false;

            // �S�u���b�N�̎擾
            GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
            foreach (GameObject[] blockXLine in blocks)
            {
                foreach (GameObject blockZLine in blockXLine)
                {
                    if (blockZLine.transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex() == transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    // �p�l�����ړ��\�p�l�����̃`�F�b�N�֐�
    public bool CheckPanelMove(bool isFront)
    {
        // �ЂƂ܂�true��Ԃ�
        if (isFront)
        {
            return true;
        }
        else
        {
            return true;
        }
    }

    public Vector2Int GetBlockLocalPosition() { return _BlockLocalPosition; }
    public void SetBlockLocalPosition(Vector2Int localPos) { _BlockLocalPosition = localPos; }

    //public GameObject GetFrontPanel() { return transform.GetChild(0); }
    //public GameObject GetBackPanel() { return transform.GetChild(1); }
}