using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using System.Globalization;

public class SVGReader : MonoBehaviour
{
    [SerializeField]
    private GameObject meshCreator;

    [SerializeField]
    private string filename;

    void Start()
    {
        //loadSVG(filename);
    }

    public void loadSVG(string fileName)
    {
        CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        ci.NumberFormat.CurrencyDecimalSeparator = ".";
        string line;

        using (StreamReader reader = new StreamReader(fileName))
        {
            while(!reader.EndOfStream)
            {
                line = reader.ReadLine();
                //Debug.Log(line);
                if (line.Contains("<rect"))
                {
                    ReadRectangle(reader, line, ci);
                }
                else if(line.Contains("<path"))
                {
                    ReadPath(reader, line, ci);
                }
                else if(line.Contains("inkscape:label=\"Chemin\""))
                {
                    ReadVisit(reader, line, ci);
                }
            }
            reader.Close();
        }
    }

    private void ReadRectangle(StreamReader reader, string line, CultureInfo ci)
    {
        string temp;
        float x1 = 0.0f, y1 = 0.0f, width = 0.0f, height = 0.0f;
        Color color = Color.black;
        //Debug.Log("Coucou je suis un cube !");
        do
        {
            line = reader.ReadLine();
            if (line.Contains("width="))
            {
                temp = line;
                temp = temp.Replace("width=\"", "");
                temp = temp.Replace("\"", "");
                temp = temp.Replace(" />", "");
                width = float.Parse(temp, NumberStyles.Any, ci);
            }
            else if (line.Contains("height="))
            {
                temp = line;
                temp = temp.Replace("height=\"", "");
                temp = temp.Replace("\"", "");
                temp = temp.Replace(" />", "");
                height = float.Parse(temp, NumberStyles.Any, ci);
                height = -height;
            }
            else if (line.Contains("x="))
            {
                temp = line;
                temp = temp.Replace("x=\"", "");
                temp = temp.Replace("\"", "");
                temp = temp.Replace(" />", "");
                x1 = float.Parse(temp, NumberStyles.Any, ci);
            }
            else if (line.Contains("y="))
            {
                temp = line;
                temp = temp.Replace("y=\"", "");
                temp = temp.Replace("\"", "");
                temp = temp.Replace(" />", "");
                y1 = float.Parse(temp, NumberStyles.Any, ci);
                y1 = -y1;
            }
            else if (line.Contains("style="))
            {
                temp = line;
                var foundS1 = temp.IndexOf("fill:");
                temp = temp.Substring(foundS1 + 5, 7);
                if (!ColorUtility.TryParseHtmlString(temp, out color))
                    color = Color.magenta;
            }
        } while (!line.Contains("/>"));

        GameObject cube = Instantiate(meshCreator);
        cube.tag = "cubeCreation";
        cube.GetComponent<Extruder>().CreateCube(x1, y1, width, height, color);
        cube.AddComponent<Rigidbody>();
        cube.AddComponent<BoxCollider>();
        color = Color.black;
    }

    private void ReadPath(StreamReader reader, string line, CultureInfo ci)
    {
        bool closed = false;
        bool localCoordinates = false;
        string temp;
        List<String> coordListX = new List<string>();
        List<String> coordListY = new List<string>();
        Color color = Color.black;
        
        //Debug.Log("Waw, je suis un chemin !");
        do
        {
            line = reader.ReadLine();
            if (line.Contains("style="))
            {
                temp = line;
                var foundS1 = temp.IndexOf("stroke:");
                temp = temp.Substring(foundS1 + 6, 7);
                if (!ColorUtility.TryParseHtmlString(temp, out color))
                    color = Color.magenta;
            }
            else if (line.Contains("d=\"M") || line.Contains("d=\"m"))
            {
                if (line.Contains("d=\"M")) localCoordinates = false;
                else if (line.Contains("d=\"m")) localCoordinates = true;
                temp = line;
                temp = temp.Replace("d=\"M ", "");
                temp = temp.Replace("d=\"m ", "");
                
                if (temp.Contains("Z\"") || temp.Contains("z\""))
                {
                    temp = temp.Replace(" Z\"", "");
                    temp = temp.Replace(" z\"", "");
                    closed = true;
                }
                else temp = temp.Replace(" \"", "");
                temp = temp.Replace("\"", "");

                String[] strlist = temp.Split(' ');
                int i = 0;
                foreach (String s in strlist)
                {
                    //Debug.Log(s);
                    if (s != "" && s != null)
                    {
                        String[] tempList = s.Split(',');
                        coordListX.Add(tempList[0]);
                        //Debug.Log("x" + i + " = " + tempList[0]);
                        coordListY.Add(tempList[1]);
                        //Debug.Log("y" + i + " = " + tempList[1]);
                        i++;
                    }
                }
            }
        } while (!line.Contains("/>"));

        float[] xCoords = new float[coordListX.ToArray().Length];
        float[] yCoords = new float[coordListY.ToArray().Length];

        for(int i = 0; i < xCoords.Length; i++)
        {
            //Debug.Log("Valeur de x : " + coordListX[i]);
            //Debug.Log("Valeur de y : " + coordListY[i]);
            xCoords[i] = float.Parse(coordListX[i], NumberStyles.Any, ci);
            yCoords[i] = float.Parse(coordListY[i], NumberStyles.Any, ci);
        }

        GameObject mesh = Instantiate(meshCreator);
        mesh.GetComponent<Extruder>().CreatePath(xCoords, yCoords, color, closed, localCoordinates);
        mesh.tag = "cubeCreation";
        //cube.AddComponent<Rigidbody>();
        //cube.AddComponent<BoxCollider>();
        color = Color.black;
    }

    private void ReadVisit(StreamReader reader, string line, CultureInfo ci)
    {
        List<float> xCoords = new List<float>();
        List<float> yCoords = new List<float>();
        List<float> rotations = new List<float>();
        String temp;
        do
        {
            line = reader.ReadLine();
            if(line.Contains("<rect"))
            {
                do
                {
                    line = reader.ReadLine();
                    if(line.Contains("x="))
                    {
                        temp = line;
                        temp = temp.Replace("x=\"", "");
                        temp.Replace("\"", "");
                        xCoords.Add(float.Parse(temp, NumberStyles.Any, ci));
                    }
                    else if (line.Contains("y="))
                    {
                        temp = line;
                        temp = temp.Replace("y=\"", "");
                        temp = temp.Replace("\"", "");
                        yCoords.Add(float.Parse(temp, NumberStyles.Any, ci));
                    }
                    else if (line.Contains("rotate"))
                    {
                        temp = line;
                        temp = temp.Replace("transform=\"", "");
                        temp = temp.Replace("\"", "");
                        temp = temp.Replace("rotate(", "");
                        temp = temp.Replace(")", "");
                        rotations.Add(float.Parse(temp, NumberStyles.Any, ci));
                    }
                } while (!line.Contains("/>"));
            }
        } while (!line.Contains("</g>"));

        //TO DO
        //CALL FUNCTIONS TO CREATE A VISIT PATH
    }
}
