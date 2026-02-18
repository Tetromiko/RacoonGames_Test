using UnityEngine;
using UnityEngine.InputSystem;

namespace Codebase
{
    public class LauncherController : MonoBehaviour
    {
        [SerializeField] private LayerMask launchPad;
        [SerializeField] private float launchThreshold;

        private Launcher _launcher;
        private bool _isFollowing;
        private float _anchorZ, _currentZ;
        private Camera _camera;

        public void Initialize(Launcher launcher)
        {
            _launcher = launcher;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!_isFollowing) return;
            if (TryGetLaunchPadMousePosition() is not { } position) return;

            _launcher.Move(position, Time.deltaTime);
            _currentZ = position.z;
        }

        public void StartFollowing()
        {
            _launcher.RequestSpawn();
            if (TryGetLaunchPadMousePosition() is not { } position) return;
            _isFollowing = true;
            _anchorZ = position.z;
        }

        public void StopFollowing()
        {
            _isFollowing = false;
            float pullDistance = _anchorZ - _currentZ;

            if (pullDistance > launchThreshold)
                _launcher.Launch(pullDistance);
        }

        private Vector3? TryGetLaunchPadMousePosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                Vector2 pos = Touchscreen.current.primaryTouch.position.ReadValue();
                var ray = _camera.ScreenPointToRay(pos);
                return Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, launchPad) 
                    ? hit.point 
                    : null;
            }
            return null;
        }
    }
}