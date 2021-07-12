using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GameManagerScript : MonoBehaviour
{
    [Header("ステージ上のTurnManagerオブジェクトをセットしてください(Hierarchy上のTurnManagerオブジェクト)")]
    [SerializeField] private GameObject _TurnManager;
    [Header("ステージ上のPlayerオブジェクトをセットしてください(Hierarchy上のPlayerオブジェクト)")]
    [SerializeField] private GameObject _Player;
    [Header("ステージ上のEnemyオブジェクトの数を入れた後それぞれのElementにセットしてください(Hierarchy上のEnemyオブジェクト)")]
    [SerializeField] private List<GameObject> _Enemy;
    [Header("ステージ上のUIFolderオブジェクトをセットしてください(Hierarchy上のUIFolderオブジェクト)")]
    [SerializeField] private GameObject _GameUI;
    [Header("ステージ上のCameraWorkオブジェクトをセットしてください(Hierarchy上のUIFolderオブジェクト)")]
    [SerializeField] private GameObject _CameraWork;

    [Header("プレハブのエネミーパワーダウンムービーをセットしてください")]
    [SerializeField] private GameObject _EnemyPowerDownMovie;
    [Header("プレハブのエネミーパワーアップムービーをセットしてください")]
    [SerializeField] private GameObject _EnemyPowerUpMovie;

    // ブロックの配列
    private GameObject[][] _Block;

    private void Awake()
    {
        AssignBlockArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject enemy in _Enemy)
        {
            enemy.GetComponent<EnemyControl>().SetLocalPosition(enemy.transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition());
            enemy.GetComponent<EnemyControl>().SetIsFront(enemy.transform.parent == enemy.transform.parent.parent.GetChild(0) ? true : false);
        }
        _Player.GetComponent<PlayerControl>().SetLocalPosition(_Player.transform.parent.parent.GetComponent<BlockConfig>().GetBlockLocalPosition());
        _Player.GetComponent<PlayerControl>().SetIsFront(_Player.transform.parent == _Player.transform.parent.parent.GetChild(0) ? true : false);

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
    //public void SwapBlockArray(Vector2Int block1, Vector2Int block2)
    //{
    //    GameObject temp = _Block[block1.x][block1.y];
    //    _Block[block1.x][block1.y] = _Block[block2.x][block2.y];
    //    _Block[block2.x][block2.y] = temp;
    //}

    // ソート済みのGameObjectリスト配列を受け取り、SwapIndex順に入れ替える
    public void SwapBlockArray(List<GameObject> swapList)
    {// 多分これでいいはず・・・
        GameObject temp = _Block[swapList[0].GetComponent<BlockConfig>().GetBlockLocalPosition().x][swapList[0].GetComponent<BlockConfig>().GetBlockLocalPosition().y];
        for (int n = 0; n < swapList.Count - 1; ++n)
        {
            _Block[swapList[n].GetComponent<BlockConfig>().GetBlockLocalPosition().x][swapList[n].GetComponent<BlockConfig>().GetBlockLocalPosition().y] = _Block[swapList[n + 1].GetComponent<BlockConfig>().GetBlockLocalPosition().x][swapList[n + 1].GetComponent<BlockConfig>().GetBlockLocalPosition().y];
        }
        _Block[swapList[swapList.Count - 1].GetComponent<BlockConfig>().GetBlockLocalPosition().x][swapList[swapList.Count - 1].GetComponent<BlockConfig>().GetBlockLocalPosition().y] = temp;
    }

    public GameObject GetTurnManager() { return _TurnManager; }
    public GameObject GetPlayer() { return _Player; }                               //プレイヤーの取得
    public List<GameObject> GetEnemys() { return _Enemy; }                          //エネミーの全取得
    public GameObject GetEnemy(int index) { return _Enemy[index]; }                 //index番のエネミーの取得
    public GameObject[][] GetBlocks() { return _Block; }                    //ブロックの全取得
    public GameObject GetBlock(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 // 負の配列は存在しないのでnullを返す
            || pos.x >= _Block.Length || pos.y >= _Block[pos.x].Length) // 配列の要素を超えた値はnullを返す
            return null;

        return _Block[pos.x][pos.y];
    }     //posにあるBlockの取得
    public GameObject GetCamera() { return _CameraWork.transform.GetChild(0).gameObject; }

    public void KillEnemy(GameObject enemy)
    {
        enemy.GetComponent<EnemyControl>().SetDestroy();
        _Enemy.Remove(enemy);
    }

    public void SetPause()
    {
        _TurnManager.GetComponent<TurnManager>().enabled = false;
        _GameUI.SetActive(false);
        GetCamera().transform.GetComponent<MainCameraScript>().enabled = false;
    }

    public void SetUnPause()
    {
        _TurnManager.GetComponent<TurnManager>().enabled = true;
        _GameUI.SetActive(true);
        GetCamera().transform.GetComponent<MainCameraScript>().enabled = true;
    }
    
    public void SetClear()
    {
        _TurnManager.GetComponent<TurnManager>().enabled = false;
        _GameUI.SetActive(false);
    }

    public void StartEnemyMovie(bool isFront)
    {
        if (isFront && !EnemyMovieChecker.Instance.GetIsPowerDownMovie() && EnemyMovieChecker.Instance.GetIsPowerUpMovie())// シングル音から条件判定
        {
            StartCoroutine(PowerDownEnemyMovie());
        }
        else if (!isFront && !EnemyMovieChecker.Instance.GetIsPowerUpMovie())// シングルトンから条件判定
        {
            StartCoroutine(PowerUpEnemyMovie());
        }
    }

    private void EnemyIsMovieDown(VideoPlayer vp) { EnemyMovieChecker.Instance.SetIsMovie(false); }

    IEnumerator PowerUpEnemyMovie()
    {
        Time.timeScale = 0f;
        // ポーズスタート
        Debug.Log("パワーアップムービー始め");
        EnemyMovieChecker.Instance.SetIsMovie(true);
        EnemyMovieChecker.Instance.SetIsPowerUpMovie();

        GameObject movieObj = Instantiate(_EnemyPowerUpMovie);
        movieObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        movieObj.transform.parent = _GameUI.transform;
        movieObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

        movieObj.GetComponent<VideoPlayer>().loopPointReached += EnemyIsMovieDown;
        movieObj.GetComponent<EnemyMovie>().SetPlayer(movieObj.GetComponent<VideoPlayer>());

        movieObj.GetComponent<EnemyMovie>().Play();

        while (EnemyMovieChecker.Instance.GetIsMovie())
        {
            yield return null;
        }

        Destroy(movieObj);

        Debug.Log("パワーアップムービー終了");

        // ポーズ終了
        Time.timeScale = 1f;
    }

    IEnumerator PowerDownEnemyMovie()
    {
        Time.timeScale = 0f;
        // ポーズスタート
        Debug.Log("パワーダウンムービー始め");
        EnemyMovieChecker.Instance.SetIsMovie(true);
        EnemyMovieChecker.Instance.SetIsPowerDownMovie();

        GameObject movieObj = Instantiate(_EnemyPowerDownMovie);
        movieObj.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        movieObj.transform.parent = _GameUI.transform;
        movieObj.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);

        movieObj.GetComponent<VideoPlayer>().loopPointReached += EnemyIsMovieDown;
        movieObj.GetComponent<EnemyMovie>().SetPlayer(movieObj.GetComponent<VideoPlayer>());

        movieObj.GetComponent<EnemyMovie>().Play();

        while (EnemyMovieChecker.Instance.GetIsMovie())
        {
            yield return null;
        }

        Destroy(movieObj);
        Debug.Log("パワーダウンムービー終了");

        // ポーズ終了
        Time.timeScale = 1f;
    }
}
