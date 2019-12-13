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
    private bool derivedFromItem;
    #endregion
    #region Event
    public delegate void OnDirectionCompletedDel();
    public event OnDirectionCompletedDel OnDirectionCompleted;
    #endregion

    public void SetDirectionOfTarget(Item item)
    {
        this.item = item;
        setting = true;
        derivedFromItem = true;
        Preparations();
        StartCoroutine(SplashText());
        Completation();
        setting = false;
    }

    protected virtual void Preparations()
    {
        if(RightArrow == null || LeftArrow == null)
        {
            derivedFromItem = false;

            GameObject itemDirectionComponent = GameObject.Find("ItemDirectionComponent");
            RightArrow = itemDirectionComponent.transform.Find("RightArrow").gameObject;
            LeftArrow = itemDirectionComponent.transform.Find("LeftArrow").gameObject;
            GuideText = itemDirectionComponent.transform.Find("GuideText").gameObject;
            RightArrow = Instantiate(RightArrow, item.transform);
            LeftArrow = Instantiate(LeftArrow, item.transform);
            GuideText = Instantiate(GuideText, item.transform);

            float itemXD, itemYD;
            var itemSpriteRender = item.gameObject.GetComponent<SpriteRenderer>();
            if (itemSpriteRender != null)
            {
                Sprite itemSprite = itemSpriteRender.sprite;
                itemXD = itemSprite.rect.width * item.transform.localScale.x / 2;
                itemYD = itemSprite.rect.height * item.transform.localScale.y / 2;
            }
            else
            {
                itemXD = item.transform.localScale.x / 2;
                itemYD = item.transform.localScale.y / 2;
            }

            Vector2 itemPos = item.transform.position;
            RightArrow.transform.position = new Vector2(itemPos.x + itemXD + 0.2f, itemPos.y - 0.1f);
            LeftArrow.transform.position = new Vector2(itemPos.x - itemXD - 0.2f, itemPos.y - 0.1f);
            GuideText.transform.position = new Vector2(itemPos.x, itemPos.y + itemYD + 0.2f);
        }

        rightArrowComp = RightArrow.AddComponent<SetDirectionComponent>();
        leftArrowComp = LeftArrow.AddComponent<SetDirectionComponent>();

        rightArrowComp.OnDirectionSelected += OnDirectionSelected;
        leftArrowComp.OnDirectionSelected += OnDirectionSelected;

        RightArrow.SetActive(true);
        LeftArrow.SetActive(true);
        GuideText.SetActive(true);
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
        if (derivedFromItem)
        {
            LeftArrow.SetActive(false);
            RightArrow.SetActive(false);
            GuideText.SetActive(false);
        }
        else
        {
            Destroy(item.transform.Find(LeftArrow.name));
            Destroy(item.transform.Find(RightArrow.name));
            Destroy(item.transform.Find(GuideText.name));
        }
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

}
