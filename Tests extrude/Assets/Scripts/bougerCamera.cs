using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bougerCamera : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 5F;
    public float sensitivityY = 5F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationY = 0F;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        if(GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().getPageActuelle() == "Visite" || GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().getPageActuelle() == "Edition" || GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().getPageActuelle() == "CreationExposition")
        {
            if (Input.GetKey(KeyCode.Z))
            {
                transform.Translate(Vector3.forward * (1.0f));
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.forward * (-1.0f));
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * (+1.0f));
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.right * (-1.0f));
            }
            if (Input.GetKey(KeyCode.Space))
            {
                transform.Translate(Vector3.up * (+1.0f));
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(Vector3.up * (-1.0f));
            }
            if (axes == RotationAxes.MouseXAndY)
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }

        }
        if (GameObject.Find("menuPrincipal").GetComponent<lesFonctionsBoutons>().getPageActuelle() == "VisiteKML")
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }
        }
    }
}
