using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Tests.UITests
{
    [TestFixture]
    public class CrearAeronaveTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        private const string UrlLogin = "http://localhost:5173/login";
        private const string UrlCreateAircraft = "http://localhost:5173/admin/create-aircraft";
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

            _wait.Until(d => d.Url.Contains("/admin/"));

            _driver.Navigate().GoToUrl(UrlCreateAircraft);

            _wait.Until(d => d.Url.Contains("/admin/create-aircraft"));
        }

        [Test]
        public void Test_CrearAeronave_MuestraMensajeDeExito()
        {
            var uniqueSuffix = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
            var model = $"TST-{uniqueSuffix}";

            FillInput(By.Id("Modelo"), model);
            FillInput(By.Id("Peso soportado por la aeronave (kg)"), "2800");
            FillInput(By.Id("Filas clase económica"), "2");
            FillInput(By.Id("Asientos por fila económica"), "4");
            FillInput(By.Id("Filas primera clase"), "1");
            FillInput(By.Id("Asientos por fila primera clase"), "2");

            var submitButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Guardar aeronave')]")));
            submitButton.Click();

            var successMessage = _wait.Until(d => d.FindElement(By.XPath("//*[contains(normalize-space(.), 'La aeronave se ha creado correctamente')]")));
            Assert.That(successMessage.Displayed, Is.True, "Debería mostrarse el mensaje de éxito al crear la aeronave.");

            var closeButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Aceptar') or contains(normalize-space(.), 'Entendido')]")));
            closeButton.Click();

            _wait.Until(d => d.Url.Contains("/admin/aircrafts"));
        }

        private void FillInput(By locator, string value)
        {
            var input = _wait.Until(d => d.FindElement(locator));
            input.Click();
            input.Clear();
            input.SendKeys(value);
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