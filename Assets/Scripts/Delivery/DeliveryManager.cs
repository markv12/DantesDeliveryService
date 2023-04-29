using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour {
    public DeliveryObject[] deliveryObjects;

    public static DeliveryManager instance;
    private readonly List<Destination> destinations = new List<Destination>(16);
    private readonly List<DOSpawnLocation> spawnLocations = new List<DOSpawnLocation>(16);
    private void Awake() {
        instance = this;
    }

    private IEnumerator Start() {
        yield return null;
        yield return null;
        for (int i = 0; i < spawnLocations.Count; i++) {
            DOSpawnLocation sl = spawnLocations[i];
            DeliveryObject newDO = Instantiate(deliveryObjects[Random.Range(0, deliveryObjects.Length)]);
            newDO.mainT.position = sl.mainT.position;
            newDO.destination = destinations[Random.Range(0, destinations.Count)];
        }
    }

    public void RegisterDestination(Destination destination) {
        destinations.Add(destination);
    }
    public void UnregisterDestination(Destination destination) {
        destinations.Remove(destination);
    }

    public void RegisterSpawnLocation(DOSpawnLocation spawnLocation) {
        spawnLocations.Add(spawnLocation);
    }
    public void UnregisterSpawnLocation(DOSpawnLocation spawnLocation) {
        spawnLocations.Remove(spawnLocation);
    }
}
