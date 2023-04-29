using Newtonsoft.Json;
using PS_G1_L1.Model.Enums;
using System.IO;
using System.Xml.Serialization;
using PS_G1_L1.utilities;
using System.Windows.Documents;
using System.Collections.Generic;

namespace PS_G1_L1.Model
{
    public class UserDefaultParameterModel
    {
        #region Attributes
        //Attributes
        private string? _userSavePath;
        private LanguageEnum _language = LanguageEnum.FR;
        private FormatEnum _format;
        public string KeyEncrypt { get; } = "EasySaveG1";
        public List<string> EncryptExtension { get; set; }
        public List<string> ExtensionFilePrioritary { get; set; }
        public List<string> BusinessSoftwareProcesses { get; set; }
        public int MaxSizeFile { get; set; }
        /// <summary>
        /// Sets or Gets the user save path
        /// </summary>
        public string? UserSavePath
        {
            get { return _userSavePath; }
            set { _userSavePath = value; }
        }

        /// <summary>
        /// Sets or Gets the language
        /// </summary>
        public LanguageEnum Language
        {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        /// Sets or Gets the format
        /// </summary>
        public FormatEnum Format
        {
            get { return _format; }
            set { _format = value; }
        }

        #endregion
        #region Constructors
        //Constructors

        //Default Constructor
        public UserDefaultParameterModel()
        {
            EncryptExtension = new List<string>();
            ExtensionFilePrioritary = new List<string>();
            BusinessSoftwareProcesses = new List<string>();
        }



        #endregion
        #region Methods
        //Methods
        public void GetData()
        {
            if (File.Exists(paths.filePathsXml["defaultSettings"]))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserDefaultParameterModel));
                using (TextReader textReader = new StreamReader(paths.filePathsXml["defaultSettings"]))
                {
                    UserDefaultParameterModel parameterModel = (UserDefaultParameterModel)xmlSerializer.Deserialize(textReader);
                    if (parameterModel != null)
                    {
                        UserSavePath = parameterModel.UserSavePath;
                        Language = parameterModel.Language;
                        Format = parameterModel.Format;
                        EncryptExtension = parameterModel.EncryptExtension;
                        ExtensionFilePrioritary = parameterModel.ExtensionFilePrioritary;
                        BusinessSoftwareProcesses = parameterModel.BusinessSoftwareProcesses;
                    }
                }
            }
            else if (File.Exists(paths.filePathsJson["defaultSettings"]))
            {
                string jsonFile = File.ReadAllText(paths.filePathsJson["defaultSettings"]);
                var param = JsonConvert.DeserializeObject<UserDefaultParameterModel>(jsonFile);
                if (param != null)
                {
                    UserSavePath = param.UserSavePath;
                    Language = param.Language;
                    Format = param.Format;
                    EncryptExtension = param.EncryptExtension;
                    ExtensionFilePrioritary = param.ExtensionFilePrioritary;
                    BusinessSoftwareProcesses = param.BusinessSoftwareProcesses;
                }
            }
            else
            {
                Format = FormatEnum.JSON;
                WriteData();
            }
        }

        public void WriteData()
        {
            switch (_format)
            {
                case FormatEnum.XML:
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserDefaultParameterModel));
                    string path = paths.filePathsXml["defaultSettings"];
                    using (TextWriter textWriter = new StreamWriter(path))
                    {
                        xmlSerializer.Serialize(textWriter, this);
                    }
                    break;
                default:
                    string json = paths.filePathsJson["defaultSettings"];
                    string jsonFile = JsonConvert.SerializeObject(this, Formatting.Indented);
                    File.WriteAllText(json, jsonFile);
                    break;
            }
        }
        #endregion
    }
}
