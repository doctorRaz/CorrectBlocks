using HostMgd.ApplicationServices;
using HostMgd.EditorInput;

using Microsoft.Win32;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Teigha.DatabaseServices;
using Teigha.Runtime;

namespace NCadSettings
{
    public class RegistrySettings
    {
        /// <summary> Настройки nanoCAD, хранящиеся в реестре </summary>
        /// <param name="RegistryHiveName">Полный путь к разделу реестра текущего профиля / конфигурации NanoCAD (без HKCU)</param>
        public RegistrySettings(string RegistryHiveName)
        {
            _registryHiveName = RegistryHiveName;
        }

        /// <summary> Полный путь к разделу реестра текущего профиля / конфигурации NanoCAD (без HKCU) </summary>
        public string RegistryHiveName
        {
            get => _registryHiveName;
        }

        /// <summary> Каталог по умолчанию для хранения проектов </summary>
        public string DefaultSaveProjectsFolder
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Path.Combine(_registryHiveName, _ioSaveProjects)))
                {
                    return key.GetValue(_saveInitDir).ToString();
                }
            }
            set
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Path.Combine(_registryHiveName, _ioSaveProjects), true))
                {
                    key.SetValue(_saveInitDir, value);
                }
            }
        }

        /// <summary> Каталог по умолчанию для хранения файлов </summary>
        public string DefaultSaveFilesFolder
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Path.Combine(_registryHiveName, _ioAllOpenFileFormats)))
                {
                    return key.GetValue(_saveFileFormatsInitDir).ToString();
                }
            }
            set
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Path.Combine(_registryHiveName, _ioAllOpenFileFormats), true))
                {
                    key.SetValue(_saveFileFormatsInitDir, value);
                }
            }
        }

        /// <summary> Каталог по умолчанию для открытия файлов </summary>
        public string DefaultOpenFilesFolder
        {
            get
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Path.Combine(_registryHiveName, _ioAllOpenFileFormats)))
                {
                    return key.GetValue(_openFileFormatsInitDir).ToString();
                }
            }
            set
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(Path.Combine(_registryHiveName, _ioAllOpenFileFormats), true))
                {
                    key.SetValue(_openFileFormatsInitDir, value);
                }
            }
        }

        private string _registryHiveName;

        private readonly string _ioSaveProjects = @"IO\SaveProjects";
        private readonly string _saveInitDir = "SaveInitDir";
        private readonly string _ioAllOpenFileFormats = @"IO\AllOpenFileFormats";
        private readonly string _openFileFormatsInitDir = "OpenInitDir";
        private readonly string _saveFileFormatsInitDir = "SaveInitDir";
    }
}


namespace drz.Tools
{
    class qsave
    {

        /// <summary>
        /// Quicks the save command. by Kpblc
        /// </summary>
        public static void QuickSaveCommand()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            PromptStringOptions fileNameOptions = new PromptStringOptions($"\nВведите имя файла")
            {
                AllowSpaces = true,
                DefaultValue = doc.Name,
                UseDefaultValue = true
            };
            PromptResult fileNameRes = ed.GetString(fileNameOptions);
            if (fileNameRes.Status != PromptStatus.OK)
            {
                return;
            }

            string fileName = fileNameRes.StringResult;

            PromptKeywordOptions options = new PromptKeywordOptions($"\nВыберите формат файла ");
            List<FileFormat> formatsList = new List<FileFormat>
        {
            new FileFormat("Acad 2018 dwg", DwgVersion.AC1032),
            new FileFormat("Acad 2013 dwg", DwgVersion.AC1027),
            new FileFormat("Acad 2010 dwg", DwgVersion.AC1024),
            new FileFormat("Acad 2007 dwg", DwgVersion.AC1021),
            new FileFormat("Acad 2004 dwg", DwgVersion.AC1800),
            new FileFormat("Acad 2018 dxf", DwgVersion.AC1032, true),
            new FileFormat("Acad 2013 dxf", DwgVersion.AC1027, true),
            new FileFormat("Acad 2010 dwg", DwgVersion.AC1024, true),
            new FileFormat("Acad 2007 dwg", DwgVersion.AC1021, true),
            new FileFormat("Acad 2004 dwg", DwgVersion.AC1800, true),
        };

            /*
            AC1032 = 0x21  2018 Final
            AC1027 = 0x1f  2013 Final
            AC1024 = 0x1d  2010 Final
            AC1021 = 0x1b  2007 Final
            AC1800 = 0x19  2004 final
            */
            foreach (FileFormat item in formatsList)
            {
                options.Keywords.Add(item.Keyword);
            }

            options.AllowNone = false;

            PromptResult fileFormatKey = ed.GetKeywords(options);

            if (fileFormatKey.Status != PromptStatus.OK)
            {
                return;
            }

            string fileFormat = fileFormatKey.StringResult;
            FileFormat entity = formatsList.First(o => o.Keyword.Equals(fileFormat));
            Database db = doc.Database;
            if (entity.SaveAsDxf)
            {
                db.DxfOut(fileName, 8, entity.FileVersion);
            }
            else
            {
                db.SaveAs(fileName, entity.FileVersion);
            }
        }

        private class FileFormat
        {
            public FileFormat(string Description, DwgVersion FileVer, bool SaveAsDxf = false)
            {
                this.SaveAsDxf = SaveAsDxf;
                this.FileVersion = FileVer;
                this.Description = Description;
            }
            public string Description { get; }
            public DwgVersion FileVersion { get; }
            public bool SaveAsDxf { get; }
            public string Keyword => Description.ToUpper().Replace(" ", "");
        }
    }



}


namespace QuickSaveAs

{
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

            Document doc =

              Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;

            Database db = doc.Database;



            // If this is the first time run...



            if (_path == "" || _base == "")

            {

                // Ask the user for a base file location



                PromptSaveFileOptions opts =

                  new PromptSaveFileOptions(

                    "Select location to save first drawing file"

                  );

                opts.Filter = "Drawing (*.dwg)|*.dwg";

                PromptFileNameResult pr =

                  ed.GetFileNameForSave(opts);



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

                        _path =

                          pr.StringResult.Substring(0, idx);

                        string fullname =

                          pr.StringResult.Substring(idx + 1);



                        // If the path has an extension (this should always

                        // be the case), extract the base file name



                        if (fullname.Contains(extSep))

                        {

                            _base =

                              fullname.Substring(

                                0,

                                fullname.LastIndexOf(extSep)

                              );

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

                  "(setvar \"CMDECHO\" 0)" +

                  "(command \"_.SAVEAS\" \"\" \"" + dwgPath2 + "\")" +

                  "(setvar \"CMDECHO\" " + ocmd.ToString() + ")" +

                  "(princ) ",

                  false,

                  false,

                  false

                );



                // Print a confirmation message for the DWG save

                // (which actually gets displayed before the queued

                // string gets executed, but anyway)



                ed.WriteMessage("\nSaved to: \"" + dwgPath + "\"");



                _count++;

            }

        }

    }

}

