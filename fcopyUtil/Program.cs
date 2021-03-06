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
            program.DirectoryLook(args[0]);
            program.FilesInDirectories();
            program.FindCopyByHash();
            Console.WriteLine("Готово!");
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
            pointer = 0;
        }
        void FindCopyByHash()
        {
            bool find = false;
            for(int i = 0; i < files.Count; i++)
            {
                string hash1 = program.Hash(files[i]);
                for (int j = 0; j < files.Count; j++)
                {
                    //Не проверяем файл с самим собой
                    if (j == i) continue;
                    string hash2 = program.Hash(files[j]);
                    //Если хеши совпали, то это копии
                    if (hash1 == hash2 && !copyIndexes.Contains(j))
                    {
                        copyIndexes.Add(j);
                        find = true;
                    }
                }
                if (find)
                {
                    //Выводим найденные копии
                    Console.WriteLine("Исходный файл {0}", files[i]);
                    for(int index = pointer; index < copyIndexes.Count; index++)
                    {
                        Console.WriteLine("\t\tФайл копия {0}", files[copyIndexes[index]]);
                    }
                    Console.WriteLine();
                    copyIndexes.Add(i);
                    pointer = copyIndexes.Count;
                    find = false;
                }
            }
        }
        string Hash(string path)
        {
            string hash = "non";
            SHA256 hashSHA256 = SHA256.Create();
            byte[] hashBytes;
            //Байты файла
            using (FileStream stream = File.OpenRead(path))
            {
                //Байты хеша SHA256
                hashBytes = hashSHA256.ComputeHash(stream);
            }
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
