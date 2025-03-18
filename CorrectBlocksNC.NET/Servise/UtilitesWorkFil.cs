using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;





#if NC
using Cad = HostMgd.ApplicationServices.Application;
#elif AC

using App = Autodesk.AutoCAD.ApplicationServices;
using Cad = Autodesk.AutoCAD.ApplicationServices.Application;
using Db = Autodesk.AutoCAD.DatabaseServices;
using Gem = Autodesk.AutoCAD.Geometry;
using Ed = Autodesk.AutoCAD.EditorInput;
using Rtm = Autodesk.AutoCAD.Runtime;

#endif



namespace dRzTools.Servise
{
    /// <summary>Утилиты и подпрограммы</summary>
    internal partial class UtilitesWorkFil
    {
        #region РАБОТА С ФАЙЛАМИ

        /// <summary>
        /// Копирует файл в назначенную дирректорию
        /// </summary>
        /// <param name="sFulPathFileCopiedFil">Полный путь копируемого файла</param>
        /// <param name="sPathDestination">Каталог куда копировать</param>
        /// <param name="sOut">Успех - Полный путь куда скопирован файл
        /// <br>Неудача - Описание ошибки</br>
        /// </param>
        /// <returns>Успех</returns>
        public static bool tryCopyTo(string sFulPathFileCopiedFil, string sPathDestination, out string sOut)
        {
            try
            {
                string sFileName = Path.GetFileName(sFulPathFileCopiedFil);
                string sPathFullDestination = Path.Combine(sPathDestination, sFileName);

                //если юзер подключает файл из папки лицензии
                //сравним пути
                if (string.Compare(sFulPathFileCopiedFil, sPathFullDestination, true) == 0)
                {//пути равны
                    sOut = sPathFullDestination;//вернем путь
                    return true;

                }

                File.Copy(sFulPathFileCopiedFil, sPathFullDestination, true);

                sOut = sPathFullDestination;
                return true;
            }
            catch (Exception ex)
            {
                sOut = ex.Message;
                sOut += "\n";
                sOut += ex.StackTrace;

                return false;
            }
        }

       

        //https://stackoverflow.com/questions/1410127/c-sharp-test-if-user-has-write-access-to-a-folder
        /// <summary>
        /// Проверка доступа к каталогу на чтение запись
        /// </summary>
        /// <param name="dirPath">Путь к каталогу</param>
        /// <param name="throwIfFails">Выбрасывать ли исключение</param>
        /// <returns>true- Writable</returns>
        internal static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(
                    Path.Combine(
                        dirPath,
                        Path.GetRandomFileName()
                    ),
                    1,
                    FileOptions.DeleteOnClose)
                )
                { }
                return true;
            }
            catch
            {
                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }

        /// <summary>
        /// Проверка файла на блокировку
        /// </summary>
        /// <param name="f">Путь к файлу</param>
        /// <returns>false - не заблокирован
        /// <br>true - заблокирован</br>
        /// </returns>
        public static bool IsFileLocked(string fpath)
        {
            try
            {
                //string fpath = f.FullName;
                FileStream fs = File.OpenWrite(fpath);
                fs.Close();
                return false;
            }

            catch (Exception) { return true; }
        }


        /// <summary>Проверка и создание каталога</summary>
        /// <param name="FilPath">Путь каталога</param>
        /// <returns>Успех</returns>
        public static bool CreateDirMod(string FilPath)
        {
            if (!Directory.Exists(FilPath))
            {
                try
                {
                    Directory.CreateDirectory(FilPath);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(FilPath + "\n" + ex.Message + "\n" + "Каталог не создан\n Проверьте права на запись в каталог\n Возможны ошибки в работе программы", SysInfo.sTitleAttribute + " " + SysInfo.sVersionFull, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// По полному пути с расширением подбор уникального имени
        /// </summary>
        /// <param name="sFUlName">Полное имя путь+имя+расширение</param>
        /// <returns>Полный путь+уникальное имя+расширение</returns>
        public static string GetFileNameUniqu(string sFUlName)
        {            //путь
            string sPathTmp = Path.GetDirectoryName(sFUlName);
            //имя без расширения
            string sFilTmp = Path.GetFileNameWithoutExtension(sFUlName);
            //расширение
            string sFilExt = Path.GetExtension(sFUlName).Replace(".", "");
            //string sFilExt = sFilExtDot.Replace( ".", "");
            //новое уникальное имя
            // перегрузка для имени файла
            sFilTmp = GetFileNameUniqu(sPathTmp, sFilTmp, sFilExt);

            return Path.Combine(sPathTmp, sFilTmp + "." + sFilExt);
        }

        /// <summary> Подбор уникального имени файла для сохранения в файл</summary>
        /// <param name="sPlotPath">путь к файлу</param>
        /// <param name="sFilName">имя файла</param>
        /// <param name="sFilExt">расширение без точки!!!</param>
        /// <returns>Имя файла без расширения</returns>
        public static string GetFileNameUniqu(string sPlotPath, string sFilName, string sFilExt)
        {
            //string filename_initial = Path.Combine(sPlotPath, sFilName + "." + sFilExt);
            // sPlotPath
            //sPlotFilName
            string filename_current =Path.Combine(sPlotPath,  $"{sFilName}.{sFilExt}");// filename_initial;
            int count = 0;
            while (File.Exists(filename_current))
            {
                count++;
                filename_current = Path.Combine(sPlotPath, $"{sFilName}({count.ToString()}).{sFilExt}");
                //filename_current = sPlotPath// Path.GetDirectoryName(filename_initial)
                //                 + Path.DirectorySeparatorChar
                //                 + sFilName// Path.GetFileNameWithoutExtension(filename_initial)
                //                 + "("
                //                 + count.ToString()
                //                 + ")"
                //                 + /*sFilExt;//*/ Path.GetExtension(filename_current/*filename_initial*/);
            }
            return Path.GetFileNameWithoutExtension(filename_current);
        }

        /// <summary>Получить список путей фалов в директории</summary>
        /// <param name="sPath">Директория с файлами</param>
        /// <param name="WithSubfolders">Учитывать поддиректории</param>
        /// <param name="sSerchPatern">Маска поиска</param>
        /// <returns>Пути к файлам</returns>
        internal static string[] GetFilesOfDir(string sPath, bool WithSubfolders, string sSerchPatern = "*.dwg")
        {
            try
            {
                return Directory.GetFiles(sPath,
                                            sSerchPatern,
                                            WithSubfolders
                                            ? SearchOption.AllDirectories
                                            : SearchOption.TopDirectoryOnly);
            }
            catch (Exception ex)
            {
#if NC||AC
                Cad.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\n" + ex.Message);
#endif
                return new string[0];
            }
        }

        /// <summary>
        /// Возвращаем путь к файлу по полному пути (для длинных 255 путей)
        /// </summary>
        /// <param name="sDWGFulName">Полный путь с именем файла</param>
        /// <returns>Путь к каталогу</returns>
        internal static string GetDirectoryNameMod(string sDWGFulName)
        {
            string[] splitPathsTmp = sDWGFulName.Split('\\');
            string[] splitPaths = new string[splitPathsTmp.Length - 1];
            Array.Copy(splitPathsTmp, splitPaths, splitPathsTmp.Length - 1);

            return string.Join("\\", splitPaths);
        }

     

        #endregion

    }

}
