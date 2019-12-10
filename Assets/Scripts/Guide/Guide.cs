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

    protected virtual void Start()
    {
        dialogue = new DialogueSystem(text, textUI);
    }

    public virtual void GuideStart(params MoveObject[] moveobj)
    {
        GuideStart(0.2f, 3f, moveobj);
    }

    public virtual void GuideStart(float wordDelay = 0.2f, float lineDelay = 3f, params MoveObject[] moveobj)
    {
        StartCoroutine(TextShow(wordDelay, lineDelay, moveobj));
    }

    protected virtual IEnumerator TextShow(float wordDelay = 0.2f, float lineDelay = 3f, params MoveObject[] moveobj)
    {
        if(moveobj != null)
        {
            foreach(var obj in moveobj)
            {
                obj.StopMove();
            }
        }

        dialogue.Show(wordDelay, lineDelay);
        while (!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        TextClose();
        if(moveobj != null) 
        {
            foreach(var obj in moveobj)
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
