using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //GameManager
    private GameManagerScript _GameManagerScript;
    //CommandUI
    private CommandUI _CommandUI;

    [Header("ターンが進んでいるかの確認用")]
    [SerializeField] private int _TurnCount = 0;

    private int _TurnTiredLimit;

    [Header("敵の行動までの待ち時間を入れてください"), SerializeField]
    private float _WaitTime;

    //最大ターン数
    [SerializeField]
    private int _TurnLimit;

    private bool _PlayerTurn = true;
    private bool _EnemyTurn  = false;
    private bool _BlockTurn  = false;

    private PlayerControl       _PlayerScript;
    private List<EnemyControl>  _EnemyScript = new List<EnemyControl>();
    private List<BlockControl>  _BlockScript = new List<BlockControl>();

    void Start()
    {
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();
        _CommandUI = GameObject.FindGameObjectWithTag("Command").GetComponent<CommandUI>();

        _PlayerScript = _GameManagerScript.GetPlayer().GetComponent<PlayerControl>();

        GameObject[][] blocks = _GameManagerScript.GetBlocks();
        List<GameObject> enemys = _GameManagerScript.GetEnemys();

        _TurnLimit = StageManager._MaxTurn;
        _TurnTiredLimit = StageManager._1Star;

        //ブロックのスクリプト取得
        foreach (var blocklist in blocks)
        {
            foreach (var block in blocklist)
            {
                if(block != null)
                    _BlockScript.Add(block.GetComponent<BlockControl>());
            }
        }

        //エネミーのスクリプト取得
        foreach (var enemy in enemys)
        {
            _EnemyScript.Add(enemy.GetComponent<EnemyControl>());
        }
    }

    void Update()
    {
        if (_GameManagerScript.GetIsMovie() || _GameManagerScript.GetCamera().GetComponent<MainCameraScript>().GetIsGameStartCameraWork())
            return;

        if (_PlayerTurn)
        {
            //時間切れ
            if (_TurnLimit <= _TurnCount)
                _PlayerScript.SetIsExist(false);

            //死んでいたらターンは渡さない
            if (!_PlayerScript.GetIsExist())
            {
                _PlayerTurn = false;
                return;
            }

            //ターン数が少なくなると疲れる
            if (_TurnTiredLimit <= _TurnCount )
                _PlayerScript.SetTired(true);

            //コマンド選択
            //_CommandUI.CommandSelect();

            if (_PlayerScript.PlayerTurn())
            {
                _PlayerTurn = false;
                if (_EnemyScript.Count > 0)
                    _EnemyTurn = true;
                else
                    _BlockTurn = true;
            }
        }
        else if (_EnemyTurn)
        {
            StartCoroutine("EnemyTurn");
            _EnemyTurn = false;
        }
        else if (_BlockTurn)
        {
            foreach (var blockScript in _BlockScript)
                blockScript.BlockTurn();

            _BlockTurn = false;
            _PlayerTurn = true;
            _TurnCount++;
        }
    }

    public int GetTurnCount() { return _TurnCount; }

    public void SetTurnCount(int turn) { _TurnCount = turn; }

    public int GetTurnLimit() { return _TurnLimit; }

    public void SetTurnLimit(int limit) { _TurnLimit = limit; }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.75f);

        foreach (var enemy in _EnemyScript)
            enemy.EnemyTurn();

        _BlockTurn = true;
    }
}
