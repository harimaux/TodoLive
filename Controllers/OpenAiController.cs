using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using OpenAI_API.Completions;

namespace TodoLive.Controllers
{
    public class OpenAiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("getresult")]
        public IActionResult GetResult([FromBody]string prompt)
        {
            string apikey = "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";
            string answer = string.Empty;
            var openai = new OpenAIAPI(apikey);
            CompletionRequest completion = new CompletionRequest();
            completion.Prompt = prompt;
            completion.Model = OpenAI_API.Models.Model.DavinciText;
            completion.MaxTokens = 2000;
            var result = openai.Completions.CreateCompletionsAsync(completion);

            if(result != null)
            {
                foreach(var item in result.Result.Completions)
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





    }
}
