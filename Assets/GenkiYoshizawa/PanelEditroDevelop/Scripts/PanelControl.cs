using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// パネルの操作を行うクラス
public class PanelControl : MonoBehaviour
{
    private GameObject _GameManager = null;

    // マーカーUIで必要な変数
    private bool _isMarkerCreate = false;
    private GameObject _Marker;
    [Header("Prefabから設定するマーカーのゲームオブジェクト(プランナーはいじる必要なし)")]
    [SerializeField] private GameObject _MarkerPrefab;

    // 光らせる方で必要な変数
    private bool _isBright = false;
    private MeshRenderer _MeshRenderer;


    private Renderer _Renderer;
    private Color _BaseColor;
    private int _ColorCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");

        _MeshRenderer = GetComponent<MeshRenderer>();

        _Renderer = this.GetComponent<Renderer>();
        _BaseColor = _Renderer.material.color;
    }

    private void Update()
    {

        if(_isBright)
        {
            float col = (Mathf.Sin(_ColorCount * 0.01f) + 1.0f) * 0.5f;
            _Renderer.material.color = _BaseColor + new Color(col, col, col);
            _ColorCount++;
        }
        else 
        {
            _Renderer.material.color = _BaseColor;
            _ColorCount = 0;

        }
    }

    //このパネルにつくギミック（子オブジェクト）をまとめて処理する
    public void TurnEndUpdate()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                continue;

            List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
            bool isThrow = false;
            foreach (GameObject enemy in enemys)
                if (transform.GetChild(i).gameObject == enemy)
                {
                    isThrow = true;
                    break;
                }
            if (isThrow)
                continue;

            transform.GetChild(i).GetComponent<GimmicControl>().TurnEndUpdate();
        }
    }

    public int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        int breakResult = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).gameObject == _GameManager.GetComponent<GameManagerScript>().GetPlayer())
                continue;

            List<GameObject> enemys = _GameManager.GetComponent<GameManagerScript>().GetEnemys();
            bool isThrow = false;
            foreach (GameObject enemy in enemys)
                if (transform.GetChild(i).gameObject == enemy)
                {
                    isThrow = true;
                    break;
                }
            if (isThrow)
                continue;

            breakResult = transform.GetChild(i).GetComponent<GimmicControl>().BreakWall(objectPosition, panelPosition, direction, lv);
            if (breakResult != 0) // 判定できる壁が合った
                break;
        }
        return breakResult;
    }

    public void AttentionPanel(bool attention)
    {
        _isMarkerCreate = attention;
        _isBright = attention;
    }
}
