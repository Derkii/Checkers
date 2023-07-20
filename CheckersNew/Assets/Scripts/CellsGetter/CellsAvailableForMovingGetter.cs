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

        public IEnumerable<CellComponent> CalculateMoves()
        {
            CellComponent startCell;

            #region Checks

            if (_chip != null && _chip.Pair != null)
            {
                startCell = (CellComponent)_chip.Pair;
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
                foreach (var chip in _moveManager.Chips)
                {
                    chip.CellsAvailableForMove.Clear();
                }
            }

            void AnyInAvailableCells()
            {
                Debug.Log("Aren't any fields available to move by this checker");
                _chip.SetMaterial(_chip.Color == ColorType.White
                    ? _moveManager.DefaultWhiteChipMaterial
                    : _moveManager.DefaultBlackChipMaterial, false);
            }

            #endregion

            CellComponent cell;

            void AddNeighbourCellToList(NeighborType type)
            {
                cell = GetNeighbourCellAvailableForMove(type, startCell);
                if (cell == null) return;
                cell.SetMaterial(_moveManager.AvailableForMoveCellMaterial);
                _chip.CellsAvailableForMove.Add(cell);
            }


            if (GameCharacteristics.IsWhitesTurn && _chip.Color == ColorType.White)
            {
                PreCalculate();

                AddNeighbourCellToList(NeighborType.TopLeft);

                AddNeighbourCellToList(NeighborType.TopRight);
                if (!_chip.CellsAvailableForMove.Any())
                {
                    AnyInAvailableCells();
                }
            }
            else if (GameCharacteristics.IsWhitesTurn == false && _chip.Color == ColorType.Black)
            {
                PreCalculate();

                AddNeighbourCellToList(NeighborType.BottomLeft);

                AddNeighbourCellToList(NeighborType.BottomRight);

                if (!_chip.CellsAvailableForMove.Any())
                {
                    AnyInAvailableCells();
                }
            }

            return _chip.CellsAvailableForMove;
        }

        private CellComponent GetNeighbourCellAvailableForMove(NeighborType type, CellComponent startCell,
            int findIterations = 4)
        {
            CellComponent cell = startCell;
            for (int i = 0; i < findIterations; i++)
            {
                cell = cell.NeighboursController.GetNeighbour(type);
                if (cell == null) return null;
                if (cell.Pair != null)
                {
                    if ((int)cell.Pair.Color == (int)_chip.Color || findIterations == 1 || i == findIterations - 1) return null;
                }
                else break;
            }

            return cell;
        }


        public CellsAvailableForMovingGetter(ChipComponent chipComponent, MoveManager moveManager)
        {
            _moveManager = moveManager;
            _chip = chipComponent;
        }
    }
}