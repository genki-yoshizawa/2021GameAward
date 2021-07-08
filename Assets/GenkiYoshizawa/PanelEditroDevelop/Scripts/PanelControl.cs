using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �p�l���̑�����s���N���X
public class PanelControl : MonoBehaviour
{
    [Header("�}�[�J�[�����炷�����ׂ�p�t���O(�Q�[���������ɂ͏���)")]
    [SerializeField] private bool _isMarker = false;
    [Header("�_�ł������͊g��̎���(�b)")]
    [SerializeField] private float _CycleTime = 1.0f;
    [Header("�_�ł������͊g��̕ω��̎d��(ture���T�C���g�Afalse���̂�����h)")]
    [SerializeField] private bool _isSinPulse = false;
    private GameObject _GameManager = null;

    // �}�[�J�[UI�ŕK�v�ȕϐ�
    private bool _isMarkerCreate = false;
    private bool _isCurMarkerCreate = false;
    private GameObject _Marker;
    [Header("�}�[�J�[�̍ő�T�C�Y(�{��)(�ŏ����̃}�[�J�[�X�P�[���̒l0.1)")]
    [SerializeField] private float _MarkerMaxScale = 0.2f;
    [Header("�}�[�J�[�̈ʒu(�v�����i�[�͂�����K�v�Ȃ�)")]
    [SerializeField] private float _MarkerOffset = 0.2f;
    [Header("Prefab����ݒ肷��}�[�J�[�̃Q�[���I�u�W�F�N�g(�v�����i�[�͂�����K�v�Ȃ�)")]
    [SerializeField] private GameObject _MarkerPrefab;

    // ���点����ŕK�v�ȕϐ�
    private bool _isBright = false;
    private bool _isCurBright = false;
    private MeshRenderer _MeshRenderer;
    //private Material[] _SaveMaterials;
    [Header("Asset����ݒ肷��f�t�H���g�}�e���A��(�I�u�W�F�N�g�ɐݒ肳��Ă���}�e���A���̐�)(�v�����i�[�͂�����K�v�Ȃ�)")]
    //[SerializeField] private Material[] _DefaultMaterial;
    // UI���s��̌o�ߎ���
    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _isBright = _isCurBright = false;

        _MeshRenderer = GetComponent<MeshRenderer>();
        //_SaveMaterials = new Material[_MeshRenderer.materials.Length];
        for(int i = 0; i < _MeshRenderer.materials.Length; ++i)
        {
            //_SaveMaterials[i] = _DefaultMaterial[i];
            //_SaveMaterials[i].CopyPropertiesFromMaterial(_MeshRenderer.materials[i]);
            //_MeshRenderer.materials[i].CopyPropertiesFromMaterial(_SaveMaterials[i]);
        }

        _PassedTime = 0.0f;
    }

    private void Update()
    {
        // �l�X�g���ꎞ�I�ɐ[�����ǃQ�[���������ɂ͌��₷���Ȃ�i�͂��j
        if (_isMarker)
        {
            if (_isMarkerCreate && !_isCurMarkerCreate)
            {//�}�[�J�[�𐶐����鏈��
                // ���񐶐��f���[�g���J��Ԃ��͌��Ȃ̂ŁA�X�e�[�W���Ƀ}�[�J�[�I�u�W�F�N�g���Z�b�g���āAStart�ŃI�u�W�F�N�g�擾�B
                // �A�N�e�B�u�t���O�A�|�W�V������؂�ւ��čs�����Ƃɂ���(�������̈ĂɂȂ�����)
                _Marker = Instantiate(_MarkerPrefab);

                bool isFront = transform.parent.GetChild(0) == transform;
                // �}�[�J�[�̉�]�ƈʒu�����킹��
                if (!isFront)
                    _Marker.transform.Rotate(Vector3.right, 180);
                _Marker.transform.position = transform.position + new Vector3(0.0f, isFront ? _MarkerOffset : -_MarkerOffset, 0.0f);

                PanelMarkerScript script = _Marker.GetComponent<PanelMarkerScript>();
                script.SetCycleTime(_CycleTime);
                script.SetMaxScale(_MarkerMaxScale);
                script.SetisSinPulse(_isSinPulse);
            }
            else if(!_isMarkerCreate && _isCurMarkerCreate)
            {//�}�[�J�[��j�����鏈��
                Destroy(_Marker);
            }

            _isCurMarkerCreate = _isMarkerCreate;
        }
        else
        {
            if (_isBright)
            {
                _PassedTime += Time.deltaTime;
                while (true)
                {
                    if (_PassedTime > _CycleTime)
                        _PassedTime -= _CycleTime;
                    else
                        break;
                }

                for(int i = 0; i < _MeshRenderer.materials.Length; ++i)
                {
                    Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

                    if (_isSinPulse)
                    {
                        //color.r = _SaveMaterials[i].color.r + Mathf.Abs((1.0f - _SaveMaterials[i].color.r) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
                        //color.g = _SaveMaterials[i].color.g + Mathf.Abs((1.0f - _SaveMaterials[i].color.g) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
                        //color.b = _SaveMaterials[i].color.b + Mathf.Abs((1.0f - _SaveMaterials[i].color.b) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
                    }
                    else
                    {
                        if (_PassedTime < _CycleTime * 0.5f)
                        {
                            //color.r = _SaveMaterials[i].color.r + (1.0f - _SaveMaterials[i].color.r) / (_CycleTime * 0.5f) * _PassedTime;
                            //color.g = _SaveMaterials[i].color.g + (1.0f - _SaveMaterials[i].color.g) / (_CycleTime * 0.5f) * _PassedTime;
                            //color.b = _SaveMaterials[i].color.b + (1.0f - _SaveMaterials[i].color.b) / (_CycleTime * 0.5f) * _PassedTime;
                        }
                        else
                        {
                            //color.r = 1.0f - (1.0f - _SaveMaterials[i].color.r) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                            //color.g = 1.0f - (1.0f - _SaveMaterials[i].color.g) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                            //color.b = 1.0f - (1.0f - _SaveMaterials[i].color.b) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
                        }
                    }

                    //color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    _MeshRenderer.materials[i].color = color;
                }
                
            }
            else if (!_isBright && _isCurBright)
            {
                // ���O�܂Ŕ��������Ă����猳�ɖ߂�
                for (int i = 0; i < _MeshRenderer.materials.Length; ++i)
                {
                    //Debug.Log(_SaveMaterials[i]);
                    //_MeshRenderer.materials[i].color = _SaveMaterials[i].color;
                }
                _PassedTime = 0.0f;
            }

            _isCurBright = _isBright;
        }
    }

    //���̃p�l���ɂ��M�~�b�N�i�q�I�u�W�F�N�g�j���܂Ƃ߂ď�������
    public void TurnEndUpdate()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                continue;

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

            transform.GetChild(i).GetComponent<GimmicControl>().TurnEndUpdate();
        }
    }

    public int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        int breakResult = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                continue;

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

            breakResult = transform.GetChild(i).GetComponent<GimmicControl>().BreakWall(objectPosition, panelPosition, direction, lv);
            if (breakResult != 0) // ����ł���ǂ�������
                break;
        }
        return breakResult;
    }

    public void AttentionPanel(bool attention)
    {
        _isMarkerCreate = attention;
        _isBright = attention;
    }
}
