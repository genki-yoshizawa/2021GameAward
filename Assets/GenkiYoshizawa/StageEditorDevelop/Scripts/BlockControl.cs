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
    public void Rotate(bool isFront, float angle, bool isScan = false)
    {
        // Block����Ăяo���ꂽ�ꍇ�͑���Block�𒲂ׂȂ�
        if (isScan)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);

            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().Rotate(isFront, angle, false);
            }
        }

        this.transform.Rotate(Vector3.up,angle);

        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        // �v���C���[�A�G�l�~�[�̌�����ς���֐����Ăяo��
        // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
        /*gameManagerScript.GetPlayer().GetComponent<PlayerControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);*/

    }

    // �u���b�N�̂Ђ�����Ԃ��֐�
    public void TurnOver(bool isFront, bool isScan = true)
    {
        // Block����Ăяo���ꂽ�ꍇ�͑���Block�𒲂ׂȂ�
        if (isScan)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);

            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().TurnOver(isFront, false);
            }
        }
        // �E����180�x��]�i�v���C���[�̌����ɂ���ĕς����ق������������j
        this.transform.Rotate(Vector3.right, 180);

        // �q�I�u�W�F�N�g���Ԃ����ւ���
        transform.GetChild(1).transform.SetSiblingIndex(0);


        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        // �v���C���[�A�G�l�~�[�̕\����ς���֐����Ăяo��
        // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
        /*gameManagerScript.GetPlayer().GetComponent<PlayerControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());*/
    }

    //�u���b�N�̓���ւ��֐�
    public void Swap(bool isFront)
    {
        List<GameObject> targetBlock = ScanTargetBlock(isFront);

        if (targetBlock == null)
            return;

        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        // �v���C���[�A�G�l�~�[�̃p�l������ւ��֐����Ăяo��
        // �����ɏ����Ă���X�N���v�g�A�֐��ŗp�ӂ��Ă��炦��ƃR�����g�A�E�g�����ōςނ̂ŏ�����
        /*gameManagerScript.GetPlayer().GetComponent<PlayerControl>().SwapMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().SwapMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());*/
        // �z��v�f����ւ�����
        // �Q�[���}�l�[�W���[���̔z�����ւ�
        gameManagerScript.SwapBlockArray(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        // ���ꂼ��̃u���b�N�̃��[�J���|�W�V���������ւ�
        Vector2Int temp = gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition();
        gameObject.GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(temp);
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
                // �����}�e���A���ł���ΑΏۃu���b�N
                // ���i�K�ł�3�ȏ�̃X���b�v�̓o�O��
                if (isFront)
                {
                    if (blockZLine.transform.GetChild(0).transform.GetComponent<PanelConfig>().GetPanelIndex() == gameObject.transform.GetChild(0).GetComponent<PanelConfig>().GetPanelIndex())
                    {
                        targetBlock.Add(blockZLine);
                    }
                }
                else
                {
                    if (blockZLine.transform.GetChild(1).transform.GetComponent<PanelConfig>().GetPanelIndex() == gameObject.transform.GetChild(1).GetComponent<PanelConfig>().GetPanelIndex())
                    {
                        targetBlock.Add(blockZLine);
                    }
                }
            }
        }

        // targetBlock���Ȃ����null��Ԃ�
        if (targetBlock == null || targetBlock.Count == 0)
            return null;

        return targetBlock;
    }
}
