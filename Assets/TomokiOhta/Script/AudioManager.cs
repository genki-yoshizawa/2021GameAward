using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    private AudioManager _AudioManager;

    [SerializeField] AudioSource[] _Source;

    [Header("âπó "),SerializeField] private float _SeVolume;
    [SerializeField] private float _BgVolume;

    [Header("ÉtÉFÅ[ÉhÉAÉEÉgÇ‹Ç≈Ç…âΩïbÇ©Ç©ÇÈÇ©"), SerializeField]
    private float _FadeOutTime;
    private float _PassedTime = 0.0f;
    private bool _IsFadeOut = false;


    private float _StartSEVolume;
    private float _StartBGMVolume;


    public static AudioManager Instance
    {
        get; private set;
    }

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _AudioManager = GetComponent<AudioManager>();


    }

    public void Start()
    {
        _Source = GetComponents<AudioSource>();

        _Source[0].volume = _SeVolume;
        _Source[0].loop = false;

        _Source[1].volume = _BgVolume;
        _Source[1].loop = true;

        _Source[2].volume = 0.0f;
        _Source[2].loop = true;

        _StartBGMVolume = _BgVolume;
        _StartSEVolume = _SeVolume;
    }

    public void Update()
    {
        if (_IsFadeOut)
        {
            float time = Time.deltaTime;
            if((_PassedTime += time) > _FadeOutTime)
            {
                _Source[1].Stop();
                _Source[1].volume = _StartBGMVolume;
                _Source[2].Stop();
                _Source[2].volume = 0.0f;

                _IsFadeOut = false;
            }
            if (_Source[1].volume <= 0.01f)  //ä€ÇﬂåÎç∑åxâ˙
                _Source[2].volume = _BgVolume * (1 - _PassedTime / _FadeOutTime);
            else
                _Source[1].volume = _BgVolume * (1 - _PassedTime / _FadeOutTime);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        _Source[1].clip = clip;
        _Source[1].Play();
    }

    //ÉQÅ[ÉÄâÊñ ópÇÃPlayBGMä÷êî
    public void PlayGameBGM(AudioClip clip1, AudioClip clip2)
    {
        _Source[1].volume = _StartBGMVolume;
        _Source[2].volume = 0.0f;


        _Source[1].clip = clip1;
        _Source[2].clip = clip2;

        _Source[1].Play();
        _Source[2].Play();
    }
    
    //îΩì]ÇµÇƒÇ©ÇÁåƒÇ—èoÇµÇƒÇ≠ÇæÇ≥Ç¢
    public void ReverseBGM(bool isFront)
    {
        if (isFront)
        {
            _Source[1].volume = _BgVolume;
            _Source[2].volume = 0.0f;
        }
        else
        {
            _Source[1].volume = 0.0f;
            _Source[2].volume = _BgVolume;
        }

    }

    public void PlaySE(AudioClip audioClip)
    {
        _Source[0].PlayOneShot(audioClip, _SeVolume);
    }

    public void SetBGMVol(float vol)
    {
        if (_Source[1].volume <= 0.01f)  //ä€ÇﬂåÎç∑åxâ˙
            _Source[2].volume = vol;
        else
            _Source[1].volume = vol;

        _BgVolume = vol;
    }

    public void ResetBGMVol()
    {
        if (_Source[1].volume <= 0.01f)  //ä€ÇﬂåÎç∑åxâ˙
            _Source[2].volume = _StartBGMVolume;
        else
            _Source[1].volume = _StartBGMVolume;

        _BgVolume = _StartBGMVolume;
    }

    public void SetSEVol( float vol) {_SeVolume = vol; }

    public void ResetSEVol() {_SeVolume = _StartSEVolume; }

    public void StopBGM()
    {
        _IsFadeOut = true;
    }

    public void StopNowBGM()
    {
        _Source[1].Stop();
        _Source[2].Stop();
    }

}
 