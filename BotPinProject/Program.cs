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
        
        static IWebDriver driver;
        static WebDriverWait wait;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        static int BotGo(int t)
        {
           //Rubricator rb = new Rubricator(driver, AppDomain.CurrentDomain.BaseDirectory);
           

            if (t == 5)
            {
                return 5;
            }
            else
            {
                logger.Debug("Попытка № {0}", t + 1);
                try
                {

                    //логинемся
                    Login(AppDomain.CurrentDomain.BaseDirectory);

                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;


                    //закрываю подписку, если предлагает               
                    try
                    {
                        wait.Until(SCond.ElementExists(By.Id("sysNotifyInvitePopup")));
                        js.ExecuteScript("$('#sysNotifyInvitePopup').dialog('close'); return false;");
                        logger.Debug("log {0}", "закрываю подписку");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug("log {0}", "Подписки нет");
                    }



                    //добавляю pin
                    wait.Until(SCond.ElementExists(By.ClassName("AddIcon")));
                    js.ExecuteScript("PinCreateLoader.open()");
                    logger.Debug("log {0}", "Нажата кнопка 'Добавить пин'");


                    //из интернета
                    //wait.Until((x) =>
                    //{
                    //    return x.FindElement(By.Id("sysPinCreatePopup")).Enabled;
                    //});
                    wait.Until(SCond.ElementExists(By.Id("sysPinCreatePopup")));
                    //wait.Until(c => c.FindElement(By.Id("sysPinCreatePopup")).Enabled);
                    js.ExecuteScript("PinCreate.open('add')");
                    logger.Debug("log {0}", "Нажата кнопка 'Из интернета'");


                    //произвольный пост
                    XmlAttributeCollection attr = GetAttr();

                    logger.Debug("log Получили атрибут с urltogo='{0}'", attr.GetNamedItem("urltogo").Value.ToString());
                    logger.Debug("log Получили атрибут с urlpic='{0}'", attr.GetNamedItem("urlpic").Value.ToString());
                    logger.Debug("log Получили атрибут с desc='{0}'", attr.GetNamedItem("desc").Value.ToString());



                    //из интернета;
                    wait.Until(SCond.ElementExists(By.Name("url")));
                    List<IWebElement> linksToClickUrl = driver.FindElements(By.Name("url")).ToList();

                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickUrl[linksToClickUrl.Count - 1]);
                    linksToClickUrl[linksToClickUrl.Count - 1].SendKeys(attr.GetNamedItem("urltogo").Value.ToString());

                    List<IWebElement> linksToClickFind = driver.FindElements(By.ClassName("wr_bordered_button")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickFind[linksToClickFind.Count - 1]);
                    linksToClickFind[linksToClickFind.Count - 1].Click();
                    logger.Debug("Начали поиск для url='{0}'", attr.GetNamedItem("urltogo").Value.ToString());


                    //sl.Sleep(10000);

                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until((x) =>
                    {
                        return driver.FindElements(By.XPath("//img[@src='https://cdn-nus-1.pinme.ru/asset/rele/img/icons/uploading_3.gif']")).ToList().Count > 0;
                    });

                    List<IWebElement> linksloadImg = driver.FindElements(By.XPath("//img[@src='https://cdn-nus-1.pinme.ru/asset/rele/img/icons/uploading_3.gif']")).ToList();
                    logger.Debug("Нашли картиноку uploading_3.gif");

                    logger.Debug("Всего количество картинок {0}", linksloadImg.Count.ToString());
                    sl.Sleep(1000);

                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    wait.Until((x) =>
                    {
                        return !linksloadImg[linksloadImg.Count - 1].Displayed;
                    });
                    logger.Debug("Дождались загрузку картинок");


                    //List<IWebElement> linksToClickimgWrap = driver.FindElements(By.ClassName("imgWrap")).ToList();
                    //linksToClickFind[0].SendKeys("ssss");
                    //driver.FindElements(By.ClassName("imgWrap")).ToList()[0].FindElement(;
                    js.ExecuteScript(string.Format("document.getElementsByClassName('imgWrap')[0].getElementsByTagName('img')[0].setAttribute('src', '{0}')", attr.GetNamedItem("urlpic").Value.ToString()));
                    sl.Sleep(1000);
                    logger.Debug("Присвоили картинке urlpic='{0}'", attr.GetNamedItem("urlpic").Value.ToString());
                    //driver.FindElement(By.CssSelector(".jcarousel-next.jcarousel-next-horizontal")).Click();

                    //for (int i = 0; i <= linksToClickimgWrap.Count - 1; i++)
                    //{
                    //    if (IsElementExists(By.XPath(string.Format("img[@src='{0}']", attr.GetNamedItem("urlpic").Value.ToString())), linksToClickimgWrap[i]))
                    //    {
                    //        findFoto = true;
                    //        break;
                    //    }
                    //    sl.Sleep(1000);
                    //    driver.FindElement(By.CssSelector(".jcarousel-next.jcarousel-next-horizontal")).Click();

                    //}
                    //findFoto = true;

                    //if (findFoto)
                    //{
                    sl.Sleep(1000);
                    //wait.Until(ExpectedConditions.ElementExists(By.Id("sysForm_id_descr")));
                    List<IWebElement> linksToClickDescr = driver.FindElements(By.Id("sysForm_id_descr")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickDescr[linksToClickDescr.Count - 1]);
                    linksToClickDescr[linksToClickDescr.Count - 1].SendKeys(attr.GetNamedItem("desc").Value.ToString());
                    logger.Debug("Присвоили описание desc='{0}'", attr.GetNamedItem("desc").Value.ToString());

                    sl.Sleep(1000);
                    //wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='Выбрать']")));
                    List<IWebElement> linksToClickToChoise = driver.FindElements(By.XPath("//input[@value='Выбрать']")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickToChoise[linksToClickToChoise.Count - 1]);
                    linksToClickToChoise[linksToClickToChoise.Count - 1].Click();
                    logger.Debug("Нажали на кнопку 'Выбрать'");


                    sl.Sleep(2000);
                    //wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='Сохранить']")));
                    List<IWebElement> linksToClickSave = driver.FindElements(By.XPath("//input[@value='Сохранить']")).ToList();
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickSave[linksToClickSave.Count - 1]);
                    linksToClickSave[linksToClickSave.Count - 1].Click();
                    logger.Debug("Нажали на кнопку 'Сохранить'");

                    sl.Sleep(10000);
                    //}
                    //else
                    //    Console.WriteLine(string.Format("битое фото в папке {0} для ссылки {1}]", attr.GetNamedItem("urltogo").Value.ToString(), attr.GetNamedItem("urlpic").Value.ToString()));


                    logger.Debug("Успешно добавлено фото");
                    driver.Close();
                    driver.Dispose();
                    return BotGo(5);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    logger.Debug("Не успешно добавлено фото");
                    driver.Close();
                    driver.Dispose();
                    return BotGo(t + 1);
                }


            }
        }


        static void Main(string[] args)
        {
            //CreateXML();
            //ClearXML();
            var r = new Rubricator(AppDomain.CurrentDomain.BaseDirectory, Resources.GoToUrl);

            if (r.SignIn(Resources.nickname, Resources.psw, 0))
            {
                r.Subscription(1000);
                r.ClickButtonAddPin(1000);
                r.ClickButtonFromInternet(1000);
                r.GetRendomAttributte();
                r.SetURL(1000);

                r.SetPicture(1000);

                r.SelectCollection(1000);
                r.SetDesc(1000);

                r.ClickButtonChoosePicture(1000);
                r.ClickButtonSave(1000);
            }
            sl.Sleep(10000);
            //BotGo(0);
            //Environment.Exit(0);

        }

        static XmlAttributeCollection GetAttr()
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(AppDomain.CurrentDomain.BaseDirectory + Resources.xml);

            // получим корневой элемент
            XmlElement xRoot = xDoc.DocumentElement;
            Random rnd = new Random();

            return xRoot.ChildNodes[rnd.Next(0, xRoot.ChildNodes.Count)].Attributes;
        }

        static void Login(string chrome)
        {
            driver = new ChromeDriver(chrome);
            driver.Navigate().GoToUrl(Resources.GoToUrl);

            logger.Debug("Перешли на сайт {0}", Resources.GoToUrl);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            wait.Until(SCond.ElementExists(By.Name("nickname")));
            
            driver.FindElement(By.Name("nickname")).SendKeys(Resources.nickname);


            wait.Until(SCond.ElementExists(By.Name("password")));
            
            driver.FindElement(By.Name("password")).SendKeys(Resources.psw);

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);

            try
            {

                logger.Debug("Пытаемся логиниться под {0}", Resources.nickname);
                wait.Until(SCond.ElementExists(By.Id("sysForm_submit")));
                driver.FindElement(By.Id("sysForm_submit")).Click();
            }
            catch (Exception ex) { }

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);


            //wait.Until((x) =>
            //{
            //    return driver.FindElements(By.ClassName("")).ToList().Count > 0;
            //});

            
            //UserNav submenu submenu_hover
        }

        //public static bool IsElementExists(By iClassName, IWebElement we)
        //{
        //    try
        //    {
        //        string i = we.FindElement(iClassName).Size.ToString();
        //        return true;
        //    }
        //    catch (NoSuchElementException)
        //    {
        //        return false;
        //    }
        //}

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
                node.Attributes.GetNamedItem("desc").Value = attDesc.Replace("2012", "");
                node.Attributes.GetNamedItem("desc").Value = attDesc.Replace("2013", "");
                node.Attributes.GetNamedItem("desc").Value = attDesc.Replace("2014", "");
                node.Attributes.GetNamedItem("desc").Value = attDesc.Replace("2015", "");
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


        //Dictionary<int, string> PinCollection = new Dictionary<int, string>();
        //1464412" max_pins="">Прически с косами</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464406" pins="1464406" max_pins="">Проблемы волос</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464408" pins="1464408" max_pins="">Ретро прически</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464396" pins="1464396" max_pins="">Рыжие прически и окрашивание</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464413" pins="1464413" max_pins="">Советы и бьюти лайф хаки</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="690753" pins="690753" max_pins="">Средние Волосы</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464407" pins="1464407" max_pins="">Стрижки для тех, кому за 50</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464409" pins="1464409" max_pins="">Тренды</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464398" pins="1464398" max_pins="">Уход за волосами советы и подборки</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464393" pins="1464393" max_pins="">Хвост</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464401" pins="1464401" max_pins="">Хорошие видео уроки</span></li><li class=""><em></em><span onclick = "javascript: return PinCreate.changeSub(this);" value="1464414" pins="1464414" max_pins="">Челки</span></li></ul>

        public Rubricator(string chromeDriverDirectory, string url)
        {
            logger = LogManager.GetCurrentClassLogger();
            driver = new ChromeDriver(chromeDriverDirectory);
            js = (IJavaScriptExecutor)driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            driver.Navigate().GoToUrl(url);

            logger.Debug(" Went to the {0}", Resources.GoToUrl);
        }

        public bool SignIn( String nickname, String password, int timeWaitMilliseconds)
        {

            wait.Until(SCond.ElementExists(By.Name("nickname")));
            driver.FindElement(By.Name("nickname")).SendKeys(Resources.nickname);


            wait.Until(SCond.ElementExists(By.Name("password")));
            driver.FindElement(By.Name("password")).SendKeys(Resources.psw);

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);

            try
            {

                logger.Debug(" Login as {0}", Resources.nickname);
                wait.Until(SCond.ElementExists(By.Id("sysForm_submit")));
                driver.FindElement(By.Id("sysForm_submit")).Click();
            }
            catch (Exception ex) { }

            //wait.Until(SCond.ElementExists(By.ClassName("UserNav")));
            sl.Sleep(1000);
            return driver.FindElements(By.ClassName("UserNav")).ToList().Count > 0;
        }

        public void Subscription(int timeWaitMilliseconds)
        {
            //закрываю подписку, если предлагает               
            try
            {
                wait.Until(SCond.ElementExists(By.Id("sysNotifyInvitePopup")));
                js.ExecuteScript("$('#sysNotifyInvitePopup').dialog('close'); return false;");
                logger.Debug("Recept. Closed Subscription");
            }
            catch (Exception ex)
            {
                logger.Debug("Recept. No subscription offered");
            }
        }

        public bool ClickButtonAddPin(int timeWaitMilliseconds)
        {
            try
            {
                //добавляю pin
                wait.Until(SCond.ElementExists(By.ClassName("AddIcon")));
                js.ExecuteScript("PinCreateLoader.open()");
                logger.Debug("Recept. Metod='ClickButtonAddPin'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='ClickButtonAddPin'. Error: {0}", ex.ToString());
                return false;
            }
        }

        public bool ClickButtonFromInternet(int timeWaitMilliseconds)
        {
            try
            {
                
                wait.Until(SCond.ElementExists(By.Id("sysPinCreatePopup")));
                js.ExecuteScript("PinCreate.open('add')");
                logger.Debug("Recept. Metod='ClickButtonFromInternet'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal.. Metod='ClickButtonFromInternet'. Error: {0}", ex.ToString());
                return false;
            }
        }

        public bool GetRendomAttributte()
        {
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
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='GetRendomAttributte'. Error: {0}", ex.ToString());
                return false;
            }

        }

        public bool SetURL(int timeWaitMilliseconds)
        {
            try
            { 
                //из интернета;
                wait.Until(SCond.ElementExists(By.Name("url")));
                List<IWebElement> linksToClickUrl = driver.FindElements(By.Name("url")).ToList();

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickUrl[linksToClickUrl.Count - 1]);
                linksToClickUrl[linksToClickUrl.Count - 1].SendKeys(attr.GetNamedItem("urltogo").Value.ToString());

                List<IWebElement> linksToClickFind = driver.FindElements(By.ClassName("wr_bordered_button")).ToList();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickFind[linksToClickFind.Count - 1]);
                linksToClickFind[linksToClickFind.Count - 1].Click();
                logger.Debug("Recept. Metod='SetURL'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='SetURL'. Error: {0}", ex.ToString());
                return false;
            }

        }



        public bool SetPicture(int timeWaitMilliseconds)
        {

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
                logger.Debug("Set picture urlpic='{0}'", attr.GetNamedItem("urlpic").Value.ToString());



                logger.Debug("Recept. Metod='SetPicture'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='SetPicture'. Error: {0}", ex.ToString());
                return false;
            }

        }


        public bool SetDesc(int timeWaitMilliseconds)
        {

            try
            {

                sl.Sleep(timeWaitMilliseconds);
                //wait.Until(ExpectedConditions.ElementExists(By.Id("sysForm_id_descr")));
                List<IWebElement> linksToClickDescr = driver.FindElements(By.Id("sysForm_id_descr")).ToList();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickDescr[linksToClickDescr.Count - 1]);
                linksToClickDescr[linksToClickDescr.Count - 1].SendKeys(attr.GetNamedItem("desc").Value.ToString());
                logger.Debug("Set desc='{0}'", attr.GetNamedItem("desc").Value.ToString());

                logger.Debug("Recept. Metod='SetDesc'");

                //((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style='display: block;'", driver.FindElement(By.ClassName("BoardsListControl")));

                //List<IWebElement> Collections = driver.FindElements(By.XPath("//div[@class='BoardsListControl']/ul/li[2]/ul/li/span")).ToList();
                
                //foreach (IWebElement Collection in Collections)
                //{
                //    PinCollection.Add(Convert.ToInt32(Collection.GetAttribute("value")), Collection.Text);
                //}

                    return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='SetDesc'. Error: {0}", ex.ToString());
                return false;
            }

        }


        public bool SelectCollection(int timeWaitMilliseconds)
        {

            try
            {
                sl.Sleep(timeWaitMilliseconds);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].style='display: block;'", driver.FindElement(By.ClassName("BoardsListControl")));
                driver.FindElement(By.XPath(string.Format("//div[@class='BoardsListControl']/ul/li[2]/ul/li/span[text()='{0}']", attr.GetNamedItem("collection").Value.ToString()))).Click();

                

                logger.Debug("Recept. Metod='SelectCollection'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='SelectCollection'. Error: {0}", ex.ToString());
                return false;
            }

        }


        public bool ClickButtonChoosePicture(int timeWaitMilliseconds)
        {
            try
            {
                sl.Sleep(timeWaitMilliseconds);
                //wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='Выбрать']")));
                List<IWebElement> linksToClickToChoise = driver.FindElements(By.XPath("//input[@value='Выбрать']")).ToList();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickToChoise[linksToClickToChoise.Count - 1]);
                linksToClickToChoise[linksToClickToChoise.Count - 1].Click();
                logger.Debug("Press the 'Выбрать' button.");

                logger.Debug("Recept. Metod='ClickButtonChoosePicture'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='ClickButtonChoosePicture'. Error: {0}", ex.ToString());
                return false;
            }
        }

        public bool ClickButtonSave(int timeWaitMilliseconds)
        {

            try
            {

                sl.Sleep(timeWaitMilliseconds);
                //wait.Until(ExpectedConditions.ElementExists(By.XPath("//input[@value='Сохранить']")));
                List<IWebElement> linksToClickSave = driver.FindElements(By.XPath("//input[@value='Сохранить']")).ToList();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].setAttribute('class','vote-link up voted')", linksToClickSave[linksToClickSave.Count - 1]);
                linksToClickSave[linksToClickSave.Count - 1].Click();

                logger.Debug("Press the 'Сохранить' button.");

                logger.Debug("Recept. Metod='Save'");
                return true;
            }
            catch (Exception ex)
            {
                logger.Debug("Refusal. Metod='Save'. Error: {0}", ex.ToString());
                return false;
            }

        }

    }


}
