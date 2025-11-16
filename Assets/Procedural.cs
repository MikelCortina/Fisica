using UnityEngine;

public class ProceduralObstacles : MonoBehaviour
{
    public GameObject cubePrefab;
    public int maxObstaclesAhead = 20;
    public float spacing = 3f;

    public float scale = 0.5f;
    public float heightMultiplier = 5f; // aumentar para ver variaciones
    public float widthMultiplier = 2f;
    public float depthMultiplier = 2f;
    public float minHeight = 1f;
    public float minWidth = 0.5f;
    public float minDepth = 0.5f;

    private float nextX = 0f;

    private float heightOffset;
    private float widthOffset;
    private float depthOffset;

    void Start()
    {
        heightOffset = Random.Range(0f, 100f);
        widthOffset = Random.Range(0f, 100f);
        depthOffset = Random.Range(0f, 100f);

        for (int i = 0; i < maxObstaclesAhead; i++)
        {
            GenerateObstacle();
        }
    }

    void Update()
    {
        float playerX = 0; // Cambiar por la posición real del jugador

        while (nextX < playerX + maxObstaclesAhead * spacing)
        {
            GenerateObstacle();
        }
    }

    void GenerateObstacle()
    {
        float height = Mathf.PerlinNoise(nextX * scale + heightOffset, 0) * heightMultiplier;
        height = Mathf.Max(height, minHeight);

        float width = Mathf.PerlinNoise(nextX * scale + widthOffset, 0) * widthMultiplier;
        width = Mathf.Max(width, minWidth);

        float depth = Mathf.PerlinNoise(nextX * scale + depthOffset, 0) * depthMultiplier;
        depth = Mathf.Max(depth, minDepth);

        Vector3 position = new Vector3(nextX, height / 2f, 0); // apoyado en suelo
        GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
        cube.transform.localScale = new Vector3(width, height, depth);

        nextX += spacing;
    }
}
