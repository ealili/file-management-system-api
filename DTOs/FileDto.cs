using System.ComponentModel.DataAnnotations;

namespace FileSystemManagementApi.DTOs;

public class FileDto
{
    [Required] public string Status { get; set; } = string.Empty;
}