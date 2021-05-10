using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private GameManagerScript _GameManagerScript;
    private bool _PlayerTurn;
    private bool _EnemyTurn;
    private bool _BlockTurn;

    private PlayerControl _PlayerScript;
    private EnemyControl  _EnemyScript;
    private BlockControl  _BlockScript;

    void Start()
    {
        _GameManagerScript = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManagerScript>();

        _PlayerTurn = false;
        _EnemyTurn  = false;
        _BlockTurn  = false;

        _PlayerScript = _GameManagerScript.GetPlayer().GetComponent<PlayerControl>();
        //_EnemyScript  = _GameManagerScript.GetPlayer().GetComponent<EnemyControl>();
        //_BlockScript  = _GameManagerScript.GetPlayer().GetComponent<BlockControl>();
    }

    void Update()
    {
        if (_PlayerTurn)
        {
            if (_PlayerScript.PlayerTurn())
            {
                _PlayerTurn = false;
                _EnemyTurn = true;
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
            //var blockScript = _GameManagerScript.GetPlayer().GetComponent<BlockControl>();
            //_EnemyTurn = blockScript.BlockTurn();
            //if (_BlockScript.BlockTurn())
            //{
            //    _BlockTurn = false;
            //    _PlayerTurn = true;
            //}
        }
    }
}
