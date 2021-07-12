using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// パネルの操作を行うクラス
public class PanelControl : MonoBehaviour
{
    [Header("マーカーか光らすか調べる用フラグ(ゲーム完成時には消す)")]
    [SerializeField] private bool _isMarker = false;
    [Header("点滅もしくは拡大の周期(秒)")]
    [SerializeField] private float _CycleTime = 1.0f;
    [Header("点滅もしくは拡大の変化の仕方(tureがサイン波、falseがのこぎり派)")]
    [SerializeField] private bool _isSinPulse = false;
    private GameObject _GameManager = null;

    // マーカーUIで必要な変数
    private bool _isMarkerCreate = false;
    private bool _isCurMarkerCreate = false;
    private GameObject _Marker;
    [Header("マーカーの最大サイズ(倍率)(最初期のマーカースケールの値0.1)")]
    [SerializeField] private float _MarkerMaxScale = 0.2f;
    [Header("マーカーの位置(プランナーはいじる必要なし)")]
    [SerializeField] private float _MarkerOffset = 0.2f;
    [Header("Prefabから設定するマーカーのゲームオブジェクト(プランナーはいじる必要なし)")]
    [SerializeField] private GameObject _MarkerPrefab;

    // 光らせる方で必要な変数
    private bool _isBright = false;
    private bool _isCurBright = false;
    private MeshRenderer _MeshRenderer;
    //private Material[] _SaveMaterials;
    [Header("Assetから設定するデフォルトマテリアル(オブジェクトに設定されているマテリアルの数)(プランナーはいじる必要なし)")]
    //[SerializeField] private Material[] _DefaultMaterial;
    // UI実行後の経過時間
    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _GameManager = GameObject.FindGameObjectWithTag("Manager");
        _isBright = _isCurBright = false;

        _MeshRenderer = GetComponent<MeshRenderer>();
        //_SaveMaterials = new Material[_MeshRenderer.materials.Length];
        for(int i = 0; i < _MeshRenderer.materials.Length; ++i)
        {
            //_SaveMaterials[i] = _DefaultMaterial[i];
            //_SaveMaterials[i].CopyPropertiesFromMaterial(_MeshRenderer.materials[i]);
            //_MeshRenderer.materials[i].CopyPropertiesFromMaterial(_SaveMaterials[i]);
        }

        _PassedTime = 0.0f;
    }

    private void Update()
    {
        //// ネストが一時的に深いけどゲーム完成時には見やすくなる（はず）
        //if (_isMarker)
        //{
        //    if (_isMarkerCreate && !_isCurMarkerCreate)
        //    {//マーカーを生成する処理
        //        // 毎回生成デリートを繰り返すは嫌なので、ステージ毎にマーカーオブジェクトをセットして、Startでオブジェクト取得。
        //        // アクティブフラグ、ポジションを切り替えて行うことにする(こっちの案になったら)
        //        _Marker = Instantiate(_MarkerPrefab);

        //        bool isFront = transform.parent.GetChild(0) == transform;
        //        // マーカーの回転と位置を合わせる
        //        if (!isFront)
        //            _Marker.transform.Rotate(Vector3.right, 180);
        //        _Marker.transform.position = transform.position + new Vector3(0.0f, isFront ? _MarkerOffset : -_MarkerOffset, 0.0f);

        //        PanelMarkerScript script = _Marker.GetComponent<PanelMarkerScript>();
        //        script.SetCycleTime(_CycleTime);
        //        script.SetMaxScale(_MarkerMaxScale);
        //        script.SetisSinPulse(_isSinPulse);
        //    }
        //    else if(!_isMarkerCreate && _isCurMarkerCreate)
        //    {//マーカーを破棄する処理
        //        Destroy(_Marker);
        //    }

        //    _isCurMarkerCreate = _isMarkerCreate;
        //}
        //else
        //{
        //    if (_isBright)
        //    {
        //        _PassedTime += Time.deltaTime;
        //        while (true)
        //        {
        //            if (_PassedTime > _CycleTime)
        //                _PassedTime -= _CycleTime;
        //            else
        //                break;
        //        }

        //        for(int i = 0; i < _MeshRenderer.materials.Length; ++i)
        //        {
        //            Color color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        //            if (_isSinPulse)
        //            {
        //                //color.r = _SaveMaterials[i].color.r + Mathf.Abs((1.0f - _SaveMaterials[i].color.r) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
        //                //color.g = _SaveMaterials[i].color.g + Mathf.Abs((1.0f - _SaveMaterials[i].color.g) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
        //                //color.b = _SaveMaterials[i].color.b + Mathf.Abs((1.0f - _SaveMaterials[i].color.b) * Mathf.Sin(Mathf.PI * _PassedTime / _CycleTime));
        //            }
        //            else
        //            {
        //                if (_PassedTime < _CycleTime * 0.5f)
        //                {
        //                    //color.r = _SaveMaterials[i].color.r + (1.0f - _SaveMaterials[i].color.r) / (_CycleTime * 0.5f) * _PassedTime;
        //                    //color.g = _SaveMaterials[i].color.g + (1.0f - _SaveMaterials[i].color.g) / (_CycleTime * 0.5f) * _PassedTime;
        //                    //color.b = _SaveMaterials[i].color.b + (1.0f - _SaveMaterials[i].color.b) / (_CycleTime * 0.5f) * _PassedTime;
        //                }
        //                else
        //                {
        //                    //color.r = 1.0f - (1.0f - _SaveMaterials[i].color.r) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
        //                    //color.g = 1.0f - (1.0f - _SaveMaterials[i].color.g) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
        //                    //color.b = 1.0f - (1.0f - _SaveMaterials[i].color.b) / (_CycleTime * 0.5f) * (_PassedTime - _CycleTime * 0.5f);
        //                }
        //            }

        //            //color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //            _MeshRenderer.materials[i].color = color;
        //        }
                
        //    }
        //    else if (!_isBright && _isCurBright)
        //    {
        //        // 直前まで発光をしていたら元に戻す
        //        for (int i = 0; i < _MeshRenderer.materials.Length; ++i)
        //        {
        //            //Debug.Log(_SaveMaterials[i]);
        //            //_MeshRenderer.materials[i].color = _SaveMaterials[i].color;
        //        }
        //        _PassedTime = 0.0f;
        //    }

        //    _isCurBright = _isBright;
        //}
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
