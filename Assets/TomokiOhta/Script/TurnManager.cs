using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //GameManager
    private GameManagerScript _GameManagerScript;
    //CommandUI
    private CommandUI _CommandUI;

    [Header("�^�[�����i��ł��邩�̊m�F�p")]
    [SerializeField] private int _TurnCount = 0;

    private int _TurnTiredLimit;

    [Header("�G�̍s���܂ł̑҂����Ԃ����Ă�������"), SerializeField]
    private float _WaitTime;

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
        _CommandUI = GameObject.FindGameObjectWithTag("Command").GetComponent<CommandUI>();

        _PlayerScript = _GameManagerScript.GetPlayer().GetComponent<PlayerControl>();

        GameObject[][] blocks = _GameManagerScript.GetBlocks();
        List<GameObject> enemys = _GameManagerScript.GetEnemys();

        _TurnLimit = StageManager._MaxTurn;
        _TurnTiredLimit = StageManager._1Star;

        //�u���b�N�̃X�N���v�g�擾
        foreach (var blocklist in blocks)
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
        if (_GameManagerScript.GetIsMovie() || _GameManagerScript.GetCamera().GetComponent<MainCameraScript>().GetIsGameStartCameraWork())
            return;

        if (_PlayerTurn)
        {
            //���Ԑ؂�
            if (_TurnLimit <= _TurnCount)
                _PlayerScript.SetIsExist(false);

            //����ł�����^�[���͓n���Ȃ�
            if (!_PlayerScript.GetIsExist())
            {
                _PlayerTurn = false;
                return;
            }

            //�^�[���������Ȃ��Ȃ�Ɣ���
            if (_TurnTiredLimit <= _TurnCount )
                _PlayerScript.SetTired(true);

            //�R�}���h�I��
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

    // �g�V���C
    // EnemyTurn��bool�l�ɂ��ăG�l�~�[�^�[�����I���܂ő҂悤�ɏ������e�����������܂���(2021/7/17)
    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(0.75f);

        bool enemyTurnEnd = false;

        while (!enemyTurnEnd)
        {
            enemyTurnEnd = true;
            foreach (var enemy in _EnemyScript)
            {
                if (!enemy.EnemyTurn())
                    enemyTurnEnd = false;
            }
            yield return null;
        }

        _BlockTurn = true;
    }
}
