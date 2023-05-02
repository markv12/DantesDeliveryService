using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public Transform[] spawnPoints;
    [Min(0)]
    public float startTimeBetweenSpawns;
    [Range(0f, 1f)]
    public float timeMultiplyPerDay;
    public GameObject eyeballEnemy;
    public GameObject jellyfishEnemy;
    public GameObject goatEnemy;

    public int jellyFishStartDay;
    public int goatStartDay;


    private float TimeBetweenSpawns {
        get {
            float result = startTimeBetweenSpawns;
            int day = DayNightManager.instance.CurrentDay;
            for (int i = 0; i < day; i++) {
                result *= timeMultiplyPerDay;
            }
            return result;
        }
    }

    private float timeSinceLastSpawn = 0;
    void Update() {
        if (DayNightManager.instance.IsNight) {
            timeSinceLastSpawn += Time.deltaTime;
            if(timeSinceLastSpawn > TimeBetweenSpawns) {
                Spawn();
                timeSinceLastSpawn -= TimeBetweenSpawns;
            }
        }
    }

    private readonly List<GameObject> validPrefabs = new List<GameObject>(4);
    private void Spawn() {
        if(Player.instance != null) {
            //Vector3 playerPos = Player.instance.transform.position;
            //Transform farthestPoint = spawnPoints[0];
            //float farthestSqrDist = float.MinValue;
            //for (int i = 0; i < spawnPoints.Length; i++) {
            //    Transform point = spawnPoints[i];
            //    float distSqr = (playerPos - point.position).sqrMagnitude;
            //    if(distSqr > farthestSqrDist) {
            //        farthestSqrDist = distSqr;
            //        farthestPoint = point;
            //    }
            //}

            validPrefabs.Clear();
            validPrefabs.Add(eyeballEnemy);
            validPrefabs.Add(eyeballEnemy);
            if(DayNightManager.instance.CurrentDay >= jellyFishStartDay) {
                validPrefabs.Add(jellyfishEnemy);
                validPrefabs.Add(jellyfishEnemy);
            }
            if (DayNightManager.instance.CurrentDay >= goatStartDay) {
                validPrefabs.Add(goatEnemy);
            }
            GameObject toSpawn = validPrefabs[Random.Range(0, validPrefabs.Count)];
            GameObject newEnemy = Instantiate(toSpawn);
            newEnemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
    }
}
