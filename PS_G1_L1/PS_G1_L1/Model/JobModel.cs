using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PS_G1_L1.Model
{
    /// <summary>
    /// Type of backup
    /// </summary>
    public enum BackUpType
    {
        FULL,
        DIFFERENTIAL
    }

    /// <summary>
    /// Backup status
    /// </summary>
    public enum BackUpStatus
    {
        WAITING,
        ACTIVE,
        END,
        FAILED,
        CANCELED
    }

    public class JobModel : IDisposable
    {
        #region Attributes
        //Attributes

        private string _name;
        private string _source;
        private string _destination;
        private BackUpType _type;
        private bool _disposed = false;

        public BackUpStatus Status { get; set; } = BackUpStatus.WAITING;

        public string TypeOfSave { get; set; }
        public bool IsEncrypt { get; set; }

        /// <summary>
        /// Sets or gets the name of the save
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// Sets or returns the path of the backup
        /// </summary>
        public string Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Sets or gets the destination path of the save
        /// </summary>
        public string Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        /// <summary>
        /// Get the type of the save
        /// </summary>
        public BackUpType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        #endregion
        #region Constructors
        //Constructors

        public JobModel(string saveName, string saveSource, string saveDestination, BackUpType saveType)
        {
            _name = saveName;
            _source = saveSource;
            _destination = saveDestination;
            _type = saveType;
        }

        public JobModel() { }

        #endregion
        #region Methods
        //Methods

        /// <summary>
        /// Checking the status of the current backup
        /// </summary>
        /// <returns>BackUpStatus : Status</returns>


        public void Dispose()
        {
            Dispose(disposing: true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //All data used for the back up are dispose
                string tempName = _name;
                _name = string.Empty;
                _source = string.Empty;
                _destination = string.Empty;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Objet sauvegarde {tempName} detruit");
                Console.ResetColor();
            }

            _disposed = true;
        }

        public override string ToString()
        {
            return $"name: {_name}, destination: {_destination}, source: {_source}, type: {_type}";
        }
        #endregion
    }
}
