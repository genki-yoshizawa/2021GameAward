using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [Header("�X�e�[�W���Player�I�u�W�F�N�g���Z�b�g���Ă�������(Hierarchy���Player�I�u�W�F�N�g)")]
    [SerializeField] private GameObject _Player;
    [Header("�X�e�[�W���Enemy�I�u�W�F�N�g�̐�����ꂽ�セ�ꂼ���Element�ɃZ�b�g���Ă�������(Hierarchy���Enemy�I�u�W�F�N�g)")]
    [SerializeField] private List<GameObject> _Enemy;
    // �u���b�N�̔z��
    private GameObject[][] _Block;

    // Start is called before the first frame update
    void Start()
    {
        AssignBlockArray();

        _Player.GetComponent<PlayerControl>().SetLocalPosition(_Player.transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition());
        _Player.GetComponent<PlayerControl>().SetIsFront(_Player.transform.parent == _Player.transform.parent.parent.GetChild(0) ? true : false);
        //foreach (GameObject enemy in _Enemy)
        //{
        //    enemy.GetComponent<EnemyControl>().SetLocalPosition(enemy.transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition());
        //    enemy.GetComponent<EnemyControl>().SetIsFront(enemy.transform.parent == enemy.transform.parent.parent.GetChild(0) ? true : false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignBlockArray()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        // xz���W�̍ŏ��l�A�ő�l�����߂�(�����Ƀu���b�N���Ȃ��Ă��ǂ�)
        Vector2 minPos = new Vector2(blocks[0].transform.position.x, blocks[0].transform.position.z);
        Vector2 maxPos = new Vector2(blocks[0].transform.position.x, blocks[0].transform.position.z);

        foreach (GameObject block in blocks)
        {
            if (minPos.x > block.transform.position.x)
                minPos.x = block.transform.position.x;
            if (minPos.y > block.transform.position.z)
                minPos.y = block.transform.position.z;

            if (maxPos.x < block.transform.position.x)
                maxPos.x = block.transform.position.x;
            if (maxPos.y < block.transform.position.z)
                maxPos.y = block.transform.position.z;
        }

        // �I�u�W�F�N�g�ԋ�����1�̎�����i�ݒ�ł���悤�ɂ����ق��������H�j
        Vector2 blockArray = maxPos - minPos;

        // _Block�̔z��T�C�Y������
        Vector2Int blockArraySize = new Vector2Int((int)blockArray.x + 1, (int)blockArray.y + 1);
        _Block = new GameObject[blockArraySize.x][];
        for(int i = 0; i < blockArraySize.x; ++i)
        {
            _Block[i] = new GameObject[blockArraySize.y];
        }

        // null�ŏ�����
        for(int x = 0; x < blockArraySize.x; ++x)
        {
            for(int z = 0; z < blockArraySize.y; ++z)
            {
                _Block[x][z] = null;
            }
        }

        // �����Ă�Ƃ����null�ɂȂ�
        for (int i = 0; i < blocks.Length; ++i)
        {
            blocks[i].GetComponent<BlockConfig>().SetBlockLocalPosition(new Vector2Int((int)(blocks[i].transform.position.x - minPos.x), (int)(blocks[i].transform.position.z - minPos.y)));
            _Block[(int)(blocks[i].transform.position.x - minPos.x)][(int)(blocks[i].transform.position.z - minPos.y)] = blocks[i];
        }
    }

    // 2�̔z��v�f(Vector2Int)���󂯎��A���̗v�f�̃u���b�N����������
    public void SwapBlockArray(Vector2Int block1, Vector2Int block2)
    {
        GameObject temp = _Block[block1.x][block1.y];
        _Block[block1.x][block1.y] = _Block[block2.x][block2.y];
        _Block[block2.x][block2.y] = temp;
        
    }

    public GameObject GetPlayer() { return _Player; }                               //�v���C���[�̎擾
    public List<GameObject> GetEnemys() { return _Enemy; }                          //�G�l�~�[�̑S�擾
    public GameObject GetEnemy(int index) { return _Enemy[index]; }                 //index�Ԃ̃G�l�~�[�̎擾
    public GameObject[][] GetBlocks() { return _Block; }                    //�u���b�N�̑S�擾
    public GameObject GetBlock(Vector2Int pos) { return _Block[pos.x][pos.y]; }     //pos�ɂ���Block�̎擾
    
}
