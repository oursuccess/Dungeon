using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDirection : MonoBehaviour
{
    #region AssignProps
    private GameObject RightArrow;
    private GameObject LeftArrow;
    private GameObject GuideText;
    #endregion
    #region CanAssignProps
    private static float TextShowTime = 0.2f;
    private static float TextShowDelay = 0.1f;
    private float textShowTime;
    private float textShowDelay;
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
    #region Routine
    private Coroutine textSplashRoutine;
    #endregion

    public void SetDirectionOfTarget(Item item)
    {
        this.item = item;
        setting = true;
        derivedFromItem = true;
        textShowTime = TextShowTime;
        textShowDelay = TextShowDelay;
        Preparations();
        textSplashRoutine = StartCoroutine(SplashText());
    }
    protected virtual void Preparations()
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
            itemXD = itemSprite.rect.width * item.transform.localScale.x * 16 / Camera.main.pixelWidth;
            itemYD = itemSprite.rect.height * item.transform.localScale.y * 16 / Camera.main.pixelHeight;
        }
        else
        {
            itemXD = item.transform.localScale.x / 2;
            itemYD = item.transform.localScale.y / 2;
        }

        Vector2 itemPos = item.transform.position;
        RightArrow.transform.position = new Vector2(itemPos.x + itemXD + 1f, itemPos.y);
        LeftArrow.transform.position = new Vector2(itemPos.x - itemXD - 1f, itemPos.y);
        GuideText.transform.position = new Vector2(itemPos.x, itemPos.y + itemYD + 1f);

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

    public void Clean()
    {
        Destroy(this);
    }

    protected virtual void Completation()
    {
        StopCoroutine(textSplashRoutine);

        Destroy(item.transform.Find(LeftArrow.name).gameObject);
        Destroy(item.transform.Find(RightArrow.name).gameObject);
        Destroy(item.transform.Find(GuideText.name).gameObject);

        OnDirectionCompleted?.Invoke();
    }
    protected virtual void OnDirectionSelected(SetDirectionComponent direction)
    {
        //根据方向为对应的移动组件赋值
        if(direction == rightArrowComp)
        {
            item.gameObject.GetComponent<MoveItem>().direction = Vector2.right;
        }
        else if(direction == leftArrowComp)
        {
            item.gameObject.GetComponent<MoveItem>().direction = Vector2.left;
        }
        rightArrowComp.OnDirectionSelected -= OnDirectionSelected;
        leftArrowComp.OnDirectionSelected -= OnDirectionSelected;

        Completation();
        setting = false;
    }
}
