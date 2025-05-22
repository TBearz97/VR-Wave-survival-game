using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public List<Transform> spawnPoints;
    public GameObject targetPos;

    public int enemyCount;
    public float spawnRadius;
    private GameObject zombies;

    private void Start()
    {
        zombies = Instantiate(new GameObject(), gameObject.transform);
        zombies.name = "Zombies";
    }

    private void Update()
    {
        if (enemyCount > 0)
        {
            Spawn();
        }
    }

    void Spawn()
    {
        int randSpawn = Random.Range(0, spawnPoints.Count);
        int randEnemy = Random.Range(0, enemies.Count);

        Transform spawnCenter = spawnPoints[randSpawn];
        GameObject enemyPrefab = enemies[randEnemy];

        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 tentativePos = new Vector3(
            spawnCenter.position.x + randomOffset.x,
            spawnCenter.position.y,
            spawnCenter.position.z + randomOffset.y
        );

        NavMeshHit hit;
        float maxCheckDistance = 5f; 

        if (NavMesh.SamplePosition(tentativePos, out hit, maxCheckDistance, NavMesh.AllAreas))
        {
            GameObject zomb = Instantiate(enemyPrefab, hit.position, Quaternion.identity, zombies.transform);
            Enemy enemyScript = zomb.GetComponent<Enemy>();
            enemyScript.SetTargetPos(targetPos);

            enemyCount = enemyCount - 1;
        }
    }
}
