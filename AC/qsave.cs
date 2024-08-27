

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if NC  
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Teigha.DatabaseServices;
using Teigha.Runtime;

#else
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

#endif


namespace drz.CorrectBlocks
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
            new FileFormat("Acad 2018 dxf", DwgVersion.AC1032, true),
            new FileFormat("Acad 2013 dxf", DwgVersion.AC1027, true),
        };

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
