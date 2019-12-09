using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public delegate void ItemDragedEvent(Item item);
    public event ItemDragedEvent OnItemDraged;

    private void OnMouseEnter()
    {
        if (enabled)
        {
            gameObject.transform.localScale *= 1.1f;
        }
    }

    private void OnMouseOver()
    {
        if (enabled)
        {
            float rotation = Random.Range(-1f, 1f) * 5;
            gameObject.transform.Rotate(new Vector3(0, 0, rotation));
        }
    }

    private void OnMouseDrag()
    {
        if (enabled)
        {
            Vector2 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            transform.position = newPos;

            OnItemDraged?.Invoke(this);
        }
    }

    private void OnMouseExit()
    {
        if (enabled)
        {
            gameObject.transform.localScale /= 1.1f;
            gameObject.transform.rotation = Quaternion.identity;
        }
    }

}
