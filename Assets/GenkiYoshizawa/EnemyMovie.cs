using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EnemyMovie : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _Player;
    /// <summary>
    /// �Đ�
    /// </summary>
    public void Play()
    {
        _Player.Play();
    }

    /// <summary>
    /// �ꎞ��~
    /// </summary>
    public void Pause()
    {
        _Player.Pause();
    }
    /// <summary>
    /// ��~
    /// </summary>
    public void Stop()
    {
        _Player.Stop();
    }

    public void SetPlayer(VideoPlayer player) { _Player = player;}
}
