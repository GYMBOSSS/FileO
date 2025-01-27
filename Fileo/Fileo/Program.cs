using System;
using System.IO;
using System.IO.Compression;

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
            Console.WriteLine("7. Найти файлы по имени");
            Console.WriteLine("8. Переименовать файл или директорию");
            Console.WriteLine("9. Просмотреть свойства файла/директории");
            Console.WriteLine("10. Создать и редактировать текстовый файл");
            Console.WriteLine("11. Найти файлы по расширению");
            Console.WriteLine("12. Архивировать файлы");
            Console.WriteLine("13. Разархивировать архив");
            Console.WriteLine("14. Выйти");
            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    GoToDirectory(ref currentDirectory);
                    break;
                case "2":
                    CopyFile(currentDirectory);
                    break;
                case "3":
                    MoveFile(currentDirectory);
                    break;
                case "4":
                    DeleteFile(currentDirectory);
                    break;
                case "5":
                    currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                    break;
                case "6":
                    CreateDirectory(currentDirectory);
                    break;
                case "7":
                    SearchFiles(currentDirectory);
                    break;
                case "8":
                    RenameFileOrDirectory(currentDirectory);
                    break;
                case "9":
                    DisplayFileInfo(currentDirectory);
                    break;
                case "10":
                    CreateAndEditTextFile(currentDirectory);
                    break;
                case "11":
                    SearchFilesByExtension(currentDirectory);
                    break;
                case "12":
                    ArchiveFiles(currentDirectory);
                    break;
                case "13":
                    ExtractArchive(currentDirectory);
                    break;
                case "14":
                    return;

                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }

    // Остальной код остается без изменений

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
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при отображении содержимого: " + ex.Message);
        }
    }

    static void CreateDirectory(string currentDirectory, string dirName = null)
    {
        if (dirName == null)
        {
            Console.Write("Введите имя новой директории: ");
            dirName = Console.ReadLine();
        }
        string newDirPath = Path.Combine(currentDirectory, dirName);
        try
        {
            Directory.CreateDirectory(newDirPath);
            Console.WriteLine("Директория успешно создана.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при создании директории: " + ex.Message);
        }
    }

    static void GoToDirectory(ref string currentDirectory)
    {
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
    }

    static void CopyFile(string currentDirectory)
    {
        Console.Write("Введите имя файла для копирования: ");
        var fileToCopy = Console.ReadLine();
        Console.Write("Введите путь назначения: ");
        var destinationPathCopy = Console.ReadLine();
        string sourceFilePath = Path.Combine(currentDirectory, fileToCopy);
        string destinationFilePath = Path.Combine(destinationPathCopy, fileToCopy);
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("Файл не найден: " + sourceFilePath);
                return;
            }
            if (!Directory.Exists(destinationPathCopy))
            {
                Console.WriteLine("Путь назначения не существует: " + destinationPathCopy);
                return;
            }
            File.Copy(sourceFilePath, destinationFilePath, true);
            Console.WriteLine("Файл успешно скопирован в: " + destinationFilePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка ввода-вывода: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при копировании файла: " + ex.Message);
        }
    }

    static void MoveFile(string currentDirectory)
    {
        Console.Write("Введите имя файла для перемещения: ");
        var fileToMove = Console.ReadLine();
        Console.Write("Введите путь назначения: ");
        var destinationPathMove = Console.ReadLine();
        string sourceFilePath = Path.Combine(currentDirectory, fileToMove);
        string destinationFilePath = Path.Combine(destinationPathMove, fileToMove);
        try
        {
            if (!File.Exists(sourceFilePath))
            {
                Console.WriteLine("Файл не найден: " + sourceFilePath);
                return;
            }
            if (!Directory.Exists(destinationPathMove))
            {
                Console.WriteLine("Путь назначения не существует: " + destinationPathMove);
                return;
            }
            File.Move(sourceFilePath, destinationFilePath);
            Console.WriteLine("Файл успешно перемещён в: " + destinationFilePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка ввода-вывода: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при перемещении файла: " + ex.Message);
        }
    }

    static void DeleteFile(string currentDirectory, string fileName = null)
    {
        if (fileName == null)
        {
            Console.Write("Введите имя файла для удаления: ");
            fileName = Console.ReadLine();
        }
        string filePath = Path.Combine(currentDirectory, fileName);
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден: " + filePath);
                return;
            }
            File.Delete(filePath);
            Console.WriteLine("Файл успешно удалён.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка ввода-вывода: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при удалении файла: " + ex.Message);
        }
    }

    static void SearchFiles(string currentDirectory)
    {
        Console.Write("Введите имя файла для поиска: ");
        string fileName = Console.ReadLine();

        try
        {
            if (!Directory.Exists(currentDirectory))
            {
                Console.WriteLine("Каталог не найден: " + currentDirectory);
                return;
            }
            var files = Directory.GetFiles(currentDirectory, fileName, SearchOption.AllDirectories);
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
            Console.WriteLine("Попробуйте запустить программу от имени администратора.");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine("Директория не найдена: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }

    static void RenameFileOrDirectory(string currentDirectory)
    {
        Console.Write("Введите имя файла или директории для переименования: ");
        var oldName = Console.ReadLine();
        Console.Write("Введите новое имя: ");
        var newName = Console.ReadLine();

        string oldPath = Path.Combine(currentDirectory, oldName);
        string newPath = Path.Combine(currentDirectory, newName);

        try
        {
            if (File.Exists(oldPath))
            {
                File.Move(oldPath, newPath);
                Console.WriteLine("Файл успешно переименован.");
            }
            else if (Directory.Exists(oldPath))
            {
                Directory.Move(oldPath, newPath);
                Console.WriteLine("Директория успешно переименована.");
            }
            else
            {
                Console.WriteLine("Файл или директория не найдены.");
            }
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка ввода-вывода: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }

    static void DisplayFileInfo(string currentDirectory)
    {
        Console.Write("Введите имя файла или директории: ");
        var name = Console.ReadLine();
        string path = Path.Combine(currentDirectory, name);

        try
        {
            if (File.Exists(path))
            {
                var fileInfo = new FileInfo(path);
                Console.WriteLine($"Имя файла: {fileInfo.Name}");
                Console.WriteLine($"Размер файла: {fileInfo.Length} байт");
                Console.WriteLine($"Дата создания: {fileInfo.CreationTime}");
                Console.WriteLine($"Дата последнего изменения: {fileInfo.LastWriteTime}");
            }
            else if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                Console.WriteLine($"Имя директории: {dirInfo.Name}");
                Console.WriteLine($"Количество файлов: {dirInfo.GetFiles().Length}");
                Console.WriteLine($"Дата создания: {dirInfo.CreationTime}");
                Console.WriteLine($"Дата последнего изменения: {dirInfo.LastWriteTime}");
            }
            else
            {
                Console.WriteLine("Файл или директория не найдены.");
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

    static void CreateAndEditTextFile(string currentDirectory)
    {
        Console.Write("Введите имя нового текстового файла: ");
        var fileName = Console.ReadLine();
        string filePath = Path.Combine(currentDirectory, fileName);

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                Console.WriteLine("Введите текст для записи в файл (введите 'exit' для завершения):");
                while (true)
                {
                    string line = Console.ReadLine();
                    if (line.ToLower() == "exit")
                        break;
                    writer.WriteLine(line);
                }
            }
            Console.WriteLine("Текст успешно записан в файл.");

            Console.WriteLine("Чтение содержимого файла:");
            using (StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string content = reader.ReadToEnd();
                Console.WriteLine(content);
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

    static void SearchFilesByExtension(string currentDirectory)
    {
        Console.Write("Введите расширение файла для поиска (например, .txt): ");
        string extension = Console.ReadLine();

        try
        {
            if (!Directory.Exists(currentDirectory))
            {
                Console.WriteLine("Каталог не найден: " + currentDirectory);
                return;
            }
            var files = Directory.GetFiles(currentDirectory, "*" + extension, SearchOption.AllDirectories);
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
            Console.WriteLine("Попробуйте запустить программу от имени администратора.");
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine("Директория не найдена: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }

    static void ArchiveFiles(string currentDirectory)
    {
        Console.Write("Введите имя архива (без расширения): ");
        string archiveName = Console.ReadLine();
        string archivePath = Path.Combine(currentDirectory, archiveName + ".zip");

        Console.Write("Введите путь к файлам для архивации: ");
        string sourcePath = Console.ReadLine();

        try
        {
            ZipFile.CreateFromDirectory(sourcePath, archivePath);
            Console.WriteLine("Файлы успешно архивированы в: " + archivePath);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine("Директория не найдена: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }

    static void ExtractArchive(string currentDirectory)
    {
        Console.Write("Введите путь к архиву: ");
        string archivePath = Console.ReadLine();

        Console.Write("Введите путь для извлечения файлов: ");
        string extractPath = Console.ReadLine();

        try
        {
            ZipFile.ExtractToDirectory(archivePath, extractPath);
            Console.WriteLine("Архив успешно извлечен в: " + extractPath);
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Ошибка доступа: " + ex.Message);
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.WriteLine("Директория не найдена: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
    }
}