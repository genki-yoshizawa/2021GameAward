using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Block(表裏合わせたパネルを便宜的にBlockとする)の操作を扱うクラス
public class BlockControl : MonoBehaviour
{
    private GameObject _GameManager = null;

    // 回転アニメーション用変数
    [Header("回転アニメーションにかける秒数")]
    [SerializeField] private float _RotateAnimTime = 1.0f;
    private bool _isRotateAnim = false;
    private float _RotateAngle = 0.0f;

    // ひっくり返しアニメーション用変数
    [Header("ひっくり返しアニメーションにかける秒数")]
    [SerializeField] private float _TurnOverAnimTime = 1.0f;
    private bool _isTurnOverAnim = false;
    private Vector3 _TurnOverAxis = Vector3.zero;

    // 入れ替えアニメーション用変数
    [Header("入れ替えアニメーションにかける秒数")]
    [SerializeField] private float _SwapAnimTime = 1.0f;
    [Header("入れ替えアニメーション時にブロックをどれだけ浮かすか")]
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
                // マネージャーのスタートエネミームービーを呼ぶ
                // この書き方はエネミーが2体以上の時に対応できない
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
            // ここなんか回りくどい書き方してる気がする
            float swapFloat = _GameManager.GetComponent<GameManagerScript>().GetPlayer().transform.GetComponent<PlayerControl>().GetIsFront() ? _SwapPanelFloat : -_SwapPanelFloat;

            transform.position = new Vector3(transform.position.x, swapFloat, transform.position.z);

            float time = Time.deltaTime;
            if (_PassedTime + time > _SwapAnimTime)
            {
                transform.position = new Vector3(transform.position.x, _StartGlobalPosition.y, transform.position.z); 
                time = _SwapAnimTime - _PassedTime;
                _PassedTime = 0.0f;
                _isSwapAnim = false;
                // ネズミのフラグも落とす
                SetIsSwapAnimEnemyFlg(false);
            }
            else
                _PassedTime += time;

            Vector3 move = (_SwapGlobalPosition - _StartGlobalPosition) * (time / _TurnOverAnimTime);
            transform.position += move;

            // 回転しながらスワップ移動する
            float angle = 360.0f * (time / _RotateAnimTime);
            this.transform.Rotate(Vector3.up, angle);
        }
    }

    // ブロックの回転関数
    public void Rotate(bool isFront, float angle, bool isScan = true)
    {
        // 最初に呼び出された時は回転させるべきブロックを一挙に調べる
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
            for (int n = 0; n < transform.childCount; ++n)// パネル枚数
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    // プレイヤーならスルー
                    if (transform.GetChild(n).GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        continue;

                    // エネミーならスルー
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
                    
                    // 子オブジェクトのギミックの回転関数呼び出し
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
    public void TurnOver(bool isFront, Vector2Int direction, bool isScan = true)
    {
        // 最初に呼び出された時は回転させるべきブロックを一挙に調べる
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
            // プレイヤーの向きによって回転軸を変える
            Vector3 rotAxis = Vector3.zero;
            if (direction.x != 0)
                rotAxis = Vector3.forward;
            else if (direction.y != 0)
                rotAxis = Vector3.right;
            _TurnOverAxis = rotAxis;
            _isTurnOverAnim = true;
            //this.transform.Rotate(rotAxis, 180);

            for (int n = 0; n < transform.childCount; ++n)// パネル枚数
            {
                for (int i = 0; i < transform.GetChild(n).childCount; ++i)
                {
                    // プレイヤーならスルー
                    if (transform.GetChild(n).GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                        continue;

                    // エネミーならスルー
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

                    // 子オブジェクトのギミックの回転関数呼び出し
                    transform.GetChild(n).GetChild(i).GetComponent<GimmicControl>().TurnOver(rotAxis);
                }
            }

            // 子オブジェクト順番を入れ替える
            transform.GetChild(1).transform.SetSiblingIndex(0);


            GameManagerScript gameManagerScript = _GameManager.GetComponent<GameManagerScript>();

            // プレイヤー、エネミーの表裏を変える関数を呼び出す
            // ここに書いてあるスクリプト、関数で用意してもらえるとコメントアウトだけで済むので助かる
            gameManagerScript.GetPlayer().GetComponent<PlayerControl>().TurnOverMySelf(gameObject.GetComponent<BlockConfig>().GetBlockLocalPosition(), rotAxis);
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

        // 配列要素入れ替え処理
        // ゲームマネージャー内の配列入れ替え
        gameManagerScript.SwapBlockArray(targetBlock);

        // それぞれのブロックのローカルポジションを入れ替え
        Vector2Int localTemp = targetBlock[targetBlock.Count - 1].GetComponent<BlockConfig>().GetBlockLocalPosition();
        for (int n = targetBlock.Count - 1; n > 0; --n) 
        {
            targetBlock[n].GetComponent<BlockConfig>().SetBlockLocalPosition(targetBlock[n - 1].GetComponent<BlockConfig>().GetBlockLocalPosition());
        }
        targetBlock[0].GetComponent<BlockConfig>().SetBlockLocalPosition(localTemp);


        // ブロックのグローバル座標を入れ替える
        for (int n = targetBlock.Count - 1; n > 0; --n)
        {
            targetBlock[n].GetComponent<BlockControl>().SetisSwapAnim();
            targetBlock[n].GetComponent<BlockControl>().SetSwapGlobalPosition(targetBlock[n - 1].transform.position);
            targetBlock[n].GetComponent<BlockControl>().SetStartGlobalPosition();
        }
        targetBlock[0].GetComponent<BlockControl>().SetisSwapAnim();
        targetBlock[0].GetComponent<BlockControl>().SetSwapGlobalPosition(targetBlock[targetBlock.Count - 1].transform.position);
        targetBlock[0].GetComponent<BlockControl>().SetStartGlobalPosition();

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
        Vector2Int blockLocalPosition = transform.GetComponent<BlockConfig>().GetBlockLocalPosition();
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
                breakResult = transform.GetChild(isFront ? 0 : 1).GetComponent<PanelControl>().BreakWall(objectPosition, blockLocalPosition, direction, lv);
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
    
    // エネミーの持つPanelSwapAnimフラグを上げ下げする
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
    public float GetTuenOverAnimTime() { return _TurnOverAnimTime; } //カメラワークに必要
    public float GetAnimPassedTime() { return _PassedTime; } //アニメーションの経過時間を渡す関数
}
