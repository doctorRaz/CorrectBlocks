/*=====================
      * нормализуем блоки: BlcNorm
      *      цвет 
      *          по слою
      *          по блоку
      *          не менять
      *      слой
      *          zero
      *              да
      *              нет
      *      установить одинаковые масштабы
      *          да
      *          нет
      *          не менять
      *      Разрешить расчленение
      *          да
      *          нет
      *          не менять
      *======================
      * обновляем атрибуты блоков: BlockNormalizedAttRef
      *      цвет
      *          по слою
      *          по блоку
      *          не менять
      *      слой
      *          zero
      *          не менять
      *=========================
      * всем примитивам меняем 
       *      тип линии:
       *          по слою
       *          по блоку
       *          не менять  
     */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

#if NC
using Teigha.DatabaseServices;
using Teigha.Runtime;

using HostMgd.ApplicationServices;
using HostMgd.EditorInput;


#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Trtm = Autodesk.AutoCAD.Runtime;
using Platform = Autodesk.AutoCAD;
using PlatformDb = Autodesk.AutoCAD;
#endif
namespace drz.CorrectBlocks
{
    /// <summary>
    ///<br> для чего?</br>
    ///<br>нормализацию блоков только в текущем пространстве</br>
    ///<br> только выбранные</br>
    ///<br> все</br>
    ///<br>нормализация примитивов чертежа(по слою или новый слой с цветом)</br>
    ///<br>переименование и слитие слоев в работе</br>
    ///<br></br>
    ///<br>интерфейс прикрутить???</br>
    /// </summary>
    public class NLB
    {

        /// <summary>
        /// словарик для ID блоков
        /// </summary>
        public static class Dict
        {
            public static Dictionary<ObjectId, string> btrOID1 = new Dictionary<ObjectId, string>();
        }

        /// <summary>
        /// Перечисления параметров нормализации блока
        /// </summary>
        [Flags()]
        public enum FBlkSet : int
        {
            /// <summary>умолчание </summary>
            fNone = 0,
            //тип Entity если оба нули, значит не менять
            /// <summary>тип Entity по слою </summary>
            fTypeBL = 1 << 1,
            /// <summary> тип Entity по блоку</summary>
            fTypeBB = 1 << 2,
            //цвет если оба нули, значит не менять
            /// <summary> цвет Entity по слою</summary>
            fColorBL = 1 << 3,
            /// <summary> цвет Entity по блоку</summary>
            fColorBB = 1 << 4,
            // вес Entity если оба нули, значит не менять
            /// <summary> вес Entity по слою</summary>
            fWeightBL = 1 << 5,
            /// <summary> вес Entity по блоку</summary>
            fWeightBB = 1 << 6,
            /// <summary>trye-Entity на слой zero<br>false-Entity слой не менять</br></summary>
            fLayerEnZero = 1 << 7,
            /// <summary>trye-топить маскировку<br>false-не топить маскировку</br>  </summary>
            fWipeBott = 1 << 8,
            //одинаковые масштабы Block если оба нули, значит не менять
            /// <summary>одинаковые масштабы Block On</summary>
            fScaleEqOn = 1 << 10,
            /// <summary>одинаковые масштабы Block Off</summary>
            fScaleEqOff = 1 << 11,
            //Разрешить расчленение Block если оба нули, значит не менять
            /// <summary>Explodable Block On</summary>
            fExplodeOn = 1 << 12,
            /// <summary>Explodable Block Off</summary>
            fExplodeOff = 1 << 13
        }




        /// <summary>
        /// Словарик дубликатов ID блоков
        /// </summary>
        public static Dictionary<ObjectId, string> btrOID;
        public static RXClass blClass = RXObject.GetClass(typeof(BlockReference));

