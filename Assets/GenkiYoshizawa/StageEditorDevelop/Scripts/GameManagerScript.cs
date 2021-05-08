using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [Header("ステージ上のPlayerオブジェクトをセットしてください(Hierarchy上のPlayerオブジェクト)")]
    [SerializeField] private GameObject _Player;
    [Header("ステージ上のEnemyオブジェクトの数を入れた後それぞれのElementにセットしてください(Hierarchy上のEnemyオブジェクト)")]
    [SerializeField] private List<GameObject> _Enemy;
    // ブロックの配列
    //private List<List<GameObject>> _Block = new List<List<GameObject>>();
    private GameObject[][] _Block;

    // Start is called before the first frame update
    void Start()
    {
        AssignBlockArray();

        //_Player.GetComponent<PlayerControl>().SetLocalPosition(_Player.transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition());
        //_Player.GetComponent<PlayerControl>().SetIsFront(_Player.transform.parent == _Player.transform.parent.parent.GetChild(0) ? true : false);
        foreach (GameObject enemy in _Enemy)
        {
            enemy.GetComponent<EnemyControl>().SetLocalPosition(enemy.transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition());
            enemy.GetComponent<EnemyControl>().SetIsFront(enemy.transform.parent == enemy.transform.parent.parent.GetChild(0) ? true : false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignBlockArray()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        // xz座標の最小値、最大値を求める(そこにブロックがなくても良い)
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

        // オブジェクト間距離が1の時限定（設定できるようにしたほうがいい？）
        Vector2 blockArray = maxPos - minPos;

        // _Blockの配列サイズを決定
        Vector2Int blockArraySize = new Vector2Int((int)blockArray.x + 1, (int)blockArray.y + 1);
        _Block = new GameObject[blockArraySize.x][];
        for(int i = 0; i < blockArraySize.x; ++i)
        {
            _Block[i] = new GameObject[blockArraySize.y];
        }

        // nullで初期化
        for(int x = 0; x < blockArraySize.x; ++x)
        {
            for(int z = 0; z < blockArraySize.y; ++z)
            {
                _Block[x][z] = null;
            }
        }

        // 抜けてるところはnullになる
        for (int i = 0; i < blocks.Length; ++i)
        {
            blocks[i].GetComponent<BlockConfig>().SetBlockLocalPosition(new Vector2Int((int)(blocks[i].transform.position.x - minPos.x), (int)(blocks[i].transform.position.z - minPos.y)));
            _Block[(int)(blocks[i].transform.position.x - minPos.x)][(int)(blocks[i].transform.position.z - minPos.y)] = blocks[i];
        }
    }

    // 2つの配列要素(Vector2Int)を受け取り、その要素のブロックを交換する
    public void SwapBlockArray(Vector2Int block1, Vector2Int block2)
    {
        GameObject temp = _Block[block1.x][block1.y];
        _Block[block1.x][block1.y] = _Block[block2.x][block2.y];
        _Block[block2.x][block2.y] = temp;
    }

    public GameObject GetPlayer() { return _Player; }                               //プレイヤーの取得
    public List<GameObject> GetEnemys() { return _Enemy; }                          //エネミーの全取得
    public GameObject GetEnemy(int index) { return _Enemy[index]; }                 //index番のエネミーの取得
    public GameObject[][] GetBlocks() { return _Block; }                    //ブロックの全取得
    public GameObject GetBlock(Vector2Int pos) { return _Block[pos.x][pos.y]; }     //posにあるBlockの取得
}
