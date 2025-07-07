using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController(AppDbContext _context) : Controller
    {
        // GET: Tasks
        [HttpGet]
        public async Task<IActionResult> Index(string? statusFilter = null, string? sortProperty = null, string? sortDirection = "") // Displays Starting Page
        {
            IQueryable<TaskItem> query = _context.Tasks;

            // Filtering
            query = statusFilter switch
            {
                "Complete" => query.Where(t => t.CompletionStatus),
                "Incomplete" => query.Where(t => !t.CompletionStatus),
                _ => query
            };

            // Sorting
            // query = sortProperty switch
            // {
            //     "Title" => query = sortDirection switch
            //     {
            //         "asc" => query.OrderBy(t => t.Title),
            //         "desc" => query.OrderByDescending(t => t.Title),
            //         _ => query,
            //     },
            //     "Status" => query = sortDirection switch
            //     {
            //         "asc" => query.OrderBy(t => t.CompletionStatus),
            //         "desc" => query.OrderByDescending(t => t.CompletionStatus),
            //         _ => query,
            //     },
            //     "DueDate" => query = sortDirection switch
            //     {
            //         "asc" => query.OrderBy(t => t.DueDate),
            //         "desc" => query.OrderByDescending(t => t.DueDate),
            //         _ => query,
            //     },
            //     _ => query
            // };

            query = sortDirection switch
            {
                "asc" => query = sortProperty switch
                {
                    "Title" => query.OrderBy(t => t.Title),
                    "Status" => query.OrderBy(t => t.CompletionStatus),
                    "DueDate" => query.OrderBy(t => t.DueDate),
                    _ => query,
                },
                "desc" => query = sortProperty switch
                {
                    "Title" => query.OrderByDescending(t => t.Title),
                    "Status" => query.OrderByDescending(t => t.CompletionStatus),
                    "DueDate" => query.OrderByDescending(t => t.DueDate),
                    _ => query,
                },
                _ => query
            };

            List<TaskItem> tasks = await query.ToListAsync();

            return View(tasks);
            //:* Return View when you want to display a page, usually on GET requests or when form validation fails (showing the form again with errors).
        }

        // GET: Tasks/Create
        [HttpGet]
        public IActionResult Create() => View(); // Displays Task Create Form

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem task) // Performs Task Post Operation
        {
            //:? When the TaskItem model provided by user is invalid we return View with the task provided so that we can return validation messages
            //:* This only works with models that have some fields defined as 'required' or with a certain length and etc., in this case TaskItem requires at least a title before its creation 
            if (!ModelState.IsValid) return View(task);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
            //:* Redirect after a successful POST operation (like Create, Edit, or Delete) to avoid resubmitting the form if the user refreshes and to navigate to another page (typically the list/index page).
        }

        // GET: Tasks/Edit/{id}
        [HttpGet] //:? HTML Forms don't support PUT and DELETE operations so if we are not doing an API we can only use GET and POST
        public async Task<IActionResult> Edit(int id) // Displays Edit Form
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        // POST: Tasks/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem task) // Performs Task Update Operation
        {
            if (id != task.Id) return NotFound();
            if (!ModelState.IsValid) return View(task);

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id) // Displays Delete Form
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        // POST: Tasks/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) // Performs Task Deletion Operation
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id) // Displays Task Details
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return PartialView("_TaskDetailsPartial", task);
        }

        // POST: Tasks/Complete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id) // Performs Task Toggle Operation
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            task.CompletionStatus = !task.CompletionStatus;

            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();

            return Ok(); // No redirect needed for AJAX
        }
    }
}
