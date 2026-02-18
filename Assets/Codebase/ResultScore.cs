using TMPro;
using UnityEngine;

namespace Codebase
{
    public class ResultScore: MonoBehaviour
    {
        [SerializeField]
        private TMP_Text scoreText;
        private ScoreCounter _scoreCounter;

        public void Initialize(ScoreCounter scoreCounter)
        {
            _scoreCounter = scoreCounter;
        }

        public void GetResult()
        {
            scoreText.text = $"Final Score: {_scoreCounter.Score}";
        }
    }
}