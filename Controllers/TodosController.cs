using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoLive.Data;
using TodoLive.Models;

namespace TodoLive.Controllers
{
    public class TodosController : Controller
    {
        private readonly ILogger<TodosController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public TodosController(ILogger<TodosController> logger, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public ApplicationDbContext Get_dbContext()
        {
            return _dbContext;
        }

        //public IEnumerable<Todos> GetCompletedPosts()
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        //    var allUserTasks = new List<Todos>();

        //    if (user != null && _dbContext.TodosDB != null)
        //    {
        //        allUserTasks = _dbContext.TodosDB
        //            .Where(x => x.OwnerId == userId && x.State != "Completed")
        //            .OrderByDescending(x => x.DateRequested)
        //            .ToList();
        //    }

        //    return allUserTasks;
        //}

        [Authorize]
        public IActionResult Index()
        {
            // Get logged user details
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            var vm = new MainVM();

            if (user != null && _dbContext.TodosDB != null)
            {
                var allUserTasks = _dbContext.TodosDB
                    .Where(x => x.OwnerId == userId && x.State != "Completed")
                    .OrderByDescending(x => x.DateRequested)
                    .ToList();
                vm.TodosList = allUserTasks;
            }

            if(user != null && _dbContext.TaskPriorityDB != null)
            {
                vm.TaskPriorityList = _dbContext.TaskPriorityDB.ToList();
            }

            

            return View(vm);
        }

        public IActionResult Error()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTask(Todos formContent)
        {

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            var vm = new MainVM();

            var newTask = new Todos
            {
                Title = formContent.Title,
                Content = formContent.Content,
                FromRequested = formContent.FromRequested,
                Priority = formContent.Priority,
                DateRequested = DateTime.Now,
                DateDue = formContent.DateDue,
                State = "Not started",
                OwnerId = userId,
            };

            _dbContext.TodosDB!.Add(newTask);
            await _dbContext.SaveChangesAsync();

            if (user != null && _dbContext.TaskPriorityDB != null)
            {
                vm.TaskPriorityList = _dbContext.TaskPriorityDB.ToList();
            }

            vm.TodosList?.Add(newTask);

            return PartialView("_Task", vm);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTask(string cardId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            int convertedId = Int32.Parse(cardId);

            if (_dbContext.TodosDB != null)
            {
                var task = await _dbContext.TodosDB.FindAsync(convertedId);

                if (task == null)
                {
                    return Content("Error - no task to delete.");
                }

                _dbContext.TodosDB.Remove(task);
                await _dbContext.SaveChangesAsync();

            }

            return RedirectToAction("Index");
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkTaskAsComplete(string cardId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            int convertedId = Int32.Parse(cardId);

            if (_dbContext.TodosDB != null)
            {
                var task = await _dbContext.TodosDB.FindAsync(convertedId);

                if (task == null)
                {
                    return Content("Error - task does not exist.");
                }

                task.DateCompleted = DateTime.Now;
                task.DateEdited = DateTime.Now;
                task.State = "Completed";

                _dbContext.TodosDB.Update(task);
                await _dbContext.SaveChangesAsync();

            }


            return RedirectToAction("Index");
        }




        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ShowCompletedTasks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var vm = new MainVM();

            if (user != null)
            {
                var allUserTasks = await _dbContext.TodosDB
                    .Where(x => x.OwnerId == userId && x.State == "Completed")
                    .OrderByDescending(x => x.DateRequested)
                    .ToListAsync();

                vm.TodosList = allUserTasks;
            }

            if (user != null && _dbContext.TaskPriorityDB != null)
            {
                vm.TaskPriorityList = _dbContext.TaskPriorityDB.ToList();
            }

            return PartialView("_Task", vm);
        }








    }
}
