using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

namespace Codebase
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private Cube cubePrefab;

        public ObjectPool<Cube> CubePool { get; private set; }
        
        private readonly HashSet<Cube> _activeCubes = new();

        public void Initialize()
        {
            CubePool = new ObjectPool<Cube>(
                createFunc: CreateCube,
                actionOnGet: OnGet,
                actionOnRelease: OnRelease,
                actionOnDestroy: cube => Destroy(cube.gameObject),
                collectionCheck: true,
                defaultCapacity: 10,
                maxSize: 50
            );
        }

        private void OnGet(Cube cube)
        {
            cube.gameObject.SetActive(true);
            _activeCubes.Add(cube);
        }

        private void OnRelease(Cube cube)
        {
            cube.gameObject.SetActive(false);
            _activeCubes.Remove(cube);
        }

        private void OnDisable() => CubePool?.Dispose();

        private Cube CreateCube()
        {
            var cube = Instantiate(cubePrefab, transform, true);
            cube.name = "PooledCube";
            cube.gameObject.SetActive(false);
            cube.hideFlags = HideFlags.DontSave;
            return cube;
        }

        public Cube Create(Vector3 position, Quaternion rotation, int value)
        {
            CubePool.Get(out Cube cube);
            cube.Initialize(value, position, rotation);
            return cube;
        }

        public void ReleaseAll()
        {
            foreach (var cube in _activeCubes.ToArray())
                CubePool.Release(cube);
        }
    }
}