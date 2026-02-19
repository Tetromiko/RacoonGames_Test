using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Codebase
{
    public class Cube : MonoBehaviour
    {
        [field: SerializeField] public int Value { get; private set; }

        [SerializeField] private List<TMP_Text> numberPlates;
        [SerializeField] private CubeSkins cubeSkins;
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float minCollisionVelocity = 1f;

        public bool IsMerging { get; private set; }

        public Vector3 LinearVelocity
        {
            get => rb.linearVelocity;
            set => rb.linearVelocity = value;
        }

        public Vector3 AngularVelocity
        {
            get => rb.angularVelocity;
            set => rb.angularVelocity = value;
        }

        private void OnDisable()
        {
            IsMerging = false;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out Cube other))
            {
                OnCubeCollision?.Invoke(this, other);
                return;
            }

            if (collision.relativeVelocity.magnitude >= minCollisionVelocity)
                OnCollision?.Invoke();
        }

        public event Action<Cube, Cube> OnCubeCollision;
        public event Action OnCollision;
        public event Action<Cube> OnRecall;

        public void Initialize(int value, Vector3 position, Quaternion rotation)
        {
            Initialize(value);
            transform.position = position;
            transform.rotation = rotation;
        }

        public void Initialize(int value)
        {
            Value = value;
            meshRenderer.material = cubeSkins.GetMaterial(value);

            foreach (var plate in numberPlates)
                plate.text = value.ToString();
        }

        public void AddForce(Vector3 direction, float force)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        public void SetKinematic(bool kinematic)
        {
            rb.isKinematic = kinematic;
        }

        public void SetMerging(bool merging)
        {
            IsMerging = merging;
        }

        public void Recall()
        {
            OnRecall?.Invoke(this);
        }
    }
}