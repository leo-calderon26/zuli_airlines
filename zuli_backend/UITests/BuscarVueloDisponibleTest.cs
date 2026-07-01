using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Tests.UITests
{
    [TestFixture]
    public class BuscarVueloDisponibleTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        private const string UrlReserva = "http://localhost:5173/";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            _driver = new ChromeDriver(options);
            _driver.Manage().Window.Maximize();

            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(60));

            _driver.Navigate().GoToUrl(UrlReserva);
            _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Buscar vuelos')]")));
        }

        [Test]
        public void Test_BuscarVueloDisponible_SjoAMsq_MuestraResultados()
        {
            FillInputByLabel("Desde", "SJO");
            FillInputByLabel("Hacia", "MSQ");
            SetDateValue(_wait.Until(d => d.FindElement(By.CssSelector("input[type='date']"))), "2026-07-06");

            var submitButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Buscar vuelos')]")));
            submitButton.Click();

            _wait.Until(d => d.Url.Contains("/buscar-vuelos"));
            _wait.Until(d => d.FindElement(By.XPath("//*[contains(normalize-space(.), 'SJO') and contains(normalize-space(.), 'MSQ')]")));

            var resultCards = _wait.Until(d =>
            {
                var cards = d.FindElements(By.XPath("//*[contains(normalize-space(.), 'Clase Turista') and contains(normalize-space(.), 'Seleccionar')]"));
                return cards.Count > 0 ? cards : null;
            });

            Assert.That(resultCards.Any(), Is.True, "La búsqueda SJO a MSQ debería mostrar al menos un vuelo disponible.");
        }

        private void FillInputByLabel(string label, string value)
        {
            var input = _wait.Until(d => d.FindElement(By.XPath($"//label[normalize-space(.)='{label}']/following-sibling::input")));
            input.Click();
            input.Clear();
            input.SendKeys(value);
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
