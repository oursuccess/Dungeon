using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    public class itemAttribute
    {
        public itemAttribute(float xPos, float yPos, int num = 1, Text numT = null)
        {
            this.xPos = xPos;
            this.yPos = yPos;
            this.numT = numT;
            this.num = num;
            this.numT.text = num.ToString();
        }
        public int num;
        public float xPos { get; private set; }
        public float yPos { get; private set; }
        public Text numT;

        public void changeNum(int addition)
        {
            num += addition;
            numT.text = num.ToString();
        }
    }

    [SerializeField]
    private GameObject[] itemObjects;
    private Dictionary<string, itemAttribute> items;

    [SerializeField]
    private float xInterD = 1;
    [SerializeField]
    private float dragD = 1f;

    [SerializeField]
    private Text numText;
   
    void Start()
    {
        float xBegin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + 1;
        float yBegin = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - 1;
        items = new Dictionary<string, itemAttribute>();

        int i = 0;
        foreach(var item in itemObjects)
        {
            float xPos = xBegin + xInterD * i;
            float yPos = yBegin;

            var obj = Instantiate(item, new Vector2(xPos, yPos), Quaternion.identity);
            obj.name = item.name;
            var it = obj.GetComponent<Item>();

            if(it != null)
            {
                Text numT = Instantiate<Text>(numText, numText.transform.parent);
                numT.name = it.name + "Text";

                //To Clean
                numT.transform.position = Camera.main.WorldToScreenPoint(new Vector3(xPos + xInterD / 4, yPos - xInterD / 4));
                numT.gameObject.SetActive(true);

                //第3个参数可能每个关卡不同
                items.Add(it.name, new itemAttribute(xPos, yPos, 2, numT)) ;

                it.OnItemDraged += OnItemDraged;
            }
            ++i;
        }
    }

    private void OnItemDraged(Item item)
    {
        if (items.ContainsKey(item.name))
        {
            var attr = items[item.name];

            if((item.transform.position - new Vector3(attr.xPos, attr.yPos)).sqrMagnitude >= dragD)
            {
                item.OnItemDraged -= OnItemDraged;

                var obj = Instantiate(item.gameObject, new Vector2(attr.xPos, attr.yPos), Quaternion.identity);
                obj.name = item.name;
                obj.GetComponent<Item>().OnItemDraged += OnItemDraged;

                attr.changeNum(-1);
                if (attr.num <= 0)
                {
                    items.Remove(item.name);
                }
            }
        }
    }
}
