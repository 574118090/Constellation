using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class User : MonoBehaviour
{
    public static User Instance { get; private set; }
    private void Awake()
    {
        Application.targetFrameRate = 30;
        Instance = this;
    }

    public List<Tree> trees;

    public void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "usersettings.json");
        trees = FileManager.Instance.LoadLocalUserData();
        TreeSelector.Instance.UpdateSelector();
    }


    public void AddTree(string name, string label)
    {
        AddTree(new Tree(name, label));
    }

    public void AddTree(Tree tree)
    {
        trees.Add(tree);
        TreeSelector.Instance.UpdateSingleSelector();

    }

    public void RemoveTree()
    {
        RootUIManager.Instance.HideRootInformation();
        RootUIManager.Instance.selectedRoot = null;
        trees.Remove(TreeViewer.Instance.GetViewTree());
        TreeSelector.Instance.UpdateSelector();
        TreeViewer.Instance.InitializeTree(null);
    }

    void OnApplicationQuit()
    {
        TreeViewer.Instance.UpdateViewTreeData();
        // 你想要执行的代码
        FileManager.Instance.SaveLocalUserData(trees);
    }
}
