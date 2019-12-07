using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pool<T> : MonoBehaviour 
{
    protected Queue<GameObject> queue = new Queue<GameObject>();

    private int freeCount = 0;
    private int opacityCount = 0;
    private int maxOpacity;
    private int preAllocCount;
    private int autoIncreCount;
    protected bool bInit = false;

    [SerializeField]
    private GameObject prefab;

    public Pool(GameObject prefab = null, int preAllocCount = 20, int maxOpacity = 100, int autoIncreCount = 3, bool bInit = true)
    {
        this.prefab = prefab;
        this.autoIncreCount = autoIncreCount;
        this.bInit = bInit;
        this.preAllocCount = preAllocCount;
        this.maxOpacity = maxOpacity;
    }

    public virtual GameObject Get(float time = 0)
    {
        if (time < 0) return null;

        GameObject res = null;
        if (freeCount > 0)
        {
            res = queue.Dequeue();
            freeCount--;
        }
        else
        {
            res = Alloc();
        }

        HandleObject(ref res, time);
        return res;
    }

    protected virtual GameObject Alloc()
    {
        GameObject res;
        if (opacityCount <= maxOpacity)
        {
            GameObject[] objects;
            int allocAmount;
            if (opacityCount == 0)
            {
                allocAmount = preAllocCount;
            }
            else
            {
                allocAmount = autoIncreCount;
            }

            objects = new GameObject[allocAmount];
            opacityCount += allocAmount;
            freeCount += allocAmount - 1;

            for (int i = 0; i < freeCount; ++i)
            {
                queue.Enqueue(objects[i]);
            }
            res = objects[freeCount];
        }
        else
        {
            res = Instantiate(prefab, transform);
        }

        res.SetActive(false);
        res.name = prefab.name;

        return res;
    }

    protected virtual void HandleObject(ref GameObject res, float time = 0)
    {
        PrefabInfo info = res.GetComponent<PrefabInfo>();
        if(info == null)
        {
            info = res.AddComponent<PrefabInfo>();
        }
        if(time > 0)
        {
            info.lifeTime = time;
        }

        res.SetActive(true);
    }

    public virtual void Recycle(GameObject obj)
    {
        if (queue.Contains(obj))
        {
            return;
        }
        if(freeCount >= opacityCount)
        {
            Destroy(obj);
        }
        else
        {
            queue.Enqueue(obj);
            obj.SetActive(false);
            freeCount++;
        }
    }
}
