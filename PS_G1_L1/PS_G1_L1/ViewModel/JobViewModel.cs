using Newtonsoft.Json;
using PS_G1_L1.Constant;
using PS_G1_L1.Model;
using PS_G1_L1.Model.Enums;
using PS_G1_L1.utilities;
using PS_G1_L1.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using MessageBox = System.Windows.MessageBox;

namespace PS_G1_L1.ViewModel
{
    public class JobViewModel : IDisposable
    {
        #region Attributes
        //Attributes

        private bool _disposed;
        public List<LogsModel> Logs { get; set; }
        public List<DailyLogsModel> DailyLogs { get; set; }
        public List<JobModel> Jobs { get; set; }

        public JobModel jobToCopy { get; set; }

        public List<JobModel> MultiJob { get; set; }
        private bool isEncrypt { get; set; } = false;

        public bool Pause = false;

        public ReadMultiWork multiView;
        private SemaphoreSlim MaxSizeFile = new SemaphoreSlim(1);
        #endregion
        #region Constructors
        //Constructors

        public JobViewModel()
        {
            MultiJob = new List<JobModel>();
        }

        #endregion
        #region Methods
        //Methods

        #region Encrypt

        private void Encrypt(string TypeOfOperation, string source, string destination)
        {
            string Key = UserDefaultParameterViewModel.Instantiate().UserDefaultParameterModel.KeyEncrypt;

            Process process = new Process();

            // Setting up the CryptoSoft process
            ProcessStartInfo cryptoSoftInfo = new ProcessStartInfo("CryptoSoft.exe");
            cryptoSoftInfo.UseShellExecute = true;
            cryptoSoftInfo.ArgumentList.Add(TypeOfOperation);
            cryptoSoftInfo.ArgumentList.Add(source);
            cryptoSoftInfo.ArgumentList.Add(destination);
            cryptoSoftInfo.ArgumentList.Add(Key);

            // Starting up the CryptoSoft process
            Process cryptoSoft = Process.Start(cryptoSoftInfo);
            cryptoSoft.WaitForExit();
            cryptoSoft.CloseMainWindow();
            cryptoSoft.Close();
            //process = Process.Start("C:\Users\pierr\OneDrive\Bureau\Projet Ugo Terminé\PS_G1_L1\PS_G1_L1\PS_G1_L1\bin\Debug\netcoreapp3.1\EasySave\Config\CryptoSoft.exe", parameters);
        }

        #endregion

        #region CreateJob

        public void CreateJob(string title, string source, string destination, BackUpType backUpType, string typeOfSave, bool isEncrypt)
        {
            JobModel backUpModel = new JobModel(title, source, destination, backUpType);
            backUpModel.TypeOfSave = typeOfSave;
            backUpModel.IsEncrypt = isEncrypt;
            WriteJobs(backUpModel);
        }

        #endregion

        #region Copy

