using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private GameManagerScript _GameManagerScript;

    [Header("ターンが進んでいるかの確認用")]
    [SerializeField] private int _TurnCount = 0;

    [Header("疲れるまでのターン数をいれてください")]
    [SerializeField] private int _TurnTiredLimit = 5;

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

        _PlayerScript = _GameManagerScript.GetPlayer().GetComponent<PlayerControl>();

        GameObject[][] blocks = _GameManagerScript.GetBlocks();
        List<GameObject> enemys = _GameManagerScript.GetEnemys();

        //ターン数取得できないなんで
        _TurnLimit = StageManager._MaxTurn;

        //ブロックのスクリプト取得
        foreach(var blocklist in blocks)
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
        if (_PlayerTurn)
        {
            if (!_PlayerScript.GetIsExist() || _TurnLimit <= _TurnCount)
                return;

            //ターン数が少なくなると疲れる
            if (_TurnLimit < _TurnTiredLimit)
                _PlayerScript.SetTired(true);

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
            foreach (var enemy in _EnemyScript)
                enemy.EnemyTurn();

            _EnemyTurn = false;
            _BlockTurn = true;
        }
        else if (_BlockTurn)
        {
            foreach (var blockScript in _BlockScript)
                blockScript.BlockTurn();

            _BlockTurn = false;
            _PlayerTurn = true;
            _TurnCount++;
            _TurnLimit--;
        }
    }

    public int GetTurnCount() { return _TurnCount; }

    public void SetTurnCount(int turn) { _TurnCount = turn; }

    public int GetTurnLimit() { return _TurnLimit; }

    public void SetTurnLimit(int limit) { _TurnLimit = limit; }
}
