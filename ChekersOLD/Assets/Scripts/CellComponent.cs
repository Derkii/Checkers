using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{

    public class CellComponent : BaseClickComponent
    {
        private Dictionary<NeighborType, CellComponent> _neighbors;

        public Coordinates CoordinatesOnField = new Coordinates();
        /// <summary>
        /// Возвращает соседа клетки по указанному направлению
        /// </summary>
        /// <param name="type">Перечисление направления</param>
        /// <returns>Клетка-сосед или null</returns>
        public CellComponent GetNeighbors(NeighborType type) => _neighbors[type];


        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_observer != null && _observer.IsReplay == false)
            {
                CallBackEvent(this, true);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_observer != null && _observer.IsReplay == false)
            {
                CallBackEvent(this, false);

            }
        }

        /// <summary>
        /// Конфигурирование связей клеток
        /// </summary>
		public void Configuration(Dictionary<NeighborType, CellComponent> neighbors)
        {
            if (_neighbors != null) return;
            _neighbors = neighbors;
        }
        public void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();

            Configuration(new Dictionary<NeighborType, CellComponent>() {

                { NeighborType.TopLeft,  FindTopDiagonalLeftCell(this)},

                { NeighborType.TopRight, FindTopDiagonalRightCell(this) }, 

                { NeighborType.BottomLeft, FindBottomDiagonalLeftCell(this) }, 

                { NeighborType.BottomRight, FindBottomDiagonalRightCell(this) }, 

                { NeighborType.Bottom, FindBottomCell(this) }, 

                { NeighborType.Top, FindTopCell(this) }, 

                { NeighborType.Right, FindRightCell(this) },

                { NeighborType.Left, FindLeftCell(this) } 

            });
        }
    }

    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight,
        /// <summary>
        /// Клетка справа от данной
        /// </summary>
        Right,
        /// <summary>
        /// Клетка слева от данной
        /// </summary>
        Left,
        /// <summary>
        /// Клетка внизу от данной
        /// </summary>
        Bottom,
        /// <summary>
        /// Клетка сверху от данной
        /// </summary>
        Top
    }
    
    
}