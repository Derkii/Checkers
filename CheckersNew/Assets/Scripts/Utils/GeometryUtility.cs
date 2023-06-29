using System.Collections.Generic;
using Cell;
using UnityEngine;
using Utils.Coordinates;

namespace Utils
{
    public static class GeometryUtility
    {
        public static Dictionary<PairOfCoordinates, CellComponent> Cells { private get; set; }

        public static CellComponent FindCellByCoordinates(PairOfCoordinates pairOfCoordinates)
        {
            return Cells.ContainsKey(pairOfCoordinates) ? Cells[pairOfCoordinates] : null;
        }

        public static CellComponent FindCellByCoordinates(VerticalCoordinates vertical,
            HorizontalCoordinates horizontal)
        {
            var pairOfCoordinates = new PairOfCoordinates(vertical, horizontal);
            return Cells.ContainsKey(pairOfCoordinates) ? Cells[pairOfCoordinates] : null;
        }

        public static CellComponent FindRightCell(CellComponent cell)
        {
            if (cell == null) return null;
            var cellCoordinatesCopy = cell.CoordinatesOnField;

            if (cellCoordinatesCopy.HorizontalToInt == 8) return null;
            cellCoordinatesCopy.HorizontalToInt += 1;
            return FindCellByCoordinates(cellCoordinatesCopy);
        }

        public static CellComponent FindLeftCell(CellComponent cell)
        {
            if (cell == null) return null;
            var cellPairOfCoordinatesCopy = cell.CoordinatesOnField;

            if (cellPairOfCoordinatesCopy.HorizontalToInt != 1)
            {
                cellPairOfCoordinatesCopy.HorizontalToInt -= 1;
                return FindCellByCoordinates(cellPairOfCoordinatesCopy);
            }

            return null;
        }

        public static CellComponent FindTopCell(CellComponent cell)
        {
            if (cell == null) return null;
            var cellPairOfCoordinatesCopy = cell.CoordinatesOnField;

            if (cellPairOfCoordinatesCopy.VerticalToInt != 8)
            {
                cellPairOfCoordinatesCopy.VerticalToInt += 1;
                return FindCellByCoordinates(cellPairOfCoordinatesCopy);
            }

            return null;
        }

        public static CellComponent FindBottomCell(CellComponent cell)
        {
            if (cell == null) return null;

            var cellCoordinatesCopy = cell.CoordinatesOnField;

            if (cellCoordinatesCopy.VerticalToInt == 1) return null;
            cellCoordinatesCopy.VerticalToInt -= 1;
            return FindCellByCoordinates(cellCoordinatesCopy);
        }

        public static CellComponent FindTopDiagonalRightCell(CellComponent cell)
        {
            return FindRightCell(FindTopCell(cell));
        }

        public static CellComponent FindTopDiagonalLeftCell(CellComponent cell)
        {
            return FindLeftCell(FindTopCell(cell));
        }

        public static CellComponent FindBottomDiagonalRightCell(CellComponent cell)
        {
            return FindRightCell(FindBottomCell(cell));
        }

        public static CellComponent FindBottomDiagonalLeftCell(CellComponent cell)
        {
            return FindLeftCell(FindBottomCell(cell));
        }

        public static CellComponent GetNearestCell(BaseClickComponent baseClick)
        {
            float distance = float.MaxValue;
            CellComponent cell = null;
            foreach (var item in Cells.Values)
            {
                var calc = Vector3.Distance(item.transform.position, baseClick.transform.position);
                if (!(calc < distance)) continue;
                cell = item;

                distance = calc;
            }

            return cell;
        }
    }
}