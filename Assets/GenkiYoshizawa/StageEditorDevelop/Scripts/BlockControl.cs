using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(表裏合わせたパネルを便宜的にBlockとする)の操作を扱うクラス
public class BlockControl : MonoBehaviour
{
    private GameObject _GameManager = null;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
    }

    // ブロックの回転関数
    public void Rotate(bool isFront, float angle, bool isScan = false)
    {
        // Blockから呼び出された場合は他のBlockを調べない
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

        // プレイヤー、エネミーの向きを変える関数を呼び出す
        // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
        /*gameManagerScript.GetPlayer().GetComponent<PlayerControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);*/

    }

    // ブロックのひっくり返し関数
    public void TurnOver(bool isFront, bool isScan = true)
    {
        // Blockから呼び出された場合は他のBlockを調べない
        if (isScan)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);

            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().TurnOver(isFront, false);
            }
        }
        // 右軸に180度回転（プレイヤーの向きによって変えたほうがいいかも）
        this.transform.Rotate(Vector3.right, 180);

        // 子オブジェクト順番を入れ替える
        transform.GetChild(1).transform.SetSiblingIndex(0);


        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        // プレイヤー、エネミーの表裏を変える関数を呼び出す
        // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
        /*gameManagerScript.GetPlayer().GetComponent<PlayerControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());*/
    }

    //ブロックの入れ替え関数
    public void Swap(bool isFront)
    {
        List<GameObject> targetBlock = ScanTargetBlock(isFront);

        if (targetBlock == null)
            return;

        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        // プレイヤー、エネミーのパネル入れ替え関数を呼び出す
        // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
        /*gameManagerScript.GetPlayer().GetComponent<PlayerControl>().SwapMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().SwapMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition());*/
        // 配列要素入れ替え処理
        // ゲームマネージャー内の配列入れ替え
        gameManagerScript.SwapBlockArray(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        // それぞれのブロックのローカルポジションを入れ替え
        Vector2Int temp = gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition();
        gameObject.GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(temp);
    }

    // GameManagerを介して同じIndexが設定されているブロックを探す
    private List<GameObject> ScanTargetBlock(bool isFront)
    {
        List<GameObject> targetBlock = new List<GameObject>();
        
        // 全ブロックの取得
        GameObject[][] blocks = _GameManager.GetComponent<GameManagerScript>().GetBlocks();
        foreach (GameObject[] blockXLine in blocks)
        {
            foreach (GameObject blockZLine in blockXLine)
            {
                // 同じマテリアルであれば対象ブロック
                // 現段階では3つ以上のスワップはバグる
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

        // targetBlockがなければnullを返す
        if (targetBlock == null || targetBlock.Count == 0)
            return null;

        return targetBlock;
    }
}
