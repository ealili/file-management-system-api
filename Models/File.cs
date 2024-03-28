using System.ComponentModel.DataAnnotations;

namespace FileSystemManagementApi.Models;

public class FileEntity
{
    [Key]
    public int Id { get; set; }
    
    public required string FileName { get; set; }
    public required int FilePages { get; set; }
    public required DateTime CreationDate { get; set; }
    public required string Status { get; set; }
}