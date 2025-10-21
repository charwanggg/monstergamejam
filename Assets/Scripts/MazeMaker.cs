using UnityEngine;
using System.Collections.Generic;
using Unity.AI.Navigation;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MazeMaker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private static MazeMaker instance;
    public static MazeMaker Instance { get { return instance; } }

    public int width = 10;
    public int length = 10;
    [SerializeField] private float cellSize = 4f;
    [SerializeField] private float density;
    [SerializeField] private Transform topLeft;
    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField] private NavMeshSurface nms;
    [SerializeField] private GameObject Ritual;
    [SerializeField] private int[] waves;
    [SerializeField] private GameObject exorcist;
    [SerializeField] private CanvasGroup loseCanvasGroup;
    [SerializeField] private CanvasGroup winCanvasGroup;
    [SerializeField] private Button backToHome;
    private bool isGoBack;
    private int currWave;
    public List<Vector3> RitualPositions = new List<Vector3>();
    private List<int>[] adjacency;
    private Dictionary<Vector2, GameObject> walls = new Dictionary<Vector2, GameObject>();
    private List<int>[] path;
    private bool[] visited;
    [SerializeField] private int numRituals;
    [SerializeField] private TextMeshProUGUI scoreText;
    private float pts;

    private List<GameObject> enemies;
    void Awake()
    {
        instance = this;
    }
   
    void Start()
    {
        isGoBack = false;
        backToHome.onClick.AddListener(GoHome);
        enemies = new List<GameObject>();
        density = Random.Range(0.3f, 0.6f);
        adjacency = new List<int>[width * length];
        path = new List<int>[width * length];
        visited = new bool[width * length];

        for (int i = 0; i < width * length; i++)
        {
            adjacency[i] = new List<int>();
            path[i] = new List<int>();
        }
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                int a = i * length + j;
                if (i > 0)
                {
                    int b = (i - 1) * length + j;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
                if (j > 0)
                {
                    int b = a - 1;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
                if (i < width - 1)
                {
                    int b = (i + 1) * length + j;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
                if (j < length - 1)
                {
                    int b = a + 1;
                    adjacency[a].Add(b);
                    MakeWall(a, b);
                }
            }
        }
        int start = Random.Range(0, width * length);
        visited[start] = true;
        DFS(start);

        for (int i = 0; i < numRituals; i++)
        {
            int r = Random.Range(0, width * length);
            Vector3 ritualPos = GetCellLocation(r);
            RitualPositions.Add(ritualPos);
            Instantiate(Ritual, ritualPos + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
        }
        StartCoroutine(startNavMeshBake());
    }
    private IEnumerator startNavMeshBake()
    {
        yield return new WaitForEndOfFrame();
        nms.BuildNavMesh();
        SpawnWave(0);
        // for (int i = 0; i < waves[0]; i++)
        // {
        //     Vector2Int r = RandomOnEdge();
        //     GameObject en = Instantiate(exorcist, GetCellLocation(r.x * length + r.y), Quaternion.identity);
        //     enemies.Add(en);
        // }
    }

    private void MakeWall(int a, int b)
    {
        float f = Random.Range(0f, 1f);
        if (f > density)
        {
            return;
        }
        Vector3 cellA = GetCellLocation(a);
        Vector3 cellB = GetCellLocation(b);
        Vector3 wallPos = (cellA + cellB) / 2;
        Vector3 direction = (cellB - cellA).normalized;
        Quaternion wallRot = Quaternion.LookRotation(direction);
        Vector2 v = new Vector2(Mathf.Max(a, b), Mathf.Min(a, b));

        if (!walls.ContainsKey(v))
        {
            GameObject wallPrefab = wallPrefabs[Random.Range(0, wallPrefabs.Length - 1)];
            GameObject wall = Instantiate(wallPrefab, wallPos, wallRot, this.transform);
            walls.Add(v, wall);
        }

    }
    float nextWave = 20f;
    float waveTimer = 30f;
    void Update()
    {
        bool isAllNull = true;
        pts += Time.unscaledDeltaTime;
        if (enemies != null)
        {
            foreach (GameObject g in enemies)
            {
                if (g != null)
                {
                    isAllNull = false;
                }
            }
            if (isAllNull)
            {
                enemies = new List<GameObject>();
                currWave++;
                SpawnWave(currWave + 2);
                nextWave = Time.time + waveTimer;
            }
        }

        if (Time.time > nextWave)
        {
            currWave++;
            SpawnWave(currWave + 2);
            nextWave = Time.time + waveTimer;
        }
    }

    void SpawnWave(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Vector2Int r = RandomOnEdge();
            GameObject en = Instantiate(exorcist, GetCellLocation(r.x * length + r.y), Quaternion.identity);
            enemies.Add(en);
        }
    }
    public void AddPoints(int p)
    {
        pts += p;
    }

    public void Lose()
    {
        //Debug.Log("YOU LOSE");
        StartCoroutine(LoseCoroutine());
    }

    private IEnumerator LoseCoroutine()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Sinner.instance.enabled = false;
        Sinner.instance.gameObject.GetComponent<PlayerMovement>().footsteps.Stop();
        Sinner.instance.gameObject.GetComponent<PlayerMovement>().enabled = false;
        scoreText.text = "Score: " + ((int)pts);
        float time = 0f;
        loseCanvasGroup.gameObject.SetActive(true);
        while (time < 1)
        {
            loseCanvasGroup.alpha = Mathf.Lerp(0f, 1f, time);
            time += Time.deltaTime;
            yield return null;
        }
        loseCanvasGroup.alpha = 1f;
        while (Time.timeScale > 0.1)
        {
            Time.timeScale -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 0f;
    }

    void GoHome()
    {
        SceneManager.LoadScene("menu");
    }
    private Vector2Int RandomOnEdge()
    {
        int edge = Random.Range(0, 4); // 0=Top, 1=Bottom, 2=Left, 3=Right
        int x = 0, y = 0;

        switch (edge)
        {
            case 0: // Top edge
                x = Random.Range(1, width - 1);
                y = length - 2;
                break;

            case 1: // Bottom edge
                x = Random.Range(1, width - 1);
                y = 1;
                break;

            case 2: // Left edge
                x = 1;
                y = Random.Range(1, length - 1);
                break;

            case 3: // Right edge
                x = width - 2;
                y = Random.Range(1, length - 1);
                break;
        }

        return new Vector2Int(x, y);
    }

    private void RemoveWall(int a, int b)
    {
        Vector2 key = new Vector2(Mathf.Max(a, b), Mathf.Min(a, b));
        if (walls.ContainsKey(key))
        {
            Destroy(walls[key]);
            walls.Remove(key);
        }
    }

    public Vector3 GetCellLocation(int ind)
    {
        int i = ind / length;
        int j = ind % length;
        float x = topLeft.position.x + cellSize * i;
        float z = topLeft.position.z + cellSize * j;
        return new Vector3(x, 0f, z);
    }

    private void DFS(int vert)
    {
        ListExtensions.Shuffle(adjacency[vert]);
        for (int n = 0; n < adjacency[vert].Count; n++)
        {
            int neighbor = adjacency[vert][n];
            if (!visited[neighbor])
            {
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