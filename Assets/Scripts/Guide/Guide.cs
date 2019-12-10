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

    void Start()
    {
        dialogue = new DialogueSystem(text, textUI);
    }

    protected virtual void TextShow(float wordDelay = 0.2f, float lineDelay = 3f)
    {
        //dialogue.Show(wordDelay, lineDelay);
    }

    protected virtual void TextClose()
    {
        dialogue.Close();
    }
}
