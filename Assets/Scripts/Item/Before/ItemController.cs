using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public class itemAttribute
    {
        public itemAttribute(int index, int num = 1, Text numT = null)
        {
            this.index = index;
            this.num = num;
            this.numT = numT;
            this.numT.text = num.ToString();
        }
        public int index; 
        public int num;
        public Text numT;

        public void changeNum(int addition)
        {
            num += addition;
            numT.text = num.ToString();
        }
    }
    #region Var
    #region Lists
    [SerializeField]
    private GameObject[] itemObjects;
    [SerializeField]
    [Range(1, 200)]
    private int[] itemObjectesNums;
    private Dictionary<string, itemAttribute> itemControllerDics;
    private List<Item> displayingItems;
    private List<MoveItem> displayingMoveItems;
    private List<Item> spawnedItems;
    private List<MoveItem> spawnedMoveItems;
    #endregion
    #region UIPos
    private Camera cam;
    private float xBegin;
    private float yBegin;
    private Text numText;
    [SerializeField]
    private float xInterD = 1;
    [SerializeField]
    private float dragD = 1f;
    #endregion
    #endregion
    void Start()
    {
        cam = Camera.main;
      
        itemControllerDics = new Dictionary<string, itemAttribute>();
        displayingItems = new List<Item>();
        displayingMoveItems = new List<MoveItem>();
        spawnedItems = new List<Item>();
        spawnedMoveItems = new List<MoveItem>();

        numText = CrossCanvasController.Instance.ItemNumText.GetComponent<Text>();

        HandleCameraPosition();

        int i = 0;
        for(; i < itemObjects.Length; ++i)
        {
            var obj = itemObjects[i];
            int itemNum = itemObjectesNums[i];
            var item = obj.GetComponent<Item>();
            if(item != null)
            {
                SetNumText(item, i);
                SpawnDisplayingItem(item, i);
            }
            ++i;
        }
        if(i != 0)
        {
            CameraController ccl = cam.GetComponent<CameraController>();
            ccl.OnCameraPositionChanged += OnCameraPositionChanged;
        }
    }
    #region HandleDisplayingPos
    private void OnCameraPositionChanged()
    {
        HandleCameraPosition();
        foreach (var item in displayingItems)
        {
            HandleDisplayingItemPosition(item);
        }
    }
    private void HandleCameraPosition()
    {
        xBegin = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + 1;
        yBegin = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - 1;
    }
    private void HandleDisplayingItemPosition(Item item)
    {
        var attr = itemControllerDics[item.name];
        float xPos = xBegin + attr.index * xInterD;
        float yPos = yBegin;
        item.transform.position = new Vector3(xPos, yPos);
        attr.numT.transform.position = cam.WorldToScreenPoint(new Vector3(xPos + xInterD / 4, yPos - xInterD / 4));
    }
    #endregion
    private void OnItemDraged(Item item)
    {
        if (itemControllerDics.ContainsKey(item.name))
        {
            var attr = itemControllerDics[item.name];
            SpawnDisplayingItem(item, attr.index);
  
            displayingItems.Remove(item);
            spawnedItems.Add(item);
            item.OnItemDraged -= OnItemDraged;
        }
    }
    private void SpawnDisplayingItem(Item item, int itemIndexInUI)
    {
        float xPos = xBegin + itemIndexInUI * xInterD;
        float yPos = yBegin;

        var obj = Instantiate(item.gameObject, new Vector2(xPos, yPos), Quaternion.identity);
        obj.name = item.name;
        var it = obj.GetComponent<Item>();
        it.OnItemDraged += OnItemDraged;
        displayingItems.Add(it);
        if(it.DirectionNeedToSet == true)
        {
            var moveItem = obj.GetComponent<MoveItem>();
            displayingMoveItems.Add(moveItem);
            moveItem.enabled = false;
            it.OnDirectionSetComplete += SetItemMove;
        }

        var attr = itemControllerDics[item.name];
        attr.changeNum(-1);
        if(attr.num <= 0)
        {
            it.enabled = false;
        }
    }
    private void SetNumText(Item item, int itemIndexInUI)
    {
        #region SetNumText
        Text numT = Instantiate<Text>(numText, numText.transform.parent);
        numT.name = item.name + "Text";

        //To Clean
        numT.transform.position = cam.WorldToScreenPoint(new Vector3(xBegin + itemIndexInUI * xInterD / 4, yBegin - xInterD / 4));
        numT.gameObject.SetActive(true);
        #endregion
        #region InitDisplayingItem
        //第3个参数可能每个关卡不同
        itemControllerDics.Add(item.name, new itemAttribute(itemIndexInUI, itemObjectesNums[itemIndexInUI], numT));
        #endregion

    }
    private void SetItemMove(Item item)
    {
        var moveItem = item.gameObject.GetComponent<MoveItem>();
#if UNITY_EDITOR
        if (displayingMoveItems.Contains(moveItem))
        {
#endif
            moveItem.enabled = true;
            displayingMoveItems.Remove(moveItem);
            spawnedMoveItems.Add(moveItem);
#if UNITY_EDITOR
        }
#endif
        item.enabled = false;
    }
}
