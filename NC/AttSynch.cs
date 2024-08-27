//using static Samples;//.AutoCAD.DatabaseServices.BlockTableRecordExtensionMethods;

//using Microsoft.VisualBasic.ApplicationServices;
//using Bushman.AutoCAD.DatabaseServices;

//using GillesChanteau;


#if NC


using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;
using Teigha.Runtime;

using acad = HostMgd.ApplicationServices.Application;
using Rtm = Teigha.Runtime;
#else

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using acad = Autodesk.AutoCAD.ApplicationServices.Application;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Platform = Autodesk.AutoCAD;
using PlatformDb = Autodesk.AutoCAD;
using Trtm = Autodesk.AutoCAD.Runtime;
#endif

namespace drz.CorrectBlocks
{
    /// <summary>
    /// Синхронизация атрибутов
    /// </summary>
    public class AttSynch
    {
        /// <summary>
        /// Синхронизация атрибутов от Андрея Бушмана
        /// https://adn-cis.org/forum/index.php?topic=10047.msg45331#msg45331
        /// </summary>
        public static void mySynch()
        {
            Document dwg = acad.DocumentManager.MdiActiveDocument;
            Editor ed = dwg.Editor;
            Database db = dwg.Database;
            TypedValue[] types = new TypedValue[] { new TypedValue((int)DxfCode.Start, "INSERT") };
            SelectionFilter sf = new SelectionFilter(types);
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "Select block reference";
            pso.SingleOnly = true;
            pso.AllowDuplicates = false;
            PromptSelectionResult psr = ed.GetSelection(pso, sf);
            //*******************************
            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction t = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)t.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockReference br = (BlockReference)t.GetObject(psr.Value[0].ObjectId, OpenMode.ForRead);
                    BlockTableRecord btr;
                    if (br.IsDynamicBlock)
                    {
                        btr = t.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    }
                    else
                    {
                        //btr = (BlockTableRecord)t.GetObject(bt[br.Name], OpenMode.ForRead);
                        btr = (BlockTableRecord)t.GetObject(br.BlockTableRecord, OpenMode.ForRead);
                    }
                    btr.AttSync(false, true, false);
                    t.Commit();
                }
            }
            else
                ed.WriteMessage("Bad selection\n");
            //********************************
        }
        /// <summary>
        /// Синхронизация атрибутов от Gilles Chanteau (Грубое обновление)
        /// </summary>
        public static void mySynchHard()
        {
            Document dwg = acad.DocumentManager.MdiActiveDocument;
            Editor ed = dwg.Editor;
            Database db = dwg.Database;
            TypedValue[] types = new TypedValue[] { new TypedValue((int)DxfCode.Start, "INSERT") };
            SelectionFilter sf = new SelectionFilter(types);
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "Select block reference";
            pso.SingleOnly = true;
            pso.AllowDuplicates = false;
            PromptSelectionResult psr = ed.GetSelection(pso, sf);
            //*******************************
            if (psr.Status == PromptStatus.OK)
            {
                using (Transaction t = db.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)t.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockReference br = (BlockReference)t.GetObject(psr.Value[0].ObjectId, OpenMode.ForRead);
                    BlockTableRecord btr;
                    if (br.IsDynamicBlock)
                    {
                        btr = t.GetObject(br.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                    }
                    else
                    {
                        btr = (BlockTableRecord)t.GetObject(bt[br.Name], OpenMode.ForRead);
                    }


                    ExtensionMethods.SynchronizeAttributes(btr);
                    t.Commit();
                }
            }
            else
                ed.WriteMessage("Bad selection\n");
            //********************************
        }
    }
}
