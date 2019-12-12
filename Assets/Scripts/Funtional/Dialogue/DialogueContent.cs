using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueContent
{
    public TextAsset text;
    public string[] textContent;

    public DialogueContent()
    {
        textContent = text.text.Split('\n');
    }

    public DialogueContent(TextAsset text)
    {
        this.text = text;
        textContent = text.text.Split('\n');
    }
}
