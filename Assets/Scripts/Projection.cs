using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour {
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private Transform _obstaclesParent;

    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    private void Start() {
        CreatePhysicsScene();
    }

    private void CreatePhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform obj in _obstaclesParent) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    private void Update() {
        foreach (var item in _spawnedObjects) {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void SimulateTrajectory(GameObject ballPrefab, Vector3 pos, Vector3 velocity) {
        var ghostObj = Instantiate(ballPrefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);

        //ghostObj.Init(velocity, true);

//        using UnityEngine;

//public class Ball : MonoBehaviour {
//        [SerializeField] private Rigidbody _rb;
//        [SerializeField] private AudioSource _source;
//        [SerializeField] private AudioClip[] _clips;
//        [SerializeField] private GameObject _poofPrefab;
//        private bool _isGhost;

//        public void Init(Vector3 velocity, bool isGhost) {
//            _isGhost = isGhost;
//            _rb.AddForce(velocity, ForceMode.Impulse);
//        }

//        public void OnCollisionEnter(Collision col) {
//            if (_isGhost) return;
//            Instantiate(_poofPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
//            _source.clip = _clips[Random.Range(0, _clips.Length)];
//            _source.Play();
//        }
//    }

    _line.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }
}