using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Codebase
{
    public class MergeManager : MonoBehaviour
{
    [SerializeField] private float mergeUpwardForce = 5f;

    private readonly HashSet<(Cube, Cube)> _processedPairs = new();
    private readonly HashSet<Cube> _mergingCubes = new();

    private void LateUpdate()
    {
        _processedPairs.Clear();
    }

    public event Action OnMerge;

    public void HandleCollision(Cube a, Cube b)
    {
        if (a == null || b == null) return;
        if (a.IsMerging || b.IsMerging) return;
        if (a.Value != b.Value) return;

        var pair = GetOrderedPair(a, b);
        if (!_processedPairs.Add(pair)) return;

        // Додатковий захист — перевіряємо HashSet до SetMerging
        if (!_mergingCubes.Add(a)) return;
        if (!_mergingCubes.Add(b))
        {
            _mergingCubes.Remove(a);
            return;
        }

        a.SetMerging(true);
        b.SetMerging(true);

        ProcessMerge(a, b);
    }

    private void ProcessMerge(Cube a, Cube b)
    {
        if (a == null || b == null) return;

        var survivor = GetSurvivor(a, b);
        var consumed = survivor == a ? b : a;

        var linearVelocity = (a.LinearVelocity + b.LinearVelocity) / 2f;
        var angularVelocity = (a.AngularVelocity + b.AngularVelocity) / 2f;

        var bounceDirection = (consumed.transform.position - survivor.transform.position).normalized;
        var bounceForce = linearVelocity.magnitude;

        _mergingCubes.Remove(a);
        _mergingCubes.Remove(b);

        survivor.SetMerging(false);
        survivor.Initialize(survivor.Value * 2);
        survivor.LinearVelocity = linearVelocity;
        survivor.AngularVelocity = angularVelocity;
        survivor.AddForce(Vector3.up, mergeUpwardForce);
        survivor.AddForce(bounceDirection, bounceForce);

        consumed.Recall();

        OnMerge?.Invoke();
    }

    private (Cube, Cube) GetOrderedPair(Cube a, Cube b)
    {
        return a.GetInstanceID() < b.GetInstanceID() ? (a, b) : (b, a);
    }

    private float GetActivity(Cube cube)
    {
        return cube.LinearVelocity.magnitude + cube.AngularVelocity.magnitude;
    }

    private Cube GetSurvivor(Cube a, Cube b)
    {
        return GetActivity(a) <= GetActivity(b) ? a : b;
    }
}
}