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
    public void Rotate(bool isFront, float angle, bool isScan = true)
    {
        // Blockから呼び出された場合は他のBlockを調べない
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
            for (int n = 0; n < transform.childCount; ++n)// パネル枚数
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    if (transform.GetChild(n).GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        continue;

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
                    
                    transform.GetChild(n).GetChild(i).GetComponent<GimmicControl>().Rotate(angle);
                }
            }
            GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

            // プレイヤー、エネミーの向きを変える関数を呼び出す
            // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
            gameManagerScript.GetPlayer().GetComponent<PlayerControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);
            foreach (GameObject enemy in gameManagerScript.GetEnemys())
                enemy.GetComponent<EnemyControl>().RotateMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), angle);
        }
    }

    // ブロックのひっくり返し関数
    public void TurnOver(bool isFront,/* Vector2Int direction,*/ bool isScan = true)
    {
        // Blockから呼び出された場合は他のBlockを調べない
        if (isScan && transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
        {
            List<GameObject> targetBlock = ScanTargetBlock(isFront);

            foreach (GameObject target in targetBlock)
            {
                target.GetComponent<BlockControl>().TurnOver(isFront,/* direction,*/ false);
            }
        }
        else
        {
            // 右軸に180度回転（プレイヤーの向きによって変えたほうがいいかも）
            Vector3 rotAxis = Vector3.zero;
            //if (direction.x != 0)
            //    rotAxis = Vector3.forward;
            //else if (direction.y != 0)
                rotAxis = Vector3.right;
            this.transform.Rotate(rotAxis, 180);

            for (int n = 0; n < transform.childCount; ++n)// パネル枚数
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    if (transform.GetChild(n).GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        continue;

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
                    
                    transform.GetChild(n).GetChild(i).GetComponent<GimmicControl>().TurnOver(rotAxis);
                }
            }

            // 子オブジェクト順番を入れ替える
            transform.GetChild(1).transform.SetSiblingIndex(0);


            GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

            // プレイヤー、エネミーの表裏を変える関数を呼び出す
            // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
            gameManagerScript.GetPlayer().GetComponent<PlayerControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition()/*, rotAxis*/);
            foreach (GameObject enemy in gameManagerScript.GetEnemys())
                enemy.GetComponent<EnemyControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition()/*, rotAxis*/);
        }
    }

    //ブロックの入れ替え関数
    public void Swap(bool isFront)
    {
        GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

        List<GameObject> targetBlock = null;
        if (transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex() != 0)
            targetBlock = ScanTargetBlock(isFront);

        if (targetBlock == null)
            return;

        // ターゲットブロックをSwapIndexで昇順ソートする
        // ラムダ式でソートしてる。なんでこの書き方なのかよく分かってない
        targetBlock.Sort((a, b) => a.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetSwapIndex() - b.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetSwapIndex());

        List<Vector2Int> targetBlockLocalPosition = new List<Vector2Int>();

        foreach (GameObject target in targetBlock)
        {
            targetBlockLocalPosition.Add(target.GetComponent<BlockConfig>().GetBlockLocalPosition());
        }

        // 自分を取り除く
        //foreach (GameObject target in targetBlock)
        //{
        //    if (gameObject == target)
        //    {
        //        targetBlock.Remove(target);
        //        break;
        //    }
        //}

        // 現段階では3つ以上のスワップはバグる(手直し中)
        // 配列要素入れ替え処理
        // ゲームマネージャー内の配列入れ替え
        //gameManagerScript.SwapBlockArray(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        gameManagerScript.SwapBlockArray(targetBlock);

        // それぞれのブロックのローカルポジションを入れ替え
        //Vector2Int localTemp = gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition();
        //gameObject.GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[0].GetComponent<BlockConfig>().GetBlockLocalPosition());
        //targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(localTemp);
        Vector2Int localTemp = targetBlock[targetBlock.Count - 1].GetComponent<BlockConfig>().GetBlockLocalPosition();
        for (int n = targetBlock.Count - 1; n > 0; --n) 
        {
            targetBlock[n].GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[n - 1].GetComponent<BlockConfig>().GetBlockLocalPosition());
        }
        targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(localTemp);


        // ブロックのグローバル座標を入れ替える
        //Vector3 globalTemp = gameObject.transform.position;
        //gameObject.transform.position = targetBlock[0].transform.position;
        //targetBlock[0].transform.position = globalTemp;
        Vector3 globalTemp = targetBlock[targetBlock.Count - 1].transform.position;
        for (int n = targetBlock.Count - 1; n > 0; --n)
        {
            targetBlock[n].transform.position = targetBlock[n - 1].transform.position;
        }
        targetBlock[0].transform.position = globalTemp;

        // プレイヤー、エネミーのパネル入れ替え関数を呼び出す
        // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
        gameManagerScript.GetPlayer().GetComponent<PlayerControl>().SwapMySelf(targetBlockLocalPosition);
        foreach (GameObject enemy in gameManagerScript.GetEnemys())
            enemy.GetComponent<EnemyControl>().SwapMySelf(targetBlockLocalPosition);

    }

    // 壁を壊す関数(破壊に失敗するとfalse)
    public bool BreakWall(bool isFront, Vector2Int objectPosition, Vector2 direction, int lv = 0)
    {
        GameObject objectBlock = null;
        Vector2Int blockLocalPosition = _GameManager.transform.GetComponent<BlockConfig>().GetBlockLocalPosition();
        if (objectPosition != blockLocalPosition)
        {// 調べるブロックにオブジェクトがいなければ
            // オブジェクトのいるブロックの取得
            objectBlock = _GameManager.transform.GetComponent<GameManagerScript>().GetBlock(objectPosition);
        }

        int breakResult = 0;

        // 自身の乗ってるパネルを先に調べる
        if (objectBlock != null)
        {
            breakResult = objectBlock.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().BreakWall(objectPosition, objectPosition, direction, lv);
        }

        switch (breakResult)
        {
            case 0:// 自身の乗ってるパネルの壁がなかった場合
                breakResult = transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().BreakWall(objectPosition, blockLocalPosition, direction);
                break;

            case 1:// 自身の乗ってるパネルの壁を壊せなかった場合
                return false;

            case 2:// 自身の乗ってるパネルの壁を壊した場合
                return true;
        }

        if (breakResult == 2) return true;// 移動先パネルの壁が壊せた
        else return false;// 移動先のパネルが壊せないor壁がない
    }

    // ターンの終わりに呼び出す関数
    public void BlockTurn()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<PanelControl>().TurnEndUpdate();
        }
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
                if (blockZLine == null) continue;
                // 同じインデックスであれば対象ブロック
                if (blockZLine.transform.GetChild(isFront ? 0 : 1).transform.GetComponent<PanelConfig>().GetPanelIndex() == gameObject.transform.GetChild(isFront ? 0 : 1).GetComponent<PanelConfig>().GetPanelIndex())
                {
                    targetBlock.Add(blockZLine);
                }
            }
        }

        // targetBlockがなければnullを返す
        if (targetBlock == null || targetBlock.Count == 0)
            return null;

        return targetBlock;
    }
    
}
