using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MovieManager : MonoBehaviour
{
    [SerializeField]
    VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += LoopPointReached;
        videoPlayer.Play();
    }


    public void LoopPointReached(VideoPlayer vp)
    {
        // ����Đ��������̏���
        SceneManager.LoadScene("MenuScene");

    }
}
