namespace FileSystemManagementApi.Exceptions.File;

public class FileNotFoundException : NotFoundException
{
    public FileNotFoundException() : base("File not found.")
    {
    }
}