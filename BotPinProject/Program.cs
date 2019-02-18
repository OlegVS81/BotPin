using BotPin.Properties;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using sl = System.Threading.Thread;
using SCond = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace BotPin
{
    class Program
    {
        

        static void Main(string[] args)
        {
            //CreateXML();
            //ClearXML();

            int m = 6;
            while (m > 0)
            {

                var r = new Rubricator(AppDomain.CurrentDomain.BaseDirectory, Resources.GoToUrl);

                int i = 6;
                while (i > 0)
                {
                    if ((int)r.chekState == 999) break;
                    r.SignIn(Resources.nickname, Resources.psw, 1000);
                    r.Subscription(1000);
                    r.ClickButtonAddPin(1000);
                    r.ClickButtonFromInternet(1000);
                    r.GetRendomAttributte();
                    r.SetURL(1000);
                    r.ClickButtonFind(1000);
                    r.SetPicture(1000);
                    r.SelectCollection(1000);
                    r.SetDescription(1000);
                    r.ClickButtonChoosePicture(1000);
                    r.ClickButtonSave(1000);
                    i--;
                }

                sl.Sleep(10000);

                if ((int)r.chekState == 999)
                { r.Close(1000); break; }
                r.Close(1000);
                m--;
            }

            Environment.Exit(0);

        }

        static void CreateXML()
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string readContents;
            using (StreamReader streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "reg.txt", Encoding.GetEncoding(1251)))
            {
                readContents = streamReader.ReadToEnd();
            }


            StringBuilder xmlStr = new StringBuilder(@"<?xml version='1.0' encoding='windows-1251'?><root>");
            string[] split = Regex.Split(readContents, "[\r\n]+");
            var list = new List<string>();
            foreach (string operand in split)
            {
                if (operand.IndexOf("RRRR") != -1)
                {
                    if (operand.IndexOf("'") != -1)
                        Console.WriteLine(operand.ToString());
                    if (operand.IndexOf(">") != -1)
                        Console.WriteLine(operand.ToString());


                    if (operand.IndexOf("<") != -1)
                        Console.WriteLine(operand.ToString());
                    //list.Add(operand.Split("RRRR")[1]);
                    xmlStr.Append(String.Format("<pic urltogo='{0}' urlpic='{1}' desc='{2}'/>", operand.Split("RRRR")[0].Split("?")[0].ToString(), operand.Split("RRRR")[1].Split("#")[0].ToString().Split("?")[0].ToString(), operand.Split("RRRR")[1].Split("#")[1].ToString()));
                }
            }

            xmlStr.Append("</root>");
            using (StreamWriter sw = new StreamWriter(String.Format("{0}reg.xml", AppDomain.CurrentDomain.BaseDirectory), false, Encoding.GetEncoding(1251)))
            {
                sw.Write(xmlStr);

            }

            Console.WriteLine(xmlStr);
        }

        static void ClearXML()
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            string attUrlpic, attDesc, attUrlGo;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(AppDomain.CurrentDomain.BaseDirectory + Resources.xml);

            XmlNodeList nodes = xDoc.GetElementsByTagName("pic");


            XmlAttribute typeAttr = xDoc.CreateAttribute("collection");

            StringBuilder xmlStr = new StringBuilder(@"<?xml version='1.0' encoding='windows-1251'?><root>");

            foreach (XmlNode node in nodes)
            {

                attUrlpic = node.Attributes.GetNamedItem("urlpic").Value.ToString();
                node.Attributes.GetNamedItem("urlpic").Value = attUrlpic.Replace("www.", "");

                attUrlGo = node.Attributes.GetNamedItem("urltogo").Value.ToString();
                node.Attributes.GetNamedItem("urltogo").Value = attUrlGo.Replace("www.", "");

                attDesc = node.Attributes.GetNamedItem("desc").Value.ToString();
                attDesc = attDesc.Replace("2012", "");
                attDesc = attDesc.Replace("2013", "");
                attDesc = attDesc.Replace("2014", "");
                attDesc = attDesc.Replace("2015", "");
                node.Attributes.GetNamedItem("desc").Value = attDesc.Replace("2011", "");

                if (Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "МУЖ"))
                    typeAttr.Value = "Мужские стрижки и прически";
                else if (Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "ДЕТ") || Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "ДЕВ"))
                    typeAttr.Value = "Детские прически";
                else if (Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "СРЕД"))
                    typeAttr.Value = "Средние Волосы";
                else if (Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "ДЛИН"))
                    typeAttr.Value = "Длинные волосы";
                else if (Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "КОРОТ"))
                    typeAttr.Value = "Короткие волосы";
                else if (Regex.IsMatch(node.Attributes.GetNamedItem("desc").Value.ToString().ToUpper(), "ВИДЕ"))
                    typeAttr.Value = "Видео уроки причесок";
                else
                    typeAttr.Value = "Волосы";

                node.Attributes.Append(typeAttr);

                if (!Regex.IsMatch(attUrlpic, @"\p{IsCyrillic}") & !Regex.IsMatch(attUrlpic, @"%"))
                {
                    xmlStr.Append(node.OuterXml);
                }
            }
            //xmlStr.Append(String.Format("<pic urltogo='{0}' urlpic='{1}' desc='{2}'/>", operand.Split("RRRR")[0].Split("?")[0].ToString(), operand.Split("RRRR")[1].Split("#")[0].ToString().Split("?")[0].ToString(), operand.Split("RRRR")[1].Split("#")[1].ToString()));

            xmlStr.Append("</root>");
            using (StreamWriter sw = new StreamWriter(String.Format("{0}reg22.xml", AppDomain.CurrentDomain.BaseDirectory), false, Encoding.GetEncoding(1251)))
            {
                sw.Write(xmlStr);

            }

            Console.WriteLine(xmlStr);
        }
    }

    class Rubricator 
    {
        
        private readonly IWebDriver driver;
        private WebDriverWait wait;
        private readonly IJavaScriptExecutor js;
        private XmlAttributeCollection attr;

        private Logger logger;

        public enum State
        {
            Start,
            SignIn,
            Subscription,
            ClickButtonAddPin,
            ClickButtonFromInternet,
            GetRendomAttributte,
            SetURL,
            ClickButtonFind,
            SetPicture,
            SelectCollection,
            SetDescription,
            ClickButtonChoosePicture,
            ClickButtonSave = 999,
            Close
        }
        
        public State chekState {  get; private set; }
        
        public Rubricator(string chromeDriverDirectory, string url)
        {
            chekState = State.Start;
            logger = LogManager.GetCurrentClassLogger();
            driver = new ChromeDriver(chromeDriverDirectory);
            js = (IJavaScriptExecutor)driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            driver.Navigate().GoToUrl(url);

            logger.Debug("Went to the {0}", Resources.GoToUrl);
            
        }

        public State SignIn(String nickname, String password, int timeWaitMilliseconds)
        {
            if (chekState == State.Start)
            {
                try
                {
                    chekState = State.SignIn;

                    sl.Sleep(timeWaitMilliseconds);
                    wait.Until(SCond.ElementExists(By.Name("nickname")));
                    driver.FindElement(By.Name("nickname")).SendKeys(Resources.nickname);


                    wait.Until(SCond.ElementExists(By.Name("password")));
                    driver.FindElement(By.Name("password")).SendKeys(Resources.psw);

                    driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);


                    logger.Debug(" Login as {0}", Resources.nickname);
                    wait.Until(SCond.ElementExists(By.Id("sysForm_submit")));
                    driver.FindElement(By.Id("sysForm_submit")).Click();
                }
                catch (Exception) { }

                //wait.Until(SCond.ElementExists(By.ClassName("UserNav")));
                sl.Sleep(timeWaitMilliseconds);

                if (driver.FindElements(By.XPath(string.Format("//*[text()='{0}']", "Вход"))).ToList().Count > 0)
                    logger.Debug("Refused. Metod='{0}'", "State.SignIn");
                else
                {
                    chekState = State.SignIn;
                    logger.Debug("Recept. Metod='{0}'", chekState);

                }
            }
            return chekState;
        }

        public State Subscription(int timeWaitMilliseconds)
        {
            if (chekState != State.SignIn) return chekState;

            chekState = chekState+1;
            //закрываю подписку, если предлагает               
            try
            {
                wait.Until(SCond.ElementExists(By.Id("sysNotifyInvitePopup")));
                js.ExecuteScript("$('#sysNotifyInvitePopup').dialog('close'); return false;");
                
                logger.Debug("Recept. Metod='{0}'", chekState);
            }
            catch (Exception )
            {
                logger.Debug("Recept. No subscription offered");
                chekState = chekState - 1;
            }

            return chekState;
        }

        public State ClickButtonAddPin(int timeWaitMilliseconds)
        {
            if (chekState == State.SignIn || chekState == State.Subscription)
            {

                chekState = State.ClickButtonAddPin;
                try
                {
                    //добавляю pin
                    wait.Until(SCond.ElementExists(By.ClassName("AddIcon")));
                    js.ExecuteScript("PinCreateLoader.open()");
                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = State.SignIn;
                }
            }
            return chekState;
        }

        public State ClickButtonFromInternet(int timeWaitMilliseconds)
        {
            if (chekState == State.ClickButtonAddPin)
            {

                chekState = State.ClickButtonFromInternet;
                try
                {

                    wait.Until(SCond.ElementExists(By.Id("sysPinCreatePopup")));
                    js.ExecuteScript("PinCreate.open('add')");
                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = State.ClickButtonAddPin;
                }
            }
            return chekState;
        }

        public State GetRendomAttributte()
        {
            if (chekState == State.ClickButtonFromInternet)
            {
                chekState = State.GetRendomAttributte;
                try
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.Load(AppDomain.CurrentDomain.BaseDirectory + Resources.xml);

                    // получим корневой элемент
                    XmlElement xRoot = xDoc.DocumentElement;
                    Random rnd = new Random();
                    attr = xRoot.ChildNodes[rnd.Next(0, xRoot.ChildNodes.Count)].Attributes;
                    logger.Debug("Got XmlAttribute-urltogo='{0}'", attr.GetNamedItem("urltogo").Value.ToString());
                    logger.Debug("Got XmlAttribute-urlpic='{0}'", attr.GetNamedItem("urlpic").Value.ToString());
                    logger.Debug("Got XmlAttribute-desc='{0}'", attr.GetNamedItem("desc").Value.ToString());
                    logger.Debug("Got XmlAttribute-collection='{0}'", attr.GetNamedItem("collection").Value.ToString());
                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = State.ClickButtonFromInternet;
                }
            }
            return chekState;
        }

        public State SetURL(int timeWaitMilliseconds)
        {
            if (chekState == State.GetRendomAttributte)
            {
                chekState = State.SetURL;
                try
                {
                    //из интернета;
                    wait.Until(SCond.ElementExists(By.Name("url")));
                    List<IWebElement> linksToClickUrl = driver.FindElements(By.Name("url")).ToList();

                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickUrl[linksToClickUrl.Count - 1]);
                    linksToClickUrl[linksToClickUrl.Count - 1].SendKeys(attr.GetNamedItem("urltogo").Value.ToString());

                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = State.GetRendomAttributte;
                }
            }
            return chekState;
        }


        public State ClickButtonFind(int timeWaitMilliseconds)
        {
            if (chekState == State.SetURL)
            {
                chekState = State.ClickButtonFind;
                try
                {

                    List<IWebElement> linksToClickFind = driver.FindElements(By.ClassName("wr_bordered_button")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickFind[linksToClickFind.Count - 1]);
                    linksToClickFind[linksToClickFind.Count - 1].Click();
                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = State.SetURL;
                }
            }
            return chekState;
        }


        public State SetPicture(int timeWaitMilliseconds)
        {
            if (chekState == State.ClickButtonFind)
            {
                chekState = chekState+1;
                try
                {
                    //WaitLoadPictures
                    sl.Sleep(timeWaitMilliseconds);
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until((x) =>
                    {
                        return driver.FindElements(By.XPath("//img[@src='https://cdn-nus-1.pinme.ru/asset/rele/img/icons/uploading_3.gif']")).ToList().Count > 0;
                    });

                    List<IWebElement> linksloadImg = driver.FindElements(By.XPath("//img[@src='https://cdn-nus-1.pinme.ru/asset/rele/img/icons/uploading_3.gif']")).ToList();

                    logger.Debug("Found {0} radial progress bars. Start waiting for pictures to be loaded.", linksloadImg.Count.ToString());
                    sl.Sleep(1000);

                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until((x) =>
                    {
                        return !linksloadImg[linksloadImg.Count - 1].Displayed;
                    });
                    logger.Debug("Pictures uploaded");

                    js.ExecuteScript(string.Format("document.getElementsByClassName('imgWrap')[0].getElementsByTagName('img')[0].setAttribute('src', '{0}')", attr.GetNamedItem("urlpic").Value.ToString()));
                    //logger.Debug("Set picture urlpic='{0}'", attr.GetNamedItem("urlpic").Value.ToString());

                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = chekState - 1;
                }
            }
            return chekState;
        }


        public State SelectCollection(int timeWaitMilliseconds)
        {
            if (chekState == State.SetPicture)
            {
                chekState = State.SelectCollection;
                try
                {
                    sl.Sleep(timeWaitMilliseconds);
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style='display: block;'", driver.FindElement(By.ClassName("BoardsListControl")));
                    driver.FindElement(By.XPath(string.Format("//div[@class='BoardsListControl']/ul/li[2]/ul/li/span[text()='{0}']", attr.GetNamedItem("collection").Value.ToString()))).Click();

                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = chekState-1;
                }
            }
            return chekState;
        }


        public State SetDescription(int timeWaitMilliseconds)
        {
            if (chekState == State.SelectCollection)
            {
                chekState = State.SetDescription;
                try
                {

                    sl.Sleep(timeWaitMilliseconds);
                    //wait.Until(ExpectedConditions.ElementExists(By.Id("sysForm_id_descr")));
                    List<IWebElement> linksToClickDescr = driver.FindElements(By.Id("sysForm_id_descr")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickDescr[linksToClickDescr.Count - 1]);
                    linksToClickDescr[linksToClickDescr.Count - 1].SendKeys(attr.GetNamedItem("desc").Value.ToString());

                    logger.Debug("Recept. Metod='{0}'", chekState);

                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = chekState - 1;
                }
            }
            return chekState;
        }


        public State ClickButtonChoosePicture(int timeWaitMilliseconds)
        {
            if (chekState == State.SetDescription)
            {
                chekState = State.ClickButtonChoosePicture;
                try
                {
                    sl.Sleep(timeWaitMilliseconds);
                    //wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='Выбрать']")));
                    List<IWebElement> linksToClickToChoise = driver.FindElements(By.XPath("//input[@value='Выбрать']")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickToChoise[linksToClickToChoise.Count - 1]);
                    linksToClickToChoise[linksToClickToChoise.Count - 1].Click();

                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = chekState - 1;
                }
            }
            return chekState;
        }

        public State ClickButtonSave(int timeWaitMilliseconds)
        {
            if (chekState == State.ClickButtonChoosePicture)
            {
                chekState = State.ClickButtonSave;
                try
                {
                    sl.Sleep(timeWaitMilliseconds);
                    //wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='Сохранить']")));
                    List<IWebElement> linksToClickSave = driver.FindElements(By.XPath("//input[@value='Сохранить']")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickSave[linksToClickSave.Count - 1]);
                    linksToClickSave[linksToClickSave.Count - 1].Click();

                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = chekState - 1;
                }
            }
            return chekState;
        }




        public State Close(int timeWaitMilliseconds)
        {
            if (driver != null)
            {
                chekState = State.Close;
                try
                {
                    
                    driver.Close();
                    driver.Dispose();
                    logger.Debug("Recept. Metod='{0}'", chekState);
                }
                catch (Exception ex)
                {
                    logger.Debug("Refused. Metod='{0}. Error: {1}", chekState, ex.ToString());
                    chekState = chekState - 1;
                }
            }
            return chekState;
        }

    }


}
