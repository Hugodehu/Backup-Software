using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace CryptoSoft
{
    internal class MessageCrypt
    {
        public string SourceFilePath { get; set; }
        public string DestinationFilePath { get; set; }
        public string OriginalKey { get; set; }

        private readonly byte[] Key;

        public MessageCrypt(string key, string sourceFilePath = "", string destinationFilePath = "")
        {
            this.Key = Encoding.UTF8.GetBytes(key);

            if (sourceFilePath == "")
            {
                Console.WriteLine("Veuillez mettre le chemin du fichier source : ");
                this.SourceFilePath = Console.ReadLine();

                while (!System.IO.File.Exists(this.DestinationFilePath))
                {
                    Console.WriteLine("\n\nLe chemin que vous venez d'entrer n'existe pas.");
                    Console.WriteLine("\nVeuillez mettre le chemin du fichier source : ");
                    this.SourceFilePath = Console.ReadLine();
                }
            }
            if (destinationFilePath == "")
            {
                Console.WriteLine("Veuillez mettre le chemin du fichier de destination : ");
                this.DestinationFilePath = Console.ReadLine();
            }
        }

        public string GetKey() { return Key.ToString(); }

        public void EncryptFile(string sourceFilePath, string destinationFilePath)
        {
            this.SourceFilePath = sourceFilePath;
            this.DestinationFilePath = destinationFilePath;

            using (FileStream sourceStream = System.IO.File.OpenRead(sourceFilePath))
            {
                using (FileStream destinationStream = System.IO.File.Create(destinationFilePath))
                {
                    //Key index
                    int index = 0;

                    Console.Clear();
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(1,1);
                    Console.Write("Work in progress : " + destinationFilePath);

                    Console.SetCursorPosition(3, 1);

                    for (int i = 0; i < sourceStream.Length; i++)
                    {
                        byte sourceByte = (byte)sourceStream.ReadByte();
                        byte keyByte = Key[index];

                        destinationStream.WriteByte((byte)(sourceByte ^ keyByte));

                        index = (index + 1) % Key.Length;

                        Console.WriteLine($"Progress : {destinationStream.Position}/{sourceStream.Length} - {destinationStream.Position * 100 / sourceStream.Length}%");
                        Console.SetCursorPosition(3, 1);
                    }
                }
            }

            Process p = Process.Start("notepad.exe", destinationFilePath);
            if (p.HasExited)
            {
                p.Close();
            }
        }

        public void Decrypt(string sourceFilePath, string destinationFilePath)
        {
            this.SourceFilePath = sourceFilePath;
            this.DestinationFilePath = destinationFilePath;

            EncryptFile(sourceFilePath, destinationFilePath);
        }
    }
}
