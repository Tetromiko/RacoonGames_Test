using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Codebase
{
    public class Cube : MonoBehaviour
    {
        [field: SerializeField] public int Value { get; private set; }

        [SerializeField] private List<TMP_Text> numberPlates;
        [SerializeField] private CubeSkins cubeSkins;

        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Rigidbody rb;
        
        public bool isMerging;
        
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

        public void AddForce(Vector3 direction, float force) =>
            rb.AddForce(direction * force, ForceMode.Impulse);

        public void SetKinematic(bool kinematic)
        {
            rb.isKinematic = kinematic;
            isMerging = kinematic;
        }

        public void Initialize(int value, Vector3 position, Quaternion rotation)
        {
            Initialize(value);
            transform.position = position;
            transform.rotation = rotation;
        }

        public void Initialize(int value)
        {
            Value = value;
            meshRenderer.material = cubeSkins.GetMaterial(Value);
            foreach (var plate in numberPlates)
                plate.text = value.ToString();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var pitch = Random.Range(2f, 5f);
            var volume = 0.1f;
            AudioClipsManager.Instance.PlaySound("hit", pitch, volume);
            if (isMerging) return;
            if (!collision.gameObject.TryGetComponent(out Cube other)) return;
            MergeManager.Instance.RegisterContact(this, other);
        }

        private void OnDisable()
        {
            isMerging = false;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}