using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("�W�����v�̍���")]
    [SerializeField] private float JumpForce = 300.0f;    // �W�����v���ɉ������

    [Header("���E�ړ����x")]
    [SerializeField, Range(0, 5)] private float RunSpeed = 2.0f;       // �����Ă���Ԃ̑��x
    
    [Header("�d�͉����x")]
    [SerializeField] private Vector3 Gravity;       // �����Ă���Ԃ̑��x

    [Header("�v���C���[�g�o")]
    [SerializeField] private int PlayerHp = 5;                //�v���C���[�̂g�o

    [Header("�n�ʂɐG��Ă��邩�ǂ���")]
    [SerializeField] private bool isGround;               // �n�ʂƐڒn���Ă��邩�Ǘ�����t���O

    private Vector3 StartPosition;                        // �����ʒu
    private Vector3 PlayerPosition;                       // ���݈ʒu

    // Start is called before the first frame update
    void Start()
    {
        StartPosition = this.transform.position;
        this.rb = GetComponent<Rigidbody>();
        //Gravity = new Vector3(Physics.gravity);
    }

    // Update is called once per frame
    void Update()
    {

        Respawn();

        Jump();

        Move();                 // ���͂ɉ����Ĉړ�����
    }



    void Move()
    {
        float x;                                      // ���E�̓��͊Ǘ�

        x = Input.GetAxisRaw("Horizontal");


        this.transform.position += new Vector3(RunSpeed * Time.deltaTime * x, 0, 0);

    }

    void Jump()
    {
        // �ڒn���Ă��鎞��Space�L�[�����ŃW�����v
        if (isGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.rb.AddForce(transform.up * this.JumpForce);
                //    isGround = false;
            }
        }
    }

    void Respawn()
    {
        if (!isGround)
            PlayerPosition = this.transform.position;

        if (PlayerPosition.y < -15)
            this.transform.position = StartPosition;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            if (!isGround)
                isGround = true;
        }
        if (other.gameObject.tag == "Death")
        {
            this.transform.position = StartPosition;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ground")
        {
            if (isGround)
                isGround = false;
        }
    }
}
