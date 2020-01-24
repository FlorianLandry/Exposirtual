using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuModele3D : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log("yayyyyyyyyyyyyyyyyyy");
        Debug.Log(this.gameObject.name);
        if(this.gameObject.name == "Repere")
        {
            GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().changementPage();
        }
        if(this.gameObject.name == "Repere(Clone)")
        {
            GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().importModele3D(this.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
