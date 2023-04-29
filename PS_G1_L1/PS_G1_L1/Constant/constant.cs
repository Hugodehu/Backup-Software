using System;
using System.Collections.Generic;

namespace PS_G1_L1.utilities
{
    public static class paths
    {
        public static IDictionary<string, string> folderPaths = new Dictionary<string, string>()
    {
        {"root","EasySave"},

        {"config","EasySave/Config"},

        {"data","EasySave/Data"},
        {"log","EasySave/Data/Log"},
        {"dailylog","EasySave/Data/Dailylog"},
        {"save","EasySave/Data/Save"},
    };
        public static IDictionary<string, string> filePathsJson = new Dictionary<string, string>()
    {
        {"defaultSettings","EasySave/Config/Default.json"},
        {"log","EasySave/Data/Log/Log.json"},
        {"dailylog",$"EasySave/Data/Dailylog/{DateTime.Today.ToString("yyyy-MM-dd")}log.json"},
        {"save","EasySave/Data/Save/SaveData.json"},
    };
        public static IDictionary<string, string> filePathsXml = new Dictionary<string, string>()
    {
        {"defaultSettings","EasySave/Config/Default.xml"},
        {"log","EasySave/Data/Log/Log.xml"},
        {"dailylog",$"EasySave/Data/Dailylog/{DateTime.Today.ToString("yyyy-MM-dd")}log.xml"},
        {"save","EasySave/Data/Save/SaveData.xml"},
    };
        public static string encryptExe = "EasySave/Config/CryptoSoft.exe";
    }
}