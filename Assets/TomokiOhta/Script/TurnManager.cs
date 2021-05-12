using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private GameManagerScript _GameManagerScript;
    private bool _PlayerTurn = true;
    private bool _EnemyTurn  = false;
    private bool _BlockTurn  = false;

    private PlayerControl       _PlayerScript;
    //private EnemyControl      _EnemyScript;
    private List<BlockControl>  _BlockScript = new List<BlockControl>();

    void Start()
    {
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();

        _PlayerScript = _GameManagerScript.GetPlayer().GetComponent<PlayerControl>();
        //_EnemyScript  = _GameManagerScript.GetPlayer().GetComponent<EnemyControl>();

        GameObject[][] blocks = _GameManagerScript.GetBlocks();

        if (blocks == null)
            Debug.Log("ƒLƒƒƒ“ƒZƒ‹");


        foreach(var blocklist in blocks)
        {
            foreach (var block in blocklist)
            {
                if(block != null)
                    _BlockScript.Add(block.GetComponent<BlockControl>());
            }
        }
    }

    void Update()
    {
        if (_PlayerTurn)
        {
            if (_PlayerScript.PlayerTurn())
            {
                _PlayerTurn = false;
                //_EnemyTurn = true;
                _BlockTurn = true;
            }
        }
        else if (_EnemyTurn)
        {
            //var enemyScript = _GameManagerScript.GetPlayer().GetComponent<EnemyControl>();
            //_EnemyTurn = enemyScript.EnemyTurn();
            //if (_EnemyScript.EnemyTurn())
            //{
            //    _EnemyTurn = false;
            //    _BlockTurn = true;
            //}
        }
        else if (_BlockTurn)
        {
            foreach (var blockScript in _BlockScript)
                blockScript.BlockTurn();

            _BlockTurn = false;
            _PlayerTurn = true;
        }
    }
}
