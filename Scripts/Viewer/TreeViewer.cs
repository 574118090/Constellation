using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class TreeViewer : MonoBehaviour
{
    public static TreeViewer Instance;

    [Header("Settings")]
    [SerializeField] private float rootSpeed;
    [SerializeField] private float interval;
    [SerializeField] private bool rootFixed = true;

    [Header("Prefabs")]
    [SerializeField] private GameObject prefabRootGameobject;
    [SerializeField] private GameObject prefabLineGameobject;

    private List<GameObject> rootGameobjects = new List<GameObject>();
    private List<GameObject> edgeGameobjects = new List<GameObject>();
    private Tree viewTree;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializeTree(in Tree tree)
    {
        if (viewTree != null)
        {
            UpdateViewTreeData();
        }
        viewTree = tree;
        UpdateViewTree();
        if(viewTree != null) Camera.main.GetComponent<CameraController>().SetCameraPosition(rootGameobjects[0].transform.position);
        else
        {
            RootUIManager.Instance.HideRootInformation();
        }
    }

    private void Update()
    {
        if (viewTree == null) return;
        UpdateEdgePositions();
    }

    public void UpdateViewTreeData()
    {
        for (int i = 0; i < rootGameobjects.Count; i++)
        {
            viewTree.rootPositions[i] = rootGameobjects[i].transform.position;
            viewTree.rootPositions[i] = new Vector3(viewTree.rootPositions[i].x, viewTree.rootPositions[i].y, 3.5f);
        }
    }

    public Tree GetViewTree()
    {
        return viewTree;
    }

    public void UpdateViewTree()
    {
        foreach(GameObject gameObject in rootGameobjects)
        {
            Destroy(gameObject);
        }
        foreach(GameObject gameObject in edgeGameobjects)
        { 
            Destroy(gameObject); 
        }
        foreach(GameObject gameObject in edgeGameobjects)
        {
            Destroy(gameObject);
        }
        edgeGameobjects.Clear();
        rootGameobjects.Clear();
        if(viewTree != null)
        {
            foreach (Root rt in viewTree.roots)
            {
                AddRoot(rt);
            }
            foreach (var item in viewTree.edges)
            {
                AddEdge(rootGameobjects[item.i], rootGameobjects[item.j]);
            }
            RootUIManager.Instance.SetSelectedRoot(rootGameobjects[0]);
            RootUIManager.Instance.DisplayRootInformation();
        }

        for(int i = 0; i < rootGameobjects.Count; i++)
        {
            float originalValue = RootUIManager.Instance.originalValue;
            float valueMultiple = RootUIManager.Instance.valueMultiple;
            float v = originalValue + valueMultiple * viewTree.roots[i].value;
            rootGameobjects[i].transform.localScale = new Vector3(v, v, v);
        }
    }

    public bool IsMainRoot(GameObject gm)
    {
        return gm == rootGameobjects[0];
    }

    public Root GetRootByGameobject(GameObject gm)
    {
        int index = rootGameobjects.IndexOf(gm);
        if (index >= 0 && index < viewTree.roots.Count)
        {
            return viewTree.roots[index];
        }
        return null; // Consider throwing an exception or error logging.
    }

    public void RemoveRootByGameobject(GameObject go)
    {
        int idx = rootGameobjects.IndexOf(go);
        if (idx <= 0) return; // Ensure you don't remove the main root

        foreach(var gm in rootGameobjects)
        {
            RemoveEdge(gm, go);
        }
        StartCoroutine(RemoveRoot(go));
        rootGameobjects.RemoveAt(idx);
        viewTree.RemoveRoot(idx);
    }

    public void AddEdge(GameObject a, GameObject b, bool check = false)
    {
        int x = rootGameobjects.IndexOf(a);
        int y = rootGameobjects.IndexOf(b);
        if (!viewTree.EdgeContain(x, y))
        {
            viewTree.AddEdge(x, y);
        }
        else if(check)
        {
            return;
        }

        GameObject newEdge = Instantiate(prefabLineGameobject);
        newEdge.transform.position = Vector3.zero;

        LineRenderer lr = newEdge.GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[] { rootGameobjects[x].transform.position, rootGameobjects[y].transform.position });

        edgeGameobjects.Add(newEdge);
    }

    public void RemoveEdge(GameObject a, GameObject b)
    {
        int x = rootGameobjects.IndexOf(a);
        int y = rootGameobjects.IndexOf(b);
        if (viewTree.EdgeContain(x, y))
        {
            int idx = viewTree.GetIndexOfEdge(x, y);
            viewTree.RemoveEdge(x, y);
            Destroy(edgeGameobjects[idx]);
            edgeGameobjects.RemoveAt(idx);
        }
        if (viewTree.EdgeContain(y, x))
        {
            int idx = viewTree.GetIndexOfEdge(y, x);
            viewTree.RemoveEdge(y, x);
            Destroy(edgeGameobjects[idx]);
            edgeGameobjects.RemoveAt(idx);
        }
    }

    public void AddRoot(Root root)
    {
        if (!viewTree.roots.Contains(root))
        {
            viewTree.AddRoot(root);

            GameObject newRoot = Instantiate(prefabRootGameobject);
            newRoot.transform.position = viewTree.rootPositions[viewTree.rootPositions.Count - 1];

            root.state = 1;

            rootGameobjects.Add(newRoot);

            RootUIManager.Instance.SetSelectedRoot(newRoot);
            RootUIManager.Instance.UpdateRootMaterialState(root.state);

            StartCoroutine(GenerateRoot(newRoot));
            StartCoroutine(FixedRootMovement(newRoot));
        }
        else
        {
            int idx = viewTree.roots.IndexOf(root);
            GameObject newRoot = Instantiate(prefabRootGameobject);
            newRoot.transform.position = viewTree.rootPositions[idx];

            rootGameobjects.Add(newRoot);
            RootUIManager.Instance.SetSelectedRoot(newRoot);
            RootUIManager.Instance.UpdateRootMaterialState(root.state);

            StartCoroutine(GenerateRoot(newRoot));
            StartCoroutine(FixedRootMovement(newRoot));
        }
    }

    public void UpdateRoots()
    {
        for(int i=0;i<viewTree.roots.Count;i++)
        {
            viewTree.rootPositions[i] = rootGameobjects[i].transform.position;
        }
    }

    private void UpdateEdgePositions()
    {
        for (int i = 0; i < viewTree.edges.Count; i++)
        {
            LineRenderer lr = edgeGameobjects[i].GetComponent<LineRenderer>();
            lr.SetPositions(new Vector3[]
            {
                rootGameobjects[viewTree.edges[i].i].transform.position,
                rootGameobjects[viewTree.edges[i].j].transform.position
            });
        }
    }

    private IEnumerator GenerateRoot(GameObject gm)
    {
        while (gm && gm.transform.position.z > 2.5f)
        {
            gm.transform.Translate(0, 0, -0.08f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator RemoveRoot(GameObject gm)
    {
        Vector3 pos = gm.transform.position;
        while (gm && pos.z < 4f)
        {
            gm.transform.Translate(0, 0, 0.08f);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gm);
    }

    private IEnumerator FixedRootMovement(GameObject gm)
    {
        Rigidbody rb = gm.GetComponent<Rigidbody>();
        while (gm && rootFixed)
        {
            rb.AddForce(UnityEngine.Random.Range(-rootSpeed, rootSpeed), UnityEngine.Random.Range(-rootSpeed, rootSpeed), 0);
            yield return new WaitForSeconds(interval);
        }
    }
}