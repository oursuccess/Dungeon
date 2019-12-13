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
    #endregion
    #region Controller
    private bool setting;
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

    public void SetDirectionOfTarget()
    {
        setting = true;
        Preparations();
        StartCoroutine(SetLocalDirection());
        Completation();
        setting = false;
    }

    protected virtual void Preparations()
    {
        rightArrowComp = RightArrow.AddComponent<SetDirectionComponent>();
        leftArrowComp = LeftArrow.AddComponent<SetDirectionComponent>();

        PrepareCollider(RightArrow);
        PrepareCollider(LeftArrow);

        rightArrowComp.OnDirectionSelected += OnDirectionSelected;
        leftArrowComp.OnDirectionSelected += OnDirectionSelected;
    }
    private IEnumerator SetLocalDirection()
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
        LeftArrow.SetActive(false);
        RightArrow.SetActive(false);
        GuideText.SetActive(false);
    }

    protected virtual void OnDirectionSelected(SetDirectionComponent direction)
    {
        //根据方向为对应的移动组件赋值
        if(direction == rightArrowComp)
        {

        }
        else if(direction == leftArrowComp)
        {

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
