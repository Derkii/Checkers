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
            if (cell is not null)
            {
                _moveManager.OnCellClick(cell);
            }
        }

        public void ImitateChipRemove(PairOfCoordinates pairOfCoordinates)
        {
            var cell = GeometryUtility.FindCellByCoordinates(pairOfCoordinates);
            if (cell is not null && cell.Pair is not null)
            {
                ((ChipComponent)cell.Pair).DestroyChip();
            }
        }

        public void ImitateChipClick(PairOfCoordinates chipPairOfCoordinates)
        {
            var cell = GeometryUtility.FindCellByCoordinates(chipPairOfCoordinates);
            if (cell is not null && cell.Pair is not null)
            {
                _moveManager.OnChipClick(cell.Pair);
            }
        }
    }
}