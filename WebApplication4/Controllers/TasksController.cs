using System.Linq;
using System.Net;
using System.Web.Http;
using WebApplication4.Models;

namespace WebApplication4.Controllers
{
    public class TasksController : ApiController
    {
        [HttpGet]
        [Route("api/tasks")]
        public IHttpActionResult GetAll(string status = null, int? priority = null, int? projectId = null, string search = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Некорректные параметры запроса");
            }

            if (priority.HasValue && (priority.Value < 1 || priority.Value > 4))
            {
                return BadRequest("Приоритет должен быть от 1 до 4");
            }

            string normalizedStatus = null;
            if (status != null && !TaskStatuses.TryNormalize(status, out normalizedStatus))
            {
                return BadRequest("Неизвестный статус задачи");
            }

            var tasks = AppData.Tasks.AsEnumerable();
            if (normalizedStatus != null)
            {
                tasks = tasks.Where(task => task.Status == normalizedStatus);
            }

            if (priority.HasValue)
            {
                tasks = tasks.Where(task => task.Priority == priority.Value);
            }

            if (projectId.HasValue)
            {
                tasks = tasks.Where(task => task.ProjectId == projectId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                tasks = tasks.Where(task => task.Title.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) >= 0 || task.Description.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return Ok(tasks.ToList());
        }

        [HttpGet]
        [Route("api/tasks/{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            var task = AppData.Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return Content(HttpStatusCode.NotFound, "Задача не найдена");
            }

            return Ok(task);
        }

        [HttpGet]
        [Route("~/api/projects/{projectId:int}/tasks")]
        public IHttpActionResult GetByProject(int projectId)
        {
            if (!AppData.Projects.Any(project => project.Id == projectId))
            {
                return Content(HttpStatusCode.NotFound, "Проект не найден");
            }

            return Ok(AppData.Tasks.Where(task => task.ProjectId == projectId).ToList());
        }

        [HttpPost]
        [Route("api/tasks")]
        public IHttpActionResult Create(CreateTaskRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title обязателен");
            }

            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 10)
            {
                return BadRequest("Description должен содержать минимум 10 символов");
            }

            if (request.Priority < 1 || request.Priority > 4)
            {
                return BadRequest("Приоритет должен быть от 1 до 4");
            }

            if (request.DueDate.HasValue && request.DueDate.Value.Date < System.DateTime.Today)
            {
                return BadRequest("Срок не может быть раньше текущей даты");
            }

            var project = AppData.Projects.FirstOrDefault(item => item.Id == request.ProjectId);
            if (project == null)
            {
                return Content(HttpStatusCode.NotFound, "Проект не найден");
            }

            if (project.IsArchived)
            {
                return Content(HttpStatusCode.Conflict, "Нельзя добавить задачу в архивированный проект");
            }

            var task = new TaskItem
            {
                Id = AppData.Tasks.Count == 0 ? 1 : AppData.Tasks.Max(item => item.Id) + 1,
                ProjectId = request.ProjectId,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Status = TaskStatuses.New,
                Priority = request.Priority,
                CreatedAt = System.DateTime.Now,
                DueDate = request.DueDate
            };
            AppData.Tasks.Add(task);

            return Content(HttpStatusCode.Created, task);
        }

        [HttpPut]
        [Route("api/tasks/{id:int}")]
        public IHttpActionResult Update(int id, UpdateTaskRequest request)
        {
            var task = AppData.Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return Content(HttpStatusCode.NotFound, "Задача не найдена");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Title))
            {
                return BadRequest("Title обязателен");
            }

            if (string.IsNullOrWhiteSpace(request.Description) || request.Description.Trim().Length < 10)
            {
                return BadRequest("Description должен содержать минимум 10 символов");
            }

            if (request.Priority < 1 || request.Priority > 4)
            {
                return BadRequest("Приоритет должен быть от 1 до 4");
            }

            if (request.DueDate.HasValue && request.DueDate.Value.Date < System.DateTime.Today)
            {
                return BadRequest("Срок не может быть раньше текущей даты");
            }

            task.Title = request.Title.Trim();
            task.Description = request.Description.Trim();
            task.Priority = request.Priority;
            task.DueDate = request.DueDate;

            return Ok(task);
        }

        [HttpPut]
        [Route("api/tasks/{id:int}/status")]
        public IHttpActionResult UpdateStatus(int id, UpdateTaskStatusRequest request)
        {
            var task = AppData.Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return Content(HttpStatusCode.NotFound, "Задача не найдена");
            }

            string requestedStatus;
            if (request == null || !TaskStatuses.TryNormalize(request.Status, out requestedStatus))
            {
                return BadRequest("Неизвестный статус задачи");
            }

            var allowed = task.Status == TaskStatuses.New && (requestedStatus == TaskStatuses.InProgress || requestedStatus == TaskStatuses.Cancelled)
                || task.Status == TaskStatuses.InProgress && (requestedStatus == TaskStatuses.Done || requestedStatus == TaskStatuses.Cancelled);

            if (!allowed)
            {
                return Content(HttpStatusCode.Conflict, "Переход в указанный статус запрещен");
            }

            task.Status = requestedStatus;
            return Ok(task);
        }

        [HttpDelete]
        [Route("api/tasks/{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var task = AppData.Tasks.FirstOrDefault(item => item.Id == id);
            if (task == null)
            {
                return Content(HttpStatusCode.NotFound, "Задача не найдена");
            }

            if (task.Status == TaskStatuses.InProgress)
            {
                return Content(HttpStatusCode.Conflict, "Нельзя удалить задачу, которая находится в работе");
            }

            AppData.Tasks.Remove(task);
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [Route("api/tasks/overdue")]
        public IHttpActionResult GetOverdue()
        {
            var tasks = AppData.Tasks.Where(task => task.DueDate.HasValue && task.DueDate.Value.Date < System.DateTime.Today && task.Status != TaskStatuses.Done && task.Status != TaskStatuses.Cancelled).ToList();
            return Ok(tasks);
        }
    }
}