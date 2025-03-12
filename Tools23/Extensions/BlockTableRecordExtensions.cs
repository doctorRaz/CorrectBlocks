using System;
using System.Collections.Generic;
using System.Linq;

using dRzTools.Servise;




#if NC
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;

using Exception = Teigha.Runtime.Exception;
#elif AC
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Exception = Autodesk.AutoCAD.Runtime.Exception;
#endif

namespace drzTools.Extensions
{
    /// <summary>
    /// кулик делал правки, проверить в бою, к AttSync добавил в класс остальные расширения
    /// </summary>
    public static class BlockTableRecordExtensions
    {

        /// <summary>
        /// Синхронизация вхождений блоков с их определением
        /// </summary>
        /// <param name="btr">Запись таблицы блоков, принятая за определение блока</param>
        /// <param name="directOnly">Следует ли искать только на верхнем уровне, или же нужно 
        /// анализировать и вложенные вхождения, т.е. следует ли рекурсивно обрабатывать блок в блоке:
        /// <br>true - только верхний;</br>
        /// <br> false - рекурсивно проверять вложенные блоки.</br></param>
        /// <param name="removeSuperfluous">
        /// Следует ли во вхождениях блока удалять лишние атрибуты (те, которых нет в определении блока).</param>
        /// <param name="setAttDefValues">
        /// Следует ли всем атрибутам, во вхождениях блока, назначить текущим значением значение по умолчанию.</param>

