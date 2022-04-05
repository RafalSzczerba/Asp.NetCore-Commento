using Commento.Enums;
using Commento.Models;
using Commento.Repository;
using Commento.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Commento.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IConfiguration _configuration { get; }
        private LoginService _loginService;
        private IHttpClientFactory _httpClient;

        public HomeController(ILogger<HomeController> logger,
            IConfiguration configuration,
            LoginService loginService,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _loginService = loginService;
            _httpClient = httpClientFactory;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Commento([FromQuery] string token, [FromQuery] string hmac)
        {
            
            if (hmac != null && token != null)
            {
                LoginRepository.Token = token;
                LoginRepository.Hmac = hmac;
            }
            else
            {
                return View();
            }
            var hmacSecured = _configuration["Commento:HMAC"];
            var tokenDecoded = _loginService.HexDecode(token);
            var hmacSecuredDecoded = _loginService.HexDecode(hmacSecured);
            var hmacDecoded = _loginService.HexDecode(hmac);
            var expectedHmac = _loginService.HmacSHA256(token, hmacSecured);

            if ((hmac != expectedHmac))
            {

                ViewBag.loginStatus = LoginStatus.InvalidHmac;
                return View();

            }
            return Redirect(nameof(Login));
        }

        public async Task<ActionResult> Login(string Email, string UserName)
        {
            string content = null;
            if (Email != null)
            {

                ViewBag.loginStatus = LoginStatus.ProperHmacVerification;
                var payload = JsonConvert.SerializeObject(new Payload
                {
                    token = LoginRepository.Token,
                    email = Email,
                    name = UserName
                });
                var payloadEncode = _loginService.StringToHex(payload);
                var hmacSecret = _configuration["Commento:HmacSecret"];
                var hmacSH256 = _loginService.HmacSHA256(payload, hmacSecret);
                var hmacHex = _loginService.StringToHex(hmacSH256);
                string loginURL = $"http://commento.io/api/oauth/sso/callback?payload={payloadEncode}&hmac={hmacHex}";
                var client = _httpClient.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, loginURL);
                var result = await client.SendAsync(request);
                content = await result.Content.ReadAsStringAsync();

            }
            if (content == null)
            {
                return View();
            }
            if (content.Contains("Error"))
            {
                ViewBag.loginStatus = LoginStatus.WrongLoginData;
                return View();
            }

            ViewBag.loginStatus = LoginStatus.Logged;
            LoginRepository.Hmac = null;
            LoginRepository.Token = null;
            return Redirect(nameof(Index));
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
