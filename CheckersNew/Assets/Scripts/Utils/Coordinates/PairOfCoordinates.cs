using System;
using UnityEngine;

namespace Utils.Coordinates
{
    [Serializable]
    public struct PairOfCoordinates
    {
        [SerializeField]
        private VerticalCoordinates _vertical;
        [SerializeField]
        private HorizontalCoordinates _horizontal;
        public HorizontalCoordinates Horizontal => _horizontal;
        public int VerticalToInt { get => (int)_vertical; set => _vertical = (VerticalCoordinates)value; }
        public int HorizontalToInt { get => (int)_horizontal; set => _horizontal = (HorizontalCoordinates)value; }
        
        public bool Equals(PairOfCoordinates other)
        {
            return _vertical == other._vertical && _horizontal == other._horizontal;
        }

        public override bool Equals(object obj)
        {
            return obj is PairOfCoordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)_vertical, (int)_horizontal);
        }

        public static bool operator !=(PairOfCoordinates a, PairOfCoordinates b)
        {
            return !a.Equals(b);
        }
        public static bool operator ==(PairOfCoordinates a, PairOfCoordinates b)
        {
            return a.Equals(b);
        }

        public PairOfCoordinates(int vertical, int horizontal)
        {
            _vertical = VerticalCoordinates.A;
            _horizontal = HorizontalCoordinates.A;
            VerticalToInt = vertical;
            HorizontalToInt = horizontal;
        }

        public PairOfCoordinates(VerticalCoordinates vertical, HorizontalCoordinates horizontal)
        {
            _vertical = vertical;
            _horizontal = horizontal;
        }
    }
}