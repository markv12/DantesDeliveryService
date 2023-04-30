using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public Transform spawnT;
    public float timeBetweenSpawns;
    public GameObject[] enemyPrefabs;

    private float timeSinceLastSpawn = 0;
    void Update() {
        if (DayNightManager.instance.IsNight) {
            timeSinceLastSpawn += Time.deltaTime;
            if(timeSinceLastSpawn > timeBetweenSpawns) {
                Spawn();
                timeSinceLastSpawn -= timeBetweenSpawns;
            }
        }
    }

    private void Spawn() {
        GameObject newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
    }
}
