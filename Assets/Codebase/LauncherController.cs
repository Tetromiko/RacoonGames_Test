using UnityEngine;
using UnityEngine.InputSystem;

namespace Codebase
{
    public class LauncherController : MonoBehaviour
    {
        [SerializeField] private float aimThreshold = 1f;
        [SerializeField] private float maxAimDistance = 5f;
        [SerializeField] private Launcher launcher;

        private Camera _camera;
        private bool _isAiming;
        private bool _isTouching;
        private Vector3 _touchStartPosition;

        private float AimRange => maxAimDistance - aimThreshold;

        private void Update()
        {
            if (!_isTouching) return;

            var touchPos = GetPlaneTouchPosition();

            if (_isAiming)
                HandleAiming(touchPos);
            else
                HandleMoving(touchPos);
        }

        public void Initialize(Launcher launcher)
        {
            this.launcher = launcher;
            _camera = Camera.main;
        }

        public void StartFollowing()
        {
            _touchStartPosition = GetPlaneTouchPosition();
            _isTouching = true;
            _isAiming = false;
            launcher.Move(_touchStartPosition);
        }

        public void StopFollowing()
        {
            if (!_isTouching) return;

            if (_isAiming)
                launcher.Launch();

            _isTouching = false;
            _isAiming = false;
        }

        private void HandleAiming(Vector3 touchPos)
        {
            var launcherPos = launcher.transform.position;
            var aimOrigin = new Vector3(_touchStartPosition.x, 0f, launcherPos.z);
            var aim = aimOrigin - touchPos;
            var pull = Mathf.Clamp(aim.magnitude - aimThreshold, 0f, AimRange) / AimRange;

            launcher.Aim(aim.normalized, pull);

            var pulledBack = Vector3.Distance(touchPos, new Vector3(touchPos.x, 0f, launcherPos.z)) < aimThreshold;
            if (pulledBack)
                _isAiming = false;
        }

        private void HandleMoving(Vector3 touchPos)
        {
            launcher.Move(touchPos);
            _touchStartPosition = touchPos;

            var draggedBack = Mathf.Max(launcher.transform.position.z - touchPos.z, 0f) > aimThreshold;
            if (draggedBack)
                _isAiming = true;
        }

        private Vector3 GetPlaneTouchPosition()
        {
            var screenPos = Touchscreen.current?.primaryTouch.position.ReadValue()
                            ?? new Vector2(Screen.width / 2f, Screen.height / 2f);

            var ray = _camera.ScreenPointToRay(screenPos);
            var plane = new Plane(Vector3.up, Vector3.up);
            plane.Raycast(ray, out var enter);

            var position = ray.GetPoint(enter);
            position.y = 0f;
            return position;
        }
    }
}