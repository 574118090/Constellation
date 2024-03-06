using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class FileDialogs
{
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName(ref OPENFILENAME lpofn);

    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName(ref OPENFILENAME lpofn);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct OPENFILENAME
    {
        public int lStructSize;
        public System.IntPtr hwndOwner;
        public System.IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public System.IntPtr lCustData;
        public System.IntPtr lpfnHook;
        public string lpTemplateName;
        public System.IntPtr pvReserved;
        public int dwReserved;
        public int FlagsEx;
    }
}

public class FileManager : MonoBehaviour
{
    public static FileManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void SelectPDFFile()
    {
        FileDialogs.OPENFILENAME ofn = new FileDialogs.OPENFILENAME();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = "PDF files (*.pdf)\0*.pdf\0All files (*.*)\0*.*\0";
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrDefExt = "pdf";
        ofn.Flags = 0x00000002 | 0x00001000; // OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST

        if (FileDialogs.GetOpenFileName(ref ofn))
        {
            RootUIManager.Instance.pathInput.text = ofn.lpstrFile;
        }
    }


    public void SaveTree()
    {
        TreeViewer.Instance.UpdateRoots();

        FileDialogs.OPENFILENAME ofn = new FileDialogs.OPENFILENAME();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = "Tree files (*.tree)\0*.tree\0All files (*.*)\0*.*\0";
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrDefExt = "tree";
        ofn.Flags = 0x00000002 | 0x00000004; // OFN_PATHMUSTEXIST | OFN_OVERWRITEPROMPT

        if (FileDialogs.GetSaveFileName(ref ofn))
        {
            string path = ofn.lpstrFile;

            Tree currentTree = TreeViewer.Instance.GetViewTree();
            if (currentTree != null)
            {
                currentTree.SaveToFile(path);
            }
            else
            {
                Debug.LogWarning("No tree is currently selected for saving.");
            }
        }
    }

    public void LoadTree()
    {
        FileDialogs.OPENFILENAME ofn = new FileDialogs.OPENFILENAME();
        ofn.lStructSize = Marshal.SizeOf(ofn);
        ofn.lpstrFilter = "Tree files (*.tree)\0*.tree\0All files (*.*)\0*.*\0";
        ofn.lpstrFile = new string(new char[256]);
        ofn.nMaxFile = ofn.lpstrFile.Length;
        ofn.lpstrDefExt = "tree";
        ofn.Flags = 0x00000002 | 0x00001000; // OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST

        if (FileDialogs.GetOpenFileName(ref ofn))
        {
            string path = ofn.lpstrFile;

            Tree loadedTree = Tree.LoadFromFile(path);
            if (loadedTree != null)
            {
                User.Instance.AddTree(loadedTree);
            }
            else
            {
                Debug.LogWarning("Failed to load the tree from the selected file.");
            }
        }
    }

    public List<Tree> LoadLocalUserData()
    {
        List<Tree> loadedTrees = new List<Tree>();

        string dataDirectoryPath = Path.Combine(Application.dataPath, "Data");

        if (!Directory.Exists(dataDirectoryPath))
        {
            Debug.Log("Data directory does not exist! Creating it...");
            Directory.CreateDirectory(dataDirectoryPath);
            return loadedTrees;
        }

        string[] treeFiles = Directory.GetFiles(dataDirectoryPath, "*.tree", SearchOption.TopDirectoryOnly);

        foreach (string treeFilePath in treeFiles)
        {
            Tree loadedTree = Tree.LoadFromFile(treeFilePath);
            if (loadedTree != null)
            {
                loadedTrees.Add(loadedTree);
            }
            else
            {
                Debug.LogWarning($"Failed to load the tree from file: {treeFilePath}");
            }
        }

        return loadedTrees;
    }

    public void SaveLocalUserData(List<Tree> trees)
    {
        string dataDirectoryPath = Path.Combine(Application.dataPath, "Data");

        if (!Directory.Exists(dataDirectoryPath))
        {
            Directory.CreateDirectory(dataDirectoryPath);
        }
        else
        {
            foreach (string file in Directory.GetFiles(dataDirectoryPath))
            {
                File.Delete(file);
            }
        }

        foreach (Tree tree in trees)
        {
            string treeFilePath = Path.Combine(dataDirectoryPath, tree.treeName + ".tree");
            tree.SaveToFile(treeFilePath);
        }
    }
}
