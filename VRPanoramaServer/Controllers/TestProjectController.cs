using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Net;
using System.Security.Claims;
using VRPanoramaServer.Database;

namespace VRPanoramaServer.Controllers
{
    //[Authorize]
    //[ApiController]
    //[Route("[controller]")]

    //public class TestProjectController : ControllerBase
    //{
    //    private readonly ApplicationDbContext _context;

    //    public TestProjectController(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    private async Task<User> GetUser(bool includeProjects = false)
    //    {
    //        string userId = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    //        if (includeProjects) return await _context.Users.Include(u => u.projects).FirstAsync(u => u.Id == userId);
    //        return await _context.Users.FindAsync(userId) ?? throw new Exception("Invalid user");
    //    }

    //    [HttpGet("GetProjects")]
    //    public async Task<IEnumerable<Project>> GetProjects()
    //    {
    //        var idk = (await GetUser(true)).projects;
    //        return (await GetUser(true)).projects ?? [];
    //    }
    //    //[HttpGet("GetProjectById/{id}")]
    //    //public ActionResult<Project> GetProjectById(Guid id)
    //    //{
    //    //    //var projects = (await GetUser()).projects ?? [];
    //    //    //return projects.ElementAt(id);
    //    //    return (Project)(_context.Find(typeof(Project), id) ?? throw new Exception($"No project with id {id}"));
    //    //}

    //    [HttpPost]
    //    [ProducesResponseType(StatusCodes.Status201Created)]
    //    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    //    public async Task<ActionResult<Project>> CreateProject(string projectName)
    //    {
    //        User user = await GetUser();
    //        Project internalProject = new()
    //        {
    //            User = user,
    //            Name = projectName,
    //            InternalName = "interN:" + projectName,
    //            UploadTime = DateTime.UtcNow,
    //            LastUpdateTime = DateTime.UtcNow
    //        };
    //        _context.Add<Project>(internalProject);
    //        _context.SaveChanges();
    //        _context.Update(user);

    //        return Created();
    //    }
    //}
}
