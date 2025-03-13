using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Teigha.DatabaseServices;

namespace dRzTools.SaveMod
{
    public class SaveTest
    {
        /// <summary>
        /// Saves the test.
        /// </summary>
        public static void Save_Test()
        {
            //Database db = HostApplicationServices.WorkingDatabase;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;
            ed.WriteMessage("CCC Ok");
            //---Y

            PromptSaveFileOptions pfso = new PromptSaveFileOptions("Сохранить файл: ");
            pfso.Filter =
                          "Drawing (*.dwg)|*.dwg|"
                        + "Design Web Format (*.dwf)|*.dwf|"
                        + "All files (*.*)|*.*";
            pfso.DialogCaption = "DialogCaption";
            pfso.DialogName = "DialogName";
            pfso.DeriveInitialFilenameFromDrawingName = true;
            pfso.InitialFileName = doc.Name;
            pfso.InitialDirectory = @"c:\temp\00";

            PromptFileNameResult pfnr = ed.GetFileNameForSave(pfso);

            if (pfnr.Status != PromptStatus.OK) return;

            string fileSaveName = pfnr.StringResult;

            db.SaveAs(fileSaveName, DwgVersion.Current);
        }


    }
}
