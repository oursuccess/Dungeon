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

    [SerializeField]
    private GameObject[] itemObjects;
    private Dictionary<string, itemAttribute> itemControllerDics;
    private List<Item> spawnedItems;
    private List<Item> displayingItems;

    [SerializeField]
    private float xInterD = 1;
    [SerializeField]
    private float dragD = 1f;

    private Text numText;

    private Camera cam;
    private float xBegin;
    private float yBegin;
   
    void Start()
    {
        cam = Camera.main;
      
        itemControllerDics = new Dictionary<string, itemAttribute>();
        spawnedItems = new List<Item>();
        displayingItems = new List<Item>();

        numText = CrossCanvasController.Instance.ItemNumText.GetComponent<Text>();

        HandleCameraPosition();

        int i = 0;
        foreach(var item in itemObjects)
        {
            float xPos = xBegin + xInterD * i;
            float yPos = yBegin;

            //正常应该是图标
            var obj = Instantiate(item, new Vector2(xPos, yPos), Quaternion.identity);
            obj.name = item.name;
            var it = obj.GetComponent<Item>();

            if(it != null)
            {
                Text numT = Instantiate<Text>(numText, numText.transform.parent);
                numT.name = it.name + "Text";

                //To Clean
                numT.transform.position = cam.WorldToScreenPoint(new Vector3(xPos + xInterD / 4, yPos - xInterD / 4));
                numT.gameObject.SetActive(true);


                //第3个参数可能每个关卡不同
                itemControllerDics.Add(it.name, new itemAttribute(i , 2, numT)) ;
                displayingItems.Add(it);

                it.OnItemDraged += OnItemDraged;
            }
            ++i;
        }
        if(i != 0)
        {
            CameraController ccl = cam.GetComponent<CameraController>();
            ccl.OnCameraPositionChanged += OnCameraPositionChanged;
        }
    }

    private void OnCameraPositionChanged()
    {
        HandleCameraPosition();
        foreach (var item in displayingItems)
        {
            var attr = itemControllerDics[item.name];
            float xPos = xBegin + attr.index * xInterD;
            float yPos = yBegin;

            item.transform.position = new Vector3(xPos, yPos);
            attr.numT.transform.position = cam.WorldToScreenPoint(new Vector3(xPos + xInterD / 4, yPos - xInterD / 4));
        }
    }

    private void HandleCameraPosition()
    {
        xBegin = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + 1;
        yBegin = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - 1;
    }

    private void OnItemDraged(Item item)
    {
        if (itemControllerDics.ContainsKey(item.name))
        {
            var attr = itemControllerDics[item.name];
            float xPos = xBegin + attr.index * xInterD;
            float yPos = yBegin;

            //if((item.transform.position - new Vector3(xPos , yPos)).sqrMagnitude >= dragD)
            displayingItems.Remove(item);
            spawnedItems.Add(item);
            item.OnItemDraged -= OnItemDraged;

            var obj = Instantiate(item.gameObject, new Vector2(xPos, yPos), Quaternion.identity);
            obj.name = item.name;
            var it = obj.GetComponent<Item>();
            it.OnItemDraged += OnItemDraged;
            displayingItems.Add(it);

            attr.changeNum(-1);
            if (attr.num <= 0)
            {
                it.enabled = false;
            }
        }
    }
}
