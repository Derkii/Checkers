using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Checkers;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IObservably
{
    [HideInInspector]
    public List<ChipComponent> Chips = new List<ChipComponent>();
    [HideInInspector]
    public Dictionary<Coordinates, CellComponent> Cells = new Dictionary<Coordinates, CellComponent>();
    public static GameManager Manager;
    private bool _isWhiteMove = true;
    private bool _isWin = false;
    private bool _isCameraMoving = false;

    public Material FocusCellMaterial, SelectedChipMaterial, AvailableForMoveCellMaterial, DefaultWhiteChipMaterial, DefaultBlackChipMaterial, DefaultWhiteCellMaterial, DefaultBlackCellMaterial;
    private Observer _observer;
    public bool IsWhiteMove { get => _isWhiteMove; set => _isWhiteMove = value; }
    public bool IsCameraMoving { get => _isCameraMoving; set => _isCameraMoving = value; }
    public bool IsWin { get => _isWin; set => _isWin = value; }
    public Action<Coordinates> ObserverOnCellClick { get; set; }
    public Action<Coordinates> ObserverOnChipClick { get; set; }
    public Action<Coordinates> ImitateCellClickAction { get; set; }
    public Action<Coordinates> ImitateChipClickAction { get; set; }
    public Action<Coordinates> ImitateChipRemoveAction { get; set; }
    public Action<Coordinates> ObserverOnChipRemoveAction { get; set; }
    public AudioClip MoveSound, AteSound;
    public Material materialBeforeChanged = null;

    [SerializeField]
    private float _timeForCameraMove;
    [SerializeField]
    private float _timeForChipsMove;
    public float TimeForChipsMove { get => _timeForChipsMove; set => _timeForChipsMove = value; }
    public float TimeForCameraMove { get => _timeForCameraMove; set => _timeForCameraMove = value; }
    private void Awake()
    {
        _observer = FindObjectOfType<Observer>();
        if (_observer != null)
        {
            _observer._observablyObject = this;
        }

        var FoundCells = FindObjectsOfType<CellComponent>();
        var FoundChips = FindObjectsOfType<ChipComponent>();

        foreach (var item in FoundChips)
        {
            Chips.Add(item);
        }

        foreach (var item in FoundCells)
        {
            if (Cells.ContainsKey(item.CoordinatesOnField) == true)
            {
                continue;
            }
            Cells.Add(item.CoordinatesOnField, item);
        }

        foreach (var item in Chips)
        {
            item.MeshRenderer.material = item.GetColor == ColorType.White ? DefaultWhiteChipMaterial : DefaultBlackChipMaterial;
            item.OnClickEventHandler += OnChipClick;
            item.OnFocusEventHandler += OnChipFocus;

        }
        foreach (var item in Cells)
        {
            item.Value.OnClickEventHandler += OnCellClick;
            item.Value.OnFocusEventHandler += OnCellFocus;
        }
        ImitateCellClickAction += ImitateCellClick;
        ImitateChipClickAction += ImitateChipClick;
        ImitateChipRemoveAction += ImitateChipRemove;
        ImitateChipRemoveAction += ImitateChipRemove;
        ObserverOnChipRemoveAction += OnChipRemove;
    }

    private void ImitateChipRemove(Coordinates coordinates)
    {
        var cell = FindCellByCoordinates(coordinates);
        if (cell != null && cell.Pair != null)
        {
            StartCoroutine(((ChipComponent)cell.Pair).DestroyChip());
        }
    }

    private void OnChipFocus(CellComponent component, bool isSelect)
    {
        if (_observer.IsReplay == false)
        {
            OnCellFocus(component, isSelect);
        }
    }
    private void OnChipRemove(Coordinates coordinates)
    {
        _observer?.OnChipRemove(coordinates);
    }
    private void OnCellFocus(CellComponent component, bool isSelect)
    {
        if (_observer.IsReplay == false)
        {
            bool isChipMovng = false;
            foreach (var item in Chips)
            {
                if (item.IsMoving == true)
                {
                    isChipMovng = true;
                    break;
                }
            }


            if (_isCameraMoving == true || isChipMovng == true)
            {
                materialBeforeChanged = component.GetColor == ColorType.White ? DefaultWhiteCellMaterial : DefaultBlackCellMaterial;
                return;
            }
            if (isSelect == true)
            {

                materialBeforeChanged = component.MeshRenderer.material;
                component.MeshRenderer.material = FocusCellMaterial;
            }
            else
            {
                component.MeshRenderer.material = materialBeforeChanged;
            }
        }
    }

    private void OnCellClick(BaseClickComponent component)
    {
        ObserverOnCellClick?.Invoke(((CellComponent)component).CoordinatesOnField);
        if (component.Pair != null)
        {
            RecalcMoves(component.Pair);
        }
        foreach (var item in Chips)
        {
            if (item.CellsAvailableForMove.ContainsValue((CellComponent)component))
            {
                _isWhiteMove = !_isWhiteMove;
                _isCameraMoving = true;
                item.Pair.Pair = null;
                StartCoroutine(item.MoveToCell((CellComponent)component, _timeForChipsMove, _timeForCameraMove, MoveSound));
                item.CellsAvailableForMove.Clear();
                CellsAndChipsMaterialsZeroing();
                break;
            }
        }
    }

    private void OnChipClick(BaseClickComponent component)
    {
        ObserverOnChipClick?.Invoke(((ChipComponent)component).CoordinatesOnField);

        RecalcMoves(component);
    }

    private void RecalcMoves(BaseClickComponent component)
    {
        ChipComponent chip = (ChipComponent)component;
        CellComponent cell;
        if (component != null && component.Pair != null)
        {
            cell = (CellComponent)component.Pair;
        }
        else
        {
            return;
        }


        if (_isCameraMoving == true)
        {
            Debug.Log("Нельзя ходить когда перемещается камера");
            return;
        }

        if (chip.GetColor == ColorType.White && _isWhiteMove == false)
        {
            Debug.Log("Сейчас ходит противник");
            return;
        }
        
        if (chip.GetColor == ColorType.Black && _isWhiteMove == true)
        {
            Debug.Log("Сейчас ходит противник");
            return;
        }
        if (_isWin == true)
        {
            Debug.Log("Игра уже закончилась!");
            return;
        }
        if (_isWhiteMove == true && chip.GetColor == ColorType.White && _isCameraMoving == false)
        {
            CellComponent availableForMoveCell = cell.GetNeighbors(NeighborType.TopLeft);
            CellsAndChipsMaterialsZeroing();
            chip.MeshRenderer.material = SelectedChipMaterial;
            foreach (var item in Chips)
            {
                item.CellsAvailableForMove.Clear();
            }

            if (availableForMoveCell != null)
            {
                if (availableForMoveCell.Pair != null)
                {
                    if ((int)availableForMoveCell.Pair.GetColor != (int)chip.GetColor)
                    {
                        availableForMoveCell = cell.GetNeighbors(NeighborType.TopLeft).GetNeighbors(NeighborType.TopLeft);

                        if (availableForMoveCell != null && availableForMoveCell.Pair == null)
                        {
                            availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                            chip.CellsAvailableForMove.Add(NeighborType.TopLeft, availableForMoveCell);
                        }
                    }
                }
                else
                {
                    availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                    chip.CellsAvailableForMove.Add(NeighborType.TopLeft, availableForMoveCell);
                }
            }

            availableForMoveCell = cell.GetNeighbors(NeighborType.TopRight);
            if (availableForMoveCell != null)
            {
                if (availableForMoveCell.Pair != null)
                {
                    if ((int)availableForMoveCell.Pair.GetColor != (int)chip.GetColor)
                    {
                        availableForMoveCell = cell.GetNeighbors(NeighborType.TopRight).GetNeighbors(NeighborType.TopRight);
                        if (availableForMoveCell != null && availableForMoveCell.Pair == null)
                        {
                            availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;
                            chip.CellsAvailableForMove.Add(NeighborType.TopRight, availableForMoveCell);
                        }

                    }
                }
                else
                {
                    availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                    chip.CellsAvailableForMove.Add(NeighborType.TopRight, availableForMoveCell);
                }

            }
            if (chip.CellsAvailableForMove.Count() <= 0)
            {
                Debug.Log("Вам некуда ходить этой шашкой, выберите другую");
                chip.MeshRenderer.material = chip.GetColor == ColorType.White ? DefaultWhiteChipMaterial : DefaultBlackChipMaterial;
            }
        }
        else if (_isWhiteMove == false && chip.GetColor == ColorType.Black && _isCameraMoving == false)
        {

            CellComponent availableForMoveCell = cell.GetNeighbors(NeighborType.BottomLeft);
            CellsAndChipsMaterialsZeroing();
            chip.MeshRenderer.material = SelectedChipMaterial;
            foreach (var item in Chips)
            {
                item.CellsAvailableForMove.Clear();
            }

            if (availableForMoveCell != null)
            {
                if (availableForMoveCell.Pair != null)
                {
                    if ((int)availableForMoveCell.Pair.GetColor != (int)chip.GetColor)
                    {
                        availableForMoveCell = cell.GetNeighbors(NeighborType.BottomLeft).GetNeighbors(NeighborType.BottomLeft);

                        if (availableForMoveCell != null && availableForMoveCell.Pair == null)
                        {
                            availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                            chip.CellsAvailableForMove.Add(NeighborType.BottomLeft, availableForMoveCell);
                        }
                    }
                }
                else
                {
                    availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                    chip.CellsAvailableForMove.Add(NeighborType.TopLeft, availableForMoveCell);

                }
            }
            availableForMoveCell = cell.GetNeighbors(NeighborType.BottomRight);

            if (availableForMoveCell != null)
            {
                if (availableForMoveCell.Pair != null)
                {

                    if ((int)availableForMoveCell.Pair.GetColor != (int)chip.GetColor)
                    {

                        availableForMoveCell = cell.GetNeighbors(NeighborType.BottomRight).GetNeighbors(NeighborType.BottomRight);
                        if (availableForMoveCell != null && availableForMoveCell.Pair == null)
                        {
                            availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                            chip.CellsAvailableForMove.Add(NeighborType.BottomRight, availableForMoveCell);
                        }
                    }

                }
                else
                {
                    availableForMoveCell.MeshRenderer.material = AvailableForMoveCellMaterial;

                    chip.CellsAvailableForMove.Add(NeighborType.BottomRight, availableForMoveCell);
                }
            }

            if (chip.CellsAvailableForMove.Count <= 0)
            {
                Debug.Log("Вам некуда ходить этой шашкой, выберите другую");
                chip.MeshRenderer.material = chip.GetColor == ColorType.White ? DefaultWhiteChipMaterial : DefaultBlackChipMaterial;
            }
        }
    }

    public void OnAte()
    {
        AudioSource.PlayClipAtPoint(AteSound, Vector3.zero, 1f);
    }

    private void CellsAndChipsMaterialsZeroing()
    {
        foreach (var item in Chips)
        {
            item.MeshRenderer.material = item.GetColor == ColorType.White ? DefaultWhiteChipMaterial : DefaultBlackChipMaterial;
        }
        foreach (var item in Cells.Values)
        {
            item.MeshRenderer.material = item.GetColor == ColorType.White ? DefaultWhiteCellMaterial : DefaultBlackCellMaterial;
        }
    }


    private void FixedUpdate()
    {
        WinCheck();

    }
    private void WinCheck()
    {
        if (Chips.Count(t => t.GetColor == ColorType.White) == 0)
        {
            Debug.Log("Черные выиграли");
            _isWin = true;
        }
        else if (Chips.Count(t => t.GetColor == ColorType.Black) == 0)
        {
            Debug.Log("Белые выиграли");
            _isWin = true;
        }
        foreach (var item in Chips)
        {
            if (item.GetColor == ColorType.Black)
            {
                if (item.CoordinatesOnField._verticalToInt == 1)
                {
                    Debug.Log("Черные выиграли");
                    _isWin = true;
                }
            }
            if (item.GetColor == ColorType.White)
            {
                if (item.CoordinatesOnField._verticalToInt == 8)
                {
                    Debug.Log("Белые выиграли");
                    _isWin = true;
                }
            }
        }
    }

    private CellComponent FindCellByCoordinates(Coordinates coordinates)
    {
        if (Cells.ContainsKey(coordinates))
        {
            return Cells[coordinates];
        }
        else
        {
            return null;
        }
    }
    private void ImitateCellClick(Coordinates cellCoordinates)
    {
        CellComponent cell = FindCellByCoordinates(cellCoordinates);
        if (cell != null)
        {
            OnCellClick(cell);
        }
    }

    private void ImitateChipClick(Coordinates chipCoordinates)
    {
        CellComponent cell = FindCellByCoordinates(chipCoordinates);
        if (cell != null && cell.Pair != null)
        {
            OnChipClick(cell.Pair);
        }
    }
}
