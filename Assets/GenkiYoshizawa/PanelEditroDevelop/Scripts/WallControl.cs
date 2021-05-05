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

    //public override void TrunEndUpdate()
    //{

    //}

    public override void TurnOver()
    {

    }

    public override void Rotate(float angle)
    {
        WallConfig config = transform.GetComponent<WallConfig>();

        Vector3 direction = new Vector3(config.GetDirection().x, 0f, config.GetDirection().y);
        
        direction = Quaternion.Euler(0f, angle, 0f) * direction;

        config.SetDirection(new Vector2(direction.x, direction.z));

    }

    public override bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        Vector2 wallDirection = transform.GetComponent<WallConfig>().GetDirection();

        // ���s���� a1b2 - a2b1 = 0
        if ((wallDirection.x * direction.y - wallDirection.y * direction.x) != 0)//���g�̌����ƃI�u�W�F�N�g�̌��������s�łȂ��Ȃ�
            return true;

        // �����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ���ǂ����ŕ���
        if (objectPosition == panelPosition)//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���ꏏ
        {
            if (wallDirection == direction) return false; //�I�u�W�F�N�g�����ƕǌ������ꏏ
            else return true;                             //�I�u�W�F�N�g�����ƕǌ������قȂ�

        }
        else//�����̃p�l���ʒu�ƃI�u�W�F�N�g�p�l���ʒu���قȂ�
        {
            if (wallDirection == direction) return true; //�I�u�W�F�N�g�����ƕǌ������ꏏ
            else return false;                           //�I�u�W�F�N�g�����ƕǌ������قȂ�
        }
    }
}
