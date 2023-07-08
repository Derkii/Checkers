
using System;

namespace Utils.Coordinates
{
    public static class CoordinatesHelper
    {
        public static PairOfCoordinates StringToCoordinates(this string str)
        {
            var vertical = VerticalCoordinates.A;
            var horizontal = HorizontalCoordinates.A;

            if (str.Length > 1)
            {
                horizontal = Enum.Parse<HorizontalCoordinates>(str.ToCharArray()[0].ToString());
                vertical = (VerticalCoordinates)int.Parse(str.ToCharArray()[1].ToString());
            }

            return new PairOfCoordinates(vertical, horizontal);
        }
        public static string CoordinatesToString(this PairOfCoordinates pairOfCoordinates)
        {
            return $"{pairOfCoordinates.Horizontal}{pairOfCoordinates.VerticalToInt}";
        }
    }
}
