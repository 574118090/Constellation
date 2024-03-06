using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrop : MonoBehaviour
{
    public static List<GameObject> buttonGroup;
    public static List<GameObject> dropGroup;

    [SerializeField]
    private GameObject dropPanel;

    private void Awake()
    {
        if (buttonGroup == null) buttonGroup = new List<GameObject>();
        if (dropGroup == null) dropGroup = new List<GameObject>();
    }

    private void Start()
    {
        buttonGroup.Add(gameObject);
        dropGroup.Add(dropPanel);
    }

    private void Update()
    {
        
    }

    public void Drop()
    {
        foreach(var t in dropGroup)
        {
            if (t.gameObject == dropPanel) continue;
            t.gameObject.SetActive(false);
        }
        dropPanel.SetActive(!dropPanel.activeSelf);

        for(int i=0; i<buttonGroup.Count; i++)
        {
            if (dropGroup[i].activeSelf) StartCoroutine(Display(buttonGroup[i].GetComponent<Image>()));
            else StartCoroutine(Hide(buttonGroup[i].GetComponent<Image>()));
        }
    }

    IEnumerator Display(Image im)
    {
        float k = im.color.r;
        while (k < 0.1886792f)
        {
            k += 0.04f;
            im.color = new Color(k, k, k);
            yield return new WaitForSeconds(0.001f);
        }
        im.color = new Color(0.1886792f, 0.1886792f, 0.1886792f);
    }

    IEnumerator Hide(Image im)
    {
        float k = im.color.r;
        while (k > 0.05660379f)
        {
            k -= 0.04f;
            im.color = new Color(k, k, k);
            yield return new WaitForSeconds(0.001f);
        }
        im.color = new Color(0.05660379f, 0.05660379f, 0.05660379f);
    }
}
