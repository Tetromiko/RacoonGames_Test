using UnityEngine;
using Random = UnityEngine.Random;

namespace Codebase
{
    public class Launcher : MonoBehaviour
    {
        [SerializeField] private Transform marker;
        [SerializeField] private Transform aimMarker;
        [SerializeField] private float minX;
        [SerializeField] private float maxX;
        [SerializeField] private float minPower;
        [SerializeField] private float maxPower;
        [SerializeField] private float verticalForce;

        private CubeSpawner _cubeSpawner;
        private Cube _cube;
        private Vector3 _currentAimDirection;
        private float _currentPull;

        private int RandomValue => Random.Range(0f, 1f) < 0.75f ? 2 : 4;
        private float LaunchPower => minPower + (maxPower - minPower) * _currentPull;

        public void Initialize(CubeSpawner cubeSpawner)
        {
            _cubeSpawner = cubeSpawner;
        }

        public void PrepareNextCube()
        {
            _cube = null;
            SpawnCube();
            Move(Vector3.zero);
        }

        public void Launch()
        {
            var launchPosition = _cube.transform.localPosition;
            var direction = new Vector3(_currentAimDirection.x, verticalForce, _currentAimDirection.z).normalized;

            _cube.gameObject.layer = LayerMask.NameToLayer("Default");
            _cube.SetKinematic(false);
            _cube.transform.parent = _cubeSpawner.transform;
            _cube.AddForce(direction, LaunchPower);

            _cube = null;
            SpawnCube();
            Move(launchPosition);
        }

        public void Move(Vector3 position)
        {
            if (_cube == null) return;

            var clampedX = Mathf.Clamp(position.x, minX, maxX);
            _cube.transform.localPosition = Vector3.right * clampedX;

            var markerPosition = _cube.transform.localPosition;
            marker.localPosition = markerPosition;
            aimMarker.localPosition = markerPosition;
        }

        public void Aim(Vector3 aimDirection, float magnitude)
        {
            _currentAimDirection = aimDirection;
            _currentPull = magnitude;
            aimMarker.localPosition = marker.localPosition + aimDirection * (magnitude + 0.5f);
        }

        private void SpawnCube()
        {
            _cube = _cubeSpawner.Spawn(transform.position, Quaternion.identity, RandomValue);
            _cube.gameObject.layer = LayerMask.NameToLayer("ChargingCube");
            _cube.transform.parent = transform;
            _cube.SetKinematic(true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.right * minX, 0.5f);
            Gizmos.DrawWireSphere(transform.position + Vector3.right * maxX, 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.right * minX, transform.position + Vector3.right * maxX);
        }
    }
}