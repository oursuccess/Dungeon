using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField]
    private DialogueContent dialogue;
    public Text dialogueText;

    public delegate void TextShowoff();
    public event TextShowoff OnTextShowOff;

    public DialogueSystem(TextAsset text, Text textUI = null)
    {
        dialogue = new DialogueContent(text);
        dialogueText = textUI;
    }
  
    public IEnumerator Show(float wordDelay = 0.2f, float lineDelay = 3f)
    {
        dialogueText.gameObject.SetActive(true);
        float time = 0f;
        foreach (var line in dialogue.textContent)
        {
            if(wordDelay != 0f)
            {
                for (int i = 0; i < line.Length; ++i)
                {
                    while (!Input.anyKeyDown && time <= wordDelay)
                    {
                        time += Time.deltaTime;
                        yield return null;
                    }
                    dialogueText.text = line.Substring(0, i);
                    time = 0f;
                }
            }
            else
            {
                dialogueText.text = line;
            }
            while (time <= lineDelay)
            {
                time += Time.deltaTime;
                if (time >= 0.3f && Input.anyKeyDown)
                {
                    break;
                }
                yield return null;
            }
            time = 0f;
        }
        OnTextShowOff?.Invoke();
        yield return true;
    }

    public virtual void Close()
    {
        dialogueText.gameObject.SetActive(false);
    }
}
