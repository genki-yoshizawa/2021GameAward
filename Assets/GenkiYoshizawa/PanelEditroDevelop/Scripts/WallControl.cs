using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallControl : GimmicControl
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
