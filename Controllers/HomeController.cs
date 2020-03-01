using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnetcore_test.Models;
using System.Net;
using System.Xml.Linq;

namespace dotnetcore_test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(IndexModel model)
        {
            var comparison = new ComparisonModel();
            comparison.Checks = new List<Check>();
            RunChecks(comparison.Checks, model);
            return View("Comparison", comparison);
        }

        private static void RunChecks(List<Check> checks, IndexModel model)
        {
            var oldXDoc = RunXmlReadChecks(checks, model.OldUrl);
            var newXDoc = RunXmlReadChecks(checks, model.NewUrl);

            var leftElement = oldXDoc.Root;
            var rightElementsAvailable = newXDoc.Elements();

            while (leftElement != null)
            {
                var check = AddCheck(checks, $"Can find element {leftElement.Name}");
                check.HasPassed = rightElementsAvailable.FirstOrDefault(e => e.Name == leftElement.Name) != null;

                // Get the next element to search for
                if (leftElement.HasElements)
                {
                    leftElement = leftElement.Elements().First();
                    rightElementsAvailable = rightElementsAvailable.Elements();
                }
                else
                {
                    leftElement = leftElement.ElementsAfterSelf().FirstOrDefault();
                }
            }
        }

        public static XDocument RunXmlReadChecks(List<Check> checks, string url)
        {
            Check check = null;

            check = AddCheck(checks, $"Can get data from {url}");

            string urlString = null;

            var client = new WebClient();

            try
            {
                urlString = client.DownloadString(url);
                check.HasPassed = true;
            }
            catch
            {
                check.HasPassed = false;
                return null;
            }

            check = AddCheck(checks, $"Data from {url} is valid XML");

            XDocument oldUrlXDoc = null;

            try
            {
                oldUrlXDoc = XDocument.Parse(urlString);
                check.HasPassed = true;
            }
            catch
            {
                check.HasPassed = false;
                return null;
            }

            return oldUrlXDoc;
        }

        private static Check AddCheck(List<Check> checks, string summary)
        {
            var check = new Check() { Summary = summary };
            checks.Add(check);
            return check;
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
