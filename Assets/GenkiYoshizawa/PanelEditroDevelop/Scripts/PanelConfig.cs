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
    [Header("�p�l�����m��Ή�������ԍ�(0���Ɩ��Ή�)")]
    [SerializeField] private int _PanelIndex;

    // Start is called before the first frame update
    void Start()
    {

    }

    public bool GetCanRotate() { return _canRotate; }
    public bool GetCanTurnOver() { return _canTurnOver; }
    public bool GetCanSwap() { return _canSwap; }
    public int GetPanelIndex() { return _PanelIndex; }

    //�ォ��n�Ԗڂ̃M�~�b�N(�q�I�u�W�F�N�g)���擾����
    //public GameObject GetGimmic(int n) { return transform.GetChild(n); }
}
