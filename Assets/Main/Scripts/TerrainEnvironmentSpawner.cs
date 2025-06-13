using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainEnvironmentSpawner : MonoBehaviour
{
    public Terrain terrain;
    public GameObject player;
    private Player script;

    [Header("Tree Details")]
    public GameObject[] treePrefabs;
    public int numberOfTrees = 10;


    [Header("Spawn Area")]
    public Vector2 areaSize = new Vector2(50f, 50f);
    public Vector2 areaCenterOffset = new Vector2(0f, 0f);

    [Header("Zombie Details")]
    public List<GameObject> enemyPrefabs;
    public GameObject aliveZombies;
    public GameObject deadZombies;
    private List<GameObject> enemyTypes;

    private void Start()
    {
        script = player.GetComponent<Player>();
        enemyTypes = new List<GameObject>();
        SpawnEnvironment();
        MovePlayer();

        foreach (GameObject enemy in enemyPrefabs)
        {
            GameObject type = Instantiate(new GameObject(), GetRandomPositionOnTerrain(), Quaternion.identity, deadZombies.transform);
            type.name = enemy.name;
            enemyTypes.Add(type);
            for (int i = 0; i < enemy.GetComponent<Enemy>().maxActive; i++)
            {
                GameObject spawn = Instantiate(enemy, GetRandomPositionOnTerrain(), Quaternion.identity, type.transform);
                spawn.GetComponent<Enemy>().graveyard = type;
            }
        }
    }

    private void Update()
    {
        if (aliveZombies.transform.childCount < script.spawnCount)
        {
            SpawnZombie();
        }
    }

    void SpawnZombie()
    {
        int rand = Random.Range(0, enemyTypes.Count);

        Transform container = enemyTypes[rand].transform;
        for (int i = 0; i < container.childCount; i++)
        {
            GameObject spawn = container.GetChild(i).gameObject;
            if (!spawn.activeInHierarchy)
            {
                spawn.SetActive(true);
                spawn.transform.SetParent(aliveZombies.transform, false);
                spawn.transform.localPosition = GetRandomPositionOnTerrain();
                return;
            }
        }
    }

    void SpawnEnvironment()
    {
        for (int i = 0; i < numberOfTrees; i++)
            SpawnOnTerrain(treePrefabs, isTree: true);
    }

    void MovePlayer()
    {
        player.transform.position = GetRandomPositionOnTerrain();
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

    public Vector3 GetRandomPositionOnTerrain()
    {
        TerrainData data = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.localPosition;

        float minX = terrainPos.x + areaCenterOffset.x - areaSize.x / 2f;
        float maxX = minX + areaSize.x;
        float minZ = terrainPos.z + areaCenterOffset.y - areaSize.y / 2f;
        float maxZ = minZ + areaSize.y;

        for (int attempts = 0; attempts < 10; attempts++)
        {
            float randX = Random.Range(minX, maxX);
            float randZ = Random.Range(minZ, maxZ);

            float normX = (randX - terrainPos.x) / data.size.x;
            float normZ = (randZ - terrainPos.z) / data.size.z;
            float terrainHeight = data.GetInterpolatedHeight(normX, normZ) + terrainPos.y;

            Vector3 tentativePosition = new Vector3(randX, terrainHeight + 1f, randZ); 

            if (NavMesh.SamplePosition(tentativePosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                float sampledY = Mathf.Max(hit.position.y, terrainHeight); 
                return new Vector3(randX, sampledY + 0.1f, randZ); 
            }
        }

        Debug.LogWarning("Could not find valid NavMesh position, returning terrain center.");
        return terrain.transform.localPosition + Vector3.up * 2f;
    }


    private void OnDrawGizmosSelected()
    {
        if (terrain == null) return;

        Gizmos.color = Color.green;
        Vector3 center = terrain.transform.localPosition + new Vector3(areaCenterOffset.x, 0f, areaCenterOffset.y);
        Vector3 size = new Vector3(areaSize.x, 0.1f, areaSize.y);
        Gizmos.DrawWireCube(center, size);
    }
}
