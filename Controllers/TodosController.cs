using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
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
                    .Where(x => x.OwnerId == userId)
                    .OrderByDescending(x => x.DateRequested)
                    .ToList();
                vm.TodosList = allUserTasks;
            }

            if(user != null && _dbContext.TaskPriorityDB != null)
            {
                vm.TaskPriorityList = _dbContext.TaskPriorityDB.ToList();
            }


            //if(vm.TodosList != null)
            //{
            //    foreach (var task in vm.TodosList)
            //    {
            //        foreach (var priorityItem in vm.TaskPriorityList)
            //        {
            //            if(task.Priority == priorityItem.PriorityName)
            //            {

            //            }
            //        }
            //    }
            //}

            

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

            try
            {
                _dbContext.TodosDB!.Add(newTask);
                await _dbContext.SaveChangesAsync();

                if (user != null && _dbContext.TaskPriorityDB != null)
                {
                    vm.TaskPriorityList = _dbContext.TaskPriorityDB.ToList();
                }

                vm.Todos = newTask;
                return PartialView("_Task", vm);
            }
            catch
            {
                return RedirectToAction("Error");
            }


        }


    }
}
