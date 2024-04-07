using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Web;
using System.Net.Http;
using TwoCaptcha.Captcha;
using _2CaptchaAPI;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;


namespace DomainChecker
{
    class Program
    {
        static void Main(string[] args)
        {
           // try 
           // {
                //create domains file .txt if not exists
                CreateFileIfNotExists(filePath("domains.txt"));

                //open browser 
                IWebDriver driver = InitializeWebDriver();
                
                //start extracting domains file by index
                for ( int i = 0 ; i < 5 ; i ++ )
                {
                    String domain = extractfile(i, filePath("domains.txt"), driver);
                    IWebElement searchBar = driver.FindElement(By.Id("searchBox"));
                    searchBar.SendKeys(domain);


                    // Find the iframe element
                    IWebElement iframe = driver.FindElement(By.TagName("iframe"));

                    // Get the value of the src attribute
                    string src = iframe.GetAttribute("src");

                    // Parse the URL
                    Uri uri = new Uri(src);

                    // Extract the value of the k parameter from the URL
                    string kValue = HttpUtility.ParseQueryString(uri.Query).Get("k");

                    // get url 
                    string currentUrl = driver.Url;



                    // Solve the CAPTCHA
                    Console.WriteLine("Solving CAPTCHA");
                    var service = new _2CaptchaAPI._2Captcha("1848330245f983448d083bea580f329f");
                    var response = service.SolveReCaptchaV2(kValue, currentUrl).Result;
                    string code = response.Response;
                    Console.WriteLine($"Successfully solved the CAPTCHA. The solve code is {code}");
                    // Set the solved CAPTCHA
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                    IWebElement recaptchaResponseElement = driver.FindElement(By.Id("g-recaptcha-response-1"));
                    js.ExecuteScript("arguments[0].removeAttribute('style');", recaptchaResponseElement);
                    js.ExecuteScript($"arguments[0].value = '{code}';", recaptchaResponseElement);

     
                    //Submit the form
                    IWebElement submitBtn = driver.FindElement(By.XPath("//button[contains(@onclick, 'search()')]"));

                    submitBtn.Click();





                

                    // Find the div element with id "threatScore"
                    IWebElement threatScoreDiv = driver.FindElement(By.Id("threatScore"));

                    // Get the text content of the div element
                    string threatScoreText = threatScoreDiv.Text;

                    // Extract numeric value using regular expression
                    Match match = Regex.Match(threatScoreText, @"\d+");
                    int threatScoreValue = 0;
                    if (match.Success)
                    {
                        threatScoreValue = int.Parse(match.Value);
                    }

                    // Check if the threat score is up to 80
                    if (threatScoreValue <= 80)
                    {
                        Console.WriteLine($"The threat score is  {threatScoreValue}.");
                    }
                    else
                    {
                        Console.WriteLine($"The threat score ({threatScoreValue}) is higher than 80.");
                    }



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
