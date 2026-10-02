using System;
using System.Collections.Generic;

namespace WebApplication4.Models
{
    public static class AppData
    {
        static AppData()
        {
            Projects = new List<Project>
            {
                new Project { Id = 1, Name = "Корпоративный портал", Owner = "Иванов И.И." },
                new Project { Id = 2, Name = "Мобильное приложение", Owner = "Петрова А.С." },
                new Project { Id = 3, Name = "Внутренняя аналитика", Owner = "Сидоров П.В." }
            };

            Tasks = new List<TaskItem>
            {
                new TaskItem { Id = 1, ProjectId = 1, Title = "Подготовить структуру сайта", Description = "Согласовать основные разделы и навигацию портала", Status = "Done", Priority = 2, CreatedAt = DateTime.Now.AddDays(-12), DueDate = DateTime.Today.AddDays(-5) },
                new TaskItem { Id = 2, ProjectId = 1, Title = "Добавить авторизацию", Description = "Реализовать вход и проверку учетных данных", Status = "InProgress", Priority = 4, CreatedAt = DateTime.Now.AddDays(-8), DueDate = DateTime.Today.AddDays(-1) },
                new TaskItem { Id = 3, ProjectId = 1, Title = "Настроить профиль пользователя", Description = "Добавить редактирование контактной информации", Status = "New", Priority = 2, CreatedAt = DateTime.Now.AddDays(-2), DueDate = DateTime.Today.AddDays(5) },
                new TaskItem { Id = 4, ProjectId = 2, Title = "Создать экран входа", Description = "Сверстать форму авторизации для приложения", Status = "New", Priority = 3, CreatedAt = DateTime.Now.AddDays(-1), DueDate = DateTime.Today.AddDays(3) },
                new TaskItem { Id = 5, ProjectId = 2, Title = "Проверить навигацию", Description = "Проверить переходы между основными экранами", Status = "Cancelled", Priority = 1, CreatedAt = DateTime.Now.AddDays(-6), DueDate = DateTime.Today.AddDays(-2) },
                new TaskItem { Id = 6, ProjectId = 2, Title = "Подключить уведомления", Description = "Добавить push-уведомления для важных событий", Status = "InProgress", Priority = 3, CreatedAt = DateTime.Now.AddDays(-4), DueDate = DateTime.Today.AddDays(2) },
                new TaskItem { Id = 7, ProjectId = 3, Title = "Собрать требования", Description = "Описать показатели для аналитических отчетов", Status = "Done", Priority = 2, CreatedAt = DateTime.Now.AddDays(-10), DueDate = DateTime.Today.AddDays(-3) },
                new TaskItem { Id = 8, ProjectId = 3, Title = "Подготовить отчет", Description = "Сформировать сводный отчет по активности команды", Status = "New", Priority = 4, CreatedAt = DateTime.Now.AddDays(-3), DueDate = DateTime.Today.AddDays(-1) }
            };
        }

        public static List<Project> Projects { get; set; }

        public static List<TaskItem> Tasks { get; set; }
    }

    public static class TaskStatuses
    {
        public const string New = "New";
        public const string InProgress = "InProgress";
        public const string Done = "Done";
        public const string Cancelled = "Cancelled";

        public static bool TryNormalize(string value, out string normalized)
        {
            if (string.Equals(value, New, StringComparison.OrdinalIgnoreCase))
            {
                normalized = New;
                return true;
            }

            if (string.Equals(value, InProgress, StringComparison.OrdinalIgnoreCase))
            {
                normalized = InProgress;
                return true;
            }

            if (string.Equals(value, Done, StringComparison.OrdinalIgnoreCase))
            {
                normalized = Done;
                return true;
            }

            if (string.Equals(value, Cancelled, StringComparison.OrdinalIgnoreCase))
            {
                normalized = Cancelled;
                return true;
            }

            normalized = null;
            return false;
        }
    }
}