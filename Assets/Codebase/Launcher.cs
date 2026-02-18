using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Codebase
{
    public class Launcher : MonoBehaviour
    {
        [SerializeField] private float minX, maxX;
        [SerializeField] private Vector2 forceDirection;
        [SerializeField] private float minForce, maxForce, maxPull;
        [SerializeField] private float spawnDelay;
        [SerializeField] private float moveSpeed;

        private float _targetX;

        private CubeSpawner _cubeSpawner;
        private LoseArea _loseArea;
        private Cube _cube;
        private Coroutine _spawnCoroutine;
        
        private Vector3 ForceDirection => new Vector3(0, forceDirection.y, forceDirection.x).normalized;
        private int RandomValue => Random.Range(0f, 1f) < 0.75f ? 2 : 4;

        public void Initialize(CubeSpawner cubeSpawner, LoseArea loseArea)
        {
            _cubeSpawner = cubeSpawner;
            _loseArea = loseArea;
        }

        public void Launch(float pullDistance)
        {
            _cube.SetKinematic(false);
            float force = Mathf.Lerp(minForce, maxForce, Mathf.Clamp01(pullDistance / maxPull));
            _cube.AddForce(ForceDirection, force);
            _spawnCoroutine = StartCoroutine(SpawnWithDelay());
        }
        
        public void StartSpawning()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
            SpawnNextCube();
        }

        public void RequestSpawn()
        {
            if (_spawnCoroutine == null) return;
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
            SpawnNextCube();
        }

        private IEnumerator SpawnWithDelay()
        {
            yield return new WaitForSeconds(spawnDelay);
            _spawnCoroutine = null;
            SpawnNextCube();
        }

        public void Move(Vector3 position, float smoothDelta)
        {
            float clampedX = Mathf.Clamp(position.x, transform.position.x + minX, transform.position.x + maxX);
            float newX = Mathf.Lerp(_cube.transform.position.x, clampedX, smoothDelta * moveSpeed);
            _cube.transform.position = new Vector3(newX, 1, transform.position.z);
        }

        
        private void SpawnNextCube()
        {
            if (_loseArea.ContainsCubes())
            {
                GameManager.Instance.Lose();
                return;
            }
    
            _cube = _cubeSpawner.Create(transform.position, Quaternion.identity, RandomValue);
            _cube.SetKinematic(true);
        }

        private void OnDrawGizmos()
        {
            DrawBoundsGizmos();
            if (_cube) DrawForceGizmos();
        }

        private void DrawBoundsGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.right * minX, 0.5f);
            Gizmos.DrawWireSphere(transform.position + Vector3.right * maxX, 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.right * minX, transform.position + Vector3.right * maxX);
        }

        private void DrawForceGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(_cube.transform.position, _cube.transform.position + ForceDirection * minForce / 10);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_cube.transform.position, _cube.transform.position + ForceDirection * maxForce / 10);
        }
    }
}