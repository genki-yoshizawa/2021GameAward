using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private GameManagerScript _GameManagerScript;

    [Header("�^�[�����i��ł��邩�̊m�F�p")]
    [SerializeField] private int _TurnCount = 0;

    [Header("����܂ł̃^�[����������Ă�������")]
    [SerializeField] private int _TurnTiredLimit = 5;

    //�ő�^�[����
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

        //�^�[�����擾�ł��Ȃ��Ȃ��
        _TurnLimit = StageManager._MaxTurn;
        //_TurnLimit = 30;

        //�u���b�N�̃X�N���v�g�擾
        foreach(var blocklist in blocks)
        {
            foreach (var block in blocklist)
            {
                if(block != null)
                    _BlockScript.Add(block.GetComponent<BlockControl>());
            }
        }

        //�G�l�~�[�̃X�N���v�g�擾
        foreach (var enemy in enemys)
        {
            _EnemyScript.Add(enemy.GetComponent<EnemyControl>());
        }
    }

    void Update()
    {
        if (_PlayerTurn)
        {
            if (_TurnLimit <= _TurnCount)
                _PlayerScript.SetDead();

            if (!_PlayerScript.GetIsExist())
                return;

            //�^�[���������Ȃ��Ȃ�Ɣ���
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
        }
    }

    public int GetTurnCount() { return _TurnCount; }

    public void SetTurnCount(int turn) { _TurnCount = turn; }

    public int GetTurnLimit() { return _TurnLimit; }

    public void SetTurnLimit(int limit) { _TurnLimit = limit; }
}
