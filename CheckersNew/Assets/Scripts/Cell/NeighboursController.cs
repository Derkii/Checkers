using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Cell
{
    public class NeighboursController
    {
        private Dictionary<NeighborType, CellComponent> _neighbours;

        public NeighboursController(CellComponent cellComponent)
        {
            _neighbours = new() {

                { NeighborType.TopLeft,  GeometryUtility.FindTopDiagonalLeftCell(cellComponent)},

                { NeighborType.TopRight, GeometryUtility.FindTopDiagonalRightCell(cellComponent) }, 

                { NeighborType.BottomLeft, GeometryUtility.FindBottomDiagonalLeftCell(cellComponent) }, 

                { NeighborType.BottomRight, GeometryUtility.FindBottomDiagonalRightCell(cellComponent) }, 

                { NeighborType.Bottom, GeometryUtility.FindBottomCell(cellComponent) }, 

                { NeighborType.Top, GeometryUtility.FindTopCell(cellComponent) }, 

                { NeighborType.Right, GeometryUtility.FindRightCell(cellComponent) },

                { NeighborType.Left, GeometryUtility.FindLeftCell(cellComponent) } 

            };
        }
        public CellComponent GetNeighbour(NeighborType type) => _neighbours[type];

    }
} 
