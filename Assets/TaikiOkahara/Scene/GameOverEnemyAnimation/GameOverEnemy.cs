using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class GameOverEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    Animator _EnemyAnimaiton;
    AnimatorController _GameOverAnimaiton;

    Vector3 _EnemyRotation;

    bool _TranslateStart = false;
    float _RotateCount = 0;

    int _Direction = 1;//正の方向に移動するか、負の方向に移動するか

    private float _MoveDistance = 0.075f * 2;

    void Start()
    {
        RuntimeAnimatorController asset = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/TaikiOkahara/Scene/GameOverEnemyAnimation/GameOverAnimationController.controller");
        _GameOverAnimaiton = asset as AnimatorController;

        _EnemyAnimaiton = this.GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        TranslateEnemyPosition();

        if(Input.GetKeyDown(KeyCode.T))
        {
            StartGameOverEnemyAnimation();   
        }
    }

    private IEnumerator GameOverAnimation()
    {
        Vector3 rot = this.transform.rotation.eulerAngles;

        while(_RotateCount <= 180.0f)
        {
            rot.x += -1.0f;
            this.transform.rotation = Quaternion.Euler(rot);
            _RotateCount += 1;
            Debug.Log("回転中");
            yield return null;
        }

        //yield return null;

        if(this.transform.position.y <= 0)
        {
            _Direction = 1;
        }
        else
        {
            _Direction = -1;
        }

        Debug.Log("回転のコルーチン終了");

        Debug.Log("移動開始");
        _TranslateStart = true;

        // 2秒待つ  
        yield return new WaitForSeconds(1.0f);

        _TranslateStart = false;
        _EnemyAnimaiton.SetBool("Stay", true);

        yield return new WaitForSeconds(2.5f);

        _EnemyAnimaiton.enabled = false;
    }

    private void TranslateEnemyPosition()
    {
        if (!_TranslateStart) return;

        this.transform.Translate(0, _MoveDistance/(60) * _Direction, 0);
    }

    public void StartGameOverEnemyAnimation()
    {
        _EnemyAnimaiton.runtimeAnimatorController = _GameOverAnimaiton;


        StartCoroutine("GameOverAnimation");
    }
}
