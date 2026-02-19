using UnityEngine;

namespace Codebase
{
    public class UIManager : MonoBehaviour
    {
        public GameObject inGameMenu;
        public GameObject postGameMenu;

        public ScoreCounter scoreCounter;
        public ResultScore resultScore;

        public void ShowInGameMenu()
        {
            inGameMenu.SetActive(true);
            postGameMenu.SetActive(false);
        }

        public void ShowPostGameMenu()
        {
            resultScore.Set(scoreCounter.Score);
            postGameMenu.SetActive(true);
            inGameMenu.SetActive(false);
        }
    }
}