        public static void AttSync(this BlockTableRecord btr, bool directOnly, bool removeSuperfluous, bool setAttDefValues)
        {
            Database db = btr.Database;
            using (WorkingDatabaseSwitcher wdb = new WorkingDatabaseSwitcher(db))
            {
                using (Transaction t = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)t.GetObject(db.BlockTableId, OpenMode.ForRead);
                    //Получаем все определения атрибутов из определения блока
                    IEnumerable<AttributeDefinition> attdefs = btr.Cast<ObjectId>()
                        .Where(n => n.ObjectClass.Name == "AcDbAttributeDefinition")
                        .Select(n => (AttributeDefinition)t.GetObject(n, OpenMode.ForRead))
                        .Where(n => !n.Constant);//Исключаем константные атрибуты, т.к. для них AttributeReference не создаются.
                    //В цикле перебираем все вхождения искомого определения блока
                    foreach (ObjectId brId in btr.GetBlockReferenceIds(directOnly, false))
                    {
                        BlockReference br = (BlockReference)t.GetObject(brId, OpenMode.ForWrite);
                        //Проверяем имена на соответствие. В том случае, если вхождение блока "A" вложено в определение блока "B", 
                        //то вхождения блока "B" тоже попадут в выборку. Нам нужно их исключить из набора обрабатываемых объектов 
                        //- именно поэтому проверяем имена.
                        if (br.Name != btr.Name)
                            continue;
                        //Получаем все атрибуты вхождения блока
                        IEnumerable<AttributeReference> attrefs = br.AttributeCollection.Cast<ObjectId>()
                            .Select(n => (AttributeReference)t.GetObject(n, OpenMode.ForWrite));
                        //Тэги существующих определений атрибутов
                        IEnumerable<string> dtags = attdefs.Select(n => n.Tag);
                        //Тэги существующих атрибутов во вхождении
                        IEnumerable<string> rtags = attrefs.Select(n => n.Tag);
                        //Если требуется - удаляем те атрибуты, для которых нет определения 
                        //в составе определения блока
                        if (removeSuperfluous)
                            foreach (AttributeReference attref in attrefs.Where(n => rtags
                                .Except(dtags).Contains(n.Tag)))
                                attref.Erase(true);
                        //Свойства существующих атрибутов синхронизируем со свойствами их определений
                        foreach (AttributeReference attref in attrefs.Where(n => dtags
                            .Join(rtags, a => a, b => b, (a, b) => a).Contains(n.Tag)))
                        {
                            AttributeDefinition ad = attdefs.First(n => n.Tag == attref.Tag);
                            //Метод SetAttributeFromBlock, используемый нами далее в коде, сбрасывает
                            //текущее значение многострочного атрибута. Поэтому запоминаем это значение,
                            //чтобы восстановить его сразу после вызова SetAttributeFromBlock.
                            string value = attref.TextString;
                            //drz запоминаем
                            Point3d tpos = attref.Position;
                            Point3d talign = attref.AlignmentPoint;
                            double trotation = attref.Rotation;
                            bool tinvisible = attref.Invisible;
                            double theight = attref.Height;
                            AttachmentPoint tjustify = attref.Justify;
                            //bool tvisible = attref.Visible;
                            //
                            attref.SetAttributeFromBlock(ad, br.BlockTransform);
                            //Восстанавливаем значение атрибута
                            attref.TextString = value;
                            //drz восстанавливаем
                            //TODO: 9999 сделать выключатель
                            attref.Position = tpos;
                            if (attref.AlignmentPoint != talign)//тут эксепшн если LockPositionInBlock и на
                                                                //статических блоках
                                                                //(!attref.LockPositionInBlock)
                            {
                                attref.AlignmentPoint = talign;
                            }
                            attref.Rotation = trotation;
                            attref.Invisible = tinvisible;//сюда же видимость атрибута
                            attref.Height = theight;
                            attref.Justify = tjustify;
                            //attref.Visible = tvisible; //не при делах
                            if (attref.IsMTextAttribute)
                            {
                            }
                            //Если требуется - устанавливаем для атрибута значение по умолчанию
                            if (setAttDefValues)
                                attref.TextString = ad.TextString;
                            attref.AdjustAlignment(db);
                        }
                        //Если во вхождении блока отсутствуют нужные атрибуты - создаём их
                        IEnumerable<AttributeDefinition> attdefsNew = attdefs.Where(n => dtags
                            .Except(rtags).Contains(n.Tag));
                        foreach (AttributeDefinition ad in attdefsNew)
                        {
                            AttributeReference attref = new AttributeReference();
                            attref.SetAttributeFromBlock(ad, br.BlockTransform);
                            attref.AdjustAlignment(db);
                            br.AttributeCollection.AppendAttribute(attref);
                            t.AddNewlyCreatedDBObject(attref, true);
                        }
                    }
                    //#if !NC20
                    btr.UpdateAnonymousBlocks();
                    //#endif
                    t.Commit();
                }
                //Если это динамический блок
                if (btr.IsDynamicBlock)// || btr.IsAnonymous)
                {
                    using (Transaction t = db.TransactionManager.StartTransaction())
                    {
                        foreach (ObjectId id in btr.GetAnonymousBlockIds())
                        {
                            BlockTableRecord _btr = (BlockTableRecord)t.GetObject(id, OpenMode.ForWrite);
                            //Получаем все определения атрибутов из оригинального определения блока
                            IEnumerable<AttributeDefinition> attdefs = btr.Cast<ObjectId>()
                                .Where(n => n.ObjectClass.Name == "AcDbAttributeDefinition")
                                .Select(n => (AttributeDefinition)t.GetObject(n, OpenMode.ForRead));
                            //Получаем все определения атрибутов из определения анонимного блока
                            IEnumerable<AttributeDefinition> attdefs2 = _btr.Cast<ObjectId>()
                                .Where(n => n.ObjectClass.Name == "AcDbAttributeDefinition")
                                .Select(n => (AttributeDefinition)t.GetObject(n, OpenMode.ForWrite));
                            //Определения атрибутов анонимных блоков следует синхронизировать 
                            //с определениями атрибутов основного блока
                            //Тэги существующих определений атрибутов
                            IEnumerable<string> dtags = attdefs.Select(n => n.Tag);
                            IEnumerable<string> dtags2 = attdefs2.Select(n => n.Tag);
                            //1. Удаляем лишние
                            foreach (AttributeDefinition attdef in attdefs2.Where(n => !dtags.Contains(n.Tag)))
                            {
                                attdef.Erase(true);
                            }
                            //2. Синхронизируем существующие
                            foreach (AttributeDefinition attdef in attdefs.Where(n => dtags
                                .Join(dtags2, a => a, b => b, (a, b) => a).Contains(n.Tag)))
                            {
                                AttributeDefinition ad = attdefs2.First(n => n.Tag == attdef.Tag);
                                //-------------drz
                                /*
                                 *attref.Position;
                                 *attref.AlignmentPoint;
                                 *attref.Rotation;
                                 *attref.Invisible;
                                 *attref.Height;
                                 *attref.Justify;
                              */
                                //ad.Position = attdef.Position;
                                //TODO: 9999 поразбираться с положением
                                ad.Rotation = attdef.Rotation;
                                //ad.Invisible = attdef.Invisible;
                                ad.Height = attdef.Height;
                                ad.Justify = attdef.Justify;
                                //---------------------drz

                                ad.TextStyleId = attdef.TextStyleId;
                                //Если требуется - устанавливаем для атрибута значение по умолчанию
                                if (setAttDefValues) ad.TextString = attdef.TextString;
                                ad.Tag = attdef.Tag;
                                ad.Prompt = attdef.Prompt;
                                ad.LayerId = attdef.LayerId;

                                ad.LinetypeId = attdef.LinetypeId;
                                ad.LineWeight = attdef.LineWeight;
                                ad.LinetypeScale = attdef.LinetypeScale;
                                ad.Annotative = attdef.Annotative;
                                ad.Color = attdef.Color;

                                ad.HorizontalMode = attdef.HorizontalMode;

                                ad.IsMirroredInX = attdef.IsMirroredInX;
                                ad.IsMirroredInY = attdef.IsMirroredInY;

                                ad.LockPositionInBlock = attdef.LockPositionInBlock;
                                ad.MaterialId = attdef.MaterialId;
                                ad.Oblique = attdef.Oblique;
                                ad.Thickness = attdef.Thickness;
                                ad.Transparency = attdef.Transparency;
                                ad.VerticalMode = attdef.VerticalMode;
                                ad.Visible = attdef.Visible;
                                ad.WidthFactor = attdef.WidthFactor;
                                ad.CastShadows = attdef.CastShadows;
                                ad.Constant = attdef.Constant;
                                ad.FieldLength = attdef.FieldLength;
                                ad.ForceAnnoAllVisible = attdef.ForceAnnoAllVisible;
                                ad.Preset = attdef.Preset;
                                ad.Prompt = attdef.Prompt;
                                ad.Verifiable = attdef.Verifiable;
                                ad.AdjustAlignment(db);
                            }
                            //3. Добавляем недостающие
                            foreach (AttributeDefinition attdef in attdefs.Where(n => !dtags2.Contains(n.Tag)))
                            {
                                AttributeDefinition ad = new AttributeDefinition();
                                ad.SetDatabaseDefaults();
                                ad.Position = attdef.Position;
                                ad.TextStyleId = attdef.TextStyleId;
                                ad.TextString = attdef.TextString;
                                ad.Tag = attdef.Tag;
                                ad.Prompt = attdef.Prompt;
                                ad.LayerId = attdef.LayerId;
                                ad.Rotation = attdef.Rotation;
                                ad.LinetypeId = attdef.LinetypeId;
                                ad.LineWeight = attdef.LineWeight;
                                ad.LinetypeScale = attdef.LinetypeScale;
                                ad.Annotative = attdef.Annotative;
                                ad.Color = attdef.Color;
                                ad.Height = attdef.Height;
                                ad.HorizontalMode = attdef.HorizontalMode;
                                ad.Invisible = attdef.Invisible;
                                ad.IsMirroredInX = attdef.IsMirroredInX;
                                ad.IsMirroredInY = attdef.IsMirroredInY;
                                ad.Justify = attdef.Justify;
                                ad.LockPositionInBlock = attdef.LockPositionInBlock;
                                ad.MaterialId = attdef.MaterialId;
                                ad.Oblique = attdef.Oblique;
                                ad.Thickness = attdef.Thickness;
                                ad.Transparency = attdef.Transparency;
                                ad.VerticalMode = attdef.VerticalMode;
                                ad.Visible = attdef.Visible;
                                ad.WidthFactor = attdef.WidthFactor;
                                ad.CastShadows = attdef.CastShadows;
                                ad.Constant = attdef.Constant;
                                ad.FieldLength = attdef.FieldLength;
                                ad.ForceAnnoAllVisible = attdef.ForceAnnoAllVisible;
                                ad.Preset = attdef.Preset;
                                ad.Prompt = attdef.Prompt;
                                ad.Verifiable = attdef.Verifiable;
                                _btr.AppendEntity(ad);
                                t.AddNewlyCreatedDBObject(ad, true);
                                ad.AdjustAlignment(db);
                            }
                            //Синхронизируем все вхождения данного анонимного определения блока
                            _btr.AttSync(directOnly, removeSuperfluous, setAttDefValues);
                        }
                        //Обновляем геометрию определений анонимных блоков, полученных на основе 
                        //этого динамического блока
                        //#if !NC20
                        btr.UpdateAnonymousBlocks();
                        //#endif
                        t.Commit();
                    }
                }
            }
        }

