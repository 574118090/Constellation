using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Root
{
    public static Root CreateMainRoot(string title, string label)
    {
        Root res = new Root();
        res.path = "´´½¨ÓÚ" + System.DateTime.Now.ToString("F");
        res.title = title;
        res.label = label;
        res.value = 0f;
        return res;
    }
    public string path, title, label;
    public int state;
    public float value;

    public Root()
    {

    }

    public Root(string path, string title, string label)
    {
        this.path = path;
        this.title = title;
        this.label = label;
        this.value = 0f;
    }
}
