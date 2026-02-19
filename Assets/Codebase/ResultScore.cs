using TMPro;
using UnityEngine;

namespace Codebase
{
    public class ResultScore : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;

        public void Set(int score)
        {
            scoreText.text = $"Final Score: {score}";
        }
    }
}