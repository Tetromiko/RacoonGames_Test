using UnityEngine;

namespace Codebase
{
    [CreateAssetMenu(fileName = "CubeSkins", menuName = "Game/CubeSkins")]
    public class CubeSkins : ScriptableObject
    {
        public Material[] materials;

        public Material GetMaterial(int value)
        {
            var index = 0;
            while (value > 2)
            {
                value >>= 1;
                index++;
            }

            return materials[index % materials.Length];
        }
    }
}