using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OpenAI_API.Completions;
using OpenAI_API;
using System.Globalization;
using System.Linq.Expressions;
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
        private readonly IConfiguration _config;

        public TodosController(ILogger<TodosController> logger, ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }


        public ApplicationDbContext Get_dbContext()
        {
            return _dbContext;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Get logged user details
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            var vm = new MainVM();

            if (user != null && _dbContext.TodosDB != null)
            {
                var allUserTasks = await _dbContext.TodosDB
                    .Where(x => x.OwnerId == userId && x.State != "Completed")
                    .OrderByDescending(x => x.DateRequested)
                    .ToListAsync();

                vm.TodosList = allUserTasks;
            }

            if(user != null && _dbContext.TaskPriorityDB != null)
            {
                vm.TaskPriorityList = await _dbContext.TaskPriorityDB.ToListAsync();
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
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

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
                vm.TaskPriorityList = await _dbContext.TaskPriorityDB.ToListAsync();
            }

            vm.TodosList?.Add(newTask);

            return PartialView("_Task", vm);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteTask(string cardId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            int convertedId = Int32.Parse(cardId);

            if (_dbContext.TodosDB != null)
            {
                var task = await _dbContext.TodosDB
                    .Where(x => x.OwnerId == userId && x.Id == convertedId)
                    .FirstOrDefaultAsync();

                if (task != null)
                {
                    _dbContext.TodosDB.Remove(task);
                    await _dbContext.SaveChangesAsync();

                }
                else
                {
                    return Content("Error - no task to delete.");
                }

            }

            return RedirectToAction("Index");
        }



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MarkTaskAsComplete(string cardId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

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
        public async Task<IActionResult> ShowSelectedTasks(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var vm = new MainVM();

            if (user != null)
            {
                var allTasks = new List<Todos>(await _dbContext.TodosDB.Where(x => x.OwnerId == userId).ToListAsync());
                IEnumerable<Todos> sortedTasks;

                switch (id)
                {
                    case "Completed":
                        sortedTasks = allTasks.Where(x => x.State == "Completed");
                        vm.TodosList = sortedTasks.ToList();
                        break;
                    case "Overdue":
                            sortedTasks = allTasks.Where(x => x.State != "Completed" && x.DateDue < DateTime.UtcNow);
                            vm.TodosList = sortedTasks.ToList();
                        break;
                    case "Urgent":
                            sortedTasks = allTasks.Where(x => x.Priority == "Urgent" && x.State != "Completed");
                            vm.TodosList = sortedTasks.ToList();
                        break;
                    case "Important":
                            sortedTasks = allTasks.Where(x => x.Priority == "Important" && x.State != "Completed");
                            vm.TodosList = sortedTasks.ToList();
                        break;
                    case "Medium":
                            sortedTasks = allTasks.Where(x => x.Priority == "Medium" && x.State != "Completed");
                            vm.TodosList = sortedTasks.ToList();
                        break;
                    case "Low":
                            sortedTasks = allTasks.Where(x => x.Priority == "Low" && x.State != "Completed");
                            vm.TodosList = sortedTasks.ToList();
                        break;
                    default:
                        // code block
                        break;
                }


            }

            if (user != null && _dbContext.TaskPriorityDB != null)
            {
                vm.TaskPriorityList = await _dbContext.TaskPriorityDB.ToListAsync();
            }

            return PartialView("_Task", vm);
        }




        [HttpPost]
        public IActionResult GPTsolveTask(string id, string value)
        {
            var jsonApiStore = _config["AppSettings:ApiKey"];
            string APIkey = string.Empty;

            if (jsonApiStore != null)
            {
                APIkey = jsonApiStore;
            }

            return Ok();

            //test

            //string answer = string.Empty;
            //var openai = new OpenAIAPI(APIkey);
            //CompletionRequest completion = new CompletionRequest();
            //completion.Prompt = value;
            ////completion.Model = OpenAI_API.Models.Model.DavinciText;
            //completion.Model = OpenAI_API.Models.Model.AdaText;
            //completion.MaxTokens = 100;
            //var result = openai.Completions.CreateCompletionsAsync(completion);

            //if (result != null)
            //{
            //    foreach (var item in result.Result.Completions)
            //    {
            //        answer = item.Text;
            //    }
            //    return Ok(answer);
            //}
            //else
            //{
            //    return BadRequest("Not Found");
            //}

        }






    }
}
