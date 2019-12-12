using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    DialogueSystem dialogue;

    [SerializeField]
    protected TextAsset text;

    protected GameObject guide;

    #region ToAssignVar
    [SerializeField]
    protected Text textUI;
    [SerializeField]
    protected Image faceImage;
    [SerializeField]
    protected Image bgImage;
    #endregion

    private MoveObject[] invokeMoveObjects;

  
    protected virtual void Start()
    {
        guide = CrossCanvasController.Instance.Guide;

        if(textUI != null && faceImage != null && bgImage != null)
        {
            dialogue = new DialogueSystem(text, textUI, faceImage, bgImage);
        }
        else
        {
            dialogue = new DialogueSystem(text, guide);
        }
    }

    public virtual void GuideStart(params MoveObject[] invokeMoveObjects)
    {
        if(invokeMoveObjects != null)
        {
            this.invokeMoveObjects = invokeMoveObjects;
        }

        TextShow();
    }

    protected virtual void TextShow() 
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
