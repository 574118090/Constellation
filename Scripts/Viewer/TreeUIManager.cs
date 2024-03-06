using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeUIManager : MonoBehaviour
{
    [SerializeField] private GameObject addTreePanel;
    
    public void ShowAddTreePenel()
    {
        addTreePanel.SetActive(true);
    }

    public void HideAddTreePenel()
    {
        addTreePanel.SetActive(false);
    }

    public void AddTree()
    {
        string str = addTreePanel.transform.GetChild(0).GetComponent<InputField>().text;
        string label = addTreePanel.transform.GetChild(1).GetComponent<InputField>().text;
        if (str != "")
        {
            User.Instance.AddTree(str, label);
            HideAddTreePenel();
        }

        addTreePanel.transform.GetChild(0).GetComponent<InputField>().text = "";
        addTreePanel.transform.GetChild(1).GetComponent<InputField>().text = "";
    }
}
