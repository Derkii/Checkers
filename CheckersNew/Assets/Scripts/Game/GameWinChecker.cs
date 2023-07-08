using Chip;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Utils.Coordinates;

namespace Game
{
    public class GameWinChecker
    {
        private MoveManager _moveManager;

        public GameWinChecker(MoveManager moveManager)
        {
            moveManager.OnChipMovedEvent += WinCheck;
        }

        private void WinCheck(ChipComponent chip, PairOfCoordinates newCoordinates)
        {
            switch (chip.Color)
            {
                case ColorType.White:
                    if (newCoordinates.VerticalToInt == 8) WhiteHadWon();
                    break;
                case ColorType.Black:
                    if (newCoordinates.VerticalToInt == 1) BlackHadWon();
                    break;
            }
        }

        private void BlackHadWon()
        {
            Debug.Log("Black won");
            Win();
        }

        private void Win()
        {
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
            GameCharacteristics.IsAlreadyWon = true;
        }

        private void WhiteHadWon()
        {
            Debug.Log("White won");
            Win();
        }
    }
}