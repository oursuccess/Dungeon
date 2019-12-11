using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField]
    private DialogueContent dialogue;

    private Image bgImage;
    private Image faceImage;
    private Text dialogueText;

    public delegate void TextShowoff();
    public event TextShowoff OnTextShowOff;
    private string pattern;

    public DialogueSystem(TextAsset text, GameObject Dialogue)
    {
        dialogue = new DialogueContent(text);

        GameObject dialogueObject = Dialogue;
        dialogueText = dialogueObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>();
        faceImage = dialogueObject.transform.Find("Face").gameObject.GetComponentInChildren<Image>();
        bgImage = dialogueObject.transform.GetComponent<Image>();

        pattern = @"<.+>.*</.+>";
    }

    public DialogueSystem(TextAsset text, Text textUI, Image faceImage, Image bgImage)
    {
        dialogue = new DialogueContent(text);
        dialogueText = textUI;
        this.faceImage = faceImage;
        this.bgImage = bgImage;

        pattern = @"<.+>.*</.+>";
    }

    protected virtual void DialogueStart()
    {
        dialogueText.gameObject.SetActive(true);
        faceImage?.gameObject.SetActive(true);
        bgImage?.gameObject.SetActive(true);
    }

    protected virtual void DialogueEnd()
    {
        dialogueText.gameObject.SetActive(false);
        faceImage?.gameObject.SetActive(false);
        bgImage?.gameObject.SetActive(false);

        OnTextShowOff?.Invoke();
    }

    public IEnumerator Show(float wordDelay = 0.2f, float lineDelay = 3f)
    {
        DialogueStart();
        float time = 0f;
        int lineNum = 0;
        foreach (var line in dialogue.textContent)
        {
            #region ShowNormalText
            if(wordDelay != 0f)
            {
                #region FoundRichTag
                //若有富文本标签
                Match match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    //富文本之前正常播放
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
                    //富文本标签之中，先加入标签，然后逐步加入实际文字
                    //在标签之中依次判断各个标签位置
                    string curText = match.Value;
                    while (curText != null && curText.Length != 0 && Regex.Match(curText, "<.+?>").Success && Regex.Match(curText, "</.+?>").Success)
                    {
                        //标签开始
                        Match matchBegin = Regex.Match(curText, @"<.+?>");
                        //标签结束
                        Match matchEnd = Regex.Match(curText, @"</.+?>");
                        
                        //标签匹配前，正常的文本显示（两个标签之中可能不从0开始）
                        for(int i = 0; i < matchBegin.Index; ++i)
                        {
                            while(!Input.anyKeyDown && time <= wordDelay)
                            {
                                time += Time.deltaTime;
                                yield return null;
                            }
                            dialogueText.text += curText[i];
                            time = 0f;
                        }

                        //标签开始，先加入标签
                        dialogueText.text += matchBegin.Value;
                        dialogueText.text += matchEnd.Value;

                        //匹配标签内文字，首先找出去掉标签开头的文件
                        string restText = curText.Remove(0, matchBegin.Index + matchBegin.Length);

                        int textLength = matchEnd.Index - matchBegin.Index - matchBegin.Length;
                        for (int i = 0; i < textLength; ++i)
                        {
                            while(!Input.anyKeyDown && time <= wordDelay)
                            {
                                time += Time.deltaTime;
                                yield return null;
                            }

                            dialogueText.text = dialogueText.text.Insert(dialogueText.text.Length - matchEnd.Length, restText[i].ToString());
                            time = 0f;
                        }

                        //继续匹配剩下的文本
                        curText = curText.Remove(0, matchEnd.Index + matchEnd.Length);
                        yield return null;
                    }

                    //匹配剩下的文本
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
                    #endregion
                }
                else
                {
                    //没有标签的文本
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
            #endregion
        }
        DialogueEnd();
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
