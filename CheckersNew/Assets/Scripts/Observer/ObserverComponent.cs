using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Game;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Utils.Coordinates;
using Zenject;

namespace Observer
{
    public class ObserverComponent : MonoBehaviour
    {
        [SerializeField, Range(0.003f, 100f)] private float _chipClickDelay;
        [SerializeField, Range(0.003f, 100f)] private float _cellClickDelay;
        [SerializeField] private float _chipRemoveDelay;
        [SerializeField] private bool _isReplaying, _isRecording;
        [Inject] private MoveManagerObservable _observable;

        private string _pathToRecordFile =>
            Path.GetFullPath("Assets\\Record.txt");
        private bool _isWhiteTurn => _observable.IsWhiteTurn;
        private string _playerColor =>_isWhiteTurn ? "Player1" : "Player2";

        public float CellClickDelay => _cellClickDelay;
        public float ChipClickDelay => _chipClickDelay;
        public float ChipRemoveDelay => _chipRemoveDelay;
        public bool IsReplaying => _isReplaying;
        public bool IsRecording => _isRecording;
        public void Start()
        {
            if (_isRecording && _isReplaying)
            {
                throw new Exception("You can't write into the record file and replay from the record file parallel");
            }
            if (_observable != null)
            {
                if (IsRecording)
                {
                    _observable.OnCellClickAction += OnCellClick;
                    _observable.OnChipClickAction += OnChipClick;
                    _observable.OnChipRemoveAction += OnChipRemove;
                }
                else
                {
                    if (IsReplaying)
                    {
                        ReplayFromRecordFile();
                    }
                }
            }
        }
#if UNITY_EDITOR
        public void OnValidate()
        {
            if (EditorApplication.isPlaying == false) return;
            
            if (_isRecording && _isReplaying)
            {
                throw new Exception("You can't write into the record file and replay from the record file parallel");
            }
        }

        [ContextMenu(nameof(ResetTextInRecordFile))]
        public void ResetTextInRecordFile()
        {
            DoesFileExist();

            File.WriteAllText(_pathToRecordFile, string.Empty);
        }
#endif
        private void WriteNewLineIntoRecordFile(string message)
        {
            DoesFileExist();

            using var stream = new FileStream(_pathToRecordFile, FileMode.Append);
            using var writer = new StreamWriter(stream);
            writer.WriteLineAsync(message);
        }

        private void DoesFileExist()
        {
            if (File.Exists(_pathToRecordFile) == false)
            {
                throw new Exception("File Record.txt isn't found");
            }
        }

        private async UniTaskVoid ReplayFromRecordFile()
        {
            var imitationController = new ImitationController(this, _observable);

            var lines = await File.ReadAllLinesAsync(_pathToRecordFile);
            foreach (var line in lines)
            {
                var splittedLine = line.Split(" ");

                //Structure of command in the Record file is: action what COORDINATES - PlayerNUM.
                //Example: white chip(Player1) ate chip on 5B, then it would be "ate chip 5B - Player1" in the Record file.

                var what = splittedLine[1];
                var action = splittedLine[0];
                var pairOfCoordinates = splittedLine[2].StringToCoordinates();
                await imitationController.Logic(what, action, pairOfCoordinates);
                while (_observable.IsCameraMoving || _observable.IsChipMoving)
                    await UniTask.Yield();
            }
        }

        

        private void OnChipRemove(PairOfCoordinates pairOfCoordinates)
        {
            if (IsRecording)
            {
                var str = $"ate chip {pairOfCoordinates.CoordinatesToString()} - {_playerColor}";
                WriteNewLineIntoRecordFile(str);
            }
        }

        private void OnChipClick(PairOfCoordinates clickedChipPairOfCoordinates)
        {
            if (IsRecording)
            {
                var str =
                    $"click chip {CoordinatesHelper.CoordinatesToString(clickedChipPairOfCoordinates)} - {_playerColor}";
                WriteNewLineIntoRecordFile(str);
            }
        }


        private void OnCellClick(PairOfCoordinates clickedCellPairOfCoordinates)
        {
            string str =
                $"click cell {CoordinatesHelper.CoordinatesToString(clickedCellPairOfCoordinates)} - {_playerColor}";
            WriteNewLineIntoRecordFile(str);
        }
    }
}