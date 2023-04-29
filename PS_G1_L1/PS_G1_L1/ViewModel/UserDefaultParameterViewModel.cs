using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PS_G1_L1.Model;
using PS_G1_L1.Model.Enums;
using PS_G1_L1.utilities;

namespace PS_G1_L1.ViewModel
{
    internal sealed class UserDefaultParameterViewModel
    {
        #region Attributes
        //Attributes
        private static UserDefaultParameterViewModel _instance;

        public UserDefaultParameterModel UserDefaultParameterModel;

        #endregion
        #region Constructors
        //Constructors

        public UserDefaultParameterViewModel()
        {
            //TODO : Enlever quand antoire aura fait le instanciate

            UserDefaultParameterModel = new UserDefaultParameterModel();
            UserDefaultParameterModel.GetData();
        }

        #endregion
        #region Methods
        //Methods

        public static UserDefaultParameterViewModel Instantiate()
        {
            if (_instance == null)
            {
                _instance = new UserDefaultParameterViewModel();
            }
            return _instance;
        }

        public bool SetUserSavePath(string path)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
                return false;
            }
            else
            {
                if (Directory.Exists(path))
                {
                    _instance.UserDefaultParameterModel.UserSavePath = path;
                    _instance.UserDefaultParameterModel.WriteData();
                    Console.WriteLine("Chemin utilisateur changé !");
                    return true;
                }
                else
                {
                    Console.WriteLine("Chemin non valide !");
                    return false;
                }
            }
        }

        public void SetLanguage(LanguageEnum language)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                _instance.UserDefaultParameterModel.Language = language;
                _instance.UserDefaultParameterModel.WriteData();
                App.ChangeCulture(language.ToString());
                Console.WriteLine("Langue changée !");

            }
        }
        public void SetFormat(FormatEnum format)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                var oldFormat = _instance.UserDefaultParameterModel.Format;
                if (oldFormat != format)
                {
                    _instance.UserDefaultParameterModel.Format = format;
                    if (oldFormat == FormatEnum.JSON)
                    {
                        // Remplacement des Settings par défaut
                        string defaultSettings = File.ReadAllText(paths.filePathsJson["defaultSettings"]);
                        UserDefaultParameterModel Settings = JsonConvert.DeserializeObject(defaultSettings) as UserDefaultParameterModel ?? new UserDefaultParameterModel();
                        Settings.Format = FormatEnum.XML;
                        File.Delete(paths.filePathsJson["defaultSettings"]);
                        XmlSerializer defaultSettingsXmlSerializer = new XmlSerializer(typeof(UserDefaultParameterModel));
                        using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["defaultSettings"]))
                        {
                            defaultSettingsXmlSerializer.Serialize(textWriter, Settings);
                        }
                        // Remplacement des logs
                        string log = File.ReadAllText(paths.filePathsJson["log"]);
                        var Logs = JsonConvert.DeserializeObject<List<LogsModel>>(log);
                        File.Delete(paths.filePathsJson["log"]);
                        XmlSerializer logXmlSerializer = new XmlSerializer(typeof(List<LogsModel>));
                        using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["log"]))
                        {
                            logXmlSerializer.Serialize(textWriter, Logs);
                        }
                        // Remplacement des Daily Logs
                        string dailyLog = File.ReadAllText(paths.filePathsJson["dailylog"]);
                        var DailyLogs = JsonConvert.DeserializeObject<List<LogsModel>>(dailyLog);
                        File.Delete(paths.filePathsJson["dailylog"]);
                        XmlSerializer dailyLogXmlSerializer = new XmlSerializer(typeof(List<LogsModel>));
                        using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["dailylog"]))
                        {
                            dailyLogXmlSerializer.Serialize(textWriter, DailyLogs);
                        }
                        // Remplacement des Jobs
                        string save = File.ReadAllText(paths.filePathsJson["save"]);
                        var SaveJobs = JsonConvert.DeserializeObject<List<JobModel>>(save);
                        File.Delete(paths.filePathsJson["save"]);
                        XmlSerializer saveXmlSerializer = new XmlSerializer(typeof(List<JobModel>));
                        using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["save"]))
                        {
                            saveXmlSerializer.Serialize(textWriter, SaveJobs);
                        }
                    }
                    else
                    {
                        // Remplacement des settings utilisateurs 
                        XmlSerializer defaultSettingsXmlSerializer = new XmlSerializer(typeof(UserDefaultParameterModel));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["defaultSettings"]))
                        {
                            UserDefaultParameterModel parameterModel = defaultSettingsXmlSerializer.Deserialize(textReader) as UserDefaultParameterModel ?? new UserDefaultParameterModel();
                            string json = paths.filePathsJson["defaultSettings"];
                            string jsonFile = JsonConvert.SerializeObject(parameterModel);
                            File.WriteAllText(json, jsonFile);

                        }
                        File.Delete(paths.filePathsXml["defaultSettings"]);
                        // Remplacement des Logs
                        XmlSerializer LogsSettingsXmlSerializer = new XmlSerializer(typeof(List<LogsModel>));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["log"]))
                        {
                            try
                            {
                                List<LogsModel> parameterModel = LogsSettingsXmlSerializer.Deserialize(textReader) as List<LogsModel> ?? new List<LogsModel>();
                                string json = paths.filePathsJson["log"];
                                string jsonFile = JsonConvert.SerializeObject(parameterModel);
                                File.WriteAllText(json, jsonFile);
                            }
                            catch
                            {
                                string json = paths.filePathsJson["log"];
                                string jsonFile = JsonConvert.SerializeObject(new List<LogsModel>());
                                File.WriteAllText(json, jsonFile);
                            }

                        }
                        File.Delete(paths.filePathsXml["log"]);
                        // Remplacement des Daily Logs
                        XmlSerializer dailyLogsSettingsXmlSerializer = new XmlSerializer(typeof(List<LogsModel>));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["dailylog"]))
                        {
                            try
                            {
                                List<LogsModel> parameterModel = dailyLogsSettingsXmlSerializer.Deserialize(textReader) as List<LogsModel> ?? new List<LogsModel>();
                                string json = paths.filePathsJson["dailylog"];
                                string jsonFile = JsonConvert.SerializeObject(parameterModel);
                                File.WriteAllText(json, jsonFile);
                            }
                            catch
                            {
                                string json = paths.filePathsJson["dailylog"];
                                string jsonFile = JsonConvert.SerializeObject(new List<LogsModel>());
                                File.WriteAllText(json, jsonFile);
                            }
                        }
                        File.Delete(paths.filePathsXml["dailylog"]);
                        // Remplacement des Jobs
                        XmlSerializer saveSettingsXmlSerializer = new XmlSerializer(typeof(List<JobModel>));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["save"]))
                        {
                            try
                            {
                                List<JobModel> parameterModel = saveSettingsXmlSerializer.Deserialize(textReader) as List<JobModel> ?? new List<JobModel>();
                                string json = paths.filePathsJson["save"];
                                string jsonFile = JsonConvert.SerializeObject(parameterModel);
                                File.WriteAllText(json, jsonFile);
                            }
                            catch
                            {
                                string json = paths.filePathsJson["save"];
                                string jsonFile = JsonConvert.SerializeObject(new List<JobModel>());
                                File.WriteAllText(json, jsonFile);
                            }
                        }
                        File.Delete(paths.filePathsXml["save"]);
                    }
                    _instance.UserDefaultParameterModel.WriteData();

                    Console.WriteLine("Format changée !");
                }
                else
                {
                    Console.WriteLine($"Format déjà sous forme {_instance.UserDefaultParameterModel.Format}");
                }
                Console.WriteLine("Press Enter...");
                Console.ReadLine();
            }
        }
        public void AddEncryptExtension(string extension)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                try
                {
                    _instance.UserDefaultParameterModel.EncryptExtension.Add(extension);
                    _instance.UserDefaultParameterModel.WriteData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void DeleteEncryptExtension(string extension)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                try
                {
                    _instance.UserDefaultParameterModel.EncryptExtension.Remove(extension);
                    _instance.UserDefaultParameterModel.WriteData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void AddExtensionFilePrioritary(string extension)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                try
                {
                    _instance.UserDefaultParameterModel.ExtensionFilePrioritary.Add(extension);
                    _instance.UserDefaultParameterModel.WriteData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        public void DeleteExtensionFilePrioritary(string extension)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                try
                {
                    _instance.UserDefaultParameterModel.ExtensionFilePrioritary.Remove(extension);
                    _instance.UserDefaultParameterModel.WriteData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void AddBusinessSoftwareProcess(string processName)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non-instancié");
                return;
            }

            try
            {
                _instance.UserDefaultParameterModel.BusinessSoftwareProcesses.Add(processName);
                _instance.UserDefaultParameterModel.WriteData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DeleteBusinessSoftwareProcess(string processName)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancié");
                return;
            }

            try
            {
                _instance.UserDefaultParameterModel.BusinessSoftwareProcesses.Remove(processName);
                _instance.UserDefaultParameterModel.WriteData();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void SetMaxFileSize(int maxFileSize)
        {
            if (_instance == null)
            {
                Console.WriteLine("Erreur objet non instancier");
            }
            else
            {
                try
                {
                    _instance.UserDefaultParameterModel.MaxSizeFile = maxFileSize;
                    _instance.UserDefaultParameterModel.WriteData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        
        #endregion
    }
}
