using System;
using System.IO;


class FileSearcher
{
    public static void SearchFiles(string directoryPath, string fileName)
    {
        try
        {
            // Проверяем, существует ли заданный каталог
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Каталог не найден: " + directoryPath);
                return;
            }

            // Получаем все файлы в указанной директории
            var files = Directory.GetFiles(directoryPath, fileName, SearchOption.AllDirectories);

            if (files.Length == 0)
            {
                Console.WriteLine("Файлы не найдены.");
            }
            else
            {
                Console.WriteLine("Найденные файлы:");
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                }
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }
    class FileManager
    {
        static void Main(string[] args)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Текущая директория: " + currentDirectory);
                Console.WriteLine("Содержимое директории:");
                DisplayDirectoryContents(currentDirectory);

                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Перейти в директорию");
                Console.WriteLine("2. Копировать файл");
                Console.WriteLine("3. Переместить файл");
                Console.WriteLine("4. Удалить файл");
                Console.WriteLine("5. Вернуться в родительскую директорию");
                Console.WriteLine("6. Создать новую директорию");
                Console.WriteLine("7. Выйти");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Введите имя директории: ");
                        var dirName = Console.ReadLine();
                        string newDir = Path.Combine(currentDirectory, dirName);
                        if (Directory.Exists(newDir))
                        {
                            currentDirectory = newDir;
                        }
                        else
                        {
                            Console.WriteLine("Директория не найдена!");
                        }
                        break;

                    case "2":
                        Console.Write("Введите имя файла для копирования: ");
                        var fileToCopy = Console.ReadLine();
                        Console.Write("Введите путь назначения: ");
                        var destinationPathCopy = Console.ReadLine();
                        CopyFile(currentDirectory, fileToCopy, destinationPathCopy);
                        break;

                    case "3":
                        Console.Write("Введите имя файла для перемещения: ");
                        var fileToMove = Console.ReadLine();
                        Console.Write("Введите путь назначения: ");
                        var destinationPathMove = Console.ReadLine();
                        MoveFile(currentDirectory, fileToMove, destinationPathMove);
                        break;

                    case "4":
                        Console.Write("Введите имя файла для удаления: ");
                        var fileToDelete = Console.ReadLine();
                        DeleteFile(currentDirectory, fileToDelete);
                        break;

                    case "5":
                        currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                        break;

                    case "6":
                        Console.Write("Введите имя новой директории: ");
                        var newDirName = Console.ReadLine();
                        CreateDirectory(currentDirectory, newDirName);
                        break;

                    case "7":
                        return;


                    default:
                        Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                        break;
                }

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        static void DisplayDirectoryContents(string path)
        {
            try
            {
                var directories = Directory.GetDirectories(path);
                foreach (var dir in directories)
                {
                    Console.WriteLine("[DIR] " + Path.GetFileName(dir));
                }

                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    Console.WriteLine("[FILE] " + Path.GetFileName(file));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отображении содержимого: " + ex.Message);
            }
        }

        static void CreateDirectory(string currentDirectory, string dirName)
        {
            string newDirPath = Path.Combine(currentDirectory, dirName);
            try
            {
                Directory.CreateDirectory(newDirPath);
                Console.WriteLine("Директория успешно создана.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании директории: " + ex.Message);
            }
        }
        static void CopyFile(string currentDirectory, string fileName, string destinationDirectory)
        {
            string sourceFilePath = Path.Combine(currentDirectory, fileName);
            string destinationFilePath = Path.Combine(destinationDirectory, fileName);
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    Console.WriteLine("Файл не найден: " + sourceFilePath);
                    return;
                }

                File.Copy(sourceFilePath, destinationFilePath, true);
                Console.WriteLine("Файл успешно скопирован в: " + destinationFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при копировании файла: " + ex.Message);
            }
        }

        static void MoveFile(string currentDirectory, string fileName, string destinationDirectory)
        {
            string sourceFilePath = Path.Combine(currentDirectory, fileName);
            string destinationFilePath = Path.Combine(destinationDirectory, fileName);
            try
            {
                if (!File.Exists(sourceFilePath))
                {
                    Console.WriteLine("Файл не найден: " + sourceFilePath);
                    return;
                }

                File.Move(sourceFilePath, destinationFilePath);
                Console.WriteLine("Файл успешно перемещён в: " + destinationFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при перемещении файла: " + ex.Message);
            }
        }


        static void DeleteFile(string currentDirectory, string fileName)
        {
            string filePath = Path.Combine(currentDirectory, fileName);
            try
            {
                File.Delete(filePath);
                Console.WriteLine("Файл успешно удалён.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при удалении файла: " + ex.Message);
            }
        }
    }
}
