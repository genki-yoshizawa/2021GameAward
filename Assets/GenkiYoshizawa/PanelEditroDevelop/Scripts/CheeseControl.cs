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
    private Vector3 _StartGlobalPosition = new Vector3(0.0f, 0.0f, 0.0f);

    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("Manager");
        _Enemys = gameManager.GetComponent<GameManagerScript>().GetEnemys();

        _isEatenAnim = false;
        _StartLocalScale = transform.localScale;
        _StartGlobalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isEatenAnim)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _EatenAnimTime)
            {
                _PassedTime = _EatenAnimTime;
                _isEatenAnim = false;
                Destroy(gameObject);
            }

            transform.position = _StartGlobalPosition + (new Vector3(_StartGlobalPosition.x, 0.0f, _StartGlobalPosition.z) - _StartGlobalPosition) * (_PassedTime / _EatenAnimTime);

            transform.localScale = _StartLocalScale + (Vector3.zero - _StartLocalScale) * (_PassedTime / _EatenAnimTime);
        }

    }

    private bool HitEnemy(Vector2Int enemyPos, int enemyRange, bool isFront)
    {
        Vector2Int enemyDirection = enemyPos - transform.GetComponent<CheeseConfig>().GetCheeseLocalPosition();
        int serchRange = transform.GetComponent<CheeseConfig>().GetRange() + enemyRange;

        bool front = transform.parent == transform.parent.parent.GetChild(0);

        // エネミーとチーズのお互いの半径の和が絶対距離より小さければヒット
        if ((Mathf.Abs(enemyDirection.x) > serchRange || Mathf.Abs(enemyDirection.y) > serchRange) || isFront != front)
            return false;

        return true;
    }

    public override void TurnEndUpdate()
    {
        foreach (GameObject enemy in _Enemys)
        {
            EnemyControl enemyScript = enemy.GetComponent<EnemyControl>();

            if (HitEnemy(enemyScript.GetLocalPosition(), enemyScript.GetCheeseSearchRange(), enemyScript.GetIsFront()))
            {
                enemyScript.SetCheese(gameObject);
            }
        }
    }

    public void Eaten()
    {
        _isEatenAnim = true;
    }

    public override bool IsWall() { return false; }
    public override bool IsCheese() { return true; }
}
