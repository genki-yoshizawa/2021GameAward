using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �p�l���̒l�������N���X
// �I�u�W�F�N�g��Get����������擾����
public class PanelConfig : MonoBehaviour
{
    [Header("��]�p�l�����ǂ���")]
    [SerializeField] private bool _canRotate = false;
    [Header("�Ђ�����Ԃ��p�l�����ǂ���")]
    [SerializeField] private bool _canTurnOver = false;
    [Header("����ւ��p�l�����ǂ���")]
    [SerializeField] private bool _canSwap = false;
    [Header("����ւ��p�l���̒ʂ��ԍ�(�������傫���Ȃ���ɓ���ւ�������)")]
    [SerializeField] private int _SwapIndex = 0;
    [Header("�p�l�����m��Ή�������ԍ�(0���Ɩ��Ή�)")]
    [SerializeField] private int _PanelIndex;

    private GameObject _GameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    public bool GetCanRotate() { return _canRotate; }
    public bool GetCanTurnOver() { return _canTurnOver; }
    public bool GetCanSwap() { return _canSwap; }
    public int GetSwapIndex() { return _SwapIndex; }
    public int GetPanelIndex() { return _PanelIndex; }
    
    public bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition,Vector2 direction)
    {
        GameManagerScript gmScript = _GameManager.GetComponent<GameManagerScript>(); 

        for(int i = 0; i < transform.childCount; ++i)
        {
            // �G�l�~�[�ƈ�v�����玟�̎q�I�u�W�F�N�g�Ɉڂ�
            List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
            bool isThrow = false;
            foreach (GameObject enemy in enemys)
                if (transform.GetChild(i).gameObject == enemy)
                {
                    isThrow = true;
                    break;
                }
            if (isThrow)
                continue;

            // �v���C���[�ł��Ȃ��M�~�b�N�̃`�F�b�N�G���^�[���ʂ�����
            if (gmScript.GetPlayer() != transform.GetChild(i).gameObject &&
                !transform.GetChild(i).GetComponent<GimmicControl>().CheckEnter(objectPosition, panelPosition, direction))
                return false;
        }

        return true;
    }

    public int CheckWallLevel(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        int wallLevel = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            wallLevel = transform.GetChild(i).GetComponent<GimmicControl>().CheckWallLevel(objectPosition, panelPosition, direction);
            if (wallLevel != 0)
                break;
        }
        return wallLevel;
    }

    //�ォ��n�Ԗڂ̃M�~�b�N(�q�I�u�W�F�N�g)���擾����
    //public GameObject GetGimmic(int n) { return transform.GetChild(n); }
}