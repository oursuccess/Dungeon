using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDirection : MonoBehaviour
{
    #region AssignProps
    [SerializeField]
    private GameObject RightArrow;
    [SerializeField]
    private GameObject LeftArrow;
    [SerializeField]
    private GameObject GuideText;
    #endregion
    #region CanAssignProps
    float textShowTime = 0.2f;
    float textShowDelay = 0.1f;
    #endregion
    #region Variables
    SetDirectionComponent rightArrowComp, leftArrowComp;
    Item item;
    #endregion
    #region Controller
    private bool setting;
    #endregion
    #region Event
    public delegate void OnDirectionCompletedDel();
    public event OnDirectionCompletedDel OnDirectionCompleted;
    #endregion

    void Start()
    {
        GameObject itemDirectionComponent = GameObject.Find("ItemDirectionComponent");
        if (RightArrow == null)
        {
            RightArrow = itemDirectionComponent.transform.Find("RightArrow").gameObject;
        }
        if(LeftArrow == null)
        {
            LeftArrow = itemDirectionComponent.transform.Find("LeftArrow").gameObject;
        }
        if(GuideText == null)
        {
            GuideText = itemDirectionComponent.transform.Find("GuideText").gameObject;
        }

        RightArrow.SetActive(false);
        LeftArrow.SetActive(false);
        GuideText.SetActive(false);
    }

    public void SetDirectionOfTarget(Item item)
    {
        this.item = item;
        setting = true;
        Preparations();
        StartCoroutine(SplashText());
        Completation();
        setting = false;
    }

    protected virtual void Preparations()
    {
        RightArrow = Instantiate(RightArrow, item.transform);
        LeftArrow = Instantiate(LeftArrow, item.transform);

        rightArrowComp = RightArrow.AddComponent<SetDirectionComponent>();
        leftArrowComp = LeftArrow.AddComponent<SetDirectionComponent>();

        Vector2 itemPos = item.transform.position;
        float itemXD = item.gameObject.GetComponent<Sprite>().rect.width * item.transform.localScale.x / 2;
        float itemYD = item.gameObject.GetComponent<Sprite>().rect.height * item.transform.localScale.y / 2;
        RightArrow.transform.position = new Vector2(itemPos.x + itemXD + 0.2f, itemPos.y - 0.1f);
        LeftArrow.transform.position = new Vector2(itemPos.x - itemXD - 0.2f, itemPos.y - 0.1f);
        GuideText.transform.position = new Vector2(itemPos.x, itemPos.y + itemYD + 0.2f);

        PrepareCollider(RightArrow);
        PrepareCollider(LeftArrow);

        rightArrowComp.OnDirectionSelected += OnDirectionSelected;
        leftArrowComp.OnDirectionSelected += OnDirectionSelected;

        GuideText = Instantiate(GuideText, item.transform);
    }

    private IEnumerator SplashText()
    {
        float passT = 0f;
        while (setting)
        {
            while(passT <= textShowDelay)
            {
                passT += Time.deltaTime;
                yield return null;
            }
            passT = 0f;
            GuideText.SetActive(true);
            
            while(passT <= textShowTime)
            {
                passT += Time.deltaTime;
                yield return null;
            }
            passT = 0f;
            GuideText.SetActive(false);
            yield return null;
        }
    }
    protected virtual void Completation()
    {
        Destroy(item.transform.Find(LeftArrow.name));
        Destroy(item.transform.Find(RightArrow.name));
        Destroy(item.transform.Find(GuideText.name));
    }

    protected virtual void OnDirectionSelected(SetDirectionComponent direction)
    {
        //根据方向为对应的移动组件赋值
        if(direction == rightArrowComp)
        {
            Debug.Log("right");
        }
        else if(direction == leftArrowComp)
        {
            Debug.Log("left");
        }
        setting = false;
    }

    protected void PrepareCollider(GameObject target)
    {
        var collider = target.AddComponent<BoxCollider2D>();
        Sprite sprite = target.GetComponent<Sprite>();
        float xSize = sprite.rect.width * target.transform.localScale.x;
        float ySize = sprite.rect.height * target.transform.localScale.y;
        collider.size = new Vector2(xSize, ySize);
        collider.isTrigger = true;
    }
}
