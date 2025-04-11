
#if NC
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;

#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
#endif
namespace drzTools.Block
{
    /// <summary>
    /// Топить
    /// </summary>
    public class Down

    {
        /// <summary>
        /// Маскировку блоков на задний план
        /// </summary>
        public static void Wipeout()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            int count = 0;
            //ObjectId SpaceId = db.CurrentSpaceId;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(
                                            db.BlockTableId,
                                            OpenMode.ForRead
                                            ) as BlockTable;
                foreach (ObjectId btrId in bt)
                {
                    BlockTableRecord btr = tr.GetObject(
                                                        btrId,
                                                        OpenMode.ForRead
                                                        ) as BlockTableRecord;
                    if (!btr.IsFromExternalReference
                        && !btr.IsDependent
                        && !btr.IsLayout)//not xref and
                                         //not xref|block and
                                         //not Layout
                    {
                        foreach (ObjectId id in btr)
                        {
                            Entity ent = tr.GetObject(id,
                                                      OpenMode.ForRead
                                                      ) as Entity;
                            if (ent != null)
                            {
                                Wipeout wpt = ent as Wipeout;
                                if (wpt != null)//если маскировка
                                {
                                    // получаем таблицу порядка отрисовки блока
                                    DrawOrderTable drawOrder = tr.GetObject(
                                                                            btr.DrawOrderTableId,
                                                                            OpenMode.ForWrite
                                                                            ) as DrawOrderTable;
                                    ObjectIdCollection ids = new ObjectIdCollection
                                    {
                                        wpt.ObjectId
                                    };
                                    drawOrder.MoveToBottom(ids);//топим маскировку
                                    count++;
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
            ed.Regen();
            ed.WriteMessage(
                "Move to bottom in blocks "
                                + count
                                + " wipeout"
                );
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Hatch()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            int count = 0;
            //ObjectId SpaceId = db.CurrentSpaceId;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(
                                            db.BlockTableId,
                                            OpenMode.ForRead
                                            ) as BlockTable;
                foreach (ObjectId btrId in bt)
                {
                    BlockTableRecord btr = tr.GetObject(
                                                        btrId,
                                                        OpenMode.ForRead
                                                        ) as BlockTableRecord;
                    if (!btr.IsFromExternalReference
                        && !btr.IsDependent
                        && !btr.IsLayout)//not xref and
                                         //not xref|block and
                                         //not Layout
                    {
                        foreach (ObjectId id in btr)
                        {
                            Entity ent = tr.GetObject(id,
                                                      OpenMode.ForRead
                                                      ) as Entity;
                            if (ent != null)
                            {
                                Hatch hatch = ent as Hatch;
                                if (hatch != null)//если маскировка
                                {
                                    // получаем таблицу порядка отрисовки блока
                                    DrawOrderTable drawOrder = tr.GetObject(
                                                                            btr.DrawOrderTableId,
                                                                            OpenMode.ForWrite
                                                                            ) as DrawOrderTable;
                                    ObjectIdCollection ids = new ObjectIdCollection
                                          {
                                              hatch.ObjectId
                                          };
                                    drawOrder.MoveToBottom(ids);//топим маскировку
                                    count++;
                                }
                            }
                        }
                    }
                }
                tr.Commit();
            }
            ed.Regen();
            ed.WriteMessage(
                "Move to bottom in blocks "
                                + count
                                + " wipeout"
                );
        }
    }
}
