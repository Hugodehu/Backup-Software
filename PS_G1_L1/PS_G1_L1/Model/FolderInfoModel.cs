using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace PS_G1_L1.Model
{
    public class FolderInfoModel : IDisposable
    {
        #region Attributes
        //Attributes

        private string _name;
        private string _location;

        private long _size;
        private long _numberOfFiles;

        private DateTime _creationDate;
        private DateTime _lastModified;

        public List<FileInfoModel> Files { get; private set; }
        public List<FolderInfoModel> Folders { get; private set; }

        private bool _disposed = false;

        /// <summary>
        /// Set or Returns the name of the folder
        /// </summary>
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        /// <summary>
        /// Set or Returns the location of the folder
        /// </summary>
        public string Location
        {
            get { return this._location; }
            set { this._location = value; }
        }

        /// <summary>
        /// Returns the size of the folder
        /// </summary>
        public long Size
        {
            get { return this._size; }
        }

        /// <summary>
        /// Returns the number of files in the folder
        /// </summary>
        public long NumberOfFiles
        {
            get { return this._numberOfFiles; }
        }

        /// <summary>
        /// Returns the date the file was created
        /// </summary>
        public DateTime CreationDate
        {
            get { return this._creationDate; }
        }

        /// <summary>
        /// Returns the last modification date of the folder
        /// </summary>
        public DateTime LastModified
        {
            get { return this._lastModified; }
        }

        #endregion
        #region Constructor
        //Constructor
        public FolderInfoModel(string path)
        {
            //Creation of a DriveInfo object to keep all informations of the folder
            DriveInfo di = new DriveInfo(path);
            Files = new List<FileInfoModel>();
            Folders = new List<FolderInfoModel>();
            _name = di.Name;
            _location = path;

            _size = di.TotalSize;

            _creationDate = File.GetCreationTime(path);
            _lastModified = File.GetLastWriteTime(path);

            GetAllItemsInDirectory(path);

            _numberOfFiles = Files.Count;

        }

        #endregion
        #region Methods
        //Methods

        public void GetAllItemsInDirectory(string path)
        {

            foreach (string folderPath in Directory.GetDirectories(path))
            {
                GetAllItemsInDirectory(folderPath);
            }

            foreach (string filePath in Directory.GetFiles(path))
            {
                //Adding the file
                Files.Add(new FileInfoModel(filePath));
            }
        }

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
                _location = string.Empty;

                _size = 0;
                _numberOfFiles = 0;

                foreach (FileInfoModel file in Files)
                    file.Dispose();

                foreach (FolderInfoModel folder in Folders)
                    folder.Dispose();

                Folders.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Objet dossier {tempName} detruit");
                Console.ResetColor();
            }

            _disposed = true;
        }

        #endregion
    }
}
