using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField]
    private DialogueContent dialogue;
    public Text dialogueText;

    public delegate void TextShowoff();
    public event TextShowoff OnTextShowOff;
    private string pattern;

    public DialogueSystem(TextAsset text, Text textUI = null)
    {
        dialogue = new DialogueContent(text);
        dialogueText = textUI;
        pattern = @"<.+>.*</.+>";
    }

    public IEnumerator Show(float wordDelay = 0.2f, float lineDelay = 3f)
    {
        dialogueText.gameObject.SetActive(true);
        float time = 0f;
        int lineNum = 0;
        foreach (var line in dialogue.textContent)
        {
            if(wordDelay != 0f)
            {
                Match match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    for(int i = 0; i < match.Index; ++i)
                    {
                        while (!Input.anyKeyDown && time <= wordDelay)
                        {
                            time += Time.deltaTime;
                            yield return null;
                        }
                        dialogueText.text += line[i];
                        time = 0f;
                    }
                    //the only difference with other three
                    string curText = match.Value;
                    while (curText != null && curText.Length != 0 && Regex.Match(curText, "<.+>").Success && Regex.Match(curText, "</.+>").Success)
                    {
                        Match matchBegin = Regex.Match(curText, @"<.+?>");
                        Match matchEnd = Regex.Match(curText, @"</.+?>");
                        dialogueText.text += matchBegin.Value;
                        dialogueText.text += matchEnd.Value;

                        string restText = curText.Remove(matchBegin.Index, matchBegin.Length);

                        int textLength = matchEnd.Index - matchBegin.Length;
                        for (int i = 0; i < textLength; ++i)
                        {
                            dialogueText.text.Insert(dialogueText.text.Length - matchEnd.Length, restText[i].ToString());
                        }
                    }

                    for (int i = match.Index + match.Length; i < line.Length; ++i)
                    {
                        while (!Input.anyKeyDown && time <= wordDelay)
                        {
                            time += Time.deltaTime;
                            yield return null;
                        }
                        dialogueText.text += line[i];
                        time = 0f;
                    }
                }
                else
                {
                    for (int i = 0; i < line.Length; ++i)
                    {
                        while (!Input.anyKeyDown && time <= wordDelay)
                        {
                            time += Time.deltaTime;
                            yield return null;
                        }
                        dialogueText.text += line[i];
                        time = 0f;
                    }
                }
            }
            else
            {
                dialogueText.text += line;
            }
            dialogueText.text += "\r\n";
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
            lineNum++;
            if(lineNum >= 3)
            {
                FlushText();
                lineNum = 0;
            }
        }
        OnTextShowOff?.Invoke();
        yield return true;
    }

    private void FlushText()
    {
        dialogueText.text = null;
    }

    public virtual void Close()
    {
        dialogueText.gameObject.SetActive(false);
    }
}
