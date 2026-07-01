using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Tests.UITests
{
    [TestFixture]
    public class ReporteIngresosTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        private const string UrlLogin = "http://localhost:5173/login";
        private const string UrlReporte = "http://localhost:5173/admin/reports/income";
        private const string Email = "admin@zuliairlines.com";
        private const string Password = "adatgg1515155*";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Maximize();

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));

            _driver.Navigate().GoToUrl(UrlLogin);

            FillInput(By.Id("businessEmail"), Email);
            FillInput(By.Id("password"), Password);

            var loginButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Iniciar sesión')]")));
            loginButton.Click();

            _wait.Until(d => d.Url.Contains("/admin/") || d.Url.EndsWith("/admin"));

            _driver.Navigate().GoToUrl(UrlReporte);

            _wait.Until(d => d.Url.Contains("/admin/reports/income"));
        }

        [Test]
        public void Test_ReporteIngresos_ConsultaSjoAMsq_MuestraTablaDeResultados()
        {
            SetSelectValueByLabel("Año", "2026");
            SetSelectValueByLabel("Origen", "SJO");
            SetSelectValueByLabel("Destino", "MSQ");

            var btnAplicar = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Aplicar filtros')]")));
            btnAplicar.Click();

            _wait.Until(d => d.FindElement(By.CssSelector("table")));

            var rows = _wait.Until(d =>
            {
                var currentRows = d.FindElements(By.CssSelector("tbody tr.border-b"));
                return currentRows.Count > 0 ? currentRows : null;
            });

            Assert.That(rows.Any(), Is.True, "La tabla debería mostrar al menos una fila de ingresos para la búsqueda SJO a MSQ.");
        }

        private void FillInput(By locator, string value)
        {
            var input = _wait.Until(d => d.FindElement(locator));
            input.Click();
            input.Clear();
            input.SendKeys(value);
        }

        private void SetSelectValueByLabel(string label, string value)
        {
            var select = _wait.Until(d => d.FindElement(By.XPath($"//label[normalize-space(.)='{label}']/following-sibling::select")));

            ((IJavaScriptExecutor)_driver).ExecuteScript(@"
                const select = arguments[0];
                const value = arguments[1];

                if (![...select.options].some(option => option.value === value)) {
                    select.appendChild(new Option(value, value));
                }

                select.value = value;
                select.dispatchEvent(new Event('input', { bubbles: true }));
                select.dispatchEvent(new Event('change', { bubbles: true }));
            ", select, value);
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