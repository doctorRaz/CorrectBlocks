using System.IO;

using Microsoft.Win32;

namespace drzTools.Settings
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
