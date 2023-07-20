using System;
using System.Linq;
using Chip;
using Observer;
using Utils.Coordinates;
using GeometryUtility = Utils.GeometryUtility;

namespace Game
{
    public class MoveManagerObservable
    {
        private readonly MoveManager _moveManager;
        public Action<PairOfCoordinates> OnCellClickAction { get; set; }
        public Action<PairOfCoordinates> OnChipClickAction { get; set; }
        public Action<PairOfCoordinates> OnChipRemoveAction { get; set; }
        public bool IsWhiteTurn => GameCharacteristics.IsWhitesTurn;

        public bool IsCameraMoving => GameCharacteristics.IsCameraMoving;


        public MoveManagerObservable(MoveManager manager)
        {
            _moveManager = manager;
        }

        public bool IsChipMoving => _moveManager.Chips.FirstOrDefault(chip => chip.IsMoving) != null;

        public void ImitateCellClick(PairOfCoordinates cellPairOfCoordinates)
        {
            var cell = GeometryUtility.FindCellByCoordinates(cellPairOfCoordinates);
            if (cell != null)
            {
                _moveManager.OnCellClick(cell);
            }
        }

        public void ImitateChipRemove(PairOfCoordinates pairOfCoordinates)
        {
            var cell = GeometryUtility.FindCellByCoordinates(pairOfCoordinates);
            if (cell != null && cell.Pair != null)
            {
                ((ChipComponent)cell.Pair).DestroyChip();
            }
        }

        public void ImitateChipClick(PairOfCoordinates chipPairOfCoordinates)
        {
            var cell = GeometryUtility.FindCellByCoordinates(chipPairOfCoordinates);
            if (cell != null && cell.Pair != null)
            {
                _moveManager.OnChipClick(cell.Pair);
            }
            else {
                if (cell == null)
                {
                    throw new Exception($"Cell doesn't exist {chipPairOfCoordinates}");
                }

                if (cell.Pair == null)
                {
                    throw new Exception($"Chip doesn't exist by coordinates {chipPairOfCoordinates}");
                }
        }
        }
    }
}