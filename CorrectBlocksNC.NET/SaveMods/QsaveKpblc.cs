
using System.Linq;
using System.Collections.Generic;








#if NC
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Runtime;
#elif AC
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
#endif

namespace dRzTools.SaveMods
{
    /// <summary>
    /// Quick Save Kpblc
    /// </summary>
    public static class QsaveKpblc
    {
        /// <summary>
        /// Quicks the save command. by Kpblc
        /// </summary>
        public static void QuickSaveKpblc()
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
#if AC || NC23
                    new FileFormat("Acad 2018 dwg", DwgVersion.AC1032),
#endif
                new FileFormat("Acad 2013 dwg", DwgVersion.AC1027),
                new FileFormat("Acad 2010 dwg", DwgVersion.AC1024),
                new FileFormat("Acad 2007 dwg", DwgVersion.AC1021),
                new FileFormat("Acad 2004 dwg", DwgVersion.AC1800),
#if AC || NC23
                new FileFormat("Acad 2018 dxf", DwgVersion.AC1032, true),
#endif
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
                FileVersion = FileVer;
                this.Description = Description;
            }
            public string Description { get; }
            public DwgVersion FileVersion { get; }
            public bool SaveAsDxf { get; }
            public string Keyword => Description.ToUpper().Replace(" ", "");
        }
    }
}
