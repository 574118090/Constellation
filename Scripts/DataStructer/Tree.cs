using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Tree
{
    public List<Vector3> rootPositions;
    public List<Root> roots;
    public List<Pair> edges;
    public string treeName;

    public Tree(string title, string label = "")
    {
        edges = new List<Pair>();
        roots = new List<Root>
        {
            Root.CreateMainRoot(title, label)
        };
        rootPositions = new List<Vector3>
        {
            new Vector3(0,0, 3.5f)
        };
        treeName = title;
    }

    public void AddRoot(Root root)
    {
        roots.Add(root);

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float randomX = UnityEngine.Random.Range(screenWidth / 3f, screenWidth / 1.5f);
        float randomY = UnityEngine.Random.Range(screenHeight / 3f, screenHeight / 1.5f);

        Vector3 randomWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(randomX, randomY, 3.5f));
        randomWorldPosition.z = 3.5f;

        rootPositions.Add(randomWorldPosition);
    }


    public void AddEdge(int i, int j)
    {
        if (EdgeContain(i, j)) return;

        edges.Add(new Pair(i, j));
    }

    public void RemoveRoot(int idx)
    {
        rootPositions.RemoveAt(idx);
        roots.RemoveAt(idx);
    }

    public void RemoveEdge(int i, int j)
    {
        edges.Remove(new Pair(i, j));
    }

    public int GetIndexOfEdge(int i, in int j)
    {
        return edges.IndexOf(new Pair(i, j));
    }

    public bool EdgeContain(int i,int j)
    {
        return edges.Contains(new Pair(i, j));
    }

    public void SaveToFile(string filePathWithoutExtension)
    {
        string jsonData = JsonUtility.ToJson(this);
        string completeFilePath = filePathWithoutExtension;
        File.WriteAllText(completeFilePath, jsonData);
    }

    public static Tree LoadFromFile(string filePathWithoutExtension)
    {
        string completeFilePath = filePathWithoutExtension;

        if (File.Exists(completeFilePath))
        {
            string jsonData = File.ReadAllText(completeFilePath);
            return JsonUtility.FromJson<Tree>(jsonData);
        }
        else
        {
            Debug.LogError("File not found: " + completeFilePath);
            return null;
        }
    }
}
