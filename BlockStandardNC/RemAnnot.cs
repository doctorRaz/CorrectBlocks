
using Bushman.AutoCAD.DatabaseServices;

using GillesChanteau;





#if NC
using App = HostMgd.ApplicationServices;
using Ed = HostMgd.EditorInput;
using Rtm = Teigha.Runtime;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;
using Teigha.Runtime;

#elif AC
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.DatabaseServices;
using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Gem = Autodesk.AutoCAD.Geometry;
using Ed = Autodesk.AutoCAD.EditorInput;
using Rtm = Autodesk.AutoCAD.Runtime;

#endif

namespace drz.Tools
{
    public class RemovAnnotate
    {
        // ���� https://adn-cis.org/forum/index.php?topic=10382.msg48321#msg48321

      
        /// <summary>
        /// ���������� �������������� ���� ������
        /// </summary>
        /// <param name="bHowHardAtrSynx">true - ������ ����� ���������<br>false - �������� �������� �� ������</br></param>
        /// <param name="bHowDfx"> 
        /// <br>true - �������� ������������� � ����������</br>
        /// <br>false - ��������� �� �������</br></param>
        public static void Rem_annt(bool bHowHardAtrSynx = true, bool bHowDfx = false)
        {
            Document doc = App.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            int count = 0;
            int countDfx = 0;
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
                        DBObject obj = tr.GetObject(btr.ObjectId, OpenMode.ForWrite, false, true);

                        //!��������� ��������������
                        //if (obj.Annotative == AnnotativeStates.False)
                        if (obj.Annotative == AnnotativeStates.True)
                        {
                            //obj.Annotative = AnnotativeStates.True;
                            obj.Annotative = AnnotativeStates.False;
                            count++;
                            // by razygraevaa on 12.10.2023 at 8:00
                            //? ���� ������ ��������� ��� ������, �� �������� �� �� ���������
                            //if (bHowHardAtrSynx)
                            //{
                            //    //!������ ��������� �������� ���� �����
                            //    ExtensionMethods.SynchronizeAttributes(btr);
                            //}
                            //else
                            //{
                            //    //!�������� �������� �� ������ ������
                            //    btr.AttSync(false, true, false);
                            //}
                        }
                    }
                    else if (bHowDfx)
                    {
                        if (!btr.IsFromExternalReference
                           && !btr.IsDependent
                           && btr.IsLayout)//not xref and
                                           //not xref|block and
                                           //Layout
                        {
                            //https://adn-cis.org/kak-sdelat-obektyi-autocad-annotativnyimi-v-.net.html
                            /*
                                opts.AddAllowedClass(typeof(DBText), false);
                                opts.AddAllowedClass(typeof(MText), false);
                                opts.AddAllowedClass(typeof(Dimension), false);
                                opts.AddAllowedClass(typeof(Leader), false);
                                opts.AddAllowedClass(typeof(Table), false);
                                opts.AddAllowedClass(typeof(Hatch), false);
                            */
                            // ��� ���� �� ������������ ����������
                            foreach (ObjectId brId in btr)
                            {
                                RXClass rxOb = brId.ObjectClass;
                                if (rxOb.DxfName == "TEXT"
                                    || rxOb.DxfName == "MTEXT"
                                    || rxOb.DxfName == "DIMENSION"
                                    || rxOb.DxfName == "MULTILEADER"
                                    || rxOb.DxfName == "ACAD_TABLE"
                                    || rxOb.DxfName == "HATCH"
                                    )
                                {
                                    DBObject obj = tr.GetObject(brId, OpenMode.ForWrite, false, true);
                                    if (obj != null)
                                    {
                                        // ��������� ��������������
                                        //if (obj.Annotative == AnnotativeStates.False)
                                        if (obj.Annotative == AnnotativeStates.True)
                                        {
                                            //obj.Annotative = AnnotativeStates.True;
                                            obj.Annotative = AnnotativeStates.False;
                                            countDfx++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                tr.Commit();
            }
            ed.Regen();
            ed.WriteMessage(
                "\nprocessed in:\nblocks = "
                                + count
                                + "\nentitys = " +
                                countDfx
                );
        }
    }
}
