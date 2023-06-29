namespace Cell
{
    /// <summary>
    /// Тип соседа клетки
    /// </summary>
    public enum NeighborType : byte
    {
        /// <summary>
        /// Клетка сверху и слева от данной
        /// </summary>
        TopLeft,
        /// <summary>
        /// Клетка сверху и справа от данной
        /// </summary>
        TopRight,
        /// <summary>
        /// Клетка снизу и слева от данной
        /// </summary>
        BottomLeft,
        /// <summary>
        /// Клетка снизу и справа от данной
        /// </summary>
        BottomRight,
        /// <summary>
        /// Клетка справа от данной
        /// </summary>
        Right,
        /// <summary>
        /// Клетка слева от данной
        /// </summary>
        Left,
        /// <summary>
        /// Клетка внизу от данной
        /// </summary>
        Bottom,
        /// <summary>
        /// Клетка сверху от данной
        /// </summary>
        Top
    }
}