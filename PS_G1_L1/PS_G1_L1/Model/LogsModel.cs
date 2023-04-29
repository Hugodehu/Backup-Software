using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS_G1_L1.Model
{
    public class LogsModel
    {
        #region Attributes
        public int id { get; set; }
        private string _name;
        private string _time =  DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        private BackUpStatus _status  = BackUpStatus.ACTIVE;
        private long? _numberFiles;
        private long? _filesSize;
        public long? RemainingFiles { get; set; }
        public long? RemainingFilesSize { get; set; }
        public string? SourceRemainingFile { get; set; }
        public string? DestinationRemainingFile { get; set; }
public string Name { get => _name; set => _name = value; }
        public string Time { get => _time; set => _time = value; }
        public BackUpStatus Status { get => _status; set => _status = value; }
        public long? NumberFiles { get => _numberFiles; set => _numberFiles = value; }
        public long? FilesSize { get => _filesSize; set => _filesSize = value; }
        #endregion
        #region Constucteur
        public LogsModel()
        {

        }
        #endregion

    }
}
