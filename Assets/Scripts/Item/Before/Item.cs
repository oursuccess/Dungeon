using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    #region Var
    #region Event
    public delegate void ItemDragedEventDel(Item item);
    public event ItemDragedEventDel OnItemDraged;
    public delegate void ItemDirectionSettedDel(Item item);
    public event ItemDirectionSettedDel OnDirectionSetComplete;
    #endregion
    #region DragNBackToNormal
    private bool draged;
    protected Vector2 basePos;
    #endregion
    #region SetDirectionVar
    public bool DirectionNeedToSet;
    private SetDirection setDirection;
    #endregion
    #region baseScale
    Vector2 localScale;
    #endregion
    #endregion
    protected virtual void Start()
    {
        basePos = transform.position;
        draged = false;
        localScale = transform.localScale;

        if (DirectionNeedToSet)
        {
            setDirection = gameObject.GetComponent<SetDirection>();
            if(setDirection == null)
            {
                setDirection = GameObject.Find("ItemDirectionComponent").GetComponent<SetDirection>(); 
            }
            OnItemDraged += setDirection.SetDirectionOfTarget;

            setDirection.OnDirectionCompleted += OnDirectionSelectComplete;
        }
    }
    private void OnMouseEnter()
    {
        if (enabled && !draged)
        {
            gameObject.transform.localScale *= 1.1f;
        }
    }
    #region Old
    //Old
    //private void OnMouseOver()
    //{
    //    if (enabled)
    //    {
    //        float rotation = Random.Range(-1f, 1f) * 5;
    //        gameObject.transform.Rotate(new Vector3(0, 0, rotation));
    //    }
    //}
    #endregion
    #region DragNBack
    private void OnMouseDrag()
    {
        if (enabled && !draged)
        {
            Vector2 newPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            transform.position = newPos;
        }
    }
    private void OnMouseExit()
    {
        if (enabled)
        {
            gameObject.transform.localScale = localScale;
            gameObject.transform.rotation = Quaternion.identity;

            if (!draged)
            {
                Vector2 newPos = transform.position;
                if ((newPos - basePos).sqrMagnitude >= 1f)
                {
                    basePos = new Vector3(newPos.x, basePos.y);
                    draged = true;
                    OnItemDraged?.Invoke(this);
                }
                else
                {
                    StartCoroutine(BackToBaseLocation());
                }
            }
        }
    }
    private IEnumerator BackToBaseLocation()
    {
        Vector2 currPos = transform.position;
        float distanceNow = (basePos - currPos).sqrMagnitude;

        float x = basePos.x - currPos.x;
        float y = basePos.y - currPos.y;
        while (distanceNow >= 0.1f)
        {
            transform.Translate(new Vector2(x*Time.deltaTime, y*Time.deltaTime));
            currPos = transform.position;
            distanceNow = (basePos - currPos).sqrMagnitude;
            yield return null;
        }

        transform.position = basePos;
        yield return true;
    }
    #endregion
    #region SetDirectionComplete
    protected virtual void OnDirectionSelectComplete()
    {
        OnDirectionSetComplete?.Invoke(this);
    }
    #endregion
}
