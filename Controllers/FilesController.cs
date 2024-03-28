using Microsoft.AspNetCore.Mvc;
using FileSystemManagementApi.DTOs;
using FileSystemManagementApi.Services.Interfaces;
using FileSystemManagementApi.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace FileSystemManagementApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = UserRoles.Admin)]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await _fileService.GetAllAsync();
        return Ok(files);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFileStatus(int id, [FromBody] FileDto updatedFileStatus)
    {
        var existingFile = await _fileService.GetByIdAsync(id);

        if (existingFile == null)
        {
            return NotFound();
        }

        existingFile.Status = updatedFileStatus.Status;

        await _fileService.UpdateAsync(existingFile);
        return NoContent();
    }
}