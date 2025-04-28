using System;

namespace drz.CorrectBlocks.Core.Enums
{
    /// <summary>
    /// Модификация блоков, расчленение, масштаб, на слой "0", ширина полилиний
    /// </summary>
    [Flags]
    public enum EntityModify
    {
        /// <summary>
        /// Ничего
        /// </summary>
        None = 0,
        /// <summary>
        /// Помещать на слой "0"
        /// </summary>
        LayerZero = 1,
        /// <summary>
        /// Устанавливать глобальную ширину полилиний в 0
        /// </summary>
        LwGlobalWidth = 2,
        /// <summary>
        /// Одинаковые масштабы для описаний блоков
        /// </summary>
        UniformScales = 4,
        /// <summary>
        /// Разрешить расчленение блоков
        /// </summary>
        Explodeable = 8,
    }
}
