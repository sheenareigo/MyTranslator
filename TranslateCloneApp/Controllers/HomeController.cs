using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using TranslateCloneApp.DTO;
using TranslateCloneApp.Models;

namespace TranslateCloneApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly List<string> mostusedlangs = new List<string>() {
            "English",
    "Spanish",
    "French",
    "German",
    "Mandarin Chinese",
    "Japanese",
    "Russian",
    "Tamil",
    "Hindi",
    "Portuguese",
    "Arabic",
    "Italian",
    "Korean",
    "Bengali"
        };

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public IActionResult Index()
        {
            ViewBag.Languages = new SelectList( mostusedlangs);
            return View();
        }

        public IActionResult VoiceToText()
        {
            ViewBag.Languages = new SelectList(mostusedlangs);
            return View();
      
        }

        [HttpPost]
        public async Task<IActionResult> Translate(string query, string selected_lang, string module)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                ViewBag.ErrorMessage = "Text to translate cannot be empty.";
                // Populate languages again in case of validation error
                ViewBag.Languages = new SelectList(mostusedlangs);
                if (module == "Text")
                {


                    return View("Index");
                }


                return View("VoiceToText");
              
            }

            if (ModelState.IsValid)
            {
                //get open api key

                var apiKey = _configuration["OpenAI:APIKey"];

                // set up httpclient
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");


                //define request

                var payload = new
                {

                    model = "gpt-4o-mini",
                    messages = new object[]
                    {

                    new { role="system",
                          content=$"Translate to {selected_lang}"},
                    new { role="user", content=query}
                    },

                    temperature = 0,
                    max_tokens = 256
                };
                string jsonpayload = JsonConvert.SerializeObject(payload);
                HttpContent content = new StringContent(jsonpayload, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                var jsonresponse = await response.Content.ReadAsStringAsync();
                var processed_response = JsonConvert.DeserializeObject<OpenAIResponse>(jsonresponse);
                ViewBag.Result = processed_response.Choices[0].Message.Content;
            }

            //respone

           
            ViewBag.Languages = new SelectList(mostusedlangs);

            if (module == "Text")
            {


                return View("Index");
            }

            
                return View("VoiceToText");

            

        }
    }
}
