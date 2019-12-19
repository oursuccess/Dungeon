using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTriggerForObjectMove : EventTrigger
{
    #region Var
    [System.Serializable]
    public class GameObjectAndMoveAttributes
    {
        [Tooltip("要移动的物体")]
        public GameObject @object;
        [Tooltip("移动方向")]
        public Vector2 moveDirection;
        [Tooltip("移动速度")]
        [Range(0, 30)]
        public float moveSpeed;
    }
    [SerializeField]
    [Tooltip("对象与移动的属性")]
    private GameObjectAndMoveAttributes[] gameObjectAndMoveAttributes;
    #endregion
    protected override void EventImpl(Player player)
    {
        foreach(var objNAttr in gameObjectAndMoveAttributes)
        {
            var obj = objNAttr.@object;
            Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
            if(rigidbody2D == null)
            {
               rigidbody2D = obj.AddComponent<Rigidbody2D>();
            }
            rigidbody2D.AddForce(objNAttr.moveDirection * objNAttr.moveSpeed);
        }
    }
}
