using System;
using System.Collections.Generic;
using System.Linq;
using Cell;
using UnityEngine;

namespace Game
{
    [Serializable]
    public class CheckersMaterialsController
    {
        [SerializeField] private Material _selectedChipMaterial,
            _focusCellMaterial,
            _availableForMoveCellMaterial,
            _defaultWhiteChipMaterial,
            _defaultBlackChipMaterial,
            _defaultWhiteCellMaterial,
            _defaultBlackCellMaterial;

        private MoveManager _moveManager;

        public Material DefaultWhiteCellMaterial => _defaultWhiteCellMaterial;

        public Material DefaultBlackCellMaterial => _defaultBlackCellMaterial;

        public Material SelectedChipMaterial => _selectedChipMaterial;

        public Material FocusCellMaterial => _focusCellMaterial;

        public Material AvailableForMoveCellMaterial => _availableForMoveCellMaterial;

        public Material DefaultWhiteChipMaterial => _defaultWhiteChipMaterial;

        public Material DefaultBlackChipMaterial => _defaultBlackChipMaterial;

        private void SetMaterialDependsOnColor(BaseClickComponent baseClickComponent, Material whiteMaterial,
            Material blackMaterial)
        {
            baseClickComponent.SetMaterial(GetMaterialDependsOnColor(baseClickComponent, whiteMaterial, blackMaterial),
                false);
        }

        public Material GetMaterialDependsOnColor(BaseClickComponent baseClickComponent, Material whiteMaterial,
            Material blackMaterial)
        {
            return
                baseClickComponent.Color == ColorType.White ? whiteMaterial : blackMaterial;
        }

        public void SetMaterialDependsOnColor(BaseClickComponent baseClick)
        {
            SetMaterialDependsOnColor(baseClick, _defaultWhiteChipMaterial, _defaultBlackChipMaterial);
        }

        public void SetMaterialDependsOnColor(CellComponent cell)
        {
            SetMaterialDependsOnColor(cell, _defaultWhiteCellMaterial, _defaultBlackCellMaterial);
        }

        public void OnFocus(CellComponent cell, bool isSelected)
        {
            if (isSelected)
            {
                cell.SetMaterial(_focusCellMaterial);
            }
            else
            {
                cell.SetPreviousMaterial();
            }
        }

        public void SetMoveManager(MoveManager moveManager)
        {
            _moveManager = moveManager;
        }

        public void CellsAndChipsMaterialsZeroing(IEnumerable<BaseClickComponent> excluditions = null)
        {
            BaseClickComponent[] baseClickComponents = null;
            if (excluditions != null)
                baseClickComponents = excluditions as BaseClickComponent[] ?? excluditions.ToArray();

            for (var index = 0; index < _moveManager.Chips.Count; index++)
            {
                var item = _moveManager.Chips[index];
                if (baseClickComponents != null && baseClickComponents.Contains(item)) continue;
                if (item == null)
                {
                    _moveManager.Chips.Remove(item);
                    continue;
                }

                SetMaterialDependsOnColor(item);
                item.PreviousMaterial = null;
            }

            var cells = _moveManager.Cells;
            foreach (var item in cells)
            {
                if (baseClickComponents != null && baseClickComponents.Contains(item.Value)) continue;
                if (item.Value == null)
                {
                    cells.Remove(item.Key);
                    continue;
                }

                SetMaterialDependsOnColor(item.Value);
                item.Value.PreviousMaterial = null;
            }
        }
    }
}