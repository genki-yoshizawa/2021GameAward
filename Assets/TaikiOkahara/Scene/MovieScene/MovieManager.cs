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

    //// Update is called once per frame
    //void Update()
    //{
        
    //}


    public void LoopPointReached(VideoPlayer vp)
    {
        // “®‰æÄ¶Š®—¹‚Ìˆ—
        SceneManager.LoadScene("MenuScene");

    }
}
