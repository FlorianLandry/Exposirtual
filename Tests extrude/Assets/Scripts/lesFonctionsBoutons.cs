using PolyToolkit;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lesFonctionsBoutons : MonoBehaviour
{
    private string pageActuelle;
    GameObject menu;
    Hashtable listButton;
    Hashtable pageModele;
    GameObject repere;
    Vector3 vecteurClone;
    Vector3 vecteurModele;
    PolyListAssetsRequest req;
    string nextPageTest;

    // Start is called before the first frame update
    void Start()
    {
        req = new PolyListAssetsRequest();
        // Only curated assets:
        req.curated = true;
        // Limit complexity to medium.
        req.maxComplexity = PolyMaxComplexityFilter.MEDIUM;
        // Only Blocks objects.
        req.formatFilter = PolyFormatFilter.BLOCKS;
        // Order from best to worst.
        req.orderBy = PolyOrderBy.BEST;
        // Up to 20 results per page.
        req.pageSize = 20;

        listButton = new Hashtable();

        pageModele = new Hashtable();

        repere = GameObject.Find("Repere");

        vecteurClone = repere.GetComponent<Transform>().position;
        

        int i = 0;

        //On récupère tous les boutons dans un Dictionnaire, pour que plus tard on puisse accéder aux inactive
        foreach (GameObject u in GameObject.FindGameObjectsWithTag("pasInit"))
        {
            Debug.Log(u.name);
            listButton.Add(u.name, u);
            u.SetActive(false);
            i++;
        }
        foreach (GameObject u in GameObject.FindGameObjectsWithTag("init"))
        {
            listButton.Add(u.name, u);
            u.SetActive(true);
            i++;
        }
        foreach (GameObject u in GameObject.FindGameObjectsWithTag("repere"))
        {
            listButton.Add(u.name, u);
            u.SetActive(false);
            i++;
        }

        //On initialise la page d'accueil
        pageActuelle = "Accueil";
    }

    public void onClickModeVisiteur()
    {
        //On change de page
        ((GameObject)(listButton["buttonModeVisiteur"])).SetActive(false);
        ((GameObject)(listButton["buttonModeEditeur"])).SetActive(false);
        ((GameObject)(listButton["Exposirtual"])).SetActive(false);
        pageActuelle = "VisiteurAccueil";
        ((GameObject)(listButton["buttonVisiteVirtuelle"])).SetActive(true);
        ((GameObject)(listButton["buttonRetour"])).SetActive(true);
    }
    public void onClickModeEditeur()
    {
        //On change de page
        ((GameObject)(listButton["buttonModeVisiteur"])).SetActive(false);
        ((GameObject)(listButton["buttonModeEditeur"])).SetActive(false);
        ((GameObject)(listButton["Exposirtual"])).SetActive(false);
        pageActuelle = "EditeurAccueil";
        ((GameObject)(listButton["buttonChargementExpo"])).SetActive(true);
        ((GameObject)(listButton["buttonCreationExpo"])).SetActive(true);
        ((GameObject)(listButton["buttonRetour"])).SetActive(true);
    }

    public void onClickChargementExpo()
    {
        //On change de page
        ((GameObject)(listButton["buttonChargementExpo"])).SetActive(false);
        ((GameObject)(listButton["buttonCreationExpo"])).SetActive(false);
        pageActuelle = "ParcourirChargementEditeur";
        ((GameObject)(listButton["buttonParcourir"])).SetActive(true);
    }

    public void onClickTestPolyYay()
    {
        vecteurClone = repere.GetComponent<Transform>().position;
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("cubeCreation"))
        {
            Destroy(g);
        }
        Debug.Log("yay !");
        var u = GameObject.Find("textRecherche");
        string textRecherche = u.GetComponent<UnityEngine.UI.Text>().text;
        Debug.Log(textRecherche);
        int i = 1;
        GameObject.Find("numeroPage").GetComponent<TextMesh>().text = i.ToString();
        // Search by keyword:
        req.keywords = textRecherche;
        
        if(nextPageTest is null)
        {
            nextPageTest = req.pageToken;
        }
        
        req.pageToken = nextPageTest;

        PolyApi.ListAssets(req, MyCallback);
    }

    void MyCallback(PolyStatusOr<PolyListAssetsResult> result)
    {
        if (!result.Ok)
        {
            // Handle error.
            Debug.Log("Oulah c'est pas bon ça");
            return;
        }
        
        nextPageTest = result.Value.nextPageToken;
        // Success. result.Value is a PolyListAssetsResult and
        // result.Value.assets is a list of PolyAssets.
        foreach (PolyAsset asset in result.Value.assets)
        {
            Debug.Log(asset.name);
            PolyApi.FetchThumbnail(asset, MyCallback2);


        }
    }

    void MyCallback2(PolyAsset asset, PolyStatus status)
    {
        Material m = new Material(repere.GetComponent<Renderer>().material);
        m.mainTexture = asset.thumbnailTexture;
        GameObject g;

        if (!status.ok)
        {
            Debug.Log("meh");
            return;
        }

        // On affiche la thumbnail sur des objets séparés (théoriquement)
        vecteurClone.x += 5;
        
        g = Instantiate(repere, vecteurClone, new Quaternion(0, 0, 180, 0));
        g.transform.localScale = new Vector3(3.60427f, 4.227693f, 1.07186f);
        g.GetComponent<Renderer>().material = m;
        g.tag = "cubeCreation";

        pageModele.Add(g, asset);
     
        
    }

    public void changementPage()
    {
        foreach (GameObject u in GameObject.FindGameObjectsWithTag("cubeCreation"))
        {
            Destroy(u);
        }
        int i = int.Parse(GameObject.Find("numeroPage").GetComponent<TextMesh>().text);
        i++;
        GameObject.Find("numeroPage").GetComponent<TextMesh>().text = i.ToString();


        vecteurClone = repere.GetComponent<Transform>().position;
        req.pageToken = nextPageTest;
        PolyApi.ListAssets(req, MyCallback);
    }

    public void importModele3D(GameObject g)
    {
        vecteurModele = g.GetComponent<Transform>().position;
        vecteurModele.y += 5;
        PolyApi.Import((PolyAsset) pageModele[g], PolyImportOptions.Default(), MyCallback3);
    }

    void MyCallback3(PolyAsset asset, PolyStatusOr<PolyImportResult> result)
    {
        if (!result.Ok)
        {
            Debug.Log("uh");
            return;
        }
        result.Value.gameObject.transform.SetPositionAndRotation(vecteurModele, new Quaternion(0, 0, 0, 0));
        result.Value.gameObject.tag = "cubeCreation";

        //Instantiate(result.Value.gameObject, vecteurModele, new Quaternion(0, 0, 0, 0)).tag = "cubeCreation";
    }

    public void afficheQuitter()
    {
        ((GameObject)(listButton["buttonQuitter"])).SetActive(true);
    }

    public void onClickParcourir()
    {
        //On ouvre l'explorateur de fichier
        if(pageActuelle == "ParcourirChargementVisiteur")
        {
            GameObject.Find("UI").GetComponent<newFichier>().parcourirSVG();
            pageActuelle = "Visite";
            foreach (GameObject u in GameObject.FindGameObjectsWithTag("pasInit"))
            {
                ((GameObject)(listButton[u.name])).SetActive(false);
            }
            foreach (GameObject u in GameObject.FindGameObjectsWithTag("init"))
            {
                ((GameObject)(listButton[u.name])).SetActive(false);
            }
        }
        else if(pageActuelle == "ParcourirChargementEditeur")
        {
            GameObject.Find("UI").GetComponent<newFichier>().parcourirSVG();
            pageActuelle = "Edition";
            foreach (GameObject u in GameObject.FindGameObjectsWithTag("pasInit"))
            {
                ((GameObject)(listButton[u.name])).SetActive(false);
            }
            foreach (GameObject u in GameObject.FindGameObjectsWithTag("init"))
            {
                ((GameObject)(listButton[u.name])).SetActive(false);
            }
        }
        else if (pageActuelle == "ParcourirChargementKML")
        {
            GameObject.Find("UI").GetComponent<newFichier>().parcourirKML();
            pageActuelle = "VisiteKML";
            foreach (GameObject u in GameObject.FindGameObjectsWithTag("pasInit"))
            {
                ((GameObject)(listButton[u.name])).SetActive(false);
            }
            foreach (GameObject u in GameObject.FindGameObjectsWithTag("init"))
            {
                ((GameObject)(listButton[u.name])).SetActive(false);
            }
            
        }
    }

    public void onClickCreerExpo()
    {
        //On change de page
        ((GameObject)(listButton["buttonChargementExpo"])).SetActive(false);
        ((GameObject)(listButton["buttonCreationExpo"])).SetActive(false);
        ((GameObject)(listButton["buttonRetour"])).SetActive(false);
        pageActuelle = "CreationExposition";
        ((GameObject)(listButton["buttonQuitter"])).SetActive(true);
        ((GameObject)(listButton["Repere"])).SetActive(true);
        ((GameObject)(listButton["page"])).SetActive(true);
        ((GameObject)(listButton["numeroPage"])).SetActive(true);
        ((GameObject)(listButton["buttonTestPoly"])).SetActive(true);
        ((GameObject)(listButton["champDeRecherche"])).SetActive(true);
    }

    public void onClickVisiteVirtuelle()
    {
        //On change de page
        Debug.Log(((GameObject)(listButton["buttonVisiteVirtuelle"])).name);
        ((GameObject)(listButton["buttonVisiteVirtuelle"])).SetActive(false);
        pageActuelle = "ParcourirChargementVisiteur";
        ((GameObject)(listButton["buttonParcourir"])).SetActive(true);
    }

    public void onClickRetour()
    {
        if(pageActuelle == "EditeurAccueil")
        {
            //On change de page en fonction de celle où l'on est
            ((GameObject)(listButton["buttonModeVisiteur"])).SetActive(true);
            ((GameObject)(listButton["buttonModeEditeur"])).SetActive(true);
            ((GameObject)(listButton["Exposirtual"])).SetActive(true);
            pageActuelle = "Accueil";
            ((GameObject)(listButton["buttonChargementExpo"])).SetActive(false);
            ((GameObject)(listButton["buttonCreationExpo"])).SetActive(false);
            ((GameObject)(listButton["buttonRetour"])).SetActive(false);
        }
        if(pageActuelle == "ParcourirChargementEditeur")
        {
            //On change de page en fonction de celle où l'on est
            ((GameObject)(listButton["buttonChargementExpo"])).SetActive(true);
            ((GameObject)(listButton["buttonCreationExpo"])).SetActive(true);
            ((GameObject)(listButton["buttonRetour"])).SetActive(true);
            pageActuelle = "EditeurAccueil";
            ((GameObject)(listButton["buttonParcourir"])).SetActive(false);
        }
        if(pageActuelle == "VisiteurAccueil")
        {
            //On change de page en fonction de celle où l'on est
            ((GameObject)(listButton["buttonModeVisiteur"])).SetActive(true);
            ((GameObject)(listButton["buttonModeEditeur"])).SetActive(true);
            ((GameObject)(listButton["Exposirtual"])).SetActive(true);
            pageActuelle = "Accueil";
            ((GameObject)(listButton["buttonVisiteVirtuelle"])).SetActive(false);
            ((GameObject)(listButton["buttonRetour"])).SetActive(false);
        }
        if(pageActuelle == "ParcourirChargementVisiteur")
        {

            //On change de page en fonction de celle où l'on est
            ((GameObject)(listButton["buttonParcourir"])).SetActive(false);
            pageActuelle = "VisiteurAccueil";
            ((GameObject)(listButton["buttonVisiteVirtuelle"])).SetActive(true);
            ((GameObject)(listButton["buttonRetour"])).SetActive(true);
        }
    }

    public void onClickQuitter()
    {
        foreach (GameObject u in GameObject.FindGameObjectsWithTag("cubeCreation"))
        {
            Destroy(u);
        }
        int i = 0;
        if(pageActuelle == "CreationExposition")
        {
            GameObject.Find("numeroPage").GetComponent<TextMesh>().text = i.ToString();
        }
        ((GameObject)(listButton["Repere"])).SetActive(false);
        ((GameObject)(listButton["buttonTestPoly"])).SetActive(false);
        ((GameObject)(listButton["champDeRecherche"])).SetActive(false);
        ((GameObject)(listButton["buttonQuitter"])).SetActive(false);
        ((GameObject)(listButton["page"])).SetActive(false);
        ((GameObject)(listButton["numeroPage"])).SetActive(false);
        vecteurClone = repere.GetComponent<Transform>().position;
        pageActuelle = "Accueil";
        ((GameObject)(listButton["buttonModeVisiteur"])).SetActive(true);
        ((GameObject)(listButton["buttonModeEditeur"])).SetActive(true);
        ((GameObject)(listButton["Exposirtual"])).SetActive(true);
    }

    public string getPageActuelle()
    {
        return this.pageActuelle;
    }

}
