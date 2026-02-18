using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Codebase
{
    public struct MergeRequest
    {
        public Cube A;
        public Cube B;

        public MergeRequest(Cube a, Cube b)
        {
            A = a;
            B = b;
        }
    }

    public class MergeManager : MonoBehaviour
    {
        public static MergeManager Instance { get; private set; }

        private ObjectPool<Cube> _cubePool;
        private readonly HashSet<(Cube, Cube)> _processedPairs = new();
        
        public event Action OnMerge;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void LateUpdate() => _processedPairs.Clear();

        public void SetPool(ObjectPool<Cube> cubePool) => _cubePool = cubePool;

        public void RegisterContact(Cube a, Cube b)
        {
            if (a == null || b == null) return;
            if (a.isMerging || b.isMerging) return;
            if (a.Value != b.Value) return;

            var pair = GetOrderedPair(a, b);
            if (!_processedPairs.Add(pair)) return;

            a.isMerging = true;
            b.isMerging = true;

            ProcessMerge(new MergeRequest(a, b));
        }

        private (Cube, Cube) GetOrderedPair(Cube a, Cube b) =>
            a.GetInstanceID() < b.GetInstanceID() ? (a, b) : (b, a);

        private void ProcessMerge(MergeRequest request)
        {
            Cube a = request.A;
            Cube b = request.B;

            if (a == null || b == null) return;

            Cube survivor = GetMoreActive(a, b);
            Cube consumed = survivor == a ? b : a;

            var linearVelocity = (a.LinearVelocity + b.LinearVelocity) / 2;
            var angularVelocity = (a.AngularVelocity + b.AngularVelocity) / 2;
            var jumpDirection = (survivor.transform.position - consumed.transform.position).normalized;

            survivor.Initialize(survivor.Value * 2);
            survivor.LinearVelocity = linearVelocity;
            survivor.AngularVelocity = angularVelocity;
            survivor.AddForce(jumpDirection, linearVelocity.magnitude);

            _cubePool.Release(consumed);

            a.isMerging = false;
            b.isMerging = false;
            
            OnMerge?.Invoke();
            AudioClipsManager.Instance.PlayRandomFromCollection("Popping");
        }

        private float GetActivity(Cube cube) =>
            cube.LinearVelocity.magnitude + cube.AngularVelocity.magnitude;

        private Cube GetMoreActive(Cube a, Cube b) =>
            GetActivity(a) >= GetActivity(b) ? a : b;
    }
}