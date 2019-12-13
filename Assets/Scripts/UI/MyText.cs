using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyText : MonoBehaviour
{
    public string SetText
    {
        set { t.text = value; }
    }

    public Text GetText {
        get { return GetComponent<Text>(); }
    }

    Text t;
    int fontSize;

    public void Start()
    {
        t = GetComponent<Text>();
        fontSize = t.fontSize;
    }

    public void Appear()
    {
        StartCoroutine(ChangeSize(fontSize));
    }

    public void Dissapear()
    {
        StartCoroutine(ChangeSize(1));
    }

    float SmoothStop(float t)
    {
        return Mathf.Pow(1-t,3);
    }

    static float AnimDur = .4f;
    IEnumerator ChangeSize(int destSize)
    {

        float counter = 0;

        while (counter < AnimDur)
        {
            counter += Time.deltaTime;
            t.fontSize = (int)(destSize * SmoothStop(counter/AnimDur));
            yield return null;
        }
        t.fontSize = destSize;
    }
}

