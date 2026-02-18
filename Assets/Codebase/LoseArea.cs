using System.Collections.Generic;
using UnityEngine;

namespace Codebase
{
    public class LoseArea : MonoBehaviour
    {
        [SerializeField]
        private List<Cube> cubes;
        
        public void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out Cube cube)) return;
            
            cubes.Add(cube);
        }
        
        public void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out Cube cube)) return;
            
            if (cubes.Contains(cube))
                cubes.Remove(cube);
        }

        public bool ContainsCubes()
        {
            return cubes.Count > 0;
        }
        
        public void Clear() => cubes.Clear();
    }
}