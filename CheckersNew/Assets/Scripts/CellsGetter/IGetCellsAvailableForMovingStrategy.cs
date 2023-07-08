using System.Collections.Generic;
using Cell;

namespace CellsGetter
{
    public interface IGetCellsAvailableForMovingStrategy
    {
        public IEnumerable<CellComponent> CalculateMoves();
    }
}