        static RXClass attDefClass = RXClass.GetClass(typeof(AttributeDefinition));

        public static void SynchronizeAttributes(this BlockTableRecord target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            Transaction tr = target.Database.TransactionManager.TopTransaction;
            if (tr == null)
                throw new Exception(ErrorStatus.NoActiveTransactions);
            List<AttributeDefinition> attDefs = target.GetAttributes(tr);
            foreach (ObjectId id in target.GetBlockReferenceIds(true, false))
            {
                BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForWrite);
                br.ResetAttributes(attDefs, tr);
            }
            if (target.IsDynamicBlock)
            {
                target.UpdateAnonymousBlocks();
                foreach (ObjectId id in target.GetAnonymousBlockIds())
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
                    attDefs = btr.GetAttributes(tr);
                    foreach (ObjectId brId in btr.GetBlockReferenceIds(true, false))
                    {
                        BlockReference br = (BlockReference)tr.GetObject(brId, OpenMode.ForWrite);
                        br.ResetAttributes(attDefs, tr);
                    }
                }
            }
        }

        private static List<AttributeDefinition> GetAttributes(this BlockTableRecord target, Transaction tr)
        {
            List<AttributeDefinition> attDefs = new List<AttributeDefinition>();
            foreach (ObjectId id in target)
            {
                if (id.ObjectClass == attDefClass)
                {
                    AttributeDefinition attDef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
                    attDefs.Add(attDef);
                }
            }
            return attDefs;
        }

        private static void ResetAttributes(this BlockReference br, List<AttributeDefinition> attDefs, Transaction tr)
        {
            Dictionary<string, string> attValues = new Dictionary<string, string>();
            foreach (ObjectId id in br.AttributeCollection)
            {
                if (!id.IsErased)
                {
                    AttributeReference attRef = (AttributeReference)tr.GetObject(id, OpenMode.ForWrite);
                    attValues.Add(attRef.Tag,
                        attRef.IsMTextAttribute ? attRef.MTextAttribute.Contents : attRef.TextString);
                    attRef.Erase();
                }
            }
            foreach (AttributeDefinition attDef in attDefs)
            {
                AttributeReference attRef = new AttributeReference();
                attRef.SetAttributeFromBlock(attDef, br.BlockTransform);
                if (attDef.Constant)
                {
                    attRef.TextString = attDef.IsMTextAttributeDefinition ?
                        attDef.MTextAttributeDefinition.Contents :
                        attDef.TextString;
                }
                else if (attValues.ContainsKey(attRef.Tag))
                {
                    attRef.TextString = attValues[attRef.Tag];
                }
                br.AttributeCollection.AppendAttribute(attRef);
                tr.AddNewlyCreatedDBObject(attRef, true);
            }
        }
    }
}
