#if NC
using HostMgd.EditorInput;
using HostMgd.ApplicationServices;
using Teigha.DatabaseServices;
using Teigha.Runtime;
#elif AC
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
#endif
using System.IO;

namespace drzTools.CadCommand
{
    public static class QSaveAsCmd
    {
        [CommandMethod("drz_QSAVEAS")]
        public static void QSaveAsCommand()
        {
            QS.QuickSaveAs();
        }

        //https://www.keanw.com/2009/10/implementing-a-quick-saveas-command-in-autocad-using-net.html
        public class QS
        {
            // Set up static variable for the path to our folder
            // of drawings, as well as the base filename and a
            // counter to make the unique filename

            static string _path = "",
                _base = "";
            static int _count = 0;
            // Various filename and path-related constants
            const string sfxSep = " ",
                extSep = ".",
                pthSep = "\\",
                lspSep = "/",
                dwgExt = ".dwg";
            // Our QuickSaveAs command

            //[CommandMethod("QSAVEAS")]
            public static void QuickSaveAs()
            {
                Document doc = Application.DocumentManager.MdiActiveDocument;
                Editor ed = doc.Editor;
                Database db = doc.Database;
                // If this is the first time run...
                if (_path == "" || _base == "")
                {
                    // Ask the user for a base file location
                    PromptSaveFileOptions opts = new PromptSaveFileOptions("Select location to save first drawing file");
                    opts.Filter = "Drawing (*.dwg)|*.dwg";
                    PromptFileNameResult pr = ed.GetFileNameForSave(opts);
                    // Delete the file, if it exists
                    // (may be a problem if the file is in use)
                    if (File.Exists(pr.StringResult))
                    {
                        try
                        {
                            File.Delete(pr.StringResult);
                        }
                        catch { }
                    }

                    if (pr.Status == PromptStatus.OK)
                    {
                        // If a file was selected, and it contains a path...
                        if (pr.StringResult.Contains(pthSep))
                        {
                            // Separate the path from the file name
                            int idx = pr.StringResult.LastIndexOf(pthSep);
                            _path = pr.StringResult.Substring(0, idx);
                            string fullname = pr.StringResult.Substring(idx + 1);

                            // If the path has an extension (this should always
                            // be the case), extract the base file name

                            if (fullname.Contains(extSep))
                            {
                                _base = fullname.Substring(0, fullname.LastIndexOf(extSep));
                            }
                        }
                    }
                }
                // Assuming the path and name were set appropriately...

                if (_path != "" && _base != "")
                {
                    string name = _base;

                    // Add our suffix if not the first time run
                    if (_count > 0)
                        name += sfxSep + _count.ToString();

                    // Our drawing is located in the base path
                    string dwgPath = _path + pthSep + name + dwgExt;

                    // Now we want to save our drawing and use the image
                    // for our tool icon
                    // Using either COM or .NET doesn't generate a
                    // thumbnail in the resultant file (or its Database)
                    // .NET:
                    // db.SaveAs(dwgPath, false, DwgVersion.Current, null);
                    // COM:
                    // AcadDocument adoc = (AcadDocument)doc.AcadDocument;
                    // adoc.SaveAs(dwgPath, AcSaveAsType.acNative, null);
                    // So we'll send commands to the command-line
                    // We'll use LISP, to avoid having to set FILEDIA to 0
                    object ocmd = Application.GetSystemVariable("CMDECHO");
                    string dwgPath2 = dwgPath.Replace(pthSep, lspSep);

                    doc.SendStringToExecute(
                        "(setvar \"CMDECHO\" 0)" + "(command \"_.SAVEAS\" \"\" \"" + dwgPath2 + "\")" +
                        "(setvar \"CMDECHO\" " + ocmd.ToString() + ")" + "(princ) ", false, false, false);

                    // Print a confirmation message for the DWG save
                    // (which actually gets displayed before the queued
                    // string gets executed, but anyway)

                    ed.WriteMessage("\nSaved to: \"" + dwgPath + "\"");
                    _count++;
                }
            }
        }
    }
}
