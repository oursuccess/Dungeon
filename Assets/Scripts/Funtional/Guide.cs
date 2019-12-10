using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    DialogueSystem dialogue;
    [SerializeField]
    protected TextAsset text;
    [SerializeField]
    protected Text textUI;

    private MoveObject[] invokeMoveObjects;

  
    protected virtual void Start()
    {
        dialogue = new DialogueSystem(text, textUI);
    }

    public virtual void GuideStart(params MoveObject[] invokeMoveObjects)
    {
        GuideStart(0.2f, 3f, invokeMoveObjects);
    }

    public virtual void GuideStart(float wordDelay = 0.2f, float lineDelay = 3f, params MoveObject[] invokeMoveObjects)
    {
        if(invokeMoveObjects != null)
        {
            this.invokeMoveObjects = invokeMoveObjects;
        }

        TextShow(wordDelay, lineDelay);
    }

    protected virtual void TextShow(float wordDelay = 0.2f, float lineDelay = 3f) 
    {
        if(invokeMoveObjects != null)
        {
            foreach (var obj in invokeMoveObjects)
            {
                obj.StopMove();
            }
        }
        dialogue.OnTextShowOff += OnTextShowoff;
        StartCoroutine(dialogue.Show());
    }

    protected virtual void OnTextShowoff()
    {
        StartCoroutine(TextShowOff());
    }

    private IEnumerator TextShowOff()
    {
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        dialogue.OnTextShowOff -= OnTextShowoff;
        TextClose();
        if(invokeMoveObjects != null) 
        {
            foreach(var obj in invokeMoveObjects)
            {
                obj.StartMove();
            }
        }
    }

    protected virtual void TextClose()
    {
        dialogue.Close();
    }
}
