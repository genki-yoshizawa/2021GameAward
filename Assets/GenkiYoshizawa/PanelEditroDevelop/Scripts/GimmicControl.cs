using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmicControl : MonoBehaviour
{
    public virtual void TurnEndUpdate()
    { }

    public virtual bool CheckEnter(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        return true;
    }

    public virtual int CheckWallLevel(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction)
    {
        return 0;
    }

    public virtual void Rotate(float angle)
    { }

    public virtual void TurnOver(Vector3 rotAxis)
    { }

    public virtual int BreakWall(Vector2Int objectPosition, Vector2Int panelPosition, Vector2 direction, int lv = 0)
    {
        return 0;
    }

    public virtual bool IsWall() { return false; }
    public virtual bool IsCheese() { return false; }

}