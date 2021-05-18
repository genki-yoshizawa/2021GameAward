using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(�\�����킹���p�l����֋X�I��Block�Ƃ���)�̒l�������N���X
// �I�u�W�F�N�g��Get����������擾����
public class BlockConfig : MonoBehaviour
{
    [Header("�\�p�l���̈ʒu(���[�J�����W)(�v�����i�[�͘M��Ȃ��Ă�����)")]
    [SerializeField] private Vector3 _FrontPanelLocalPosition = new Vector3(0f, 0f, 0f);
    [Header("���p�l���̈ʒu(���[�J�����W)(�v�����i�[�͘M��Ȃ��Ă�����)")]
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
    public bool CheckPanelRotate(bool isFront)
    {
        return transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetCanRotate();
    }
    // �p�l�����Ђ�����Ԃ��p�l�����̃`�F�b�N�֐�
    public bool CheckPanelTurnOver(bool isFront)
    {
        return transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetCanTurnOver();
    }
    // �p�l��������ւ��p�l�����̃`�F�b�N�֐�
    public bool CheckPanelSwap(bool isFront)
    {
        // canSwap��false��������Index��0�̂Ƃ�false
        if (!transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetCanSwap() || transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() == 0)
            return false;

        // �S�u���b�N�̎擾
        GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
        foreach (GameObject[] blockXLine in blocks)
        {
            foreach (GameObject blockZLine in blockXLine)
            {
                if (blockZLine == gameObject)
                    continue;

                if (blockZLine.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() == transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex())
                    return true;
            }
        }
        
        return false;
    }
    // �p�l�����ړ��\�p�l�����̃`�F�b�N�֐�
    public bool CheckPanelMove(bool isFront, Vector2Int objectPosition, Vector2 direction)
    {
        GameObject objectBlock = null;
        if (objectPosition != _BlockLocalPosition)
        {// ���ׂ�u���b�N�ɃI�u�W�F�N�g�����Ȃ����
            // �I�u�W�F�N�g�̂���u���b�N�̎擾
            objectBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlock(objectPosition);
        }

        // �I�u�W�F�N�g�̂��Ȃ��u���b�N�𒲂ׂĒʂ邱�Ƃ��ł���΃I�u�W�F�N�g�̂���u���b�N�𒲂ׂ�
        if (transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckEnter(objectPosition, _BlockLocalPosition, direction))
        {
            if (objectBlock != null)
                return objectBlock.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckEnter(objectPosition, objectPosition, direction);
            else return true;
        }
        
        return false;
    }

    //�ǂ̃��x�����`�F�b�N����֐�(2���d�Ȃ��Ă���ꍇ�͎����̏��p�l���̂�(2���d�Ȃ��Ă��邱�Ƃ̔���͂ł��Ȃ�))(0�͕ǂȂ�)
    public int CheckWallLevel(bool isFront, Vector2Int objectPosition, Vector2 direction)
    {
        int wallLevel = 0;

        GameObject objectBlock = null;
        if (objectPosition != _BlockLocalPosition)
        {// ���ׂ�u���b�N�ɃI�u�W�F�N�g�����Ȃ����
            // �I�u�W�F�N�g�̂���u���b�N�̎擾
            objectBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlock(objectPosition);
        }

        if (objectBlock != null)
        {
            wallLevel = objectBlock.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckWallLevel(objectPosition, objectPosition, direction);
        }

        if (wallLevel == 0)
        {
            wallLevel = transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().CheckWallLevel(objectPosition, _BlockLocalPosition, direction);
        }

        return wallLevel;
    }

    public void PanelAttention(bool isFront) { transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().AttentionPanel(true); }
    public void PanelRemoveAttention(bool isFront) { transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().AttentionPanel(false); }


    public Vector2Int GetBlockLocalPosition() { return _BlockLocalPosition; }
    public void SetBlockLocalPosition(Vector2Int localPos) { _BlockLocalPosition = localPos; }

    //public GameObject GetFrontPanel() { return transform.GetChild(0); }
    //public GameObject GetBackPanel() { return transform.GetChild(1); }
}