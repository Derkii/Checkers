using System.Linq;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public abstract class BaseClickComponent : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Цветовая сторона игрового объекта"), SerializeField]
        protected ColorType _color;

        protected Observer _observer;

        /// <summary>
        /// Возвращает цветовую сторону игрового объекта
        /// </summary>
        public ColorType GetColor => _color;

        /// <summary>
        /// Возвращает или устанавливает пару игровому объекту
        /// </summary>
        /// <remarks>У клеток пара - фишка, у фишек - клетка</remarks>
        public BaseClickComponent Pair { get; set; }

        /// <summary>
        /// Событие клика на игровом объекте
        /// </summary>
        public event ClickEventHandler OnClickEventHandler;

        /// <summary>
        /// Событие наведения и сброса наведения на объект
        /// </summary>
        public event FocusEventHandler OnFocusEventHandler;

        //При навадении на объект мышки, вызывается данный метод
        //При наведении на фишку, должна подсвечиваться клетка под ней
        //При наведении на клетку - подсвечиваться сама клетка
        public abstract void OnPointerEnter(PointerEventData eventData);

        //Аналогично методу OnPointerEnter(), но срабатывает когда мышка перестает
        //указывать на объект, соответственно нужно снимать подсветку с клетки
        public abstract void OnPointerExit(PointerEventData eventData);

        //При нажатии мышкой по объекту, вызывается данный метод
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_observer != null && _observer.IsReplay == false)
            {
                OnClickEventHandler?.Invoke(this);
            }
        }
        protected GameManager _gameManager;
        public MeshRenderer MeshRenderer;
        public CellComponent FindCellByCoordinates(Coordinates coordinates)
        {
            _gameManager = FindObjectOfType<GameManager>();
            if (_gameManager.Cells.ContainsKey(coordinates))
            {
                return _gameManager.Cells[coordinates];
            }
            else
            {
                return null;
            }
        }
        public CellComponent FindCellByCoordinates(Vertical_Coordinates vertical, Horizontal_Coordinates horizontal)
        {
            _gameManager = FindObjectOfType<GameManager>();
            Coordinates coordinates = new Coordinates(vertical, horizontal);
            if (_gameManager.Cells.ContainsKey(coordinates))
            {
                return _gameManager.Cells[coordinates];
            }
            else
            {
                return null;
            }
        }
        public CellComponent FindRightCell(CellComponent cell)
        {
            if (cell == null) return null;
            Coordinates cellCoordinatesCopy = cell.CoordinatesOnField;

            if (cellCoordinatesCopy._horizontalToInt != 8)
            {
                cellCoordinatesCopy._horizontalToInt += 1;
                return FindCellByCoordinates(cellCoordinatesCopy);
            }
            else
            {
                return null;
            }
        }
        public CellComponent FindLeftCell(CellComponent cell)
        {
            if (cell == null) return null;
            Coordinates cellCoordinatesCopy = cell.CoordinatesOnField;

            if (cellCoordinatesCopy._horizontalToInt != 1)
            {
                cellCoordinatesCopy._horizontalToInt -= 1;
                return FindCellByCoordinates(cellCoordinatesCopy);
            }
            else
            {
                return null;
            }
        }
        public CellComponent FindTopCell(CellComponent cell)
        {

            if (cell == null) return null;
            Coordinates cellCoordinatesCopy = cell.CoordinatesOnField;

            if (cellCoordinatesCopy._verticalToInt != 8)
            {
                cellCoordinatesCopy._verticalToInt += 1;
                return FindCellByCoordinates(cellCoordinatesCopy);
            }
            else
            {
                return null;
            }
        }
        public CellComponent FindBottomCell(CellComponent cell)
        {
            if (cell == null) return null;

            Coordinates cellCoordinatesCopy = cell.CoordinatesOnField;

            if (cellCoordinatesCopy._verticalToInt != 1)
            {
                cellCoordinatesCopy._verticalToInt -= 1;
                return FindCellByCoordinates(cellCoordinatesCopy);
            }
            else
            {
                return null;
            }
        }
        public CellComponent FindTopDiagonalRightCell(CellComponent cell)
        {
            return FindRightCell(FindTopCell(cell));

        }
        public CellComponent FindTopDiagonalLeftCell(CellComponent cell)
        {
            return FindLeftCell(FindTopCell(cell));
        }
        public CellComponent FindBottomDiagonalRightCell(CellComponent cell)
        {
            return FindRightCell(FindBottomCell(cell));

        }
        public CellComponent FindBottomDiagonalLeftCell(CellComponent cell)
        {
            return FindLeftCell(FindBottomCell(cell));
        }
        protected CellComponent GetNearestCell(ChipComponent chip)
        {
            float distance = float.MaxValue;
            CellComponent cell = null;
            _gameManager = FindObjectOfType<GameManager>();
            foreach (var item in _gameManager.Cells.Values)
            {
                var calc = Vector3.Distance(item.transform.position, chip.transform.position);
                if (calc < distance)
                {
                    cell = item;

                    distance = calc;
                }
            }

            return cell;
        }
        //Этот метод можно вызвать в дочерних классах (если они есть) и тем самым пробросить вызов
        //события из дочернего класса в родительский
        protected void CallBackEvent(CellComponent target, bool isSelect)
        {
            OnFocusEventHandler?.Invoke(target, isSelect);
        }

        protected virtual void Awake()
        {
            MeshRenderer = GetComponent<MeshRenderer>();
            _gameManager = FindObjectOfType<GameManager>();
            _observer = FindObjectOfType<Observer>();
        }
    }

    public enum ColorType
    {
        White,
        Black
    }

    public delegate void ClickEventHandler(BaseClickComponent component);
    public delegate void FocusEventHandler(CellComponent component, bool isSelect);
}