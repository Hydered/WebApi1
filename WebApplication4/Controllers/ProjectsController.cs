using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class ProjectsController : ApiController
    {
        [HttpGet]
        [Route("api/projects")]
        public IHttpActionResult GetAll()
        {
            return Ok(AppData.Projects);
        }

        [HttpGet]
        [Route("api/projects/{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            var project = AppData.Projects.FirstOrDefault(item => item.Id == id);
            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Проект не найден");
            }

            return Ok(project);
        }

        [HttpPost]
        [Route("api/projects")]
        public IHttpActionResult Create(CreateProjectRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Owner))
            {
                return BadRequest("Поля Name и Owner обязательны");
            }

            var name = request.Name.Trim();
            if (AppData.Projects.Any(existingProject => string.Equals(existingProject.Name, name, System.StringComparison.OrdinalIgnoreCase)))
            {
                return Content(HttpStatusCode.Conflict, "Проект с таким именем уже существует");
            }

            var project = new Project
            {
                Id = AppData.Projects.Count == 0 ? 1 : AppData.Projects.Max(item => item.Id) + 1,
                Name = name,
                Owner = request.Owner.Trim(),
                IsArchived = false
            };
            AppData.Projects.Add(project);

            return Content(HttpStatusCode.Created, project);
        }

        [HttpPut]
        [Route("api/projects/{id:int}/archive")]
        public IHttpActionResult Archive(int id)
        {
            var project = AppData.Projects.FirstOrDefault(item => item.Id == id);
            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Проект не найден");
            }

            if (project.IsArchived)
            {
                return Content(HttpStatusCode.Conflict, "Проект уже архивирован");
            }

            project.IsArchived = true;
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/projects/{id:int}/stats")]
        public IHttpActionResult GetStats(int id)
        {
            var project = AppData.Projects.FirstOrDefault(item => item.Id == id);
            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Проект не найден");
            }

            var tasks = AppData.Tasks.Where(task => task.ProjectId == id).ToList();
            var stats = new
            {
                ProjectId = project.Id,
                ProjectName = project.Name,
                TotalTasks = tasks.Count,
                NewTasks = tasks.Count(task => task.Status == TaskStatuses.New),
                InProgressTasks = tasks.Count(task => task.Status == TaskStatuses.InProgress),
                DoneTasks = tasks.Count(task => task.Status == TaskStatuses.Done),
                CancelledTasks = tasks.Count(task => task.Status == TaskStatuses.Cancelled),
                OverdueTasks = tasks.Count(task => task.DueDate.HasValue && task.DueDate.Value.Date < System.DateTime.Today && task.Status != TaskStatuses.Done && task.Status != TaskStatuses.Cancelled)
            };

            return Ok(stats);
        }
    }
}