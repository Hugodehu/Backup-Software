using System.Collections.Generic;
using PS_G1_L1.Model.Enums;
using PS_G1_L1.ViewModel;

namespace PS_G1_L1.Constant
{
    public static class UserConst
    {
        public static string? UserSavePath;
        public static LanguageEnum UserLanguage;
        public static FormatEnum UserFormat;
        public static List<string> EncryptExtension;
        public static List<string> ExtensionFilePrioritary;
        public static List<string> BusinessSoftwareProcesses;
        public static int MaxSizeFile;

        static UserConst()
        {
            var user = UserDefaultParameterViewModel.Instantiate();
            UserSavePath = user.UserDefaultParameterModel.UserSavePath;
            UserLanguage = user.UserDefaultParameterModel.Language;
            UserFormat = user.UserDefaultParameterModel.Format;
            EncryptExtension = user.UserDefaultParameterModel.EncryptExtension ?? new List<string>();
            ExtensionFilePrioritary = user.UserDefaultParameterModel.ExtensionFilePrioritary ?? new List<string>();
            BusinessSoftwareProcesses = user.UserDefaultParameterModel.BusinessSoftwareProcesses ?? new List<string>();
            MaxSizeFile = user.UserDefaultParameterModel.MaxSizeFile;
        }
    }
}