using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TodoLive.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using OpenAI_API;
using OpenAI_API.Completions;

namespace TodoLive.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment, IConfiguration config)
        {
            _logger = logger;
            _environment = environment;
            _config = config;
        }

        [HttpPost]
        [Route("gpt")]
        public IActionResult GetResult([FromBody] string prompt)
        {
            var jsonApiStore = _config["AppSettings:ApiKey"];
            string APIkey = string.Empty;

            if(jsonApiStore != null)
            {
                APIkey = jsonApiStore;
            }

            string answer = string.Empty;
            var openai = new OpenAIAPI(APIkey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompt;
            //completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.Model = OpenAI_API.Models.Model.AdaText;
            completion.MaxTokens = 100;
            var result = openai.Completions.CreateCompletionsAsync(completion);

            if (result != null)
            {
                foreach (var item in result.Result.Completions)
                {
                    answer = item.Text;
                }
                return Ok(answer);
            }
            else
            {
                return BadRequest("Not Found");
            }

        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Privacy()
        {

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }








    }
}


//[HttpPost]
//[Route("gpt")]
//public IActionResult GetResult([FromBody] string prompt)
//{
//    string apikey = _config.GetSection("AppSettings")["ApiKey"];

//    string answer = string.Empty;
//    var openai = new OpenAIAPI(apikey);
//    CompletionRequest completion = new CompletionRequest();
//    completion.Prompt = prompt;
//    //completion.Model = OpenAI_API.Models.Model.DavinciText;
//    completion.Model = OpenAI_API.Models.Model.AdaText;
//    completion.MaxTokens = 100;
//    var result = openai.Completions.CreateCompletionsAsync(completion);

//    if (result != null)
//    {
//        foreach (var item in result.Result.Completions)
//        {
//            answer = item.Text;
//        }
//        return Ok(answer);
//    }
//    else
//    {
//        return BadRequest("Not Found");
//    }

//}