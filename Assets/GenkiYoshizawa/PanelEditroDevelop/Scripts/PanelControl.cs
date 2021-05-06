using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �p�l���̑�����s���N���X
public class PanelControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    //���̃p�l���ɂ��M�~�b�N�i�q�I�u�W�F�N�g�j���܂Ƃ߂ď�������
    public void TurnEndUpdate()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).GetComponent<GimmicControl>().TurnEndUpdate();
        }
    }

    public int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        int breakResult = 0;

        for (int i = 0; i < transform.childCount; ++i)
        {
            breakResult = transform.GetChild(i).GetComponent<GimmicControl>().BreakWall(objectPosition, panelPosition, direction, lv);
            if (breakResult != 0) // ����ł���ǂ�������
                break;
        }
        return breakResult;
    }
}
