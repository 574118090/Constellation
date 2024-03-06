using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject treeContent, prefabTreeLabel;
    private List<GameObject> treeLabel;

    public static TreeSelector Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        treeLabel = new List<GameObject>();
    }

    public void UpdateSelector()
    {
        foreach(GameObject go in treeLabel)
        {
            Destroy(go);
        }
        treeLabel.Clear();
        for(int i=0;i<User.Instance.trees.Count;i++)
        {
            AddInTreeLabel(User.Instance.trees[i].treeName);
        }
    }

    public void UpdateSingleSelector()
    {
        AddInTreeLabel(User.Instance.trees[User.Instance.trees.Count-1].treeName);
    }

    public void AddInTreeLabel(string name, string info = "ERROR or DEBUG")
    {
        GameObject tp = Instantiate(prefabTreeLabel);
        tp.transform.SetParent(treeContent.transform, false);
        tp.transform.GetChild(0).GetComponent<Text>().text = name;
        tp.transform.GetChild(1).GetComponent<Text>().text = info;
        tp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -(treeLabel.Count * 110));
        Addlistener(tp.GetComponent<Button>(), treeLabel.Count);
        treeLabel.Add(tp);
        UpdateContentSize();
    }

    void UpdateContentSize()
    {
        RectTransform contentRect = treeContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, treeLabel.Count * 110);
    }

    void Addlistener(Button button, int paramater)
    {
        button.onClick.AddListener(delegate { OnSelect(paramater); });
    }

    private void OnSelect(int index = 0)
    {
        if (TreeViewer.Instance.GetViewTree() == User.Instance.trees[index]) return;
        TreeViewer.Instance.InitializeTree(User.Instance.trees[index]);
    }
}
