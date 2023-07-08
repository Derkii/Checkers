using System;
using System.Collections;
using System.Collections.Generic;
using Cell;
using CellsGetter;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using GeometryUtility = Utils.GeometryUtility;
using DG.Tweening;
using Utils.Coordinates;

namespace Chip
{
    public class ChipComponent : BaseClickComponent
    {
        private RaycastHit[] _results = new RaycastHit[2];
        private IGetCellsAvailableForMovingStrategy _getCellsStrategy;
        public Action<PairOfCoordinates> OnChipMoved;
        public bool IsMoving;
        public IGetCellsAvailableForMovingStrategy GetCellsStrategy => _getCellsStrategy;
        public List<CellComponent> CellsAvailableForMove { get; set; } = new();


        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (IsMoving) return;
            if (_observer != null && _observer.IsReplaying == false)
            {
                OnFocusEventInvoke((CellComponent)Pair, true);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (IsMoving) return;
            if (_observer != null && _observer.IsReplaying == false)
            {
                OnFocusEventInvoke((CellComponent)Pair, false);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _getCellsStrategy = new CellsAvailableForMovingGetter(this, _moveManager);
        }

        private void Start()
        {
            var cell = GeometryUtility.GetNearestCell(this);
            Pair = cell;
        }

        public async UniTaskVoid MoveToCell(CellComponent cell, float timeForMove, float timeForCameraRotate)
        {
            IsMoving = true;
            var position = transform.position;
            var cellPosition = cell.transform.position;
            var endPosition = new Vector3(cellPosition.x, position.y, cellPosition.z);
            var tween = transform.DOMove(endPosition, timeForMove);
            await tween.AsyncWaitForCompletion();
            IsMoving = false;
            Pair = cell;
            OnChipMoved?.Invoke(cell.CoordinatesOnField);

            await UniTask.WaitForFixedUpdate();

            var count = Physics.RaycastNonAlloc(new Ray(position, (endPosition - position).normalized),
                _results, 2f);
            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    var raycastHit = _results[index];
                    if (raycastHit.transform != transform &&
                        raycastHit.transform.TryGetComponent(out ChipComponent chip))
                    {
                        chip.DestroyChip();
                        break;
                    }
                }
            }

            await UniTaskHelper.CameraRotate(timeForCameraRotate);
        }

        public void DestroyChip()
        {
            void Remove()
            {
                _moveManager.OnChipRemoveActionInvoke(CoordinatesOnField);
                Pair.Pair = null;
                Pair.SetPreviousMaterial();
                Destroy(gameObject);
                _moveManager.Chips.RemoveAt(_moveManager.Chips.FindIndex(t => t == this));
            }

            if (_observer.IsReplaying)
                UniTaskHelper.DoAfterSeconds(Remove, _observer.ChipRemoveDelay);
            else
            {
                Remove();
            }
        }
    }
}