using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DomainChecker
{
    class Program
    {
        static void Main(string[] args)
        {
           // try 
           // {
                CreateFileIfNotExists(filePath("domains.txt"));
                IWebDriver driver = InitializeWebDriver();
                for (int i = 0 ; i<100 ; i++ )
                {
                    String domain = extractfile(i, filePath("domains.txt"), driver);
                    IWebElement searchBar = driver.FindElement(By.Id("searchBox"));
                    searchBar.SendKeys(domain);
                    string value = driver.FindElement(By.Id("recaptcha-token")).GetAttribute("value");
                    Console.WriteLine(value);

                    driver.Navigate().Refresh();
                }


         //   }
       //     catch (Exception) { }
            

        }
        private static string extractfile(int index, string filePath, IWebDriver driver)
        {
            string result = null;
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length > 0) result = lines[index];
                else
                {
                    Console.WriteLine($"--------------The file is empty  {filePath}");
                    driver.Quit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("--------------An error occurred: " + ex.Message);
                driver.Quit();
            }
            return result;
        }

        private static IWebDriver InitializeWebDriver()
        {

            IWebDriver driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            try
            {
                string url = "https://brightcloud.com/tools/url-ip-lookup.php";
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);
            }
            catch (Exception)
            {
                Console.WriteLine("--------------error in openning browser!!");
            }

            return driver;
        }
        private static string filePath(string filename)
        {
            // Get the current directory
            string currentDirectory = Directory.GetCurrentDirectory();

            // file
            string filePath = Path.Combine(currentDirectory, filename);
            


            return filePath;

        }

        private static void CreateFileIfNotExists(string filePath)
        {
            try
            {
                // Check if the file already exists
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"File '{Path.GetFileName(filePath)}' already exists in the following path:");
                    Console.WriteLine(filePath);
                }
                else
                {
                    // Create the file if it doesn't exist
                    using (File.Create(filePath))

                        Console.WriteLine($"File '{Path.GetFileName(filePath)}' created successfully in the following path:");
                    Console.WriteLine(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }





        

  
    }
}
