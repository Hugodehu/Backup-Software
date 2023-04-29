using System;
using System.Collections.Generic;
using System.Text;

namespace PS_G1_L1.Model
{
    public class DailyLogsModel
    {
        #region Attributes
        public string Name { get; set; }
        public string Time { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        public string RepositorySource { get; set; }
        public string RepositoryDestination { get; set; }
        public long FilesSize { get; set; }
        public string FileTransferTime { get; set; }
        public BackUpType BackUpType { get; set; }
        public string FileEncryptTime { get; set; } = "0";
        #endregion
        #region Constructeur
        public DailyLogsModel() { }
        #endregion
    }
}
