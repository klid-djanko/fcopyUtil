using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace fcopyUtil
{
    class Program
    {
        static Program program = new Program();
        List<string> directories = new List<string>();
        List<string> files = new List<string>();
        List<int> copyIndexes = new List<int>();
        int pointer = 0;
        static void Main(string[] args)
        {
            string tst = @"C:\Users\Herman\Desktop\test";
            program.DirectoryLook(tst);
            program.FilesInDirectories();
            program.FindCopyByHash();
            Console.WriteLine();
        }

        void DirectoryLook(string path)
        {
            //Получаем список директорий и запоминаем
            string[] dirs = Directory.GetDirectories(path);
            if (dirs.Length != 0)
            {
                foreach (string dir in dirs)
                    if (!directories.Contains(dir)) directories.Add(dir);
                //Переходим рекурсивно для каждой новой директории смотрим её дочерние
                for (int i = pointer; i < directories.Count; i++)
                {
                    //Запомним "положение" рекурсии в переменную pointer
                    pointer++;
                    DirectoryLook(directories[i]);
                    //Выход из рекурсии
                    if (pointer == directories.Count) break;
                }
            }
        }
        void FilesInDirectories()
        {
            foreach(string pth in directories)
            {
                string[] tmp = Directory.GetFiles(pth);
                foreach (string file in tmp) files.Add(file);
            }
        }
        void FindCopyByHash()
        {
            for(int i = 0; i < files.Count; i++)
            {
                string hash1 = program.Hash(files[i]);
                for (int j = 0; j < files.Count; j++)
                {
                    //Не проверяем файл с самим собой
                    if (j == i) continue;
                    string hash2 = program.Hash(files[j]);
                    //Если хеши совпали, то это копии
                    if (hash1 == hash2)
                    {
                        Console.WriteLine("Найдены копии:\r\nФайл {0}\r\nФайл {1}\r\n", files[i], files[j]);
                    }
                }
            }
        }
        string Hash(string path)
        {
            string hash = "non";
            //Байты файла
            byte[] fileBytes = File.ReadAllBytes(path);
            SHA256 hashSHA256 = SHA256.Create();
            //Байты хеша SHA256
            byte[] hashBytes = hashSHA256.ComputeHash(fileBytes);
            if (hashBytes != null) hash = "";
            //Байты хеша SHA256 в строковом виде
            foreach (byte bt in hashBytes)
            {
                hash += bt.ToString("X2");
            }
            return hash;
        }
    }
}
