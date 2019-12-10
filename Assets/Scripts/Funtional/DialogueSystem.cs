using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField]
    private DialogueContent dialogue;
    public Text dialogueText;

    public DialogueSystem(TextAsset text, Text textUI = null)
    {
        dialogue = new DialogueContent(text);
        dialogueText = textUI;
    }

    public virtual void Show(float wordDelay = 0.2f, float lineDelay = 3f)
    {
        dialogueText.gameObject.SetActive(true);
        float time = 0f;
        foreach (var line in dialogue.textContent)
        {
            if(wordDelay != 0)
            {
                for (int i = 0; i < line.Length; ++i)
                {
                    dialogueText.text = line.Substring(0, i);
                    while (!Input.anyKeyDown && time <= wordDelay)
                    {
                        time += Time.deltaTime;
                    }
                    time = 0f;
                }
            }
            else
            {
                dialogueText.text = line;
            }
            while (!Input.anyKeyDown && time <= lineDelay)
            {
                time += Time.deltaTime;
            }
            time = 0f;
        }
    }

    public virtual void Close()
    {
        dialogueText.gameObject.SetActive(false);
    }
}
