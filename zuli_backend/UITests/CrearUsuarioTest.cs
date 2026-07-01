using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Tests.UITests
{
    [TestFixture]
    public class CrearUsuarioTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        private const string UrlLogin = "http://localhost:5173/login";
        private const string UrlCreateUser = "http://localhost:5173/admin/create-user";
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

            _wait.Until(d => d.Url.Contains("/admin/"));

            _driver.Navigate().GoToUrl(UrlCreateUser);
            _wait.Until(d => d.Url.Contains("/admin/create-user"));
        }

        [Test]
        public void Test_CrearUsuario_MuestraMensajeDeExito()
        {
            var uniqueNumber = Random.Shared.Next(100000000, 999999999);

            FillInput(By.Id("Cédula"), uniqueNumber.ToString());
            FillInput(By.Id("Primer Nombre"), "Usuario");
            FillInput(By.Id("Primer Apellido"), "Prueba");
            FillInput(By.Id("Segundo Apellido"), "Automatizada");
            FillInput(By.Id("Correo"), $"usuario.prueba.{uniqueNumber}@zuliairlines.com");
            SelectRole("Operator");

            var submitButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Crear Usuario')]")));
            submitButton.Click();

            var successMessage = _wait.Until(d => d.FindElement(By.XPath("//*[contains(normalize-space(.), 'Usuario creado correctamente')]")));
            Assert.That(successMessage.Displayed, Is.True, "Debería mostrarse el mensaje de éxito al crear el usuario.");

            var closeButton = _wait.Until(d => d.FindElement(By.XPath("//button[contains(normalize-space(.), 'Aceptar') or contains(normalize-space(.), 'Entendido')]")));
            closeButton.Click();

            _wait.Until(d => d.Url.Contains("/admin/users"));
        }

        private void FillInput(By locator, string value)
        {
            var input = _wait.Until(d => d.FindElement(locator));
            input.Click();
            input.Clear();
            input.SendKeys(value);
        }

        private void SelectRole(string value)
        {
            var select = _wait.Until(d => d.FindElement(By.TagName("select")));
            new SelectElement(select).SelectByValue(value);
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