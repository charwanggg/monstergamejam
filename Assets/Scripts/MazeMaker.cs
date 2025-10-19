using UnityEngine;
using System.Collections.Generic;

public class MazeMaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int width = 10;
    [SerializeField] private int length = 10;
    [SerializeField] private float cellSize = 4f;
    [SerializeField] private Transform topLeft;

    [SerializeField] private GameObject wallPrefab;
    private List<int>[] adjacency;
    private Dictionary<Vector2, GameObject> walls = new Dictionary<Vector2, GameObject>();
    private List<int>[] path; 
    private bool[] visited;


    void Start()
    {
        adjacency = new List<int>[width * length];
        path = new List<int>[width * length];
        visited = new bool[width * length];
        
        for (int i = 0; i < width * length; i++) {
            adjacency[i] = new List<int>();
            path[i] = new List<int>();
        }
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                int a = i * length + j;
                if (i > 0) {
                    int b = (i - 1) * length + j;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
                if (j > 0) {
                    int b = a - 1;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
                if (i < width - 1) {
                    int b = (i + 1) * length + j;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
                if (j < length - 1) {
                    int b = a + 1;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
            }
        }
        int start = Random.Range(0, width * length);
        visited[start] = true;
        DFS(start);
    }

    private void MakeWall(int a, int b) {
        Vector3 cellA = GetCellLocation(a);
        Vector3 cellB = GetCellLocation(b);
        Vector3 wallPos = (cellA + cellB) / 2;
        Vector3 direction = (cellB - cellA).normalized;
        Quaternion wallRot = Quaternion.LookRotation(direction);
        Vector2 v = new Vector2(Mathf.Max(a, b), Mathf.Min(a, b));
        if (!walls.ContainsKey(v)) {
            GameObject wall = Instantiate(wallPrefab, wallPos, wallRot, this.transform);
            walls.Add(v, wall);
        }
        
    }

    private void RemoveWall(int a, int b) {
        Vector2 key = new Vector2(Mathf.Max(a, b), Mathf.Min(a, b));
        if (walls.ContainsKey(key)) {
            Destroy(walls[key]);
            walls.Remove(key);
        } 
    }

    private Vector3 GetCellLocation(int ind) {
        int i = ind / length;
        int j = ind % length;
        float x = topLeft.position.x + cellSize * i;
        float z = topLeft.position.z + cellSize * j;
        return new Vector3(x, 0f, z);
    }

    private void DFS(int vert) {
        ListExtensions.Shuffle(adjacency[vert]);
        for (int n = 0; n < adjacency[vert].Count; n++) {
            int neighbor = adjacency[vert][n];
            if (!visited[neighbor]) {
                visited[neighbor] = true;
                path[vert].Add(neighbor);
                //path[neighbor].Add(vert);
                RemoveWall(vert, neighbor);
                DFS(neighbor);
            }
        }
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}