        public void Copy(string title, string source, string destination, bool encrypt, object sender, BackUpType backUpType)
        {
            DateTime time = DateTime.Now;
            DailyLogsModel daily = new DailyLogsModel();
            daily.Name = title;
            daily.RepositorySource = source;

            daily.BackUpType = backUpType;

            LogsModel log = new LogsModel();
            log.Name = title;
            isEncrypt = encrypt;

            if (File.Exists(source))
            {
                FileInfo fileInfo = new FileInfo(source);
                if (encrypt && UserConst.EncryptExtension.Contains(fileInfo.Extension))
                {
                    destination = destination + "\\" + Path.GetFileNameWithoutExtension(source) + "Encrypt" + Path.GetExtension(source);
                    daily.RepositoryDestination = destination;
                }
                else
                {
                    destination = destination + "\\" + Path.GetFileName(source);
                    daily.RepositoryDestination = destination;
                }
                //System.Windows.Forms.MessageBox.Show(destination);
            }

            if (File.Exists(source))
            {
                //Keep all info from file
                FileInfo fileInfo = new FileInfo(source);
                log.NumberFiles = 1;
                log.FilesSize = fileInfo.Length;
                daily.FilesSize = fileInfo.Length;
                log.RemainingFiles = 1;
                log.RemainingFilesSize = fileInfo.Length;
                log.SourceRemainingFile = source;
                log.DestinationRemainingFile = destination;
                log.Status = BackUpStatus.ACTIVE;
                WriteLogs(log);
                long fileLength = fileInfo.Length;

                //System.Windows.Forms.MessageBox.Show("Démarrage la sauvegarde");

                //Check if the copy is encrypt
                if (encrypt && UserConst.EncryptExtension.Contains(fileInfo.Extension))
                {
                    CheckBusinessSoftware();

                    while (Pause)
                    {
                        Thread.Sleep(100);
                    }
                    if (UserConst.MaxSizeFile <= fileInfo.Length)
                    {
                        System.Windows.MessageBox.Show("MaxSize pris");
                        MaxSizeFile.Wait();
                        Encrypt("ENCRYPT", source, destination);
                        MaxSizeFile.Release();
                        System.Windows.MessageBox.Show("MaxSize release");

                    }
                    else
                    {
                        Encrypt("ENCRYPT", source, destination);
                    }
                    
                }
                else
                {
                    CheckBusinessSoftware();

                    while (Pause)
                    {
                        Thread.Sleep(100);
                    }
                    if (UserConst.MaxSizeFile <= fileInfo.Length)
                    {
                        MaxSizeFile.Wait();
                        CopyFile(destination, fileInfo, fileLength, log, sender, backUpType);
                        MaxSizeFile.Release();
                    }
                    else
                    {
                        CopyFile(destination, fileInfo, fileLength, log, sender, backUpType);
                    }
                }
                log.Status = BackUpStatus.END;
                WriteLogs(log);

            }
            else
            {
                //Keep all info from directory
                int totalFiles = GetTotalNumberOfFilesInDirectory(source);
                int copiedFiles = 0;
                log.NumberFiles = totalFiles;
                log.FilesSize = GetTotalSizeOfFiles(source);
                daily.FilesSize = GetTotalSizeOfFiles(source);
                log.RemainingFiles = GetTotalNumberOfFilesInDirectory(source);
                log.RemainingFilesSize = GetTotalSizeOfFiles(source);
                log.SourceRemainingFile = source;
                log.DestinationRemainingFile = destination;
                WriteLogs(log);

                //System.Windows.Forms.MessageBox.Show("Démarrage la sauvegarde");
                CopyDirectory(source, destination, ref copiedFiles, totalFiles, log, sender, backUpType);
                log.Status = BackUpStatus.END;
                WriteLogs(log);
            }
            DateTime end = DateTime.Now;
            var test = end - time;
            daily.FileTransferTime = test.ToString(@"hh\:mm\:ss\.fff");
            WriteDailyLogs(daily);
        }

        #endregion

        #region GetTotalSizeOfFiles

        private long GetTotalSizeOfFiles(string source)
        {
            long totalSize = 0;

            string[] files = Directory.GetFiles(source);

            foreach (string file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            string[] directories = Directory.GetDirectories(source);

            foreach (string subDirectory in directories)
            {
                totalSize += GetTotalSizeOfFiles(subDirectory);
            }

            return totalSize;
        }

        #endregion

        #region CopyFile

        private void CopyFile(string destination, FileInfo fileInfo, long fileLength, LogsModel log, object sender, BackUpType backUpType)
        {
            long bytesCopied = 0;

            var workerLocal = sender as BackgroundWorker;
            workerLocal.ReportProgress(0, "0%");

            if (backUpType == BackUpType.FULL)
            {
                using (FileStream sourceStream = fileInfo.OpenRead())
                {
                    using (FileStream destinationStream = File.Create(destination))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        log.RemainingFiles -= 1;
                        while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            CheckBusinessSoftware();
                            
                            while (Pause)
                            {
                                Thread.Sleep(100);
                            }
                            log.RemainingFilesSize -= bytesRead;
                            WriteLogs(log);
                            destinationStream.Write(buffer, 0, bytesRead);
                            bytesCopied += bytesRead;

                            long percent = bytesCopied * 100 / fileLength;

                            workerLocal.ReportProgress((int)percent, $"{percent}%");
                            Thread.Sleep(10);
                        }
                    }
                }
            }

