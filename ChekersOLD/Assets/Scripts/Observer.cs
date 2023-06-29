using Checkers;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class Observer : MonoBehaviour
{

    [HideInInspector]
    public IObservably _observablyObject;
    public bool IsReplay = false;
    public bool IsRecording = false;
    [SerializeField, Range(0.003f, 100f)]
    private float _chipClickDelay;
    [SerializeField, Range(0.003f, 100f)]
    private float _cellClickDelay;
    public float ChipRemoveDelay;
    private string _pathToRecordFile;
    private bool _isWhiteMove = true;
    private bool _isCameraMoving = false;
    public bool IsWhiteMove { get => _isWhiteMove; set => _isWhiteMove = value; }
    private float _cameraRotationTime = 0;


    private float _timeForCameraMove;
    private float _timeForChipsMove;
    private void Awake()
    {
        _pathToRecordFile = Path.GetFullPath("Assets\\Record.txt");

    }

    private void WriteRecordFile(string message)
    {
        if (File.Exists(_pathToRecordFile) == false)
        {
            Debug.LogError("Не найден файл Record.txt");
            return;
        }

        using (var stream = new FileStream(_pathToRecordFile, FileMode.Append))
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(message);
            }
        }
    }
    private void WriteNewLineRecordFile(string message)
    {
        WriteRecordFile(message + "\n");
    }
    private void Start()
    {
        if (_observablyObject != null)
        {
            _observablyObject.ObserverOnCellClick += OnCellClick;
            _observablyObject.ObserverOnChipClick += OnChipClick;
        }
        StartCoroutine(ReadRecordFile());
    }
    private void Update()
    {
        _isWhiteMove = _observablyObject.IsWhiteMove;
        _timeForCameraMove = _observablyObject.TimeForCameraMove;
        _timeForChipsMove = _observablyObject.TimeForChipsMove;
        _isCameraMoving = _observablyObject.IsCameraMoving;
        if (_isCameraMoving == true)
        {
            _cameraRotationTime += Time.deltaTime;
        }
    }
    private IEnumerator ReadRecordFile()
    {
        if (IsReplay == true && IsRecording == false)
        {
            int lineCounter = 0;
            string newLine;
            string[] lines = File.ReadAllLines(_pathToRecordFile);
            foreach (var line in lines)
            {
                newLine = line;
                lineCounter++;
                if (lineCounter == lines.Length || lineCounter == lines.Length - 1)
                {
                    newLine = newLine.Replace(GetPlayer(), GetPlayer(!IsWhiteMove));
                    Debug.Log("Запись закончилась");
                }
                if (newLine.Length <= 0) continue;
                float AdditionDelay;


                AdditionDelay = _timeForCameraMove + _timeForChipsMove - (_cameraRotationTime + _timeForChipsMove);

                _cameraRotationTime = 0;
                string What = "";
                string Action = "";
                Coordinates Coordinates;

                if (newLine.Contains("cell"))
                {
                    What = "cell";
                }
                else if (newLine.Contains("chip"))
                {
                    What = "chip";
                }
                if (newLine.Contains("click"))
                {
                    Action = "click";
                }
                else if (newLine.Contains("ate"))
                {
                    Action = "ate";
                }
                Coordinates = StringToCoordinates(newLine);

                if (What == "cell")
                {
                    if (Action == "click")
                    {
                        yield return new WaitForSeconds(_cellClickDelay + AdditionDelay);
                        _observablyObject?.ImitateCellClickAction(Coordinates);
                    }

                }
                if (What == "chip")
                {
                    if (Action == "click")
                    {
                        yield return new WaitForSeconds(_chipClickDelay + AdditionDelay);
                        _observablyObject?.ImitateChipClickAction(Coordinates);
                    }
                    if (Action == "ate")
                    {
                        yield return new WaitForSeconds(ChipRemoveDelay + AdditionDelay);
                        _observablyObject?.ImitateChipRemoveAction(Coordinates);
                    }
                }
            }
            yield return null;
        }

    }
    private string GetPlayer()
    {
        return IsWhiteMove == true ? "Player1" : "Player2";
    }
    private string GetPlayer(bool isWhiteMove)
    {
        return isWhiteMove == true ? "Player1" : "Player2";
    }
    private Coordinates StringToCoordinates(string str)
    {
        Vertical_Coordinates vertical = Vertical_Coordinates.A;
        Horizontal_Coordinates horizontal = Horizontal_Coordinates.A;

        if (str.Length > 1)
        {
            if (str.Contains("A"))
            {
                horizontal = Horizontal_Coordinates.A;
            }
            if (str.Contains("B"))
            {
                horizontal = Horizontal_Coordinates.B;
            }
            if (str.Contains("C"))
            {
                horizontal = Horizontal_Coordinates.C;
            }
            if (str.Contains("D"))
            {
                horizontal = Horizontal_Coordinates.D;
            }
            if (str.Contains("E"))
            {
                horizontal = Horizontal_Coordinates.E;
            }
            if (str.Contains("F"))
            {
                horizontal = Horizontal_Coordinates.F;
            }
            if (str.Contains("G"))
            {
                horizontal = Horizontal_Coordinates.G;
            }
            if (str.Contains("H"))
            {
                horizontal = Horizontal_Coordinates.H;
            }
            if (str.Contains("1"))
            {
                vertical = Vertical_Coordinates.A;
            }
            if (str.Contains("2"))
            {
                vertical = Vertical_Coordinates.B;
            }
            if (str.Contains("3"))
            {
                vertical = Vertical_Coordinates.C;
            }
            if (str.Contains("4"))
            {
                vertical = Vertical_Coordinates.D;
            }
            if (str.Contains("5"))
            {
                vertical = Vertical_Coordinates.E;
            }
            if (str.Contains("6"))
            {
                vertical = Vertical_Coordinates.F;
            }
            if (str.Contains("7"))
            {
                vertical = Vertical_Coordinates.G;
            }
            if (str.Contains("8"))
            {
                vertical = Vertical_Coordinates.H;
            }
        }

        return new Coordinates(vertical, horizontal);
    }

    public void OnChipRemove(Coordinates coordinates)
    {
        if (IsRecording == true)
        {
            string str = $"ate chip {CompileCoordinatesToNormalCoordinates(coordinates)}";
            WriteNewLineRecordFile(str);
            Debug.Log(str);
        }
    }

    private void OnChipClick(Coordinates clickedChipCoordinates)
    {
        if (IsRecording == true)
        {
            string str = $"{GetPlayer()}: click on chip {CompileCoordinatesToNormalCoordinates(clickedChipCoordinates)}";
            WriteNewLineRecordFile(str);
            Debug.Log(str);
        }
    }



    private void OnCellClick(Coordinates clickedCellCoordinates)
    {
        if (IsRecording == true)
        {
            string str = $"{GetPlayer()}: click on cell {CompileCoordinatesToNormalCoordinates(clickedCellCoordinates)}";
            WriteNewLineRecordFile(str);
            Debug.Log(str);
        }
    }

    private string CompileCoordinatesToNormalCoordinates(Coordinates coordinates)
    {
        return $"{coordinates._horizontal}{coordinates._verticalToInt}";
    }

}

public interface IObservably
{
    public bool IsCameraMoving { get; set; }
    public bool IsWhiteMove { get; set; }
    public float TimeForCameraMove { get; set; }
    public float TimeForChipsMove { get; set; }
    public Action<Coordinates> ObserverOnCellClick { get; set; }
    public Action<Coordinates> ObserverOnChipClick { get; set; }
    public Action<Coordinates> ImitateCellClickAction { get; set; }
    public Action<Coordinates> ImitateChipClickAction { get; set; }
    public Action<Coordinates> ImitateChipRemoveAction { get; set; }
    public Action<Coordinates> ObserverOnChipRemoveAction { get; set; }

}