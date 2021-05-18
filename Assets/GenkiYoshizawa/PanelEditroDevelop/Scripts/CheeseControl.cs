using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseControl : GimmicControl
{
    private List<GameObject> _Enemys = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("Manager");
        _Enemys = gameManager.GetComponent<GameManagerScript>().GetEnemys();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool HitEnemy(Vector2Int enemyPos, int enemyRange)
    {
        Vector2Int enemyDirection = enemyPos - transform.GetComponent<CheeseConfig>().GetCheeseLocalPosition();
        int serchRange = transform.GetComponent<CheeseConfig>().GetRange() + enemyRange;

        // �G�l�~�[�ƃ`�[�Y�̂��݂��̔��a�̘a����΋�����菬������΃q�b�g
        if (Mathf.Abs(enemyDirection.x) > serchRange || Mathf.Abs(enemyDirection.y) > serchRange)
            return false;

        return true;
    }

    public override void TurnEndUpdate()
    {
        //foreach (GameObject enemy in _Enemys)
        //{
        //    EnemyControl enemyScript = enemy.GetComponent<EnemyControl>();

        //    if (HitCheese(enemyScript.GetLocalPosition(), GetCheeseSearchRange()))
        //    {
        //        enemyScript.SetCheese(gameObject);
        //    }
        //}
    }
}
