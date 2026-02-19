using TMPro;
using UnityEngine;

namespace Codebase
{
    public class ScoreCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        [field: SerializeField] public int Score { get; private set; }

        public void IncreaseScore()
        {
            Score++;
            UpdateVisuals();
        }

        public void ResetScore()
        {
            Score = 0;
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            text.text = $"Score: {Score}";
        }
    }
}