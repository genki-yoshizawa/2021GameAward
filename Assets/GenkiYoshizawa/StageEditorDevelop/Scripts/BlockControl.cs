using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(�\�����킹���p�l����֋X�I��Block�Ƃ���)�̑���������N���X
public class BlockControl : MonoBehaviour
{
    private GameObject _GameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    // �u���b�N�̉�]�֐�
    public void Rotate(bool isFront, float angle, bool isScan = true)
    {
        // Block����Ăяo���ꂽ�ꍇ�͑���Block�𒲂ׂȂ�
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
            this.transform.Rotate(Vector3.up, angle);
            for (int n = 0; n < transform.childCount; ++n)// �p�l������
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    if (transform.GetChild(n).GetChild(i).gameObject != _GameManager.GetComponent<GameManagerScript>().GetPlayer())
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
    public void TurnOver(bool isFront, bool isScan = true)
    {
        // Block����Ăяo���ꂽ�ꍇ�͑���Block�𒲂ׂȂ�
        if (isScan && transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);

            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().TurnOver(isFront, false);
            }
        }
        else
        {
            // �E����180�x��]�i�v���C���[�̌����ɂ���ĕς����ق������������j
            Vector3 rotAxis = Vector3.right;
            this.transform.Rotate(rotAxis, 180);

            for (int n = 0; n < transform.childCount; ++n)// �p�l������
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    if (transform.GetChild(n).GetChild(i).gameObject != _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        transform.GetChild(n).GetChild(i).GetComponent<GimmicControl>().TurnOver(rotAxis);
                }
            }

            // �q�I�u�W�F�N�g���Ԃ����ւ���
            transform.GetChild(1).transform.SetSiblingIndex(0);


            GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

            // �v���C���[�A�G�l�~�[�̕\����ς���֐����Ăяo��
            // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
            gameManagerScript.GetPlayer().GetComponent<PlayerControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());
            foreach (GameObject enemy in gameManagerScript.GetEnemys())
                enemy.GetComponent<EnemyControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());
        }
    }

    //�u���b�N�̓���ւ��֐�
    public void Swap(bool isFront)
    {
        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        List<GameObject> targetBlock = null;
        if (transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
            targetBlock = ScanTargetBlock(isFront);

        List<Vector2Int> targetBlockLocalPosition = new List<Vector2Int>();

        if (targetBlock == null)
            return;
        
        foreach (GameObject target in targetBlock)
        {
            targetBlockLocalPosition.Add(target.GetComponent<BlockConfig>().GetBlockLocalPosition());
        }

        foreach (GameObject target in targetBlock)
        {
            if (gameObject == target)
            {
                targetBlock.Remove(target);
                break;
            }
        }
        
        // �z��v�f����ւ�����
        // �Q�[���}�l�[�W���[���̔z�����ւ�
        gameManagerScript.SwapBlockArray(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        // ���ꂼ��̃u���b�N�̃��[�J���|�W�V���������ւ�
        Vector2Int localTemp = gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition();
        gameObject.GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(localTemp);

        // �u���b�N�̃O���[�o�����W�����ւ���
        Vector3 globalTemp = gameObject.transform.position;
        gameObject.transform.position = targetBlock[0].transform.position;
        targetBlock[0].transform.position = globalTemp;

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
        Vector2Int blockLocalPosition = _GameManager.transform.GetComponent<BlockConfig>().GetBlockLocalPosition();
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
                breakResult = transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().BreakWall(objectPosition, blockLocalPosition, direction);
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
                // ���i�K�ł�3�ȏ�̃X���b�v�̓o�O��
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
}
