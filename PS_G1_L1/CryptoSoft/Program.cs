// See https://aka.ms/new-console-template for more information
using CryptoSoft;
using System;

namespace CryptoSoft
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            //Verifier les arguments
            if (args[0] == "ENCRYPT")
            {
                Console.WriteLine("Encryptage");

                Console.WriteLine($"Argument 1 (Opération) : {args[0]}");
                Console.WriteLine($"Argument 2 (Source) : {args[1]}");
                Console.WriteLine($"Argument 3 (Destination) : {args[2]}");
                Console.WriteLine($"Argument 4 (Clé) : {args[3]}");

                MessageCrypt message = new MessageCrypt(args[3], args[1], args[2]);
                message.EncryptFile(args[1], args[2]);
                Console.WriteLine("Opération faite");
            }
            else if (args[0] == "DECRYPT")
            {
                Console.WriteLine("Decryptage");
                MessageCrypt message = new MessageCrypt(args[1], args[2], args[3]);

                message.Decrypt(args[2], args[3]);
                Console.WriteLine("Opération faite");
            }
            else
            {
                Console.WriteLine($"Error arguments : {args[0]} not exist");

                Console.WriteLine("\nArguments :");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"Argument num : {i} | Value : {args[i]}");
                }

                Console.WriteLine($"Operation : {args[0]} doesn't exist.");
            }
        }
    }
}