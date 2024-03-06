using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RootUIManager : MonoBehaviour
{
    public static RootUIManager Instance { get; private set; }

    [Header("AddRoot UI Components")]
    [SerializeField] private GameObject addRootPanel;
    [SerializeField] public InputField pathInput;
    [SerializeField] private InputField titleInput;
    [SerializeField] private InputField labelInput;

    [Header("Right UI Components")]
    [SerializeField] private Text pathInfoText;
    [SerializeField] private InputField titleInfoText;
    [SerializeField] private InputField labelInfoText;
    [SerializeField] private GameObject rightPanel;
    [SerializeField] private Color[] rightPanelColor;
    [SerializeField] private GameObject[] rootButtons;
    [SerializeField] private Slider rootValueSlider;

    [Header("rootSettingMenu UI Components")]
    [SerializeField] private GameObject rootSettingMenu;
    private bool isInConnectState = false, isInDisconnectState = false;

    public Material[] rootStateMaterials;

    private bool isDragging = false;
    [NonSerialized] public GameObject selectedRoot;
    [SerializeField] private GameObject selectedRootIcon;
    public float originalValue, valueMultiple;

    private Ray ray;
    private RaycastHit hit;
    private bool rayHit;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideRootInformation();
    }

    private void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        rayHit = Physics.Raycast(ray, out hit);

        HandleDragging();
        RootSettingMenu();

        if (TreeViewer.Instance.GetViewTree() == null)
        {
            selectedRootIcon.transform.position = new Vector3(0, 0, 100);
        }

        if (rightPanel.activeSelf) 
        {
            float v = originalValue + valueMultiple * rootValueSlider.value;
            if (selectedRoot!=null && TreeViewer.Instance.GetRootByGameobject(selectedRoot) != null)
            {
                TreeViewer.Instance.GetRootByGameobject(selectedRoot).value = rootValueSlider.value;
                selectedRoot.transform.localScale = new Vector3(v, v, v);
            }
        }

        if (selectedRoot != null)
        {
            selectedRootIcon.transform.position = selectedRoot.transform.position;
            selectedRootIcon.transform.Rotate(0, 1f, 0);
        }
    }

    private void RootSettingMenu()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // 执行射线检测
            if (rayHit)
            {
                // 检查被点击的物体是否具有标签"Root"
                if (hit.collider.CompareTag("Root"))
                {
                    SetSelectedRoot(hit.collider.gameObject);
                    MoveSelectedIcon();
                    DisplayRootInformation();
                    rootSettingMenu.SetActive(true);
                    Vector3 clickPosition = Camera.main.WorldToScreenPoint(hit.point);
                    RectTransform rectTransform = rootSettingMenu.GetComponent<RectTransform>();
                    rectTransform.position = clickPosition;
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            if(!IsPointerOverUIObject())
                rootSettingMenu.SetActive(false);
        }
        else if(Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            rootSettingMenu.SetActive(false);
        }
    }

    private void HandleDragging()
    {
        if (isDragging)
        {
            if (!(Physics.Raycast(ray, out hit) && (hit.collider.gameObject == selectedRoot)))
            {
                DragSelectedRoot();
            }
            if (Input.GetMouseButtonUp(0)) EndDragging();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            BeginDragging();
        }
    }

    private void MoveSelectedIcon()
    {
        selectedRootIcon.SetActive(true);
        selectedRootIcon.transform.position = selectedRoot.transform.position;
    }

    public string GetCurrentRootPath()
    {
        return TreeViewer.Instance.GetRootByGameobject(selectedRoot).path;
    }


    public void EnableEdgeCreationMode()
    {
        rootSettingMenu.SetActive(false);
        isInConnectState = true;
        isInDisconnectState = false;
    }

    public void EnableEdgeRemoveMode()
    {
        rootSettingMenu.SetActive(false);
        isInConnectState = false;
        isInDisconnectState = true;
    }

    public void ShowAddRootPanel()
    {
        isInConnectState = isInDisconnectState = false;
        addRootPanel.SetActive(true);
    }

    public void HideAddRootPanel()
    {
        addRootPanel.SetActive(false);
    }

    public void SetSelectedRoot(GameObject root)
    {
        if (rightPanel.activeSelf) UpdateRootInformation();
        selectedRoot = root;

        if (ReaderUIManager.Instance.GetUrlType(TreeViewer.Instance.GetRootByGameobject(root).path) < 0)
        {
            ReaderUIManager.Instance.readerButton.SetActive(false);
        }
        else
        {
            ReaderUIManager.Instance.readerButton.SetActive(true);
        }
    }

    public void AddNewRoot()
    {
        TreeViewer.Instance.AddRoot(new Root(pathInput.text, titleInput.text, labelInput.text));
        DisplayRootInformation();
        HideAddRootPanel();

        pathInput.text = "";
        titleInput.text = "";
        labelInput.text = "";
    }

    public void RemoveSelectedRoot()
    {
        if (selectedRoot)
        {
            TreeViewer.Instance.RemoveRootByGameobject(selectedRoot);
            selectedRootIcon.SetActive(false);
            HideRootInformation();
        }
    }

    private void BeginDragging()
    {
        if (rayHit && hit.collider.CompareTag("Root"))
        {
            if (isInConnectState)
            {
                if (hit.collider.gameObject != selectedRoot)
                {
                    TreeViewer.Instance.AddEdge(selectedRoot, hit.collider.gameObject, true);
                    isInConnectState = false;
                }
                return;
            }
            if (isInDisconnectState)
            {
                if (hit.collider.gameObject != selectedRoot)
                {
                    TreeViewer.Instance.RemoveEdge(selectedRoot, hit.collider.gameObject);
                    isInDisconnectState = false;
                }
                return;
            }

            SetSelectedRoot(hit.collider.gameObject);
            MoveSelectedIcon();
            DisplayRootInformation();

            if (selectedRoot) isDragging = true;
        }
        else if (hit.collider == null && !IsPointerOverUIObject())
        {
            HideRootInformation();
        }
    }

    public void UpdateRootInformation()
    {
        if (selectedRoot && TreeViewer.Instance.GetRootByGameobject(selectedRoot)!=null)
        {
            TreeViewer.Instance.GetRootByGameobject(selectedRoot).title = titleInfoText.text;
            TreeViewer.Instance.GetRootByGameobject(selectedRoot).label = labelInfoText.text;
        }
    }

    public void DisplayRootInformation()
    {
        Root rootData = TreeViewer.Instance.GetRootByGameobject(selectedRoot);
        rightPanel.SetActive(true);
        pathInfoText.text = " ";
        titleInfoText.text = " ";
        labelInfoText.text = " ";

        rightPanel.transform.GetChild(0).GetComponent<Image>().color = rightPanelColor[rootData.state];
        pathInfoText.text = rootData.path;
        titleInfoText.text = rootData.title;
        labelInfoText.text = rootData.label;
        rootValueSlider.value = rootData.value;

        if (TreeViewer.Instance.IsMainRoot(selectedRoot))
        {
            foreach (var item in rootButtons)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in rootButtons)
            {
                item.SetActive(true);
            }
        }
    }

    public void HideRootInformation()
    {
        UpdateRootInformation();
        isInConnectState = false;
        isInDisconnectState = false;
        rightPanel.SetActive(false);
    }

    private void EndDragging()
    {
        isDragging = false;
    }

    private void DragSelectedRoot()
    {
        if (!selectedRoot) return;

        Vector3 mouseWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPoint.z = selectedRoot.transform.position.z;
        Vector3 dragDirection = mouseWorldPoint - selectedRoot.transform.position;

        selectedRoot.GetComponent<Rigidbody>().velocity = dragDirection * 10f;
        MoveSelectedIcon();
    }

    public void UpdateRootMaterialState(int state)
    {
        if (!TreeViewer.Instance.IsMainRoot(selectedRoot))
        {
            TreeViewer.Instance.GetRootByGameobject(selectedRoot).state = state;
            selectedRoot.GetComponent<Renderer>().material = rootStateMaterials[state];
        }
        DisplayRootInformation();
    }

    public bool IsPointerOverUIObject()
    {
        EventSystem eventSystem = EventSystem.current;

        if (eventSystem == null)
            return false;

        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("UI"))
            {
                return true;
            }
        }

        return false;
    }
}
