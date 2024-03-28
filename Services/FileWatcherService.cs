using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using FileSystemManagementApi.Models;
using FileSystemManagementApi.Data;

namespace FileSystemManagementApi.Services
{
    public class FileWatcherService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly FileSystemWatcher _fileWatcher;

        public FileWatcherService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;

            string sourceDirectory = _configuration["FilePaths:SourceDirectory"] ??
                                     throw new InvalidOperationException("Source directory configuration is missing.");

            // Initialize file watcher
            _fileWatcher = new FileSystemWatcher
            {
                Path = sourceDirectory,
                Filter = "*.pdf",
                EnableRaisingEvents = true
            };

            _fileWatcher.Created += async (sender, e) =>
            {
                // Delay to ensure the file is fully written and closed
                await Task.Delay(1000);

                // Process the file
                ProcessFile(e.FullPath);
            };

            // Start processing & file watching
            Start();
        }

        public void Start()
        {
            // Start watching for file creation
            _fileWatcher.EnableRaisingEvents = true;

            // Process Existing files
            string sourceDirectory = _configuration["FilePaths:SourceDirectory"] ??
                                     throw new InvalidOperationException("Source directory configuration is missing.");

            string[] existingFiles = Directory.GetFiles(sourceDirectory, "*.pdf");

            foreach (string filePath in existingFiles)
            {
                ProcessFile(filePath);
            }
        }


        private void ProcessFile(string filePath)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Move the file to the destination directory
                    string destinationDirectory = _configuration["FilePaths:DestinationDirectory"] ??
                                                  throw new InvalidOperationException(
                                                      "Destination directory configuration is missing.");

                    string fileName = Path.GetFileName(filePath);
                    string destinationPath = Path.Combine(destinationDirectory, fileName);

                    // Create a new db record containing file info
                    var file = new FileEntity
                    {
                        FileName = fileName,
                        FilePages = GetNumberOfPages(filePath),
                        CreationDate = DateTime.Now,
                        Status = "Imported" // At first import status is Imported
                    };

                    // Save the file
                    dbContext.Files.Add(file);
                    dbContext.SaveChanges();

                    // Move file to destination directory
                    File.Move(filePath, destinationPath);

                    // Update the status to "Moved" after successful move
                    file.Status = "Moved";
                    dbContext.SaveChanges();

                    // Print metadata information
                    // PrintMetadataInfo(destinationPath);

                    // Write to the console inside a lock
                    lock (Console.Out)
                    {
                        Console.WriteLine($"Moved {fileName} to {destinationDirectory}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file {filePath}: {ex.Message}");
            }
        }

        private void PrintMetadataInfo(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            lock (Console.Out)
            {
                Console.WriteLine($"File: {fileInfo.FullName}");
                Console.WriteLine($"Size: {fileInfo.Length} bytes");
                Console.WriteLine($"Creation Time: {fileInfo.CreationTime}");
                Console.WriteLine($"Last Access Time: {fileInfo.LastAccessTime}");
                Console.WriteLine($"Last Write Time: {fileInfo.LastWriteTime}");

                int numberOfPages = GetNumberOfPages(filePath);
                Console.WriteLine($"Number of Pages: {numberOfPages}");

                Console.WriteLine(new string('-', 40));
            }
        }

        private int GetNumberOfPages(string filePath)
        {
            using (PdfDocument document = PdfReader.Open(filePath, PdfDocumentOpenMode.ReadOnly))
            {
                return document.PageCount;
            }
        }
    }
}