using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Tasks
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<TaskItem> tasks = await _context.Tasks.ToListAsync();
            return View(tasks);
        }

        // GET: Tasks/ShowCreateForm
        [HttpGet]
        public IActionResult ShowCreateForm() => View();

        // POST: Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem task)
        {
            //:? When the TaskItem model provided by user is invalid we return View with the task provided so that we can return validation messages
            //:* This only works with models that have some fields defined as 'required' or with a certain length and etc., in this case TaskItem requires at least a title before its creation 
            if (!ModelState.IsValid) return View(task);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Update/{id}
        [HttpGet] //:? HTML Forms don't support PUT and DELETE operations so if we are not doing an API we can only use GET and POST
        public async Task<IActionResult> Edit(int id)
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        // POST: Tasks/Update/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem task)
        {
            if (id != task.Id) return NotFound();
            if (!ModelState.IsValid) return View(task);

            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }

        // POST: Tasks/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tasks/Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            TaskItem? task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            return View(task);
        }
    }
}
