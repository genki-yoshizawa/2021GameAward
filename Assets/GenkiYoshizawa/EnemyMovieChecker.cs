using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovieChecker : SingletonMonoBehaviour<EnemyMovieChecker>
{
    private bool _isPowerDownMovie = false;
    private bool _isPowerUpMovie = false;

    private bool _isMovie = false;
        
    public void SetIsPowerDownMovie() { _isPowerDownMovie = true; }
    public void SetIsPowerUpMovie() { _isPowerUpMovie = true; }
    public void SetIsMovie(bool movie) { _isMovie = movie; }

    public bool GetIsPowerDownMovie() { return _isPowerDownMovie; }
    public bool GetIsPowerUpMovie() { return _isPowerUpMovie; }
    public bool GetIsMovie() { return _isMovie; }

    override protected void Awake()
    {
        // �q�N���X��Awake���g���ꍇ��
        // �K���e�N���X��Awake��Call����
        // ������GameObject�ɃA�^�b�`����Ȃ��悤�ɂ��܂�.
        base.Awake();
    }
}
