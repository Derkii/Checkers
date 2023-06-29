using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Checkers
{
    public class ChipComponent : BaseClickComponent
    {
        public Coordinates CoordinatesOnField = new Coordinates();

        public Dictionary<NeighborType, CellComponent> CellsAvailableForMove = new Dictionary<NeighborType, CellComponent>();
        public bool IsMoving;


        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_observer != null && _observer.IsReplay == false)
            {
                CallBackEvent((CellComponent)Pair, true);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (_observer != null && _observer.IsReplay == false)
            {
                CallBackEvent((CellComponent)Pair, false);
            }
        }

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            var cell = GetNearestCell(this);
            StartCoroutine(MoveToCell(cell, 0, false));
            Pair = cell;

        }

        public IEnumerator MoveToCell(CellComponent cell, float timeForMove, bool isRotate)
        {
            IsMoving = true;

            var currentTime = 0f;

            if (cell.CoordinatesOnField._verticalToInt == 8 && _color == ColorType.White)
            {
                _gameManager.IsWin = true;
            }

            Vector3 startPosition = transform.position;
            Vector3 endPosition = new Vector3(cell.transform.position.x, transform.position.y, cell.transform.position.z);

            while (currentTime < timeForMove)
            {
                transform.position = Vector3.Slerp(startPosition, endPosition, 1 - (timeForMove - currentTime) / timeForMove);
                currentTime += Time.deltaTime;
                yield return null;
            }
            transform.position = endPosition;
            this.CoordinatesOnField = cell.CoordinatesOnField;
            IsMoving = false;
            cell.Pair = this;
            Pair = cell;

            if (isRotate == true)
            {
                StartCoroutine(CameraRotate(0.4f));
            }

        }
        public IEnumerator MoveToCell(CellComponent cell, float timeForMove, float timeForCameraRotate,AudioClip clip)
        {
            IsMoving = true;

            var currentTime = 0f;

            if (cell.CoordinatesOnField._verticalToInt == 8 && _color == ColorType.White)
            {
                _gameManager.IsWin = true;
            }

            Vector3 startPosition = transform.position;
            Vector3 endPosition = new Vector3(cell.transform.position.x, transform.position.y, cell.transform.position.z);
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, 1f);
            while (currentTime < timeForMove)
            {
                transform.position = Vector3.Slerp(startPosition, endPosition, 1 - (timeForMove - currentTime) / timeForMove);
                currentTime += Time.deltaTime;
                yield return null;
            }
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, 1f);
            transform.position = endPosition;
            this.CoordinatesOnField = cell.CoordinatesOnField;
            IsMoving = false;
            cell.Pair = this;
            Pair = cell;

            StartCoroutine(CameraRotate(timeForCameraRotate));
        }
        private void OnCollisionEnter(Collision collision)
        {
            var collisionChip = collision.gameObject.GetComponent<ChipComponent>();

            if (collisionChip != null)
            {
                if (collisionChip.IsMoving == true)
                {
                    StartCoroutine(DestroyChip(collisionChip));
                }
            }

        }

        public IEnumerator DestroyChip(ChipComponent collisionChip)
        {
            while (true)
            {
                if (_gameManager.IsWhiteMove == true)
                {
                    if (collisionChip.CoordinatesOnField == FindBottomDiagonalRightCell((CellComponent)Pair).CoordinatesOnField || collisionChip.CoordinatesOnField == FindBottomDiagonalLeftCell((CellComponent)Pair).CoordinatesOnField)
                    {
                        _observer.OnChipRemove(CoordinatesOnField);
                        if (_observer.IsReplay == true)
                        {
                            yield return new WaitForSeconds(_observer.ChipRemoveDelay);
                        }
                        _gameManager.Chips.Remove(this);
                        Destroy(gameObject);
                        break;
                    }
                }
                else
                {
                    if (collisionChip.CoordinatesOnField == FindTopDiagonalRightCell((CellComponent)Pair).CoordinatesOnField || collisionChip.CoordinatesOnField == FindTopDiagonalLeftCell((CellComponent)Pair).CoordinatesOnField)
                    {
                        _gameManager.ObserverOnChipRemoveAction(CoordinatesOnField);
                        if (_observer.IsReplay == true)
                        {
                            yield return new WaitForSeconds(_observer.ChipRemoveDelay);
                        }
                        _gameManager.Chips.Remove(this);
                        Destroy(gameObject);
                        break;
                    }
                }

                yield return null;
            }
            yield return null;

        }
        public IEnumerator DestroyChip()
        {
            _gameManager.Chips.Remove(this);
            _gameManager.ObserverOnChipRemoveAction(CoordinatesOnField);
            Destroy(gameObject);
            yield return null;
        }
        public void OnDestroy()
        {
            _gameManager.Chips.Remove(this);
            _gameManager.OnAte();
        }
        private IEnumerator CameraRotate(float timeForMove)
        {
            if (_gameManager.IsWin == false)
            {
                _gameManager.IsCameraMoving = true;

                Camera mainCam = Camera.main;
                var currentTime = 0f;
                Quaternion end = mainCam.transform.parent.rotation * Quaternion.Euler(new Vector3(0, 180, 0));
                Quaternion start = mainCam.transform.parent.rotation;

                while (currentTime < timeForMove)
                {
                    if (_gameManager.IsWin == false)
                    {
                        mainCam.transform.parent.rotation = Quaternion.Slerp(start, end, 1 - (timeForMove - currentTime) / timeForMove);
                        currentTime += Time.deltaTime;
                        yield return null;
                    }
                    else
                    {
                        break;
                    }
                }
                if (_gameManager.IsWin == false)
                {
                    mainCam.transform.parent.rotation = end;
                }

                _gameManager.IsCameraMoving = false;
            }
        }
    }
}
