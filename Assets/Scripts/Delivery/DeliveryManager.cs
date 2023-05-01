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
            SpawnDOAtLocation(sl);
            SelectDestinationsWithADO();
        }
    }

    private void SpawnDOAtLocation(DOSpawnLocation sl) {
        DeliveryObject newDO = Instantiate(deliveryObjects[Random.Range(0, deliveryObjects.Length)]);
        newDO.mainT.position = sl.mainT.position;
        Destination destination = destinations[Random.Range(0, destinations.Count)];
        newDO.destination = destination;
        destination.currentDO = newDO;
        sl.currentDO = newDO;
        newDO.spawnLocation = sl;
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

    public void OnDeliveryCompleted() {
        bool allStarted = true;
        for (int i = 0; i < spawnLocations.Count; i++) {
            if (spawnLocations[i].currentDO != null) {
                allStarted = false;
            }
        }
        if (allStarted) {
            for (int i = 0; i < spawnLocations.Count; i++) {
                DOSpawnLocation sl = spawnLocations[i];
                if (sl != null && sl.currentDO == null) {
                    SpawnDOAtLocation(sl);
                }
            }
        }
    }

    public void SetActiveDO(DeliveryObject currentDO) {
        if (currentDO == null) {
            SelectDestinationsWithADO();
        } else {
            for (int i = 0; i < destinations.Count; i++) {
                Destination destination = destinations[i];
                destination.SetDestinationSelected(currentDO.destination == destination);
            }
        }
    }

    private void SelectDestinationsWithADO() {
        for (int i = 0; i < destinations.Count; i++) {
            Destination destination = destinations[i];
            destination.SetDestinationSelected(destination.currentDO != null);
        }
    }
}
