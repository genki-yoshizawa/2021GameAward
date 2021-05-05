using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmicControl : MonoBehaviour
{
    private static readonly Vector2 VECTOR2_ZERO = new Vector2(0.0f, 0.0f);

    public virtual void TurnEndUpdate()
    { }

    public virtual bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        return true;
    }

    public virtual void Rotate(float angle)
    { }

    public virtual void TurnOver()
    { }

    public virtual void WallBreak()
    { }


}