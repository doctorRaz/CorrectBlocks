using System;
using System.Collections.Generic;
using System.Text;

using drz.CorrectBlocks.Core.Enums;

namespace drz.CorrectBlocks.Core.Settings
{
    /// <summary>
    /// Хранение настроек
    /// </summary>
    public class EntityPropertiesSettings
    {
        public EntityProperties Linetype { get; set; } = EntityProperties.ByBlock;
        public EntityProperties Color { get; set; } = EntityProperties.ByBlock;
        public EntityProperties Lineweight { get; set; } = EntityProperties.ByBlock;
        public EntityDrawOrder Wipeout { get; set; } = EntityDrawOrder.Back;
        public EntityDrawOrder Hatch { get; set; } = EntityDrawOrder.Back;
        public EntityModify Mofify { get; set; } = EntityModify.LayerZero //todo переименовать в Modify
                                                   | EntityModify.LwGlobalWidth
                                                   | EntityModify.UniformScales
                                                   | EntityModify.Explodeable;
    }
}
