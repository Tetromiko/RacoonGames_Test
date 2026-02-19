using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Codebase
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private Cube cubePrefab;
        [SerializeField] private MergeManager mergeManager;
        [SerializeField] private CubeAudioManager audioManager;

        private const int PoolDefaultCapacity = 10;
        private const int PoolMaxSize = 50;

        private readonly List<Cube> _activeCubes = new();

        public ObjectPool<Cube> CubePool { get; private set; }

        public void Initialize(MergeManager mergeManager, CubeAudioManager audioManager)
        {
            this.mergeManager = mergeManager;
            this.audioManager = audioManager;

            CubePool = new ObjectPool<Cube>(
                CreateCube,
                OnGet,
                OnRelease,
                cube => Destroy(cube.gameObject),
                true,
                PoolDefaultCapacity,
                PoolMaxSize
            );
        }

        public Cube Spawn(Vector3 position, Quaternion rotation, int value)
        {
            CubePool.Get(out var cube);
            cube.Initialize(value, position, rotation);
            cube.OnRecall += OnRecall;
            return cube;
        }

        public void ResetAllCubes()
        {
            var cubesToRelease = new List<Cube>(_activeCubes);

            foreach (var cube in cubesToRelease)
            {
                if (!cube.gameObject.activeSelf) continue;
                cube.OnRecall -= OnRecall;
                CubePool.Release(cube);
            }
        }

        private Cube CreateCube()
        {
            var cube = Instantiate(cubePrefab, transform, true);
            cube.name = "PooledCube";
            cube.gameObject.SetActive(false);
            cube.hideFlags = HideFlags.DontSave;
            return cube;
        }

        private void OnGet(Cube cube)
        {
            cube.gameObject.SetActive(true);
            _activeCubes.Add(cube);
            cube.OnCollision += audioManager.PlayHit;
            cube.OnCubeCollision += mergeManager.HandleCollision;
        }

        private void OnRelease(Cube cube)
        {
            cube.OnCollision -= audioManager.PlayHit;
            cube.OnCubeCollision -= mergeManager.HandleCollision;
            cube.gameObject.SetActive(false);
            _activeCubes.Remove(cube);
        }

        private void OnRecall(Cube cube)
        {
            CubePool.Release(cube);
            cube.OnRecall -= OnRecall;
        }
    }
}