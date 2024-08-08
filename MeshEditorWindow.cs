using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MeshEditorWindow : EditorWindow
{
    private MeshFilter meshFilter;
    private Mesh mesh;
    private HashSet<int> selectedFaces = new HashSet<int>();
    private bool ctrlHeld = false;

    [MenuItem("Tools/Mesh Editor")]
    public static void ShowWindow()
    {
        GetWindow<MeshEditorWindow>("Mesh Editor");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Mesh Editor Tool", EditorStyles.boldLabel);

        meshFilter = (MeshFilter)EditorGUILayout.ObjectField("Mesh Filter", meshFilter, typeof(MeshFilter), true);

        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            mesh = meshFilter.sharedMesh;

            if (GUILayout.Button("Refresh Mesh"))
            {
                RefreshMesh();
            }

            if (GUILayout.Button("Remove Selected Faces"))
            {
                RemoveSelectedFaces();
            }

            if (GUILayout.Button("Save New Mesh"))
            {
                SaveNewMesh();
            }
        }
    }

    private void RefreshMesh()
    {
        selectedFaces.Clear();
    }

    private void RemoveSelectedFaces()
    {
        List<int> newTriangles = new List<int>();

        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            if (!selectedFaces.Contains(i))
            {
                newTriangles.Add(mesh.triangles[i * 3]);
                newTriangles.Add(mesh.triangles[i * 3 + 1]);
                newTriangles.Add(mesh.triangles[i * 3 + 2]);
            }
        }

        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        selectedFaces.Clear();
        Debug.Log("Selected faces removed.");
    }

    private void SaveNewMesh()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save New Mesh", "NewMesh", "asset", "Please enter a file name to save the mesh to");
        if (path.Length != 0)
        {
            Mesh newMesh = Instantiate(mesh);
            AssetDatabase.CreateAsset(newMesh, path);
            AssetDatabase.SaveAssets();
            Debug.Log("New mesh saved to " + path);
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (meshFilter == null || mesh == null)
        {
            return;
        }

        Event e = Event.current;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == meshFilter.transform)
                {
                    int triangleIndex = hit.triangleIndex;
                    if (ctrlHeld)
                    {
                        SelectConnectedFaces(triangleIndex);
                    }
                    else
                    {
                        if (!selectedFaces.Add(triangleIndex))
                        {
                            selectedFaces.Remove(triangleIndex);
                        }
                    }
                    e.Use();
                }
            }
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
        {
            ctrlHeld = true;
        }

        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl)
        {
            ctrlHeld = false;
        }

        HighlightSelectedFaces();
    }

    private void HighlightSelectedFaces()
    {
        if (selectedFaces.Count == 0)
        {
            return;
        }

        foreach (int i in selectedFaces)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            Vector3 v0 = meshFilter.transform.TransformPoint(vertices[triangles[i * 3]]);
            Vector3 v1 = meshFilter.transform.TransformPoint(vertices[triangles[i * 3 + 1]]);
            Vector3 v2 = meshFilter.transform.TransformPoint(vertices[triangles[i * 3 + 2]]);

            Handles.DrawSolidRectangleWithOutline(new Vector3[] { v0, v1, v2, v0 }, new Color(1, 0, 0, 0.25f), Color.red);
        }
    }

    private void SelectConnectedFaces(int startTriangleIndex)
    {
        Queue<int> toProcess = new Queue<int>();
        HashSet<int> processed = new HashSet<int>();

        toProcess.Enqueue(startTriangleIndex);

        while (toProcess.Count > 0)
        {
            int current = toProcess.Dequeue();
            if (processed.Contains(current))
                continue;

            processed.Add(current);
            selectedFaces.Add(current);

            List<int> connected = GetConnectedFaces(current);
            foreach (int face in connected)
            {
                if (!processed.Contains(face))
                {
                    toProcess.Enqueue(face);
                }
            }
        }
    }

    private List<int> GetConnectedFaces(int triangleIndex)
    {
        List<int> connectedFaces = new List<int>();
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;

        int[] currentTriangle = new int[]
        {
            triangles[triangleIndex * 3],
            triangles[triangleIndex * 3 + 1],
            triangles[triangleIndex * 3 + 2]
        };

        for (int i = 0; i < triangles.Length / 3; i++)
        {
            if (i == triangleIndex)
                continue;

            int[] checkTriangle = new int[]
            {
                triangles[i * 3],
                triangles[i * 3 + 1],
                triangles[i * 3 + 2]
            };

            if (currentTriangle.Intersect(checkTriangle).Count() >= 2)
            {
                connectedFaces.Add(i);
            }
        }

        return connectedFaces;
    }
}
