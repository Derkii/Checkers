using System;

namespace Checkers
{
    public enum Vertical_Coordinates
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8
    }
    public enum Horizontal_Coordinates
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8
    }   

    [Serializable]
    public struct Coordinates
    {
        public static bool operator !=(Coordinates a, Coordinates b)
        {
            return !a.Equals(b);
        }
        public static bool operator ==(Coordinates a, Coordinates b)
        {
            return a.Equals(b);
        }
        public Vertical_Coordinates _vertical;
        public Horizontal_Coordinates _horizontal;
        public int _verticalToInt { get => (int)_vertical; set => _vertical = (Vertical_Coordinates)value; }
        public int _horizontalToInt { get => (int)_horizontal; set => _horizontal = (Horizontal_Coordinates)value; }

        public Coordinates(int vertical, int horizontal)
        {
            _vertical = Vertical_Coordinates.A;
            _horizontal = Horizontal_Coordinates.A;
            _verticalToInt = vertical;
            _horizontalToInt = horizontal;
        }

        public Coordinates(Vertical_Coordinates vertical, Horizontal_Coordinates horizontal)
        {
            _vertical = vertical;
            _horizontal = horizontal;
        }

    }
}

