using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

using drz.CorrectBlocks.Core.Settings;

namespace drz.CorrectBlocks.Core
{
    /// <summary>
    /// Чтение сохранение настроек
    /// </summary>
    public class AppSetttings
    {
        static AppSetttings()
        {
            LoadSettings();
        }

        public static ApplicationSettings Settings
        {
            get => _settings;
            set => _settings = value ?? new ApplicationSettings();
        }

        public static void SaveSettings()
        {
            string folder = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (FileStream writer = new FileStream(_configPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                _serializer.Serialize(writer, _settings);
            }
        }

        public static string ConfigPath => _configPath;

        private static void LoadSettings()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    using (FileStream stream = new FileStream(_configPath, FileMode.Open))
                    {
                        _settings = (ApplicationSettings)_serializer.Deserialize(stream);
                    }
                }
                else
                {
                    _settings = new ApplicationSettings();
                }
            }
            catch
            {
                _settings = new ApplicationSettings();
            }
        }

        private static ApplicationSettings _settings;
        private static readonly XmlSerializer _serializer = new XmlSerializer(typeof(ApplicationSettings));
        private static readonly string _configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                                                                  "Config",
                                                                  "drzTools.config");//think Assembly.GetExecutingAssembly() вернет имя этой сборки
    }
}
