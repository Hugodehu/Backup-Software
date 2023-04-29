using Newtonsoft.Json;
using PS_G1_L1;
using PS_G1_L1.Model;
using PS_G1_L1.ViewModel;
using PS_G1_L1.View;
using PS_G1_L1.utilities;
using PS_G1_L1.Model.Enums;
using System.IO;
using System.Xml.Serialization;
using PS_G1_L1.Constant;
using System;
using System.Globalization;
using System.Threading;

namespace PS_G1_L1.ViewModel
{
    public sealed class InstallerViewModel
    {
        private static InstallerViewModel _instance;
        public static InstallerViewModel Instantiate()
        {
            if (_instance == null)
            {
                _instance = new InstallerViewModel();
            }

            return _instance;
        }
        private InstallerViewModel()
        {
            //LangHelper.ChangeLanguage("fr");

            CheckFolderInstall();
            CheckUserDefaultParameter();
            CheckUserLanguage();
            CheckSaveFile();
        }
        private void CheckFolderInstall()
        {
            try
            {
                foreach (var (key, path) in paths.folderPaths)
                {
                    Console.WriteLine(path);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CheckUserDefaultParameter()
        {
            UserDefaultParameterViewModel settings = UserDefaultParameterViewModel.Instantiate();
            try
            {
                if (File.Exists(paths.filePathsJson["defaultSettings"]))
                {
                    string json = File.ReadAllText(paths.filePathsJson["defaultSettings"]);
                    try
                    {
                        UserDefaultParameterModel parameters = JsonConvert.DeserializeObject<UserDefaultParameterModel>(json) ?? new UserDefaultParameterModel();
                        settings.UserDefaultParameterModel = parameters;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if (File.Exists(paths.filePathsXml["defaultSettings"]))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserDefaultParameterModel));
                    using (TextReader textReader = new StreamReader(paths.filePathsXml["defaultSettings"]))
                    {
                        settings.UserDefaultParameterModel = (UserDefaultParameterModel)xmlSerializer.Deserialize(textReader);
                    }
                }
                else
                {
                    string json = JsonConvert.SerializeObject(settings, Formatting.Indented);

                    File.WriteAllText(paths.filePathsJson["defaultSettings"], json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void CheckUserLanguage()
        {
            try
            {
                if(UserConst.UserLanguage == LanguageEnum.FR)
                {
                    CultureInfo _info = CultureInfo.CreateSpecificCulture("fr-FR");
                    Thread.CurrentThread.CurrentCulture = _info;
                    Thread.CurrentThread.CurrentUICulture = _info;
                }
                if(UserConst.UserLanguage == LanguageEnum.EN)
                {
                    CultureInfo _info = CultureInfo.CreateSpecificCulture("en-EN");
                    Thread.CurrentThread.CurrentCulture = _info;
                    Thread.CurrentThread.CurrentUICulture = _info;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CheckSaveFile()
        {
            try
            {
                if (UserConst.UserFormat == FormatEnum.JSON)
                {
                    foreach (var (key, path) in paths.filePathsJson)
                    {
                        if (!File.Exists(path))
                        {
                            File.WriteAllText(path, "");
                        }
                    }
                }
                else if (UserConst.UserFormat == FormatEnum.XML)
                {
                    foreach (var (key, path) in paths.filePathsXml)
                    {
                        if (!File.Exists(path))
                        {
                            File.WriteAllText(path, "");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}