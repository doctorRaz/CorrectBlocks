using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drzTools.Enum
{
    public enum BlockNormalizeEnum
    {
        /// <summary>умолчание </summary>
        Default = 0,
        /// <summary>тип Entity по слою </summary>
        LineTypeByLayer = 1 << 1,
        //тип линий Entity если оба нули, значит не менять
        /// <summary> тип Entity по блоку</summary>
        LineTypeByBlock = 1 << 2,
        //цвет если оба нули, значит не менять
        /// <summary> цвет Entity по слою</summary>
        ColorByLayer = 1 << 3,
        /// <summary> цвет Entity по блоку</summary>
        ColorByBlock = 1 << 4,
        // вес Entity если оба нули, значит не менять
        /// <summary> вес Entity по слою</summary>
        LineWeightByLayer = 1 << 5,
        /// <summary> вес Entity по блоку</summary>
        LineWeightByBlock = 1 << 6,
        /// <summary>trye-Entity на слой zero<br>false-Entity слой не менять</br></summary>
        SetLayer0 = 1 << 7,
        /// <summary>trye-топить маскировку<br>false-не топить маскировку</br>  </summary>
        SetWipeoutBack = 1 << 8,
        /// <summary>trye-топить штриховку<br>false-не топить маскировку</br>  </summary>
        SetHatchtBack = 1 << 9,
        //одинаковые масштабы Block если оба нули, значит не менять
        /// <summary>одинаковые масштабы Block On</summary>
        EqualScaleOn = 1 << 10,
        /// <summary>одинаковые масштабы Block Off</summary>
        EqualScaleOff = 1 << 11,
        //Разрешить расчленение Block если оба нули, значит не менять
        /// <summary>Explodable Block On</summary>
        SetBlockExplodeable = 1 << 12,
        /// <summary>Explodable Block Off</summary>
        SetBlockUnexplodeable = 1 << 13
    }
}
