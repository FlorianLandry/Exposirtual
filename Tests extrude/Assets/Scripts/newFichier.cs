using SimpleFileBrowser;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class newFichier : MonoBehaviour
{
    [SerializeField]
    private SVGReader svg;

    bool isClicked;
    string fileToLoad;
    // Start is called before the first frame update
    void Start()
    {
        //svg = new SVGReader();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ShowLoadDialogCoroutineSVG()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        fileToLoad = FileBrowser.Result;

        if (FileBrowser.Success)
        {
            // If a file was chosen, read its bytes via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result);

            if (fileToLoad.EndsWith(".svg"))
            {
                svg.loadSVG(fileToLoad);
            }

        }
        GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().afficheQuitter();
    }

    IEnumerator ShowLoadDialogCoroutineKML()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        fileToLoad = FileBrowser.Result;

        if (FileBrowser.Success)
        {
            // If a file was chosen, read its bytes via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result);

            if (fileToLoad.EndsWith(".kml"))
            {
                /*
             * MALIK
             * Tu implémentes tes trucs par ici
             *Et normalement ça devrait pouvoir marcher
             *
             */
            }

        }
        GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().afficheQuitter();
    }

    public void parcourirSVG()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".svg", ".kml"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
        FileBrowser.SetDefaultFilter(".svg");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ShowLoadDialogCoroutineSVG());
    }

    public void parcourirKML()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".svg", ".kml"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
        FileBrowser.SetDefaultFilter(".svg");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
        StartCoroutine(ShowLoadDialogCoroutineKML());
    }

    public string getFileToLoad()
    {
        return fileToLoad;
    }

    private void OnGUI()
    {

    }
}
