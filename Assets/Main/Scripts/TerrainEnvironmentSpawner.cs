using UnityEngine;
using UnityEngine.AI;

public class TerrainEnvironmentSpawner : MonoBehaviour
{
    public Terrain terrain;

    public GameObject[] treePrefabs;
    public GameObject[] bushPrefabs;
    public GameObject grassPrefab;

    public int numberOfTrees = 10;
    public int numberOfBushes = 20;
    public int grassDensity = 1000;

    private void Start()
    {
        SpawnEnvironment();
        SpawnGrassField();
    }

    void SpawnEnvironment()
    {
        for (int i = 0; i < numberOfTrees; i++)
            SpawnOnTerrain(treePrefabs, isTree: true);

        for (int i = 0; i < numberOfBushes; i++)
            SpawnOnTerrain(bushPrefabs, isTree: false);
    }

    void SpawnGrassField()
    {
        if (grassPrefab == null) return;

        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        for (int i = 0; i < grassDensity; i++)
        {
            float randX = Random.Range(0f, data.size.x);
            float randZ = Random.Range(0f, data.size.z);
            float height = data.GetInterpolatedHeight(randX / data.size.x, randZ / data.size.z);
            Vector3 worldPos = new Vector3(
                randX + terrainPos.x,
                height + terrainPos.y,
                randZ + terrainPos.z
            );

            Instantiate(grassPrefab, worldPos, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
        }
    }

    void SpawnOnTerrain(GameObject[] prefabs, bool isTree)
    {
        Vector3 position = GetRandomPositionOnTerrain();

        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);

        if (isTree)
        {
            var obs = obj.GetComponent<NavMeshObstacle>();
            if (obs == null)
            {
                obs = obj.AddComponent<NavMeshObstacle>();
            }
            obs.carving = true;
            obs.carveOnlyStationary = false;
        }
        else
        {
            var obs = obj.GetComponent<NavMeshObstacle>();
            if (obs != null) Destroy(obs);
        }
    }

    Vector3 GetRandomPositionOnTerrain()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        float randX = Random.Range(0f, data.size.x);
        float randZ = Random.Range(0f, data.size.z);
        float height = data.GetInterpolatedHeight(randX / data.size.x, randZ / data.size.z);

        return new Vector3(
            randX + terrainPos.x,
            height + terrainPos.y,
            randZ + terrainPos.z
        );
    }
}