            else
            {
                DateTime sourceModifiedTime = File.GetLastWriteTime(fileInfo.DirectoryName);
                DateTime destinationModifiedTime = File.Exists(destination) ? File.GetLastWriteTime(destination) : DateTime.MinValue;
                if (sourceModifiedTime > destinationModifiedTime)
                {
                    using (FileStream sourceStream = fileInfo.OpenRead())
                    {
                        using (FileStream destinationStream = File.Create(destination))
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead;

                            log.RemainingFiles -= 1;
                            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                CheckBusinessSoftware();

                                while (Pause)
                                {
                                    Thread.Sleep(100);
                                }
                                log.RemainingFilesSize -= bytesRead;
                                WriteLogs(log);
                                destinationStream.Write(buffer, 0, bytesRead);
                                bytesCopied += bytesRead;

                                long percent = bytesCopied * 100 / fileLength;

                                workerLocal.ReportProgress((int)percent, $"{percent}%");
                            }
                        }
                    }
                }
                else
                {
                    log.RemainingFiles -= 0;
                    log.RemainingFilesSize -= 0;
                    WriteLogs(log);
                }
                log.DestinationRemainingFile = null;
                log.SourceRemainingFile = null;
                WriteLogs(log);
            }
        }

        #endregion

        #region GetTotalNumberOfFilesInDirectory

        private static int GetTotalNumberOfFilesInDirectory(string directory)
        {
            int totalFiles = Directory.GetFiles(directory).Length;

            string[] directories = Directory.GetDirectories(directory);

            foreach (string subDirectory in directories)
            {
                totalFiles += GetTotalNumberOfFilesInDirectory(subDirectory);
            }

            return totalFiles;
        }

        #endregion

        #region CopyDirectory

        private void CopyDirectory(string sourceDirectory, string destinationDirectory, ref int copiedFiles, int totalFiles, LogsModel log, object sender, BackUpType backUpType)
        {
            string[] files = Directory.GetFiles(sourceDirectory);
            string[] directories = Directory.GetDirectories(sourceDirectory);
            List<string> highPriorityFiles = new List<string>();
            List<string> lowPriorityFiles = new List<string>();
            var workerLocal = sender as BackgroundWorker;
            workerLocal.ReportProgress(0, string.Format("0"));

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            if (backUpType == BackUpType.FULL)
            {

                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        FileInfo fileInfo = new FileInfo(file);
                        if (UserConst.ExtensionFilePrioritary.Contains(fileInfo.Extension))
                        {
                            highPriorityFiles.Add(file);
                        }
                        else
                        {
                            lowPriorityFiles.Add(file);
                        }
                    }
                }
                foreach (string file in highPriorityFiles)
                {

                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = destinationDirectory + "\\" + Path.GetFileName(file);
                        FileInfo fileInfo = new FileInfo(file);
                        if (isEncrypt && UserConst.EncryptExtension.Contains(fileInfo.Extension))
                        {
                            CheckBusinessSoftware();
                            
                            while (Pause)
                            {
                                Thread.Sleep(100);
                            }
                            destFile = destinationDirectory + "\\" + Path.GetFileNameWithoutExtension(file) + "Encrypt" + Path.GetExtension(file);
                            if (UserConst.MaxSizeFile <= fileInfo.Length)
                            {
                                MaxSizeFile.Wait();
                                Encrypt("ENCRYPT", file, destFile);
                                MaxSizeFile.Release();
                            }
                            else
                            {
                                Encrypt("ENCRYPT", file, destFile);
                            }
                            copiedFiles++;
                            log.RemainingFilesSize -= fileInfo.Length;
                            log.RemainingFiles -= 1;

                            int progressPercentage = copiedFiles * 100 / totalFiles;

                            workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                            Thread.Sleep(50);
                        }
                        else
                        {
                            CheckBusinessSoftware();
                            
                            while (Pause)
                            {
                                Thread.Sleep(100);
                            }
                            long fileSize = new FileInfo(file).Length;
                            try
                            {
                                if (UserConst.MaxSizeFile <= fileInfo.Length)
                                {
                                    MaxSizeFile.Wait();
                                    File.Copy(file, destFile, true);
                                    MaxSizeFile.Release();
                                }
                                else
                                {
                                    File.Copy(file, destFile, true);
                                }
                            }
                            catch (Exception ex) 
                            {
                                Console.WriteLine(ex.Message);
                            }
                            copiedFiles++;
                            log.RemainingFilesSize -= fileSize;
                            log.RemainingFiles -= 1;

                            int progressPercentage = copiedFiles * 100 / totalFiles;

                            workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                            Thread.Sleep(50);
                        }
                    }

                }
                foreach (string file in lowPriorityFiles)
                {

                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = destinationDirectory + "\\" + Path.GetFileName(file);
                        FileInfo fileInfo = new FileInfo(file);
                        if (isEncrypt && UserConst.EncryptExtension.Contains(fileInfo.Extension))
                        {
                            CheckBusinessSoftware();
                            
                            while (Pause)
                            {
                                Thread.Sleep(100);
                            }
                            destFile = destinationDirectory + "\\" + Path.GetFileNameWithoutExtension(file) + "Encrypt" + Path.GetExtension(file);
                            if (UserConst.MaxSizeFile <= fileInfo.Length)
                            {
                                MaxSizeFile.Wait();
                                Encrypt("ENCRYPT", file, destFile);
                                MaxSizeFile.Release();
                            }
                            else
                            {
                                Encrypt("ENCRYPT", file, destFile);
                            }
                            copiedFiles++;
                            log.RemainingFilesSize -= fileInfo.Length;
                            log.RemainingFiles -= 1;

                            int progressPercentage = copiedFiles * 100 / totalFiles;

                            workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                            Thread.Sleep(10);
                        }
                        else
                        {
                            CheckBusinessSoftware();
                            
                            while (Pause)
                            {
                                Thread.Sleep(100);
                            }
                            long fileSize = new FileInfo(file).Length;
                            try
                            {
                                if (UserConst.MaxSizeFile <= fileInfo.Length)
                                {
                                    MaxSizeFile.Wait();
                                    File.Copy(file, destFile, true);
                                    MaxSizeFile.Release();
                                }
                                else
                                {
                                    File.Copy(file, destFile, true);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                            copiedFiles++;
                            log.RemainingFilesSize -= fileSize;
                            log.RemainingFiles -= 1;

                            int progressPercentage = copiedFiles * 100 / totalFiles;

                            workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                            Thread.Sleep(50);
                        }
                    }

                }
            }
            else
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        FileInfo fileInfo = new FileInfo(file);
                        if (UserConst.ExtensionFilePrioritary.Contains(fileInfo.Extension))
                        {
                            highPriorityFiles.Add(file);
                        }
                        else
                        {
                            lowPriorityFiles.Add(file);
                        }
                    }
                }
                foreach (string file in highPriorityFiles)
                {

                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = destinationDirectory + "\\" + Path.GetFileName(file);

                        if (File.Exists(file))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (isEncrypt && UserConst.EncryptExtension.Contains(fileInfo.Extension))
                            {
                                CheckBusinessSoftware();
                                
                                while (Pause)
                                {
                                    Thread.Sleep(100);
                                }
                                destFile = destinationDirectory + "\\" + Path.GetFileNameWithoutExtension(file) + "Encrypt" + Path.GetExtension(file);
                                if (UserConst.MaxSizeFile <= fileInfo.Length)
                                {
                                    MaxSizeFile.Wait();
                                    Encrypt("ENCRYPT", file, destFile);
                                    MaxSizeFile.Release();
                                }
                                else
                                {
                                    Encrypt("ENCRYPT", file, destFile);
                                }
                                copiedFiles++;
                                log.RemainingFilesSize -= fileInfo.Length;
                                log.RemainingFiles -= 1;

                                int progressPercentage = copiedFiles * 100 / totalFiles;

                                workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                                Thread.Sleep(10);
                            }
                            else
                            {
                                CheckBusinessSoftware();
                                
                                while (Pause)
                                {
                                    Thread.Sleep(100);
                                }
                                DateTime sourceModifiedTime = File.GetLastWriteTime(file);
                                DateTime destinationModifiedTime = File.Exists(destFile) ? File.GetLastWriteTime(destFile) : DateTime.MinValue;

                                if (sourceModifiedTime > destinationModifiedTime)
                                {
                                    long fileSize = new FileInfo(file).Length;
                                    try
                                    {
                                        if (UserConst.MaxSizeFile <= fileInfo.Length)
                                        {
                                            MaxSizeFile.Wait();
                                            File.Copy(file, destFile, true);
                                            MaxSizeFile.Release();
                                        }
                                        else
                                        {
                                            File.Copy(file, destFile, true);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }

                                    copiedFiles++;
                                    log.RemainingFilesSize -= fileSize;
                                    log.RemainingFiles -= 1;
                                }
                                else
                                {
                                    long fileSize = new FileInfo(file).Length;
                                    log.RemainingFilesSize -= fileSize;
                                    log.RemainingFiles -= 1;
                                    WriteLogs(log);
                                    copiedFiles++;
                                }

                                int progressPercentage = copiedFiles * 100 / totalFiles;

                                workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
                foreach (string file in lowPriorityFiles)
                {

                    if (File.Exists(file))
                    {
                        string fileName = Path.GetFileName(file);
                        string destFile = destinationDirectory + "\\" + Path.GetFileName(file);

                        if (File.Exists(file))
                        {
                            FileInfo fileInfo = new FileInfo(file);
                            if (isEncrypt && UserConst.EncryptExtension.Contains(fileInfo.Extension))
                            {
                                CheckBusinessSoftware();
                                
                                while (Pause)
                                {
                                    Thread.Sleep(100);
                                }
                                destFile = destinationDirectory + "\\" + Path.GetFileNameWithoutExtension(file) + "Encrypt" + Path.GetExtension(file);
                                if (UserConst.MaxSizeFile <= fileInfo.Length)
                                {
                                    MaxSizeFile.Wait();
                                    Encrypt("ENCRYPT", file, destFile);
                                    MaxSizeFile.Release();
                                }
                                else
                                {
                                    Encrypt("ENCRYPT", file, destFile);
                                }
                                copiedFiles++;
                                log.RemainingFilesSize -= fileInfo.Length;
                                log.RemainingFiles -= 1;

                                int progressPercentage = copiedFiles * 100 / totalFiles;

                                workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                                Thread.Sleep(10);
                            }
                            else
                            {
                                CheckBusinessSoftware();
                                
                                while (Pause)
                                {
                                    Thread.Sleep(100);
                                }
                                DateTime sourceModifiedTime = File.GetLastWriteTime(file);
                                DateTime destinationModifiedTime = File.Exists(destFile) ? File.GetLastWriteTime(destFile) : DateTime.MinValue;

                                if (sourceModifiedTime > destinationModifiedTime)
                                {
                                    long fileSize = new FileInfo(file).Length;
                                    try
                                    {
                                        if (UserConst.MaxSizeFile <= fileInfo.Length)
                                        {
                                            MaxSizeFile.Wait();
                                            File.Copy(file, destFile, true);
                                            MaxSizeFile.Release();
                                        }
                                        else
                                        {
                                            File.Copy(file, destFile, true);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }

                                    copiedFiles++;
                                    log.RemainingFilesSize -= fileSize;
                                    log.RemainingFiles -= 1;
                                }
                                else
                                {
                                    long fileSize = new FileInfo(file).Length;
                                    log.RemainingFilesSize -= fileSize;
                                    log.RemainingFiles -= 1;
                                    WriteLogs(log);
                                    copiedFiles++;
                                }

                                int progressPercentage = copiedFiles * 100 / totalFiles;

                                workerLocal.ReportProgress(progressPercentage, $"{copiedFiles}/{totalFiles} - {progressPercentage}%");
                                Thread.Sleep(10);
                            }
                        }
                    }
                }
            }
            foreach (string directory in directories)
            {
                string directoryName = Path.GetFileName(directory);
                string destDirectory = Path.Combine(destinationDirectory, directoryName);
                CopyDirectory(directory, destDirectory, ref copiedFiles, totalFiles, log, sender, backUpType);

                //System.Windows.Forms.MessageBox.Show(directory);
            }
            log.DestinationRemainingFile = null;
            log.SourceRemainingFile = null;
            WriteLogs(log);
        }

        #endregion

        #region GetJobs

        public void GetJobs()
        {
            switch (UserConst.UserFormat)
            {

                case Model.Enums.FormatEnum.XML:
                    if (File.Exists(paths.filePathsXml["save"]))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<JobModel>));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["save"]))
                        {
                            Jobs = xmlSerializer.Deserialize(textReader) as List<JobModel> ?? new List<JobModel>();
                        }
                    }
                    else
                    {
                        Jobs = new List<JobModel>();
                    }
                    break;
                default:
                    if (File.Exists(paths.filePathsJson["save"]))
                    {
                        string json = System.IO.File.ReadAllText(paths.filePathsJson["save"]);
                        Jobs = JsonConvert.DeserializeObject<List<JobModel>>(json) ?? new List<JobModel>();
                    }
                    else
                    {
                        Jobs = new List<JobModel>();
                    }
                    break;

            }
        }

        #endregion

        #region GetJob

        public JobModel? GetJob(int id)
        {
            GetJobs();
            try
            {
                return Jobs[id];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        #endregion

        #region GetLogs

        public void GetLogs()
        {
            switch (UserConst.UserFormat)
            {
                case FormatEnum.XML:
                    if (File.Exists(paths.filePathsXml["log"]))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LogsModel>));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["log"]))
                        {
                            try
                            {
                                Logs = xmlSerializer.Deserialize(textReader) as List<LogsModel> ?? new List<LogsModel>();
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        Logs = new List<LogsModel>();
                    }
                    break;
                default:
                    if (System.IO.File.Exists(paths.filePathsJson["log"]))
                    {
                        string json = System.IO.File.ReadAllText(paths.filePathsJson["log"]);
                        Logs = JsonConvert.DeserializeObject<List<LogsModel>>(json) ?? new List<LogsModel>();
                    }
                    else
                    {
                        Logs = new List<LogsModel>();
                    }
                    break;
            }
        }

        #endregion

        #region GetDailyLogs
        public void GetDailyLogs()
        {
            switch (UserConst.UserFormat)
            {
                case FormatEnum.XML:
                    if (File.Exists(paths.filePathsXml["dailylog"]))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<DailyLogsModel>));
                        using (TextReader textReader = new StreamReader(paths.filePathsXml["dailylog"]))
                        {
                            try
                            {
                                DailyLogs = xmlSerializer.Deserialize(textReader) as List<DailyLogsModel> ?? new List<DailyLogsModel>();
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        Logs = new List<LogsModel>();
                    }
                    break;
                default:
                    if (System.IO.File.Exists(paths.filePathsJson["dailylog"]))
                    {
                        string json = System.IO.File.ReadAllText(paths.filePathsJson["dailylog"]);
                        DailyLogs = JsonConvert.DeserializeObject<List<DailyLogsModel>>(json) ?? new List<DailyLogsModel>();
                    }
                    else
                    {
                        DailyLogs = new List<DailyLogsModel>();
                    }
                    break;
            }
        }
        #endregion

        #region WriteJobs

        public void WriteJobs(JobModel? jobsModel)
        {
            if (jobsModel != null)
            {
                List<JobModel> JobLogs = new List<JobModel>();
                switch (UserConst.UserFormat)
                {
                    case FormatEnum.XML:

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<JobModel>));
                        if (File.Exists(paths.filePathsXml["save"]))
                        {
                            using (TextReader textReader = new StreamReader(paths.filePathsXml["save"]))
                            {
                                try
                                {
                                    JobLogs = xmlSerializer.Deserialize(textReader) as List<JobModel> ?? new List<JobModel>();
                                }
                                catch
                                {
                                }
                            }
                        }
                        if (JobLogs.Count > 5)
                        {
                            JobLogs.Remove(JobLogs[0]);
                        }
                        foreach (JobModel job in JobLogs)
                        {
                            if (job.Name == jobsModel.Name)
                            {
                                JobLogs.Remove(job);
                            }
                        }
                        JobLogs.Add(jobsModel);
                        using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["save"]))
                        {
                            xmlSerializer.Serialize(textWriter, JobLogs);
                        }
                        break;

                    default:
                        if (File.Exists(paths.filePathsJson["save"]))
                        {
                            string json = File.ReadAllText(paths.filePathsJson["save"]);
                            JobLogs = JsonConvert.DeserializeObject<List<JobModel>>(json) ?? new List<JobModel>();
                        }
                        if (JobLogs.Count > 5)
                        {
                            JobLogs.Remove(JobLogs[0]);
                        }
                        foreach (JobModel job in JobLogs)
                        {
                            if (job.Name == jobsModel.Name)
                            {
                                JobLogs.Remove(job);
                            }
                        }
                        JobLogs.Add(jobsModel);
                        string outputJson = JsonConvert.SerializeObject(JobLogs, Formatting.Indented);
                        File.WriteAllText(paths.filePathsJson["save"], outputJson);
                        break;
                }
            }
            else
            {
                switch (UserConst.UserFormat)
                {
                    case FormatEnum.XML:

                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<JobModel>));
                        using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["save"]))
                        {
                            xmlSerializer.Serialize(textWriter, Jobs);
                        }
                        break;

                    default:
                        string outputJson = JsonConvert.SerializeObject(Jobs, Formatting.Indented);
                        File.WriteAllText(paths.filePathsJson["save"], outputJson);
                        break;
                }
            }
        }

        #endregion

        #region WriteLogs

        public void WriteLogs(LogsModel log)
        {
            GetLogs();
            if (Logs.Count > 0)
            {


                if (Logs.Last().Name == log.Name)
                {
                    log.id = Logs.Last().id;
                    Logs[log.id - 1] = log;
                }
                else
                {
                    log.id = Logs.Count + 1;
                    Logs.Add(log);
                }
            }
            else
            {
                log.id = 1;
                Logs.Add(log);
            }
            switch (UserConst.UserFormat)
            {
                case FormatEnum.XML:
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LogsModel>));


                    using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["log"]))
                    {
                        xmlSerializer.Serialize(textWriter, Logs);
                    }
                    break;
                default:
                    string fileName = paths.filePathsJson["log"];
                    bool fileAvailable = false;
                    // Attendre que le fichier soit disponible pour l'écriture
                    while (!fileAvailable)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(fileName, FileMode.Truncate, FileAccess.Write, FileShare.None))
                            {
                                if (fs.CanWrite)
                                {
                                    fileAvailable = true;
                                    string outputJson = JsonConvert.SerializeObject(Logs, Formatting.Indented);
                                    using (StreamWriter sw = new StreamWriter(fs))
                                    {
                                        // Écrire du texte dans le fichier
                                        sw.WriteLine(outputJson);
                                        sw.Flush();
                                    }
                                }
                            }
                        }
                        catch (IOException)
                        {
                            // Le fichier est utilisé par un autre processus
                            // Attendre un peu avant de réessayer
                            Thread.Sleep(100);
                        }
                    }
                    break;
            }
        }
        public void WriteDailyLogs(DailyLogsModel dailyLog)
        {
            GetDailyLogs();
            DailyLogs.Add(dailyLog);
            switch (UserConst.UserFormat)
            {
                case FormatEnum.XML:
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<LogsModel>));
                    using (TextWriter textWriter = new StreamWriter(paths.filePathsXml["dailylog"]))
                    {
                        xmlSerializer.Serialize(textWriter, DailyLogs);
                    }
                    break;
                default:
                    string outputJson = JsonConvert.SerializeObject(DailyLogs, Formatting.Indented);
                    File.WriteAllText(paths.filePathsJson["dailylog"], outputJson);
                    break;
            }
        }

        #endregion

        #region SaveJobs

        public void SaveJobs(int id, object sender)
        {
            try
            {
                Copy(Jobs[id].Name, Jobs[id].Source, Jobs[id].Destination, Jobs[id].IsEncrypt, sender, Jobs[id].Type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        #endregion

        #region SaveMultipleJobs

        public void SaveMultipleJobs(int id, object sender, ReadMultiWork multiWorkView)
        {
            try
            {
                this.multiView = multiWorkView;
                Copy(Jobs[id].Name, Jobs[id].Source, Jobs[id].Destination, Jobs[id].IsEncrypt, sender, Jobs[id].Type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        #endregion

        #region ModifyJobs

        public void ModifyJobs(int id, string name, string source, string destination, BackUpType backUpType, string typeOfSave, bool isEncrypt)
        {
            try
            {
                var job = Jobs[id];
                job.Name = name;
                job.Source = source;
                job.Destination = destination;
                job.Type = backUpType;
                job.TypeOfSave = typeOfSave;
                job.IsEncrypt = isEncrypt;
                Jobs[id] = job;
                WriteJobs(null);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        #endregion

        #region DeleteJob

        public void DeleteJob(int id)
        {
            try
            {
                var job = Jobs[id];
                Jobs.Remove(job);
                WriteJobs(null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion

        #region CheckBusinessSoftware

        private void CheckBusinessSoftware()
        {
            foreach (string processName in UserConst.BusinessSoftwareProcesses)
            {
                Process[] processes = Process.GetProcessesByName(processName);
                foreach (Process process in processes)
                {
                    MessageBox.Show("Logiciel métier détecté, veuillez le fermer afin de continuer la sauvegarde.");
                    process.WaitForExit();
                }
            }
        }

        #endregion

        #region Dispose and Destructor

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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.ResetColor();
            }

            _disposed = true;
        }

        public void DeleteJob()
        {
            Dispose();
        }

        ~JobViewModel()
        {
            DeleteJob();
        }

        #endregion

        #endregion
    }
}
