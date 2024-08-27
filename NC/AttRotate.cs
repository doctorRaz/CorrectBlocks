using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#if NC
using Teigha.DatabaseServices;

using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Cad = HostMgd.ApplicationServices.Application;

#else
using Autodesk.AutoCAD.ApplicationServices; 
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Trtm = Autodesk.AutoCAD.Runtime;
using Platform = Autodesk.AutoCAD;
using PlatformDb = Autodesk.AutoCAD;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
#endif
namespace drz.CorrectBlocks
{
    /// <summary>
    ////крутим атрибуты блоков
    /// </summary>
    public class AtrRotate
    {
        /// <summary>
        /// Крутит в ВЫБРАННЫХ блоках все атрибуты в ноль
        /// </summary>
        public static void AttAllRotateSel()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            if (doc == null) return;
            Editor ed = doc.Editor;

            //btrOID = new Dictionary<ObjectId, string>();//счетчик
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
                        //передаем выбор в обработку
                        ObjectId[] ids = psr.Value.GetObjectIds();
                        AtR(db, ids);
                    }
                }
                else if (pr.StringResult == "Текущее")
                {
                    s.Start();

                    s.Start();
                    LayoutManager layoutMgr = LayoutManager.Current;
                    /*
                     https://adn-cis.org/editor.selectall-s-filtrom-vyibora-primitivov-i-sloyov.html
                    */

                    TypedValue[] filterlist = new TypedValue[2];
                    filterlist[0] = new TypedValue(0, "INSERT");
                    filterlist[1] = new TypedValue(410, layoutMgr.CurrentLayout);
                    SelectionFilter filter = new SelectionFilter(filterlist);
                    PromptSelectionResult psr = ed.SelectAll(filter);//проверить на статус
                    if (psr.Status == PromptStatus.OK)
                    {
                        ObjectId[] ids = psr.Value.GetObjectIds();//получили ID блоков
                                                                  //селект алл блоков текущего пространства
                        AtR(db, ids);
                    }
                }
                else if (pr.StringResult == "Все")//  все
                {
                    s.Start();
                    TypedValue[] filterlist = new TypedValue[1];
                    filterlist[0] = new TypedValue(0, "INSERT");
                    SelectionFilter filter = new SelectionFilter(filterlist);
                    PromptSelectionResult psr = ed.SelectAll(filter);//проверить на статус
                    if (psr.Status == PromptStatus.OK)
                    {
                        ObjectId[] ids = psr.Value.GetObjectIds();//получили ID блоков
                                                                  //селект алл блоков текущего пространства
                        AtR(db, ids);
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
     
                ed.WriteMessage(
                     "\nвыполнено за {0}",
                //    btrOID.Count,
                    s.Elapsed
                    );
            }
        }

        /// <summary>
        /// Функция крутит атрибуты блоков
        /// </summary>
        public static void AtR(Database db, ObjectId[] ids)
        {

            if (ids.Length < 1)
            {
                return;
            }
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (ObjectId brId in ids)
                {
                    BlockReference br = tr.GetObject(brId, OpenMode.ForRead) as BlockReference;

                    foreach (ObjectId attId in br.AttributeCollection)//по атрибутам
                    {
                        if (!attId.IsErased)
                        {
                            AttributeReference att = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                            if (att != null)
                            {
                                att.UpgradeOpen();
                                att.Rotation = 0;
                                att.DowngradeOpen();
                            }
                        }
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Крутит в блоке все атрибуты в ноль
        /// </summary>
        public static void AttAllRotate()
        {
            Document dwg = Cad.DocumentManager.MdiActiveDocument;
            Editor ed = dwg.Editor;
            Database db = dwg.Database;
            TypedValue[] types = new TypedValue[] { new TypedValue((int)DxfCode.Start, "INSERT") };
            SelectionFilter sf = new SelectionFilter(types);
            PromptSelectionOptions pso = new PromptSelectionOptions
            {
                MessageForAdding = "Выберите блок с атрибутами",
                SingleOnly = true,
                AllowDuplicates = false
            };
            PromptSelectionResult psr = ed.GetSelection(pso, sf);
            //*******************************
            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction tr = db.TransactionManager.StartTransaction())
                {
                    BlockReference br = (BlockReference)tr.GetObject(psr.Value[0].ObjectId, OpenMode.ForRead);
                    foreach (ObjectId attId in br.AttributeCollection)//по атрибутам
                    {
                        if (!attId.IsErased)
                        {
                            AttributeReference att = tr.GetObject(attId, OpenMode.ForRead) as AttributeReference;
                            if (att != null)
                            {
                                att.UpgradeOpen();
                                att.Rotation = 0;
                                att.DowngradeOpen();
                            }
                        }
                    }
                    tr.Commit();
                }
                ed.Regen();
            }
            else ed.WriteMessage("Bad selectionа.\n");
        }
        //*******************************
        ///  <summary>
        /// Крутит выбранный атрибут блока
        /// </summary>
        public static void AttSingleRotate()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;
            Editor ed = doc.Editor;
            // by razygraevaa on 24.08.2023 at 12:05  Database db = doc.Database;
            PromptNestedEntityResult rs =
              ed.GetNestedEntity("\nВыбери атрибут: ");
            if (rs.Status == PromptStatus.OK)
            {
                List<ObjectId> ids = new List<ObjectId>(rs.GetContainers());
                ids.Reverse(); // Реверсируем коллекцию примитивов для подсветки
                ids.Add(rs.ObjectId);
#pragma warning disable 0618
                using (AttributeReference att = ids[0].Open(OpenMode.ForRead) as AttributeReference)
                {
                    if (att != null)
                    {
                        att.UpgradeOpen();
                        att.Rotation = 0;
                    }
                }
#pragma warning restore
                ed.Regen();
            }
            else
            {
                ed.WriteMessage("Промахнулся");
            }
        }
    }
}