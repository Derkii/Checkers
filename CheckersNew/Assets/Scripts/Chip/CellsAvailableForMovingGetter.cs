using System;
using System.Collections.Generic;
using System.Linq;
using Cell;
using Game;
using UnityEngine;

namespace Chip
{
    public class CellsAvailableForMovingGetter
    {
        private ChipComponent _chip;
        private MoveManager _moveManager;

        public readonly Dictionary<NeighborType, CellComponent> CellsAvailableForMove =
            new();

        
        public void CalculateMoves()
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

            if (GameCharacteristics.HasAlreadyWon)
            {
                Debug.Log("Game has already finished");
                return;
            }
            if (GameCharacteristics.IsCameraMoving)
            {
                Debug.Log("You can't move while camera is moving");
                return;
            }

            if (_chip.IsMoving)
            {
                Debug.Log("You can't move while another chip is moving");
                return;
            }

            if (_chip.Color == ColorType.White
                != GameCharacteristics.IsWhiteTurn)
            {
                Debug.Log("Not your turn!");
                return;
            }

            void PreCalculate()
            {
                _moveManager.CellsAndChipsMaterialsClear(new[] { _chip.Pair });
                _chip.SetMaterial(_moveManager.SelectedChipMaterial);
                foreach (var _ in _moveManager.Chips)
                {
                    CellsAvailableForMove.Clear();
                }
            }

            void IfArentAnyInAvailableCells()
            {
                if (CellsAvailableForMove.Any()) return;
                Debug.Log("Вам некуда ходить этой шашкой, выберите другую");
                _chip.SetMaterial(_chip.Color == ColorType.White
                    ? _moveManager.DefaultWhiteChipMaterial
                    : _moveManager.DefaultBlackChipMaterial, false);
            }

            if (GameCharacteristics.IsWhiteTurn && _chip.Color == ColorType.White &&
                GameCharacteristics.IsCameraMoving == false)
            {
                PreCalculate();

                CheckNeighbourCellsToAvailableForMove(NeighborType.TopLeft, cell);

                CheckNeighbourCellsToAvailableForMove(NeighborType.TopRight, cell);

                IfArentAnyInAvailableCells();
            }
            else if (GameCharacteristics.IsWhiteTurn == false && _chip.Color == ColorType.Black &&
                     GameCharacteristics.IsCameraMoving == false)
            {
                PreCalculate();

                CheckNeighbourCellsToAvailableForMove(NeighborType.BottomLeft, cell);

                CheckNeighbourCellsToAvailableForMove(NeighborType.BottomRight, cell);

                IfArentAnyInAvailableCells();
            }
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

                cell.SetMaterial(_moveManager.AvailableForMoveCellMaterial);

                CellsAvailableForMove.Add(type, cell);
            }
            else
            {
                cell.SetMaterial(_moveManager.AvailableForMoveCellMaterial);

                CellsAvailableForMove.Add(type, cell);
            }
        }


        public CellsAvailableForMovingGetter(ChipComponent chipComponent, MoveManager moveManager)
        {
            _moveManager = moveManager;
            _chip = chipComponent;
        }
    }
}