using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
//using Autodesk.AutoCAD.ApplicationServices.Core;
//using Autodesk.AutoCAD.DatabaseServices;
//using Autodesk.AutoCAD.LayerManager;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Win32;

using Teigha.DatabaseServices;
using Teigha.LayerManager;

using App = HostMgd.ApplicationServices;
using DialogResult = System.Windows.Forms.DialogResult;

//using DialogResult = Teigha.LayerManager.DialogResult;

namespace drz.Tools
{


    [StandardModule]
    public sealed class LFIO
    {
        /// <summary>
        ////фильтры
        /// </summary>
         private static List<string> FltExt = new List<string>();

        /// <summary>
        /// Экспорт
        /// </summary>
        public static void FilterExport()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Layer Filter Export (*.lfe)|*.lfe";
                saveFileDialog.Title = "Сохранить файл фильтра слоев";
                saveFileDialog.OverwritePrompt = false;
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FltExt.Clear();
                    LayerFilterCollection nestedFilters = App.Application.DocumentManager.MdiActiveDocument.Database.LayerFilters.Root.NestedFilters;

                    FilterExtractSub("", nestedFilters);
                    using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.Default))
                    {
                        //try
                        //{
                        foreach (string text in FltExt)
                        {
                            streamWriter.WriteLine(text);
                        }
                        //}
                        //finally
                        //{
                        //List<string>.Enumerator enumerator;
                        //((IDisposable)enumerator).Dispose();
                        //}
                        streamWriter.Close();
                    }
                    App.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DS> Total of (" + FltExt.Count.ToString() + ") Filters Exported");
                }
            }
        }

        /// <summary>
        ////Извлечение
        /// </summary>
        /// <param name="FltLev"></param>
        /// <param name="FltCol"></param>
        public static void FilterExtractSub(string FltLev, LayerFilterCollection FltCol)
        {
            try
            {
                foreach (object obj in FltCol)
                {
                    LayerFilter layerFilter = (LayerFilter)obj;
                    if (Operators.CompareString(layerFilter.Name.ToUpper(), "XREF", false) != 0)
                    {
                        if (Operators.CompareString(layerFilter.GetType().Name, "LayerFilter", false) == 0)
                        {
                            FltExt.Add(FltLev + layerFilter.Name + "<LayerFilter>¦" + layerFilter.FilterExpression);
                        }
                        else if (Operators.CompareString(layerFilter.GetType().Name, "LayerGroup", false) == 0)
                        {
                            LayerGroup layerGroup = (LayerGroup)layerFilter;
                            List<string> list = new List<string>();
                            Database database = App.Application.DocumentManager.MdiActiveDocument.Database;
                            using (Transaction transaction = database.TransactionManager.StartTransaction())
                            {
                                //LayerTable layerTable = (LayerTable)database.LayerTableId.GetObject(1);
                                LayerTable layerTable = (LayerTable)database.LayerTableId.GetObject(Teigha.DatabaseServices.OpenMode.ForWrite);// by razygraevaa on 25.03.2024 at 12:39
                                try
                                {
                                    foreach (object obj2 in layerGroup.LayerIds)
                                    {
                                        ObjectId objectId;
                                        // by razygraevaa on 25.03.2024 at 16:20
                                        if (obj2 != null)
                                        {
                                            objectId = (ObjectId)obj2;
                                        }
                                        else
                                        {
                                            objectId = default(ObjectId);
                                        }

                                        //ObjectId objectId = ((obj2 != null) ? ((ObjectId)obj2) : default(ObjectId));
                                        if (objectId.IsNull)// by razygraevaa on 25.03.2024 at 16:18
                                        {
                                            continue;
                                        }
                                        LayerTableRecord layerTableRecord = (LayerTableRecord)transaction.GetObject(objectId, 0);
                                        list.Add(layerTableRecord.Name);
                                    }
                                }
                                finally
                                {
                                    //IEnumerator enumerator2;
                                    //if (enumerator2 is IDisposable)
                                    //{
                                    //    (enumerator2 as IDisposable).Dispose();
                                    //}
                                }
                            }
                            FltExt.Add(FltLev + layerFilter.Name + "<LayerGroup>¦" + Strings.Join(list.ToArray(), ","));
                        }
                        if (layerFilter.NestedFilters.Count > 0)
                        {
                            FilterExtractSub(string.Concat(new string[]
                            {
                                FltLev,
                                layerFilter.Name,
                                "<",
                                layerFilter.GetType().Name,
                                ">\\"
                            }), layerFilter.NestedFilters);
                        }
                    }
                }
            }
            finally
            {
                //IEnumerator enumerator;
                //if (enumerator is IDisposable)
                //{
                //    (enumerator as IDisposable).Dispose();
                //}
            }
        }

        /// <summary>
        /// Импорт
        /// </summary>
        public static void FilterImport()
        {
            checked
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Layer Filter Export (*.lfe)|*.lfe";
                    openFileDialog.Title = "Открыть файл фильтров слоев";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        FltExt.Clear();
                        using (StreamReader streamReader = new StreamReader(openFileDialog.FileName, Encoding.Default))
                        {
                            while (streamReader.Peek() >= 0)
                            {
                                string text = streamReader.ReadLine();
                                FltExt.Add(text);
                            }
                            streamReader.Close();
                        }
                        Database database = App.Application.DocumentManager.MdiActiveDocument.Database;
                        LayerFilterTree layerFilters = database.LayerFilters;
                        try
                        {
                            foreach (string text2 in FltExt)
                            {
                                string text3 = "";
                                string text4 = "";
                                string text5 = "";
                                List<string> list = new List<string>(text2.Split(new char[] { '¦' }));
                                LayerFilterCollection layerFilterCollection = null;
                                if (!list[0].Contains("\\"))
                                {
                                    layerFilterCollection = layerFilters.Root.NestedFilters;
                                    text3 = list[0].Substring(0, list[0].IndexOf("<"));
                                    text4 = list[0].Substring(list[0].IndexOf("<"));
                                    text5 = list[1];
                                }
                                else
                                {
                                    layerFilterCollection = layerFilters.Root.NestedFilters;
                                    List<string> list2 = new List<string>(list[0].Split(new char[] { '\\' }));
                                    int num = list2.Count - 2;
                                    for (int i = 0; i <= num; i++)
                                    {
                                        string text6 = list2[i];
                                        string text7 = text6.Substring(0, text6.IndexOf("<"));
                                        //try
                                        //{
                                            foreach (object obj in layerFilterCollection)
                                            {
                                                LayerFilter layerFilter = (LayerFilter)obj;
                                                if (Operators.CompareString(layerFilter.Name.ToUpper(), text7.ToUpper(), false) == 0)
                                                {
                                                    layerFilterCollection = layerFilter.NestedFilters;
                                                    break;
                                                }
                                            }
                                        //}
                                        //finally
                                        //{
                                            //IEnumerator enumerator2;
                                            //if (enumerator2 is IDisposable)
                                            //{
                                            //    (enumerator2 as IDisposable).Dispose();
                                            //}
                                        //}
                                    }
                                    string text8 = list2[list2.Count - 1];
                                    text3 = text8.Substring(0, text8.IndexOf("<"));
                                    text4 = text8.Substring(text8.IndexOf("<"));
                                    text5 = list[1];
                                }
                                object obj2 = null;
                                try
                                {
                                    foreach (object obj3 in layerFilterCollection)
                                    {
                                        LayerFilter layerFilter2 = (LayerFilter)obj3;
                                        if (Operators.CompareString(layerFilter2.Name.ToUpper(), text3.ToUpper(), false) == 0)
                                        {
                                            obj2 = layerFilter2;
                                            break;
                                        }
                                    }
                                }
                                finally
                                {
                                    //IEnumerator enumerator3;
                                    //if (enumerator3 is IDisposable)
                                    //{
                                    //    (enumerator3 as IDisposable).Dispose();
                                    //}
                                }
                                if (obj2 == null)
                                {
                                    if (Operators.CompareString(text4, "<LayerFilter>", false) == 0)
                                    {
                                        obj2 = new LayerFilter();
                                        if (text5.Length > 0)
                                        {
                                            NewLateBinding.LateSet(obj2, null, "FilterExpression", new object[] { text5 }, null, null);
                                        }
                                    }
                                    else if (Operators.CompareString(text4, "<LayerGroup>", false) == 0)
                                    {
                                        obj2 = new LayerGroup();
                                        using (Transaction transaction = database.TransactionManager.StartTransaction())
                                        {
                                            LayerTable layerTable = (LayerTable)database.LayerTableId.GetObject(Teigha.DatabaseServices.OpenMode.ForWrite);
                                            List<string> list3 = new List<string>(text5.Split(new char[] { ',' }));
                                            try
                                            {
                                                foreach (string text9 in list3)
                                                {
                                                    if (layerTable.Has(text9))
                                                    {
                                                        LayerTableRecord layerTableRecord = (LayerTableRecord)transaction.GetObject(layerTable[text9], 0);
                                                        NewLateBinding.LateCall(NewLateBinding.LateGet(obj2, null, "LayerIds", new object[0], null, null, null), null, "Add", new object[] { layerTableRecord.ObjectId }, null, null, null, true);
                                                    }
                                                }
                                            }
                                            finally
                                            {
                                                //List<string>.Enumerator enumerator4;
                                                //((IDisposable)enumerator4).Dispose();
                                            }
                                        }
                                    }
                                    NewLateBinding.LateSet(obj2, null, "Name", new object[] { text3 }, null, null);
                                    layerFilterCollection.Add((LayerFilter)obj2);
                                }
                            }
                        }
                        finally
                        {
                            //List<string>.Enumerator enumerator;
                            //((IDisposable)enumerator).Dispose();
                        }
                        database.LayerFilters = layerFilters;
                        App.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DS> Total of (" + FltExt.Count.ToString() + ") Filters Imported or Updated.");
                    }
                }
            }
        }

    }
}
