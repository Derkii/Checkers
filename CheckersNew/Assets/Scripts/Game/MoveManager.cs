using System.Collections.Generic;
using System.Linq;
using Cell;
using Chip;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine;
using Utils.Coordinates;
using Zenject;
using GeometryUtility = Utils.GeometryUtility;

namespace Game
{
    public class MoveManager : MonoBehaviour
    {
        [SerializeField] private float _timeForCameraMove;
        [SerializeField] private float _timeForChipsMove;
        [SerializeField] private CheckersMaterialsController _checkersMaterialsController;
        [Inject] private MoveManagerObservable _moveManagerObservable;
        private GameWinChecker _winChecker;
        public List<ChipComponent> Chips { get; private set; }
        public Dictionary<PairOfCoordinates, CellComponent> Cells = new();
        public Material SelectedChipMaterial => _checkersMaterialsController.SelectedChipMaterial;
        public Material AvailableForMoveCellMaterial => _checkersMaterialsController.AvailableForMoveCellMaterial;
        public Material DefaultWhiteChipMaterial => _checkersMaterialsController.DefaultWhiteChipMaterial;
        public Material DefaultBlackChipMaterial => _checkersMaterialsController.DefaultBlackChipMaterial;
        public delegate void OnChipMoved(ChipComponent chip, PairOfCoordinates newCoordinates);
        public event OnChipMoved OnChipMovedEvent;

        private void Awake()
        {
            _checkersMaterialsController.SetMoveManager(this);
            _winChecker = new GameWinChecker(this);
            var allCells = FindObjectsOfType<CellComponent>();
            var allChips = FindObjectsOfType<ChipComponent>();
            Chips = allChips.ToList();
            Cells = allCells.ToDictionary(t => t.CoordinatesOnField, t => t);
            GeometryUtility.Cells = Cells;
            OnChipMovedEvent += (_, _) =>
                GameCharacteristics.IsWhitesTurn = !GameCharacteristics.IsWhitesTurn;
            InitCells();
        }

        private void InitCells()
        {
            foreach (var coordinates in Cells.Select(t => t.Value.CoordinatesOnField).GetDuplicates())
            {
                Debug.Log(coordinates + " " + $"{Cells.Where(T => T.Value.CoordinatesOnField == coordinates).Select(t => t.Value.name)}");
            }
            foreach (var chip in Chips)
            {
                chip.OnChipMoved += pair => OnChipMovedEvent?.Invoke(chip, pair);
                chip.OnClickEventHandler += OnChipClick;
                chip.OnFocusEventHandler += OnChipFocus;
            }

            foreach (var cell in Cells.Values)
            {
                cell.OnClickEventHandler += OnCellClick;
                cell.OnFocusEventHandler += OnCellFocus;
            }
        }


        private void OnChipFocus(BaseClickComponent component, bool isSelected)
        {
            OnCellFocus(component, isSelected);
        }


        private void OnCellFocus(BaseClickComponent cell, bool isSelected)
        {
            _checkersMaterialsController.OnFocus(cell as CellComponent, isSelected);
        }

        public void OnCellClick(BaseClickComponent component)
        {
            _moveManagerObservable.OnCellClickAction?.Invoke(component.CoordinatesOnField);
            if (component.Pair != null)
            {
                OnChipClick(component.Pair);
            }
            else
            {
                var chip = Chips.FirstOrDefault(t =>
                    t.CellsAvailableForMove.Contains(component as CellComponent));
                if (chip == null)
                {
                    CellsAndChipsMaterialsClear(new[] { component });
                    foreach (var chipComponent in Chips)
                    {
                        chipComponent.CellsAvailableForMove.Clear();
                    }
                }
                else
                {
                    chip.Pair.Pair = null;
                    CellsAndChipsMaterialsClear();
                    chip.CellsAvailableForMove.Clear();

                    chip.MoveToCell((CellComponent)component, _timeForChipsMove, _timeForCameraMove);
                }
            }
        }

        public void OnChipClick(BaseClickComponent baseClick)
        {
            _moveManagerObservable.OnChipClickAction?.Invoke(baseClick.CoordinatesOnField);

            var chipComponent = (ChipComponent)baseClick;
            chipComponent.CellsAvailableForMove = chipComponent.GetCellsStrategy.CalculateMoves().ToList();
        }

        public void OnChipRemoveActionInvoke(PairOfCoordinates pairOfCoordinatesOnField)
        {
            _moveManagerObservable.OnChipRemoveAction?.Invoke(pairOfCoordinatesOnField);
        }

        public void CellsAndChipsMaterialsClear(IEnumerable<BaseClickComponent> excluditions = null)
        {
            _checkersMaterialsController.CellsAndChipsMaterialsZeroing(excluditions);
        }
    }
}