using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    [Header("ジャンプの高さ")]
    [SerializeField] private float JumpForce = 300.0f;    // ジャンプ時に加える力

    [Header("左右移動速度")]
    [SerializeField] private float RunSpeed = 2.0f;       // 走っている間の速度

    [Header("プレイヤーＨＰ")]
    [SerializeField] private int PlayerHp = 5;                //プレイヤーのＨＰ

    [Header("地面に触れているかどうか")]
    [SerializeField] private bool isGround;               // 地面と接地しているか管理するフラグ

    private Vector3 StartPosition;                        // 初期位置
    private Vector3 PlayerPosition;                       // 現在位置

    // Start is called before the first frame update
    void Start()
    {
        StartPosition = this.transform.position;
        this.rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGround)
            PlayerPosition = this.transform.position;

        if (PlayerPosition.y < -15)
            this.transform.position = StartPosition;

        Jump();

        Move();                 // 入力に応じて移動する
    }



    void Move()
    {
        float x;                                      // 左右の入力管理

        x = Input.GetAxisRaw("Horizontal");


        this.transform.position += new Vector3(RunSpeed * Time.deltaTime * x, 0, 0);

    }

    void Jump()
    {
        // 接地している時にSpaceキー押下でジャンプ
        if (isGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.rb.AddForce(transform.up * this.JumpForce);
                //    isGround = false;
            }
        }
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
