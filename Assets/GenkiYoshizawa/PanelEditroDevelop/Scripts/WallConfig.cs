using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallConfig : MonoBehaviour
{
    [Header("壁レベルの設定")]
    [SerializeField] private int _WallLevel = 1;
    [Header("壁復活ターンの設定")]
    [SerializeField] private int _WallRebornTurn = 1;
    [Header("デフォルトの壁の位置(片方は必ず0、両方0はダメ)")]
    [SerializeField, TooltipAttribute("X軸の移動"), Range(-1, 1)] private int _XAxis = 0;
    [SerializeField, TooltipAttribute("Z軸の移動"), Range(-1, 1)] private int _ZAxis = 0;
    [Header("パネル中心からどれだけずらすか(プランナーは弄らなくてもいい)")]
    [SerializeField] private float _LocalOffset = 0.45f;

    // 壁の向き
    private Vector2 _Direction = new Vector3(1f, 0f);
    // 壁の破壊フラグ
    private bool _isBreak = false;
    private int _BreakCount = 0;
    // 一時的な措置用の変数
    private Vector3 _Scale;

    private void Awake()
    {
        if (_XAxis != 0 && _ZAxis != 0 || _XAxis == 0 && _ZAxis == 0)
        {
            Debug.LogError(gameObject.name + ":デフォルトの壁の位置が不正です。");
            //UnityEditor.EditorApplication.isPlaying = false;
        }

        transform.localPosition = new Vector3(_XAxis * _LocalOffset, transform.localPosition.y, _ZAxis * _LocalOffset);
    }

    // Start is called before the first frame update
    void Start()
    {
        _isBreak = false;
        _BreakCount = 0;

        Vector3 direction = new Vector3(1f, 0f, 0f);

        float angle = 0f;

        if(_XAxis == 0)
        {
            if (_ZAxis > 0) angle = -90;
            if (_ZAxis < 0) angle = 90;
        }
        else
        {
            if (_XAxis < 0) angle = 180; 
        }

        direction = Quaternion.Euler(0f, angle, 0f) * direction;
        direction = Quaternion.Euler(-transform.parent.eulerAngles) * direction;

        _Direction = new Vector2(direction.x, direction.z);

        transform.Rotate(Vector3.up, angle);

        // 一時的な措置用の処理
        _Scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BreakWall()
    {
        _isBreak = true;
        _BreakCount = 0;
        // 一時的な措置
        //transform.localScale = new Vector3(0f, 0f, 0f);
        transform.GetComponent<WallControl>().SetisBreakAnim();
    }

    private void RebornWall()
    {
        _isBreak = false;
        //transform.localScale = _Scale;
        transform.GetComponent<WallControl>().SetisRebornAnim();
    }

    public void AddBreakCount()
    {
        _BreakCount += 1;
        if (_BreakCount >= _WallRebornTurn) RebornWall();
    }

    public Vector2 GetDirection() { return _Direction; }
    public bool GetIsBreak() { return _isBreak; }
    public int GetWallLevel() { return _WallLevel; }
    public int GetBreakCount() { return _BreakCount; }
    public void SetDirection(Vector2 directiron) { _Direction = directiron; }
}
