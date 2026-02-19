using System;
using UnityEngine;

namespace Codebase
{
    public class GameOverTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Cube cube))
            {
                cube.Recall();
                OnGameOver?.Invoke();
            }
        }

        public event Action OnGameOver;
    }
}