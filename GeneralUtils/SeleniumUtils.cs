using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;

namespace QuizTools.GeneralUtils
{
    enum Browser
    {
        Chrome,
        Edge,
        Firefox,
        InternetExplorer,
        Safari,
    }
    public static class SeleniumUtils
    {
        public static bool TryGetElement(this IWebDriver driver, By by, out IWebElement element)
        {
            try
            {
                element = driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                element = null;
                return false;
            }
        }
        public static bool ElementExists(this IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        public static void WaitUntilPageIsLoaded(this IWebDriver driver, int timeoutInSeconds = 10)
        {
            if (timeoutInSeconds <= 0)
                timeoutInSeconds = 10;
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            for (int i = 0; i < timeoutInSeconds; i++)
            {
                Thread.Sleep(1000);
                if (js.ExecuteScript("return document.readyState").ToString() == "complete")
                    break;
            }
        }
        public static void SetLocalStorageItem(this IWebDriver driver, string key, string value)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            js.ExecuteScript(@"
                localStorage.setItem(arguments[0], arguments[1]);
                ", key, value);
        }

        public static string? GetLocalStorageItem(this IWebDriver driver, string key)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            return js.ExecuteScript(@"
                return localStorage.getItem(arguments[0]);
                ", key)?.ToString();

        }

        public static IWebDriver GetBrowser()
        {
            IDriverConfig config = null;
            IWebDriver driver = null;

            string binary = Settings.Current.BrowserBinaryLocation;

            switch (Settings.Current.Browser)
            {
                case Browser.Chrome:
                    config = new ChromeConfig();
                    new DriverManager().SetUpDriver(config);
                    ChromeOptions chromeOptions = new ChromeOptions();
                    if (!string.IsNullOrEmpty(binary))
                        chromeOptions.BinaryLocation = binary;
                    driver = new ChromeDriver(chromeOptions);
                    break;

                case Browser.Edge:
                    config = new EdgeConfig();
                    new DriverManager().SetUpDriver(config);
                    EdgeOptions edgeOptions = new EdgeOptions();
                    if (!string.IsNullOrEmpty(binary))
                        edgeOptions.BinaryLocation = binary;
                    driver = new EdgeDriver(edgeOptions);
                    break;

                case Browser.Firefox:
                    config = new FirefoxConfig();
                    new DriverManager().SetUpDriver(config);
                    FirefoxOptions firefoxOptions = new FirefoxOptions();
                    if (!string.IsNullOrEmpty(binary))
                        firefoxOptions.BinaryLocation = binary;
                    driver = new FirefoxDriver(firefoxOptions);
                    break;

                case Browser.InternetExplorer:
                    config = new InternetExplorerConfig();
                    new DriverManager().SetUpDriver(config);
                    driver = new InternetExplorerDriver();
                    break;

                case Browser.Safari:
                    driver = new SafariDriver();
                    break;

                default:
                    throw new ArgumentException($"Unsupported browser: {Settings.Current.Browser}");
            }

            return driver;
        }
    }
}
