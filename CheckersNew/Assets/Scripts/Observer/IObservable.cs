using System;
using Utils.Coordinates;

namespace Observer
{
    public interface IObservable
    {
        public bool IsCameraMoving { get; }
        public bool IsWhiteMove { get; }
        public Action<PairOfCoordinates> OnCellClickAction { get; set; }

        public Action<PairOfCoordinates> OnChipClickAction { get; set; }
        public Action<PairOfCoordinates> OnChipRemoveAction { get; set; }
        public bool IsChipMoving { get; }

        public void ImitateCellClick(PairOfCoordinates pairOfCoordinates);

        public void ImitateChipClick(PairOfCoordinates pairOfCoordinates);

        public void ImitateChipRemove(PairOfCoordinates pairOfCoordinates);

    }
}