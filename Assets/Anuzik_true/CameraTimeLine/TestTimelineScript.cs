using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[RequireComponent(typeof(PlayableDirector))]

public class TestTimelineScript : MonoBehaviour
{
    [SerializeField] private PlayableAsset[] _playables;
    private PlayableDirector _director;


    // Start is called before the first frame update
    void Start()
    {
        this._director = this.GetComponent<PlayableDirector>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.P))
       {
            PlayTimeLine(0);

            Debug.Log("P�������ꂽ��");
        }

    }


    void PlayTimeLine(int id)
    {
        //this._director.playableAsset = this._playables[id];
        this._director.Play(this._playables[id]);
        Debug.Log("�^�C�����C���������Ă�͂�����");
    }



}
