namespace drz.CorrectBlocks.Core.Enums
{
    /// <summary>
    /// Порядок следования
    /// </summary>
    public enum EntityDrawOrder
    {
        /// <summary>
        /// Не менять порядок следования
        /// </summary>
        DoNotChange,
        /// <summary>
        /// Передвинуть вперед всех остальных примитивов
        /// </summary>
        Forward,
        /// <summary>
        /// Передвинуть позади всех остальных примитивов
        /// </summary>
        Back,
    }
}
