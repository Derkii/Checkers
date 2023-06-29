using Chip;
using UnityEditor;
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
            HadWon();
        }

        private void HadWon()
        {
#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
            GameCharacteristics.HasAlreadyWon = true;
        }

        private void WhiteHadWon()
        {
            Debug.Log("White won");
            HadWon();
        }
    }
}