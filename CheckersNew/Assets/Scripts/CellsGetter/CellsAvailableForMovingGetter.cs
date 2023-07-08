using System;
using System.Collections.Generic;
using System.Linq;
using Cell;
using Chip;
using Game;
using UnityEngine;

namespace CellsGetter
{
    public class CellsAvailableForMovingGetter : IGetCellsAvailableForMovingStrategy
    {
        private ChipComponent _chip;
        private MoveManager _moveManager;

        private readonly Dictionary<NeighborType, CellComponent> _cellsAvailableForMove =
            new();

        
        public IEnumerable<CellComponent> CalculateMoves()
        {
            CellComponent cell;

            if (_chip is not null && _chip.Pair is not null)
            {
                cell = (CellComponent)_chip.Pair;
            }
            else
            {
                throw new NotImplementedException();
            }

            if (GameCharacteristics.IsAlreadyWon)
            {
                Debug.Log("Game has already finished");
                return null;
            }
            if (GameCharacteristics.IsCameraMoving)
            {
                Debug.Log("You can't move while camera is moving");
                return null;
            }

            if (_chip.IsMoving)
            {
                Debug.Log("You can't move while another chip is moving");
                return null;
            }

            if (_chip.Color == ColorType.White
                != GameCharacteristics.IsWhitesTurn)
            {
                Debug.Log("Not your turn");
                return null;
            }

            void PreCalculate()
            {
                _moveManager.CellsAndChipsMaterialsClear(new[] { _chip.Pair });
                _chip.SetMaterial(_moveManager.SelectedChipMaterial);
                foreach (var _ in _moveManager.Chips)
                {
                    _cellsAvailableForMove.Clear();
                }
            }

            void AnyInAvailableCells()
            {  
                Debug.Log("Aren't any fields available to move by this checker");
                _chip.SetMaterial(_chip.Color == ColorType.White
                    ? _moveManager.DefaultWhiteChipMaterial
                    : _moveManager.DefaultBlackChipMaterial, false);
            }

            if (GameCharacteristics.IsWhitesTurn && _chip.Color == ColorType.White &&
                GameCharacteristics.IsCameraMoving == false)
            {
                PreCalculate();

                CheckNeighbourCellsToAvailableForMove(NeighborType.TopLeft, cell);

                CheckNeighbourCellsToAvailableForMove(NeighborType.TopRight, cell);
                if (!_cellsAvailableForMove.Any())
                {
                    AnyInAvailableCells();
                }
            }
            else if (GameCharacteristics.IsWhitesTurn == false && _chip.Color == ColorType.Black &&
                     GameCharacteristics.IsCameraMoving == false)
            {
                PreCalculate();

                CheckNeighbourCellsToAvailableForMove(NeighborType.BottomLeft, cell);

                CheckNeighbourCellsToAvailableForMove(NeighborType.BottomRight, cell);

                if (!_cellsAvailableForMove.Any())
                {
                    AnyInAvailableCells();
                }
            }

            return _cellsAvailableForMove.Values;
        }

        private void CheckNeighbourCellsToAvailableForMove(NeighborType type, CellComponent startCell)
        {
            var cell = startCell.NeighboursController.GetNeighbour(type);
            if (cell is null) return;

            if (cell.Pair is not null)
            {
                if ((int)cell.Pair.Color == (int)_chip.Color) return;

                cell = cell.NeighboursController.GetNeighbour(type);

                if (cell is null || cell.Pair is not null) return;
            }
            cell.SetMaterial(_moveManager.AvailableForMoveCellMaterial);

            _cellsAvailableForMove.Add(type, cell);
        }


        public CellsAvailableForMovingGetter(ChipComponent chipComponent, MoveManager moveManager)
        {
            _moveManager = moveManager;
            _chip = chipComponent;
        }
    }
}