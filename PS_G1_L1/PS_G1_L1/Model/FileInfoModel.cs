using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS_G1_L1.Model
{
    public class FileInfoModel : IDisposable
    {
        #region Attributes
        //Attributes

        private string _name { get; set; }
        private string _extension;
        private string _location;
        private long _size;
        private DateTime _creationDate;
        private DateTime _lastModified;
        private bool _disposed = false;

        /// <summary>
        /// Sets or Returns the name of the file
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Returns the extension of the file
        /// </summary>
        public string Extension
        {
            get { return this._extension; }
        }

        /// <summary>
        /// Sets or Returns the path of the file
        /// </summary>
        public string Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        /// <summary>
        /// Returns the size of the file
        /// </summary>
        public long Size
        {
            get { return this._size; }
        }

        /// <summary>
        /// Returns the creation date of the file
        /// </summary>
        public DateTime CreationDate
        {
            get { return this._creationDate; }
        }

        /// <summary>
        /// Returns the last modification date of the file
        /// </summary>
        public DateTime LastModified
        {
            get { return this._lastModified; }
        }

        #endregion
        #region Constructors
        //Constructors

        /// <summary>
        /// Class of objects used for files
        /// </summary>
        /// <param name="path">Takes the path of the file</param>
        public FileInfoModel(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            _name = fileInfo.Name;
            _location = path;
            _extension = fileInfo.Extension;

            _size = fileInfo.Length;

            _creationDate = fileInfo.CreationTime;
            _lastModified = fileInfo.LastWriteTime;
        }
        #endregion
        #region Methods
        //Methods

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                string tempName = _name;
                _name = string.Empty;
                _extension = string.Empty;
                _location = string.Empty;

                _size = 0;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Objet fichier {tempName} detruit");
                Console.ResetColor();
            }

            _disposed = true;
        }

        #endregion
    }
}
