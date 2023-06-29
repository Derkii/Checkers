using System;
using System.Collections;
using Cell;
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
        private CellsAvailableForMovingGetter _cellsAvailableForMovingGetter;
        public Action<PairOfCoordinates> OnChipMoved;
        public bool IsMoving;

        public CellsAvailableForMovingGetter CellsAvailableForMovingGetter => _cellsAvailableForMovingGetter;


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
            _cellsAvailableForMovingGetter = new CellsAvailableForMovingGetter(this, _moveManager);
        }

        private void Start()
        {
            var cell = GeometryUtility.GetNearestCell(this);
            Pair = cell;
        }

        public IEnumerator MoveToCell(CellComponent cell, float timeForMove, float timeForCameraRotate)
        {
            IsMoving = true;
            var position = transform.position;
            Vector3 startPosition = position;
            var cellPosition = cell.transform.position;
            Vector3 endPosition = new Vector3(cellPosition.x, position.y, cellPosition.z);
            var tween = transform.DOMove(endPosition, timeForMove);
            yield return tween.WaitForCompletion();
            IsMoving = false;
            Pair = cell;
            OnChipMoved?.Invoke(cell.CoordinatesOnField);

            yield return new WaitForFixedUpdate();

            var count = Physics.RaycastNonAlloc(new Ray(startPosition, (endPosition - startPosition).normalized),
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

            Array.Clear(_results, 0, _results.Length);
            yield return UniTaskHelper.CameraRotate(timeForCameraRotate);
        }

        public void DestroyChip()
        {
            void Remove()
            {
                _moveManager.OnChipRemoveActionInvoke(CoordinatesOnField);
                Pair.Pair = null;
                Pair.SetPreviousMaterial();
                Destroy(gameObject);
                _moveManager.Chips.RemoveAt(_moveManager.Chips.FindIndex(t => t.name == name));
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