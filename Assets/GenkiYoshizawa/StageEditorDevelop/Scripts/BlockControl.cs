using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(�\�����킹���p�l����֋X�I��Block�Ƃ���)�̑���������N���X
public class BlockControl : MonoBehaviour
{
    private GameObject _GameManager = null;

    // ��]�A�j���[�V�����p�ϐ�
    [Header("��]�A�j���[�V�����ɂ�����b��")]
    [SerializeField] private float _RotateAnimTime = 1.0f;
    private bool _isRotateAnim = false;
    private float _RotateAngle = 0.0f;

    // �Ђ�����Ԃ��A�j���[�V�����p�ϐ�
    [Header("�Ђ�����Ԃ��A�j���[�V�����ɂ�����b��")]
    [SerializeField] private float _TurnOverAnimTime = 1.0f;
    private bool _isTurnOverAnim = false;
    private Vector3 _TurnOverAxis = Vector3.zero;

    // ����ւ��A�j���[�V�����p�ϐ�
    [Header("����ւ��A�j���[�V�����ɂ�����b��")]
    [SerializeField] private float _SwapAnimTime = 1.0f;
    [Header("����ւ��A�j���[�V�������Ƀu���b�N���ǂꂾ����������")]
    [SerializeField] private float _SwapPanelFloat = 0.3f;
    private bool _isSwapAnim = false;
    private Vector3 _SwapGlobalPosition = Vector3.zero;
    private Vector3 _StartGlobalPosition = Vector3.zero;

    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _isRotateAnim = false;
        _RotateAngle = 0.0f;

        _isTurnOverAnim = false;
        _TurnOverAxis = Vector3.zero;

        _isSwapAnim = false;
        _SwapGlobalPosition = Vector3.zero;
        _SwapPanelFloat += transform.position.y;
        _StartGlobalPosition = transform.position;

