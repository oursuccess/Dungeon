using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlime : Item 
{

    private void OnDisable()
    {
        transform.rotation = Quaternion.identity;
    }

}
