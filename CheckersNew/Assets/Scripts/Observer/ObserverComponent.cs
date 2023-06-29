using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utils.Coordinates;
using Zenject;

namespace Observer
{
    public class ObserverComponent : MonoBehaviour
    {
        public bool IsReplaying;
        public bool IsRecording;
        [SerializeField, Range(0.003f, 100f)] private float _chipClickDelay;
        [SerializeField, Range(0.003f, 100f)] private float _cellClickDelay;
        [SerializeField] private float _chipRemoveDelay;
        [Inject] private IObservable _observable;

        public float ChipRemoveDelay => _chipRemoveDelay;
        private string _pathToRecordFile =>
                Path.GetFullPath("Assets\\Record.txt");

        private bool _isWhiteMove => _observable.IsWhiteMove;
        public float CellClickDelay => _cellClickDelay;
        public float ChipClickDelay => _chipClickDelay;

#if UNITY_EDITOR
        [ContextMenu(nameof(ResetTextInRecordFile))]
        public void ResetTextInRecordFile()
        {
            if (File.Exists(_pathToRecordFile) == false)
            {
                throw new Exception("File Record.txt isn't found");
            }

            File.WriteAllText(_pathToRecordFile, string.Empty);
        }
#endif
        private void WriteNewLineIntoRecordFile(string message)
        {
            if (File.Exists(_pathToRecordFile) == false)
            {
                throw new Exception("File Record.txt isn't found");
            }

            using var stream = new FileStream(_pathToRecordFile, FileMode.Append);
            using var writer = new StreamWriter(stream);
            writer.WriteLineAsync(message);
        }

        private async UniTask ReplayFromRecordFile()
        {
            var imitationController = new ImitationController(this, _observable);

            var lines = await File.ReadAllLinesAsync(_pathToRecordFile);
            foreach (var line in lines)
            {
                var splittedLine = line.Split(" ");

                //Structure of command in the Record file is: action what COORDINATES - PlayerNUM.
                //For instance white chip(Player1) ate chip on 5B, then it would be "ate chip 5B - Player1" in the Record file.

                var what = splittedLine[1];
                var action = splittedLine[0];
                var pairOfCoordinates = CoordinatesHelper.StringToCoordinates(splittedLine[2]);
                await imitationController.Logic(what, action, pairOfCoordinates);
                while (_observable.IsCameraMoving || _observable.IsChipMoving)
                    await UniTask.Yield();
            }
        }

        private string GetPlayer()
        {
            return _isWhiteMove ? "Player1" : "Player2";
        }

        private void OnChipRemove(PairOfCoordinates pairOfCoordinates)
        {
            if (IsRecording)
            {
                var str = $"ate chip {CoordinatesHelper.ConvertCoordinatesToString(pairOfCoordinates)} - {GetPlayer()}";
                WriteNewLineIntoRecordFile(str);
            }
        }

        private void OnChipClick(PairOfCoordinates clickedChipPairOfCoordinates)
        {
            if (IsRecording)
            {
                var str =
                    $"click chip {CoordinatesHelper.ConvertCoordinatesToString(clickedChipPairOfCoordinates)} - {GetPlayer()}";
                WriteNewLineIntoRecordFile(str);
            }
        }


        private void OnCellClick(PairOfCoordinates clickedCellPairOfCoordinates)
        {
            string str =
                $"click cell {CoordinatesHelper.ConvertCoordinatesToString(clickedCellPairOfCoordinates)} - {GetPlayer()}";
            WriteNewLineIntoRecordFile(str);
        }


        public void Start()
        {
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
    }
}