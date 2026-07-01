using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Tests.UITests
{
    [TestFixture]
    public class ConsultarFechasValidasTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        private const string UrlReporte = "http://localhost:5173/admin/reports/flights";
        private const string UrlLogin = "http://localhost:5173/login";
        private const string Email = "geijomontoya@gmail.com";
        private const string Password = "Adatgg1515155*";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Maximize();

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));

            _driver.Navigate().GoToUrl(UrlLogin);

            var emailInput = _wait.Until(d => d.FindElement(By.Id("businessEmail")));
            emailInput.Click();
            emailInput.Clear();
            emailInput.SendKeys(Email);

            var passwordInput = _wait.Until(d => d.FindElement(By.Id("password")));
            passwordInput.Click();
            passwordInput.Clear();
            passwordInput.SendKeys(Password);

            var loginButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Iniciar sesión')]")));
            loginButton.Click();

            _wait.Until(d => d.Url.Contains("/admin/") || d.Url.EndsWith("/admin"));

            _driver.Navigate().GoToUrl("http://localhost:5173/admin/");

            _wait.Until(d => d.Url.Contains("/admin/") || d.Url.EndsWith("/admin"));

            _driver.Navigate().GoToUrl(UrlReporte);

            _wait.Until(d => d.Url.Contains("/admin/reports/flights"));
        }

        [Test]
        public void Test_ConsultaFechasValidas_MuestraTablaDeResultados()
        {
            var dateInputs = _wait.Until(d => d.FindElements(By.CssSelector("input[type='date']")));
            Assert.That(dateInputs.Count, Is.GreaterThanOrEqualTo(2), "Se esperaban al menos dos campos de fecha en el filtro.");

            SetDateValue(dateInputs[0], "2026-07-01");
            SetDateValue(dateInputs[1], "2026-07-31");

            var btnAplicar = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Aplicar filtros')]")));
            btnAplicar.Click();

            _wait.Until(d => d.FindElement(By.CssSelector("table")));

            var rows = _wait.Until(d =>
            {
                var currentRows = d.FindElements(By.CssSelector("tbody tr.border-b"));
                return currentRows.Count > 0 ? currentRows : null;
            });

            Assert.That(rows.Any(), Is.True, "La tabla debería mostrar al menos una fila de datos luego de aplicar el filtro.");
        }

        private void SetDateValue(IWebElement element, string value)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript(@"
                arguments[0].value = arguments[1];
                arguments[0].dispatchEvent(new Event('input', { bubbles: true }));
                arguments[0].dispatchEvent(new Event('change', { bubbles: true }));
            ", element, value);
        }

        [TearDown]
        public void TearDown()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver.Dispose();
            }
        }
    }
}