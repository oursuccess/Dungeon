using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDirectionComponent : MonoBehaviour
{
    public delegate void DirectionSelectedDel(SetDirectionComponent direction);
    public event DirectionSelectedDel OnDirectionSelected;
    private void OnMouseOver()
    {
        //发光
    }

    private void OnMouseDown()
    {
        OnDirectionSelected?.Invoke(this);
    }

}
