using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public Transform[] spawnPoints;
    public float timeBetweenSpawns;
    public GameObject eyeballEnemy;
    public GameObject goatEnemy;
    public GameObject jellyfishEnemy;

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
        if(Player.instance != null) {
            Vector3 playerPos = Player.instance.transform.position;
            Transform farthestPoint = spawnPoints[0];
            float farthestSqrDist = float.MinValue;
            for (int i = 0; i < spawnPoints.Length; i++) {
                Transform point = spawnPoints[i];
                float distSqr = (playerPos - point.position).sqrMagnitude;
                if(distSqr > farthestSqrDist) {
                    farthestSqrDist = distSqr;
                    farthestPoint = point;
                }
            }

            GameObject toSpawn = Random.Range(0, 3) == 0 ? eyeballEnemy : goatEnemy;
            GameObject newEnemy = Instantiate(toSpawn);
            newEnemy.transform.position = farthestPoint.position;
        }
    }
}
