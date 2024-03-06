using Paroxe.PdfRenderer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Rendering;

public class ReaderUIManager : MonoBehaviour
{
    public static ReaderUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public GameObject readerBackground;
    public GameObject readerButton;
    public GameObject readerPanel;
    public PDFViewer reader;
    public GameObject downloadDialog;

    public void OpenPDFReader()
    {
        readerBackground.SetActive(true);
        string path = RootUIManager.Instance.GetCurrentRootPath();
        int value = GetUrlType(path);

        readerPanel.SetActive(true);
        if (value == 1) reader.LoadDocumentFromWeb(path);
        else reader.LoadDocumentFromFile(path);
    }

    public void ClosePDFReader()
    {
        readerBackground.SetActive(false);
        readerPanel.SetActive(false);
        downloadDialog.SetActive(false);
    }

    public int GetUrlType(string url)
    {
        Regex localPathRegex = new Regex(@"^([a-zA-Z]:\\|\\\\)");

        Regex websiteRegex = new Regex(@"^(http|https)://");

        Regex pdfExtensionRegex = new Regex(@"\.pdf$");

        if (!pdfExtensionRegex.IsMatch(url))
        {
            return -2;
        }
        else if (localPathRegex.IsMatch(url))
        {
            return 2;
        }
        else if (websiteRegex.IsMatch(url))
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

}
