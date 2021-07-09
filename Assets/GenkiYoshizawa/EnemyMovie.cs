using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EnemyMovie : MonoBehaviour
{
    [SerializeField]
    private VideoPlayer _Player;
    /// <summary>
    /// çƒê∂
    /// </summary>
    public void Play()
    {
        _Player.Play();
    }

    /// <summary>
    /// àÍéûí‚é~
    /// </summary>
    public void Pause()
    {
        _Player.Pause();
    }
    /// <summary>
    /// í‚é~
    /// </summary>
    public void Stop()
    {
        _Player.Stop();
    }

    public void SetPlayer(VideoPlayer player) { _Player = player;}
}
