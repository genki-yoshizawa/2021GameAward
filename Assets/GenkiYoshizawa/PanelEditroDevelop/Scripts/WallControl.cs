using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl : GimmicControl
{
    // �j��A�j���[�V�����ɕK�v�ȕϐ�
    [Header("�u���b�N�j��A�j���[�V�����ɂ�����b��")]
    [SerializeField] private float _BreakAnimTime = 1.0f;
    private bool _isBreakAnim = false;

    // �Č��A�j���[�V�����ɕK�v�ȕϐ�
    [Header("�u���b�N�����A�j���[�V�����ɂ�����b��")]
    [SerializeField] private float _RebornAnimTime = 1.0f;
    private bool _isRebornAnim = false;

    private Vector3 _StartLocalScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _StartGlobalPosition = new Vector3(0.0f, 0.0f, 0.0f);
    private float _PassedTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _isBreakAnim = false;
        _isRebornAnim = false;

        _StartLocalScale = transform.localScale;
        _StartGlobalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBreakAnim)
        {
            float time = Time.deltaTime;
            if ( (_PassedTime += time) > _BreakAnimTime)
            {
                _PassedTime = _BreakAnimTime;
                _isBreakAnim = false;
            }

            // y���W�������Ȃ���y�T�C�Y��0�Ɍ����킹��
            transform.position = _StartGlobalPosition + (new Vector3(_StartGlobalPosition.x ,0.0f,_StartGlobalPosition.z) - _StartGlobalPosition) * (_PassedTime / _BreakAnimTime);

            transform.localScale = _StartLocalScale + (new Vector3(_StartLocalScale.x, 0.0f, _StartLocalScale.z) - _StartLocalScale) * (_PassedTime / _BreakAnimTime);

            if (!_isBreakAnim)
                _PassedTime = 0.0f;
        }

        if (_isRebornAnim)
        {
            float time = Time.deltaTime;
            if ((_PassedTime += time) > _RebornAnimTime)
            {
                _PassedTime = _RebornAnimTime;
                _isRebornAnim = false;
            }

            // y���W���グ�Ȃ���y�T�C�Y�����̃T�C�Y�ɂɌ����킹��
            //float move = (0.0f - _StartGlobalPosition.y) * (time / _BreakAnimTime);
            //transform.position = new Vector3(transform.position.x, transform.position.y - move, transform.position.z);
            Vector3 beforePos = new Vector3(_StartGlobalPosition.x, 0.0f, _StartGlobalPosition.z);
            transform.position = beforePos + (_StartGlobalPosition - beforePos) * (_PassedTime / _RebornAnimTime);

            Vector3 beforeScale = new Vector3(_StartLocalScale.x, 0.0f, _StartLocalScale.z);
            transform.localScale = beforeScale + (_StartLocalScale - beforeScale) * (_PassedTime / _RebornAnimTime);

            if (!_isRebornAnim)
                _PassedTime = 0.0f;
        }
    }

    public override void TurnEndUpdate()
    {
        WallConfig config = transform.GetComponent<WallConfig>();

        if (config.GetIsBreak()) config.AddBreakCount();
    }

    public override void Rotate(float angle)
    {
        // �����x�N�g���̉�]
        WallConfig config = transform.GetComponent<WallConfig>();

        Vector3 direction = new Vector3(config.GetDirection().x, 0f, config.GetDirection().y);
        
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        config.SetDirection(new Vector2(direction.x, direction.z));

    }

    public override void TurnOver(Vector3 rotAxis)
    {
        // �����x�N�g���̉�]
        WallConfig config = transform.GetComponent<WallConfig>();

        Vector3 direction = new Vector3(config.GetDirection().x, 0f, config.GetDirection().y);

        direction = Quaternion.Euler(rotAxis * 180) * direction;

        config.SetDirection(new Vector2(direction.x, direction.z));

    }

    public override bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // ���s���� a1b2 - a2b1 = 0
        if (transform.GetComponent<WallConfig>().GetIsBreak() || //���Ă���or
            (Mathf.RoundToInt(wallDirection.x) * Mathf.RoundToInt(direction.y) - Mathf.RoundToInt(wallDirection.y) * Mathf.RoundToInt(direction.x)) != 0)//���g�̌����ƃI�u�W�F�N�g�̌��������s�łȂ��Ȃ�
        {
            Debug.Log(wallDirection.x * direction.y - wallDirection.y * direction.x);
            Debug.Log(wallDirection.x * direction.y - wallDirection.y * direction.x != 0);
            return true;
        }
        // �����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ���ǂ����ŕ���
        if (objectPosition == panelPosition)//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ
        {
            if (wallDirection == direction)//�I�u�W�F�N�g�����ƕǌ������ꏏ
                return false;
        }
        else//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���قȂ�
        {
            if (wallDirection != direction)//�I�u�W�F�N�g�����ƕǌ������ꏏ
                return false;
        }

        return true;
    }

    public override int CheckWallLevel(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // ���s���� a1b2 - a2b1 = 0
        if (transform.GetComponent<WallConfig>().GetIsBreak() || (wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//���Ă���or���g�̌����ƃI�u�W�F�N�g�̌��������s�łȂ��Ȃ�
            return 0;

        // �����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ���ǂ����ŕ���
        if (objectPosition == panelPosition)//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ
        {
            if (wallDirection == direction)//�I�u�W�F�N�g�����ƕǌ������ꏏ
                return transform.GetComponent<WallConfig>().GetWallLevel();
        }
        else//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���قȂ�
        {
            if (wallDirection != direction)//�I�u�W�F�N�g�����ƕǌ������ꏏ
                return transform.GetComponent<WallConfig>().GetWallLevel();
        }

        return 0;
    }

    // �ǂ��󂵂���2,�󂹂Ȃ�������1,���݂��Ȃ��Ȃ�0��Ԃ�
    // enum�Ƃ��ɂ������������H�������E�E�E
    public override int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // ���s���� a1b2 - a2b1 = 0
        if (transform.GetComponent<WallConfig>().GetIsBreak() || Mathf.RoundToInt(wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//���Ă���or���g�̌����ƃI�u�W�F�N�g�̌��������s�łȂ��Ȃ�
        {
            return 0;
        }
        // �����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ���ǂ����ŕ���
        if (objectPosition == panelPosition)//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ
        {
            if (wallDirection == direction)//�I�u�W�F�N�g�����ƕǌ������ꏏ
            {
                if(transform.GetComponent<WallConfig>().GetWallLevel() > lv)
                {// �󂹂Ȃ�
                    return 1;
                }
                else
                {// �󂹂�
                    transform.GetComponent<WallConfig>().BreakWall();
                    return 2;
                }
            }
        }
        else//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���قȂ�
        {
            if (wallDirection != direction)//�I�u�W�F�N�g�����ƕǌ������ꏏ
            {
                if (transform.GetComponent<WallConfig>().GetWallLevel() > lv)
                {// �󂹂Ȃ�
                    return 1;
                }
                else
                {// �󂹂�
                    transform.GetComponent<WallConfig>().BreakWall();
                    return 2;
                }
            }
        }

        return 0;
    }

    public void SetisBreakAnim() { _isBreakAnim = true; }
    public void SetisRebornAnim() { _isRebornAnim = true; }
}