        _PassedTime = 0.0f;
    }

    private void Update()
    {
        if (_isRotateAnim)
        {
            float time = Time.deltaTime;
            if (_PassedTime + time > _RotateAnimTime)
            {
                time = _RotateAnimTime - _PassedTime;
                _PassedTime = 0.0f;
                _isRotateAnim = false;
            }
            else
                _PassedTime += time;

            float angle = _RotateAngle * (time / _RotateAnimTime);
            this.transform.Rotate(Vector3.up * angle, Space.World);
        }

        if (_isTurnOverAnim)
        {
            float time = Time.deltaTime;
            if (_PassedTime + time > _TurnOverAnimTime)
            {
                time = _TurnOverAnimTime - _PassedTime;
                _PassedTime = 0.0f;
                _isTurnOverAnim = false;
                // �}�l�[�W���[�̃X�^�[�g�G�l�~�[���[�r�[���Ă�
                // ���̏������̓G�l�~�[��2�̈ȏ�̎��ɑΉ��ł��Ȃ�
                foreach (GameObject enemy in _GameManager.GetComponent<GameManagerScript>().GetEnemys())
                    _GameManager.gameObject.GetComponent<GameManagerScript>().StartEnemyMovie(enemy.GetComponent<EnemyControl>().GetIsFront());

            }
            else
                _PassedTime += time;

            float angle = 180.0f * (time / _TurnOverAnimTime);
            this.transform.Rotate(_TurnOverAxis * angle, Space.World);
        }

        if (_isSwapAnim)
        {
            // �����Ȃ񂩉�肭�ǂ����������Ă�C������
            float swapFloat = _GameManager.GetComponent<GameManagerScript>().GetPlayer().transform.GetComponent<PlayerControl>().GetIsFront() ? _SwapPanelFloat : -_SwapPanelFloat;

            transform.position = new Vector3(transform.position.x, swapFloat, transform.position.z);

            float time = Time.deltaTime;
            if (_PassedTime + time > _SwapAnimTime)
            {
                transform.position = new Vector3(transform.position.x, _StartGlobalPosition.y, transform.position.z); 
                time = _SwapAnimTime - _PassedTime;
                _PassedTime = 0.0f;
                _isSwapAnim = false;
                // �l�Y�~�̃t���O�����Ƃ�
                SetIsSwapAnimEnemyFlg(false);
            }
            else
                _PassedTime += time;

            Vector3 move = (_SwapGlobalPosition - _StartGlobalPosition) * (time / _TurnOverAnimTime);
            transform.position += move;

            // ��]���Ȃ���X���b�v�ړ�����
            float angle = 360.0f * (time / _RotateAnimTime);
            this.transform.Rotate(Vector3.up, angle);
        }
    }

    // �u���b�N�̉�]�֐�
    public void Rotate(bool isFront, float angle, bool isScan = true)
    {
        // �ŏ��ɌĂяo���ꂽ���͉�]������ׂ��u���b�N���ꋓ�ɒ��ׂ�
        if (isScan && transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);
            
            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().Rotate(isFront, angle, false);
            }
        }
        else
        {
            _isRotateAnim = true;
            _RotateAngle = angle;
            //this.transform.Rotate(Vector3.up, angle);
            for (int n = 0; n < transform.childCount; ++n)// �p�l������
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    // �v���C���[�Ȃ�X���[
                    if (transform.GetChild(n).GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        continue;

                    // �G�l�~�[�Ȃ�X���[
                    List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
                    bool isThrow = false;
                    foreach (GameObject enemy in enemys)
                        if (transform.GetChild(n).GetChild(i).gameObject == enemy)
                        {
                            isThrow = true;
                            break;
                        }
                    if (isThrow)
                        continue;
                    
                    // �q�I�u�W�F�N�g�̃M�~�b�N�̉�]�֐��Ăяo��
                    transform.GetChild(n).GetChild(i).GetComponent<GimmicControl>().Rotate(angle);
                }
            }
            GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

            // �v���C���[�A�G�l�~�[�̌�����ς���֐����Ăяo��
            // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
            gameManagerScript.GetPlayer().GetComponent<PlayerControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);
            foreach (GameObject enemy in gameManagerScript.GetEnemys())
                enemy.GetComponent<EnemyControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);
        }
    }

    // �u���b�N�̂Ђ�����Ԃ��֐�
    public void TurnOver(bool isFront, Vector2Int direction, bool isScan = true)
    {
        // �ŏ��ɌĂяo���ꂽ���͉�]������ׂ��u���b�N���ꋓ�ɒ��ׂ�
        if (isScan && transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);

            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().TurnOver(isFront, direction, false);
            }
        }
        else
        {
            // �v���C���[�̌����ɂ���ĉ�]����ς���
            Vector3 rotAxis = Vector3.zero;
            if (direction.x != 0)
                rotAxis = Vector3.forward;
            else if (direction.y != 0)
                rotAxis = Vector3.right;
            _TurnOverAxis = rotAxis;
            _isTurnOverAnim = true;
            //this.transform.Rotate(rotAxis, 180);

            for (int n = 0; n < transform.childCount; ++n)// �p�l������
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    // �v���C���[�Ȃ�X���[
                    if (transform.GetChild(n).GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        continue;

                    // �G�l�~�[�Ȃ�X���[
                    List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
                    bool isThrow = false;
                    foreach (GameObject enemy in enemys)
                        if (transform.GetChild(n).GetChild(i).gameObject == enemy)
                        {
                            isThrow = true;
                            break;
                        }
                    if (isThrow)
                        continue;

                    // �q�I�u�W�F�N�g�̃M�~�b�N�̉�]�֐��Ăяo��
                    transform.GetChild(n).GetChild(i).GetComponent<GimmicControl>().TurnOver(rotAxis);
                }
            }

            // �q�I�u�W�F�N�g���Ԃ����ւ���
            transform.GetChild(1).transform.SetSiblingIndex(0);


            GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

            // �v���C���[�A�G�l�~�[�̕\����ς���֐����Ăяo��
            // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
            gameManagerScript.GetPlayer().GetComponent<PlayerControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), rotAxis);
            foreach (GameObject enemy in gameManagerScript.GetEnemys())
                enemy.GetComponent<EnemyControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition()/*, rotAxis*/);
        }
    }

    //�u���b�N�̓���ւ��֐�
    public void Swap(bool isFront)
    {
        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        List<GameObject> targetBlock = null;
        if (transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
            targetBlock = ScanTargetBlock(isFront);

        if (targetBlock == null)
            return;

        // �^�[�Q�b�g�u���b�N��SwapIndex�ŏ����\�[�g����
        // �����_���Ń\�[�g���Ă�B�Ȃ�ł��̏������Ȃ̂��悭�������ĂȂ�
        targetBlock.Sort((a, b) => a.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetSwapIndex() - b.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetSwapIndex());

        List<Vector2Int> targetBlockLocalPosition = new List<Vector2Int>();

        foreach (GameObject target in targetBlock)
        {
            targetBlockLocalPosition.Add(target.GetComponent<BlockConfig>().GetBlockLocalPosition());
        }

        // �z��v�f����ւ�����
        // �Q�[���}�l�[�W���[���̔z�����ւ�
        gameManagerScript.SwapBlockArray(targetBlock);

        // ���ꂼ��̃u���b�N�̃��[�J���|�W�V���������ւ�
        Vector2Int localTemp = targetBlock[targetBlock.Count - 1].GetComponent<BlockConfig>().GetBlockLocalPosition();
        for (int n = targetBlock.Count - 1; n > 0; --n) 
        {
            targetBlock[n].GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[n - 1].GetComponent<BlockConfig>().GetBlockLocalPosition());
        }
        targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(localTemp);


        // �u���b�N�̃O���[�o�����W�����ւ���
        for (int n = targetBlock.Count - 1; n > 0; --n)
        {
            targetBlock[n].GetComponent<BlockControl>().SetisSwapAnim();
            targetBlock[n].GetComponent<BlockControl>().SetSwapGlobalPosition(targetBlock[n - 1].transform.position);
            targetBlock[n].GetComponent<BlockControl>().SetStartGlobalPosition();
        }
        targetBlock[0].GetComponent<BlockControl>().SetisSwapAnim();
        targetBlock[0].GetComponent<BlockControl>().SetSwapGlobalPosition(targetBlock[targetBlock.Count - 1].transform.position);
        targetBlock[0].GetComponent<BlockControl>().SetStartGlobalPosition();

        // �v���C���[�A�G�l�~�[�̃p�l������ւ��֐����Ăяo��
        // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
        gameManagerScript.GetPlayer().GetComponent<PlayerControl>().SwapMySelf(targetBlockLocalPosition);
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().SwapMySelf(targetBlockLocalPosition);

    }

    // �ǂ��󂷊֐�(�j��Ɏ��s�����false)
    public bool BreakWall(bool isFront, Vector2Int objectPosition, Vector2 direction, int lv = 0)
    {
        GameObject objectBlock = null;
        Vector2Int blockLocalPosition = transform.GetComponent<BlockConfig>().GetBlockLocalPosition();
        if (objectPosition != blockLocalPosition)
        {// ���ׂ�u���b�N�ɃI�u�W�F�N�g�����Ȃ����
            // �I�u�W�F�N�g�̂���u���b�N�̎擾
            objectBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlock(objectPosition);
        }

        int breakResult = 0;

        // ���g�̏���Ă�p�l�����ɒ��ׂ�
        if (objectBlock != null)
        {
            breakResult = objectBlock.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().BreakWall(objectPosition, objectPosition, direction, lv);
        }

        switch (breakResult)
        {
            case 0:// ���g�̏���Ă�p�l���̕ǂ��Ȃ������ꍇ
                breakResult = transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().BreakWall(objectPosition, blockLocalPosition, direction, lv);
                break;

            case 1:// ���g�̏���Ă�p�l���̕ǂ��󂹂Ȃ������ꍇ
                return false;

            case 2:// ���g�̏���Ă�p�l���̕ǂ��󂵂��ꍇ
                return true;
        }

        if (breakResult == 2) return true;// �ړ���p�l���̕ǂ��󂹂�
        else return false;// �ړ���̃p�l�����󂹂Ȃ�or�ǂ��Ȃ�
    }

    // �^�[���̏I���ɌĂяo���֐�
    public void BlockTurn()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<PanelControl>().TurnEndUpdate();
        }
    }

    // GameManager����ē���Index���ݒ肳��Ă���u���b�N��T��
    private List<GameObject> ScanTargetBlock(bool isFront)
    {
        List<GameObject> targetBlock = new List<GameObject>();
        
        // �S�u���b�N�̎擾
        GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
        foreach (GameObject[] blockXLine in blocks)
        {
            foreach (GameObject blockZLine in blockXLine)
            {
                if (blockZLine == null) continue;
                // �����C���f�b�N�X�ł���ΑΏۃu���b�N
                if (blockZLine.transform.GetChild(isFront ? 0 : 1).transform.GetComponent<PanelConfig>().GetPanelIndex() == gameObject.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex())
                {
                    targetBlock.Add(blockZLine);
                }
            }
        }

        // targetBlock���Ȃ����null��Ԃ�
        if (targetBlock == null || targetBlock.Count == 0)
            return null;

        return targetBlock;
    }
    
    // �G�l�~�[�̎���PanelSwapAnim�t���O���グ��������
    private void SetIsSwapAnimEnemyFlg(bool flg)
    {
        foreach(GameObject e in _GameManager.GetComponent<GameManagerScript>().GetEnemys())
        {
            if(e.GetComponent<EnemyControl>().GetLocalPosition() != GetComponent<BlockConfig>().GetBlockLocalPosition())
                continue;

            e.GetComponent<EnemyControl>().SetIsPanelAnimation(flg);
        }
    }

    public void SetisSwapAnim() {
        _isSwapAnim = true;
    }
    public void SetSwapGlobalPosition(Vector3 pos) { _SwapGlobalPosition = pos; }
    public void SetStartGlobalPosition() { _StartGlobalPosition = transform.position; }

    public bool GetisSwapAnim() { return _isSwapAnim; }
    public bool GetisTurnOverAnim() { return _isTurnOverAnim; }
    public float GetTuenOverAnimTime() { return _TurnOverAnimTime; } //�J�������[�N�ɕK�v
    public float GetAnimPassedTime() { return _PassedTime; } //�A�j���[�V�����̌o�ߎ��Ԃ�n���֐�
}
