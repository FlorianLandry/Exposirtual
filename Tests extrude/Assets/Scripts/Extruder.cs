using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Extruder : MonoBehaviour
{

    void Start()
    {

    }

    private void CreateCube()
    {
        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++) colors[i] = Color.black;

        renderCube(vertices, colors);
    }

    public void CreateCube(float x1, float y1, float width, float height, Color color)
    {
        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++) colors[i] = color;

        renderCube(vertices, colors);

        transform.localScale = new Vector3(width, 25, height);
        transform.position = new Vector3(x1, 0, y1);
    }

    public void CreatePath(float[] coordsX, float[] coordsY, Color color, bool closed, bool localCoordinates)
    {
        List<Vector3> verticesList = new List<Vector3>();

        verticesList.Add(new Vector3(coordsX[0] + 0.0f, 0, coordsY[0] + 0.0f));
        verticesList.Add(new Vector3(coordsX[0] - 0.0f, 0, coordsY[0] - 0.0f));
        verticesList.Add(new Vector3(coordsX[0] + 0.0f, 25, coordsY[0] + 0.0f));
        verticesList.Add(new Vector3(coordsX[0] - 0.0f, 25, coordsY[0] - 0.0f));

        for (int i = 1; i < coordsX.Length - 1; i++) /*coordsX.Length - 1*/
        {
            var lastPoint = new Vector3(coordsX[i - 1], 0, coordsY[i - 1]);
            var currPoint = new Vector3(coordsX[i], 0, coordsY[i]);
            var heading = currPoint - lastPoint;
            var distance = heading.magnitude;
            var direction = heading / distance;

            if(localCoordinates)
            {
                verticesList.Add(new Vector3(verticesList[verticesList.Count - 1].x + coordsX[i], 0, verticesList[verticesList.Count - 1].z + coordsY[i]));
                verticesList.Add(new Vector3(verticesList[verticesList.Count - 2].x + coordsX[i], 0, verticesList[verticesList.Count - 2].z + coordsY[i]));
                verticesList.Add(new Vector3(verticesList[verticesList.Count - 3].x + coordsX[i], 25, verticesList[verticesList.Count - 3].z + coordsY[i]));
                verticesList.Add(new Vector3(verticesList[verticesList.Count - 4].x + coordsX[i], 25, verticesList[verticesList.Count - 4].z + coordsY[i]));
            }
            else
            {
                verticesList.Add(new Vector3(coordsX[i], 0, coordsY[i]));
                verticesList.Add(new Vector3(coordsX[i], 0, coordsY[i]));
                verticesList.Add(new Vector3(coordsX[i], 25, coordsY[i]));
                verticesList.Add(new Vector3(coordsX[i], 25, coordsY[i]));
            }
        }

        Vector3[] vertices = verticesList.ToArray();
        //Debug.Log(vertices.Length);

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; i++) colors[i] = color;

        renderPath(vertices, colors, closed);
    }

    private void renderCube(Vector3[] vertices, Color[] colors)
    {
        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.ClearBlendShapes();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }

    private void renderPath(Vector3[] vertices, Color[] colors, bool closed)
    {
        //Debug.Log("Rendu d'un chemin :");

        List<int> trianglesList = new List<int>();

        for(int i = 4; i < vertices.Length; i+=4)
        {
            //Debug.Log("     Cube no " + i/4 + " : (" + vertices[i].x + ", 0, " + vertices[i].z + ")");
            //Côté droit
            ajoutTriangle(trianglesList, i + 3, i - 1, i - 3);
            ajoutTriangle(trianglesList, i - 3, i + 1, i + 3);
            //Côté gauche
            ajoutTriangle(trianglesList, i - 4, i - 2, i);
            ajoutTriangle(trianglesList, i, i - 2, i + 2);
            //Dessus
            ajoutTriangle(trianglesList, i - 1, i + 3, i + 2);
            ajoutTriangle(trianglesList, i + 2, i - 2, i - 1);
            //Dessous
            ajoutTriangle(trianglesList, i - 4, i, i + 1);
            ajoutTriangle(trianglesList, i + 1, i - 3, i - 4);
        }

        if(closed)
        {
            //Debug.Log("Le chemin est fermé");
            //Côté droit
            ajoutTriangle(trianglesList, vertices.Length - 1, 3, 1);
            ajoutTriangle(trianglesList, 1, vertices.Length - 3, vertices.Length - 1);
            //Côté gauche
            ajoutTriangle(trianglesList, 0, 2, vertices.Length - 4);
            ajoutTriangle(trianglesList, vertices.Length - 4, 2, vertices.Length - 2);
            //Dessus
            ajoutTriangle(trianglesList, 3, vertices.Length - 1, vertices.Length - 2);
            ajoutTriangle(trianglesList, vertices.Length - 2, 2, 3);
            //Dessous
            ajoutTriangle(trianglesList, 0, vertices.Length - 4, vertices.Length - 3);
            ajoutTriangle(trianglesList, vertices.Length - 3, 1, 0);
        }
        else
        {
            //Debug.Log("Fermetur des deux cotés du chemin");
            //Face début
            ajoutTriangle(trianglesList, 1, 2, 0);
            ajoutTriangle(trianglesList, 1, 3, 2);

            //Face fin
            ajoutTriangle(trianglesList, vertices.Length - 4, vertices.Length - 2, vertices.Length - 3);
            ajoutTriangle(trianglesList, vertices.Length - 2, vertices.Length - 1, vertices.Length - 3);
        }

        int[] triangles = trianglesList.ToArray();

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.ClearBlendShapes();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();
        transform.localScale = new Vector3(1, 1, -1);
    }

    private void ajoutTriangle(List<int> list, int v1, int v2, int v3)
    {
        list.Add(v1);
        list.Add(v2);
        list.Add(v3);
    }
}