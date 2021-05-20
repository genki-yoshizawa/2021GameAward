using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseControl : GimmicControl
{
    private List<GameObject> _Enemys = null;

    // 食べられるアニメーションに必要な変数
    [Header("食べられるアニメーションにかける秒数")]
    [SerializeField] private float _EatenAnimTime = 1.0f;
    private bool _isEatenAnim = false;
    private Vector3 _StartLocalScale = new Vector3(1.0f, 1.0f, 1.0f);

    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("Manager");
        _Enemys = gameManager.GetComponent<GameManagerScript>().GetEnemys();

        _isEatenAnim = false;
        _StartLocalScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        if (_isEatenAnim)
        {
            float time = Time.deltaTime;
            if (_PassedTime + time > _EatenAnimTime)
            {
                time = _EatenAnimTime - _PassedTime;
                _PassedTime = 0.0f;
                _isEatenAnim = false;
                Destroy(gameObject);
            }

            Vector3 scale = _StartLocalScale * (time / _EatenAnimTime);
            transform.localScale = transform.localScale - scale;
        }

    }

    private bool HitEnemy(Vector2Int enemyPos, int enemyRange)
    {
        Vector2Int enemyDirection = enemyPos - transform.GetComponent<CheeseConfig>().GetCheeseLocalPosition();
        int serchRange = transform.GetComponent<CheeseConfig>().GetRange() + enemyRange;

        // エネミーとチーズのお互いの半径の和が絶対距離より小さければヒット
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

    public void Eaten()
    {
        _isEatenAnim = true;
    }
}
