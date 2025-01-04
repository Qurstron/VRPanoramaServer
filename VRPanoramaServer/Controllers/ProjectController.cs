using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using System.Security.Claims;
using System.Text.Json;
using VRPanoramaServer.Database;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VRPanoramaServer.Controllers
{
    public class FileUploadRequest
    {
        public required IFormFile File { get; set; }
        public string? ExistingProjectId { get; set; }
    }


    [Route("projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProjectController> _logger;
        private static string projectPath;
        private static string thumbnailPath;

        public ProjectController(ApplicationDbContext context, ILogger<ProjectController> logger)
        {
            _context = context;
            _logger = logger;

        }
        public static void Init()
        {
            string basePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "files");
            // A quick hack for linux
            basePath = basePath.Remove(0, 5); // removes "/app/"
            projectPath = Path.Combine(basePath, "projects");
            thumbnailPath = Path.Combine(basePath, "thumbnails");
            ValidateDir(projectPath);
            ValidateDir(thumbnailPath);
        }

        private async Task<User> GetUser(bool includeProjects = false)
        {
            string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            if (includeProjects) return await _context.Users.Include(u => u.projects).FirstAsync(u => u.Id == userId);
            return await _context.Users.FindAsync(userId) ?? throw new Exception("Invalid user");
        }
        private async Task<Project> GetProject(string id)
        {
            return (await _context.Projects.FindAsync(new Guid(id))) ?? throw new Exception("No matching project id found");
        }

        private static void ValidateDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Console.WriteLine("No directory found! Creating one at " + projectPath);
            }
        }
        private async void ReciveFile(IFormFile file, User user, Project? project = null)
        {
            string fileName;
            string filePath;
            string configName = "";

            if (project != null) fileName = project.InternalName;
            else fileName = Guid.NewGuid().ToString();
            filePath = Path.Combine(projectPath, fileName + ".zip");

            await file.CopyToAsync(new FileStream(filePath, FileMode.Create));

            using ZipArchive zip = ZipFile.Open(filePath, ZipArchiveMode.Read);
            if (!zip.Entries.Any(e => e.Name == "config.json"))
            {
                System.IO.File.Delete(filePath);
                throw new Exception("No config.json file in zip");
            }
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                if (entry.Name == "config.json")
                {
                    StreamReader streamReader = new(entry.Open());
                    Config config = JsonSerializer.Deserialize<Config>(streamReader.ReadToEnd()) ?? throw new Exception("unable to deserialize config");
                    configName = config.name;
                }
                else if (entry.Name == "thumbnail.png")
                {
                    entry.ExtractToFile(Path.Combine(thumbnailPath, fileName + ".png"));
                }
            }

            if (project != null)
            {
                project.Name = configName;
                project.LastUpdateTime = DateTime.Now;
                _context.Update<Project>(project);
            }
            else
            {
                Project internalProject = new()
                {
                    User = user,
                    Name = configName,
                    InternalName = fileName,
                    UploadTime = DateTime.UtcNow,
                    LastUpdateTime = DateTime.UtcNow
                };
                _context.Add<Project>(internalProject);
            }
            _context.SaveChanges();
        }
        private string GetZipPath(Project project)
        {
            ValidateDir(projectPath);
            return Path.Combine(projectPath, project.InternalName + ".zip");
        }
        private string GetThumbnailPath(Project project)
        {
            ValidateDir(thumbnailPath);
            return Path.Combine(thumbnailPath, project.InternalName + ".png");
        }

        [HttpGet("overview")]
        public IEnumerable<PanoramaEntry> Get()
        {
            List<PanoramaEntry> result = [];

            foreach (Project project in _context.Projects)
            {
                try
                {
                    result.Add(new()
                    {
                        Id = project.ProjectID.ToString(),
                        Name = project.Name,
                        Size = new FileInfo(GetZipPath(project)).Length
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Unable to get info on project {p}, {ex}",
                        project.ProjectID.ToString(),
                        ex.Message);
                }
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(string id)
        {
            try
            {
                string filePath = GetZipPath(await GetProject(id));
                if (!System.IO.File.Exists(filePath)) return NotFound();
                return File(System.IO.File.OpenRead(filePath), "application/zip");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("thumbnails/{id}")]
        public async Task<IActionResult> GetImage(string id)
        {
            try
            {
                string filePath = GetThumbnailPath(await GetProject(id));
                if (!System.IO.File.Exists(filePath)) return NotFound();
                return File(System.IO.File.OpenRead(filePath), "image/jpeg");
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> PostProject([FromForm] FileUploadRequest model)
        {
            var file = model.File;
            if (file.Length <= 0) return BadRequest();
            if (Path.GetExtension(file.FileName) != ".zip") return BadRequest();

            try
            {
                User user = await GetUser();

                if (model.ExistingProjectId != null)
                {
                    Project existingProject = await GetProject(model.ExistingProjectId);
                    ReciveFile(file, user, existingProject);
                    return Accepted();
                }

                ReciveFile(file, user);
                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "pain");
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                User user = await GetUser(true);
                Project project = await GetProject(id);
                if (project.User != user) return Forbid();

                _context.Remove(project);
                _context.SaveChanges();
                System.IO.File.Delete(GetZipPath(project));
                System.IO.File.Delete(GetThumbnailPath(project));

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}