        /// <summary>
        /// Диалог настройки нормализации блоков
        /// <br> Все по слою (VL-CMDF "БЛКДЛГ" "Д" "Д" "С" "С" "С" "Д" "Д")</br>
        /// <br> Все по слою (VL-CMDF "drz-blc-SetDlg" "Д" "Д" "С" "С" "С" "Д" "Д")</br>
        /// </summary>
        public static void GetBlc_SetDlg()
        {
            /*настройки нормализации блоков по умолчанию/настроить,
            *   если настроить либо простынка вопросов либо форма
            * обновлять ли атрибуты да/нет (VL-CMDF "БЛКДЛГ" "Д" "Д" "с" "С" "С" "Д" "Д")
            *   если да, менять ли положение или как АТТСИНХ
            */

            Document doc = Application.DocumentManager.MdiActiveDocument;
          // by razygraevaa on 24.08.2023 at 12:06  Database db = doc.Database;
            if (doc == null) return;
            Editor ed = doc.Editor;

            FBlkSet fBlck = FBlkSet.fNone;

            PromptKeywordOptions pko = new PromptKeywordOptions("\nРазрешить расчленение блоков? ")
            {
                AllowNone = false
            };
            pko.Keywords.Add("Да");
            pko.Keywords.Add("Нет");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            PromptResult pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем
                }
                else if (pr.StringResult == "Да")
                {
                    fBlck |= FBlkSet.fExplodeOn;
                }
                else if (pr.StringResult == "Нет")
                {
                    fBlck |= FBlkSet.fExplodeOff;
                }
            }
            else
            {
                return;
            }
            pko = new PromptKeywordOptions("\nУстановить равный масштаб блоков? ")
            {
                AllowNone = false
            };
            //pko.Keywords.Clear();
            pko.Keywords.Add("Да");
            pko.Keywords.Add("Нет");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем
                }
                else if (pr.StringResult == "Да")
                {
                    fBlck |= FBlkSet.fScaleEqOn;
                }
                else if (pr.StringResult == "Нет")
                {
                    fBlck |= FBlkSet.fScaleEqOff;
                }
            }
            else
            {
                return;
            }

            pko = new PromptKeywordOptions("\nУстановить тип примитивов? ")
            {
                AllowNone = false
            };
            //pko.Keywords.Clear();
            pko.Keywords.Add("поСлою");
            pko.Keywords.Add("поБлоку");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем fTypeBB
                }
                else if (pr.StringResult == "поСлою")
                {
                    fBlck |= FBlkSet.fTypeBL;
                }
                else if (pr.StringResult == "поБлоку")
                {
                    fBlck |= FBlkSet.fTypeBB;
                }
            }
            else
            {
                return;
            }

            pko = new PromptKeywordOptions("\nУстановить цвет примитивов? ")
            {
                AllowNone = false
            };
            //pko.Keywords.Clear();
            pko.Keywords.Add("поСлою");
            pko.Keywords.Add("поБлоку");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем fTypeBB
                }
                else if (pr.StringResult == "поСлою")
                {
                    fBlck |= FBlkSet.fColorBL;
                }
                else if (pr.StringResult == "поБлоку")
                {
                    fBlck |= FBlkSet.fColorBB;
                }
            }
            else
            {
                return;
            }

            pko = new PromptKeywordOptions("\nУстановить толщину примитивов? ")
            {
                AllowNone = false
            };
            //pko.Keywords.Clear();
            pko.Keywords.Add("поСлою");
            pko.Keywords.Add("поБлоку");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем 
                }
                else if (pr.StringResult == "поСлою")
                {
                    fBlck |= FBlkSet.fWeightBL;
                }
                else if (pr.StringResult == "поБлоку")
                {
                    fBlck |= FBlkSet.fWeightBB;
                }
            }
            else
            {
                return;
            }

            pko = new PromptKeywordOptions("\nПереместить примитивы на слой 0? ")
            {
                AllowNone = false
            };
            //pko.Keywords.Clear();
            pko.Keywords.Add("Да");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем 
                }
                else if (pr.StringResult == "Да")
                {
                    fBlck |= FBlkSet.fLayerEnZero;
                }
            }
            else
            {
                return;
            }

            pko = new PromptKeywordOptions("\nПереместить маскировки назад? ")
            {
                AllowNone = false
            };
            //pko.Keywords.Clear();
            pko.Keywords.Add("Да");
            pko.Keywords.Add("неМенять");
            pko.Keywords.Default = "неМенять";
            pr = ed.GetKeywords(pko);
            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "неМенять")
                {
                    //пропускаем 
                }
                else if (pr.StringResult == "Да")
                {
                    fBlck |= FBlkSet.fWipeBott;
                }
            }
            else
            {
                return;
            }
            if (fBlck != FBlkSet.fNone)
            {
                GetBlc(fBlck);
            }
            else
            {
                ed.WriteMessage("\nНет опций для изменения");
            }
        }
        /// <summary>
        /// "Тонкая" нормализация блоков
        /// </summary>
        public static void GetBlc(FBlkSet fBlck)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            if (doc == null) return;
            Editor ed = doc.Editor;

            btrOID = new Dictionary<ObjectId, string>();//счетчик
            Stopwatch s = new Stopwatch();//часики

            PromptKeywordOptions pko = new PromptKeywordOptions("\nКакие блоки обрабатывать? ")
            {
                AllowNone = false
            };
            pko.Keywords.Add("Все");
            pko.Keywords.Add("Текущее");
            pko.Keywords.Add("выБрать");
            pko.Keywords.Default = "выБрать";
            PromptResult pr = ed.GetKeywords(pko);//запрос откуда брать блоки

            if (pr.Status == PromptStatus.OK)
            {
                if (pr.StringResult == "выБрать")
                {
                    SelectionFilter filter = new SelectionFilter(
                        new TypedValue[]
                        {
                            new TypedValue((int)DxfCode.Start, "INSERT")
                        }
                        );
                    PromptSelectionOptions pso = new PromptSelectionOptions
                    {
                        MessageForAdding = "Выберите блоки:"
                    };
                    PromptSelectionResult psr = ed.GetSelection(pso, filter);

                    if (psr.Status == PromptStatus.OK)
                    {
                        s.Start();
                        //открываем транзакцию, нафиг не нужна
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            ed.WriteMessage("\nВыбрали {0} объектов", psr.Value.Count);

                            SelectionSet selSet = psr.Value;
                            foreach (ObjectId brId in selSet.GetObjectIds())
                            {
                                CheckBlock(brId, tr, fBlck);//по одному пуляем ID, там рзберутся
                            }
                            tr.Commit();
                        }
                    }
                }
                else if (pr.StringResult == "Текущее")
                {
                    s.Start();

                    ObjectId SpaceId = db.CurrentSpaceId;//ID активного пространства
                    BlockTableRecord btrs;

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        btrs = tr.GetObject(
                            SpaceId, OpenMode.ForRead
                            ) as BlockTableRecord;//получаем запись BlockTableRecord активного пространства


                        foreach (ObjectId brId in btrs)//в активном пространстве перебираем все что в нем находится
                        {
                            if (brId.ObjectClass == blClass)
                            {
                                CheckBlock(brId, tr, fBlck);//по одному пуляем ID, там рзберутся
                            }
                        }
                        tr.Commit();
                    }
                }
                else if (pr.StringResult == "Все")//  все
                {
                    s.Start();
                    //https://forum.nanocad.ru/index.php?/topic/11453-bystraya-paketnaya-rabota-s-dwg/
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

                        IEnumerable<BlockTableRecord> blockTableRecords = bt.Cast<ObjectId>()
                          .Select(id => tr.GetObject(id, OpenMode.ForRead) as BlockTableRecord)
                          .Where(btr => btr != null)
                          .Where(btr => btr.IsDependent == false)// TODO переделать чтоб менять масштаб внешним ссылкам
                          .Where(btr => btr.IsFromExternalReference == false)
                          .Where(btr => btr.IsLayout == false);

                        ed.WriteMessage(
                "\nВыбрано {0} описаний блоков",
                blockTableRecords.Count()
                );
                        foreach (BlockTableRecord btr in blockTableRecords)
                        {
                            if (!btrOID.ContainsKey(btr.ObjectId))
                            {
                                RebuildBlk(btr, tr, fBlck);
                            }
                        }
                        tr.Commit();
                    }
                }
                else
                {
                    ed.WriteMessage("\nОтмена");
                    return;
                }
                ed.Regen();
                s.Stop();
                var resultTime = s.Elapsed;
                //Console.WriteLine("Elapsed Time: {0} ms", s.ElapsedMilliseconds);
                ed.WriteMessage(
                     "\nИзменено {0} опеределений блоков за {1}",
                    btrOID.Count,
                    s.Elapsed
                    );
            }
        }
        /// <summary>
        /// Разбор блоков обновляем дин перебором анонимных
        /// </summary>
        /// <param name="brId">ObjectId выбранного блока </param>
        /// <param name="fBlck">Прокидываем дальше ключи параметров нормализации блоков</param>
        public static void CheckBlock(ObjectId brId, Transaction tr, FBlkSet fBlck)
        {

            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //Database db = doc.Database;
            //if (doc == null) return;
            //Editor ed = doc.Editor;

            //смотрим, что прилетело
            BlockReference br = tr.GetObject(brId, OpenMode.ForRead) as BlockReference;
            BlockTableRecord btr;
            if (br.IsDynamicBlock)
            {
                btr = tr.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                if (!btrOID.ContainsKey(btr.ObjectId)
                    && !btr.IsFromExternalReference
                    && !btr.IsDependent
                    )
                {
                    if (RebuildBlk(btr, tr, fBlck))
                    {
                        foreach (ObjectId id in btr.GetAnonymousBlockIds())
                        {
                            BlockTableRecord btra = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                            if (!btrOID.ContainsKey(btra.ObjectId))
                                RebuildBlk(btra, tr, fBlck);
                        }
                    }
                }
            }
            else
            {
                btr = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);
                if (!btrOID.ContainsKey(btr.ObjectId)
                    && !btr.IsFromExternalReference
                    && !btr.IsDependent)//TODO для внешних ссылок
                {
                    RebuildBlk(btr, tr, fBlck);
                }
            }
        }
        //-----------------
        /// <summary>
        /// обрабатываем потроха блоков с рекурсией обновляем дин по Enum
        /// </summary>
        /// <param name="btr"></param>
        /// <param name="tr"></param>
        /// <param name="fBlck"></param>
        /// 
        public static bool RebuildBlk(BlockTableRecord btr, Transaction tr, FBlkSet fBlck)
        {
            if (btrOID.ContainsKey(btr.ObjectId))//тут словарик статик, проверка на дубликаты
                return false;//уходим
            btrOID.Add(btr.ObjectId, btr.Name);//в конец обработки или нет?

            //тип линии
            string stypeEn = "ByLayer";
            bool btypeEn = true;
            if (fBlck.HasFlag(FBlkSet.fTypeBB))//если по блоку
                stypeEn = "ByBlock";
            else if (fBlck.HasFlag(FBlkSet.fTypeBL))//по слою
                stypeEn = "ByLayer";
            else//никак
                btypeEn = false;

            //вес
            LineWeight weightEn = LineWeight.ByLayer;
            bool bweightEn = true;
            if (fBlck.HasFlag(FBlkSet.fWeightBB))//если по блоку
                weightEn = LineWeight.ByBlock;
            else if (fBlck.HasFlag(FBlkSet.fWeightBL))
                weightEn = LineWeight.ByLayer;
            else
                bweightEn = false;

            //цвет
            short icolorEn = 256;//по умолчанию по слою
            bool bcolorEn = true;
            if (fBlck.HasFlag(FBlkSet.fColorBB))//если по блоку
                icolorEn = 0;
            else if (fBlck.HasFlag(FBlkSet.fColorBL))
                icolorEn = 256;
            else
                bcolorEn = false;

            //слой
            string layerEn = "0";

            //тут меняем общие свойства блока 
            //масштаб, 
            if (fBlck.HasFlag(FBlkSet.fScaleEqOn) && (btr.BlockScaling != BlockScaling.Uniform))//одинаковые вкл
            {
                btr.UpgradeOpen();
                btr.BlockScaling = BlockScaling.Uniform;//общий масштаб

            }
            else if (fBlck.HasFlag(FBlkSet.fScaleEqOff) && (btr.BlockScaling != BlockScaling.Any))//одинаковые выкл
            {
                btr.UpgradeOpen();
                btr.BlockScaling = BlockScaling.Any;//свой масштаб
            }

            //расчленение fExplodeOn fExplodeOff
            if (fBlck.HasFlag(FBlkSet.fExplodeOn) && !btr.Explodable)//вкл
            {
                btr.UpgradeOpen();
                btr.Explodable = true; //разрешаем разбивать блоки
            }
            else if (fBlck.HasFlag(FBlkSet.fExplodeOff) && btr.Explodable)//выкл
            {
                btr.UpgradeOpen();
                btr.Explodable = false;
            }

            foreach (ObjectId id in btr)
            {
                //проверяем тип
                if (id.ObjectClass == blClass)
                {
                    CheckBlock(id, tr, fBlck);
                }

                //красим примитивы
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;//может быстрее будет открывать сразу на запись?
                try
                {
                    if (ent != null)
                    {
                        //тип линии
                        if (btypeEn
                            && ent.Linetype != stypeEn
                            )
                        {
                            ent.UpgradeOpen();
                            ent.Linetype = stypeEn;
                        }

                        //цвет
                        if (bcolorEn
                            && ent.ColorIndex != icolorEn
                            )
                        {
                            ent.UpgradeOpen();
                            ent.ColorIndex = icolorEn;
                        }

                        // вес линии
                        if (bweightEn
                            && ent.LineWeight != weightEn
                            )
                        {
                            ent.UpgradeOpen();
                            ent.LineWeight = weightEn;
                        }

                        //слой зеро "*ADSK_CONSTRAINTS"
                        //Contains
                        //https://docs.microsoft.com/ru-ru/dotnet/csharp/how-to/search-strings#code-try-2
                        if (fBlck.HasFlag(FBlkSet.fLayerEnZero)
                            && ent.Layer != layerEn
                            && !ent.Layer.Contains("*")
                            )
                        {
                            ent.UpgradeOpen();
                            ent.Layer = layerEn;
                        }

                        //топим маскировки
                        if (fBlck.HasFlag(FBlkSet.fWipeBott))
                        {
                            Wipeout wpt = ent as Wipeout;
                            if (wpt != null)//если маскировка
                            {
                                // получаем таблицу порядка отрисовки блока
                                DrawOrderTable drawOrder =
                                                     tr.GetObject(btr.DrawOrderTableId,
                                                    OpenMode.ForRead)
                                                     as DrawOrderTable;

                                ObjectIdCollection idsw = new ObjectIdCollection
                                {
                                    wpt.ObjectId
                                };
                                drawOrder.UpgradeOpen();
                                drawOrder.MoveToBottom(idsw); //топим маскировку
                            }
                        }
                    }

                }

                catch (System.Exception ex)
                {
                    Document doc = Application.DocumentManager.MdiActiveDocument;
                    Editor ed = doc.Editor;
                    ed.WriteMessage(ex.ToString());
                    continue;
                }
            }
            return true;
        }

    }
}