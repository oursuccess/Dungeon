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
    private string path;

    public delegate void TextShowoff();
    public event TextShowoff OnTextShowOff;
    private string pattern;

    #region properties
    private static float WordDelay = 0.3f;
    private static float LineDelay = 1f;

    private float wordDelay;
    private float lineDelay;
    private Color bgColor;
    private Color faceColor;
    #endregion

    public DialogueSystem(TextAsset text, GameObject Dialogue)
    {
        dialogue = new DialogueContent(text);

        GameObject dialogueObject = Dialogue;
        dialogueText = dialogueObject.transform.Find("Text").gameObject.GetComponentInChildren<Text>();
        faceImage = dialogueObject.transform.Find("Face").gameObject.GetComponentInChildren<Image>();
        bgImage = dialogueObject.transform.GetComponent<Image>();

        InitProperties();
    }

    public DialogueSystem(TextAsset text, Text textUI, Image faceImage, Image bgImage)
    {
        dialogue = new DialogueContent(text);
        dialogueText = textUI;
        this.faceImage = faceImage;
        this.bgImage = bgImage;

        InitProperties();
    }

    protected virtual void DialogueStart()
    {
        dialogueText.gameObject.SetActive(true);
        faceImage?.gameObject.SetActive(true);
        bgImage?.gameObject.SetActive(true);
    }

    protected virtual void DialogueEnd()
    {
        dialogueText.text = null;
        dialogueText.gameObject.SetActive(false);
        if(faceImage != null)
        {
            faceImage.color = faceColor;
            faceImage.overrideSprite = null;

            faceImage.gameObject.SetActive(false);
        }
        if(bgImage != null)
        {
            bgImage.color = bgColor;
            bgImage.overrideSprite = null;

            bgImage.gameObject.SetActive(false);
        }

        OnTextShowOff?.Invoke();
    }

    public IEnumerator Show()
    {
        DialogueStart();
        float time = 0f;
        int lineNum = 0;
        foreach (var line in dialogue.textContent)
        {
            #region modifiedTag
            //以#@开头的文本为高级文本
            if (line.StartsWith("#@"))
            {
                var tags = Regex.Replace(line, @"\s" ,"").Substring(2).Split(';');
                foreach(var tag in tags)
                {
                    var pair = tag.Split('=');
                    if(pair.Length == 2)
                    {
                        var variable = pair[0].ToUpper();
                        var property = pair[1];
                        switch(variable)
                        {
                            case "FACEIMAGE":
                                {
                                    string faceImagePath = "Image/Face/" + property;
                                    Debug.Log(faceImagePath);
                                    faceImage.overrideSprite = Resources.Load(faceImagePath, typeof(Sprite)) as Sprite;
                                }
                                break;
                            case "BGIMAGE":
                                {
                                    string BGImagePath = "Image/BG/" + property;
                                    bgImage.overrideSprite = Resources.Load(BGImagePath, typeof(Sprite)) as Sprite;
                                    bgImage.color = new Color(1, 1, 1);
                                }
                                break;
                            case "BGCOLOR":
                                {
                                    var color = property.Split(',');
                                    bgImage.color = new Color(float.Parse(color[0]), float.Parse(color[1]), float.Parse(color[2]));
                                }
                                break;
                            case "WORDSPEED":
                                {
                                    wordDelay = float.Parse(property);
                                }
                                break;
                            case "LINESPEED":
                                {
                                    lineDelay = float.Parse(property);
                                }
                                break;
                            default:
                                break;
                        }
                    }
#if UNITY_EDITOR
                    else
                    {
                        Debug.LogWarning("文本标签错误!\n " + line);
                    }
#endif
                }
            }
            #endregion
            #region ShowNormalText
            else
            {
                //若字母出现没有时间间隔
                if (wordDelay != 0f)
                {
                    #region FoundRichTag
                    //若有富文本标签
                    Match match = Regex.Match(line, pattern);
                    if (match.Success)
                    {
                        //富文本之前正常播放
                        for (int i = 0; i < match.Index; ++i)
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
                            for (int i = 0; i < matchBegin.Index; ++i)
                            {
                                while (!Input.anyKeyDown && time <= wordDelay)
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
                                while (!Input.anyKeyDown && time <= wordDelay)
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
                //行间时间
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

                //三行后删除第一行文本
                if (lineNum >= 3)
                {
                    dialogueText.text = dialogueText.text.Remove(0, dialogueText.text.IndexOf("\r\n"));
                    lineNum = 3;
                }
                #endregion
            }
        }
        DialogueEnd();
        yield return true;
    }

    private void InitProperties()
    {
        //把pattern放在这里，就相当于确认了使用<>作为所有对话系统的特殊标签，在使用ugui时暂时没有太大问题
        pattern = @"<.+>.*</.+>";

        wordDelay = WordDelay;
        lineDelay = LineDelay;

        bgColor = bgImage.color;
        faceColor = faceImage.color;
    }

    private void ResetProperties()
    {
        wordDelay = WordDelay;
        lineDelay = LineDelay;

        bgImage.color = bgColor;
        faceImage.color = faceColor;
    }

    public static void SetTextSpeed(float wordSpeed, float lineSpeed)
    {
        WordDelay = wordSpeed;
        LineDelay = lineSpeed;
    }

    public virtual void Close()
    {
        dialogueText.gameObject.SetActive(false);
    }
}
