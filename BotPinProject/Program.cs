﻿using BotPin.Properties;
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

namespace BotPin
{
    class Program
    {

        static IWebDriver driver;
        static WebDriverWait wait;

        private static Logger logger = LogManager.GetCurrentClassLogger();


        static int BotGo(int t)
        {


            if (t == 5)
            {
                return 5;
            }
            else
            {
                logger.Debug("  Попытка № {0}", t+1);
                try
                {

                    //логинемся
                    Login(AppDomain.CurrentDomain.BaseDirectory);

                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;


                    //закрываю подписку, если предлагает               
                    try
                    {
                        wait.Until(ExpectedConditions.ElementExists(By.Id("sysNotifyInvitePopup")));
                        js.ExecuteScript("$('#sysNotifyInvitePopup').dialog('close'); return false;");
                        logger.Debug("log {0}", "закрываю подписку");
                    }
                    catch (Exception ex)
                    {
                        logger.Debug("log {0}", "Подписки нет");
                    }



                    //добавляю pin
                    wait.Until(ExpectedConditions.ElementExists(By.ClassName("AddIcon")));
                    js.ExecuteScript("PinCreateLoader.open()");
                    logger.Debug("log {0}", "Нажата кнопка 'Добавить пин'");


                    //из интернета
                    //wait.Until((x) =>
                    //{
                    //    return x.FindElement(By.Id("sysPinCreatePopup")).Enabled;
                    //});
                    wait.Until(ExpectedConditions.ElementExists(By.Id("sysPinCreatePopup")));
                    js.ExecuteScript("PinCreate.open('add')");
                    logger.Debug("log {0}", "Нажата кнопка 'Из интернета'");


                    //произвольный пост
                    XmlAttributeCollection attr = getAttr();

                    logger.Debug("log Получили атрибут с urltogo='{0}'", attr.GetNamedItem("urltogo").Value.ToString());
                    logger.Debug("log Получили атрибут с urlpic='{0}'", attr.GetNamedItem("urlpic").Value.ToString());
                    logger.Debug("log Получили атрибут с desc='{0}'", attr.GetNamedItem("desc").Value.ToString());



                    //из интернета;
                    wait.Until(ExpectedConditions.ElementExists(By.Name("url")));
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
                    return BotGo(t+1);
                }


            }
        }


        static void Main(string[] args)
        {

            BotGo(0);
            Environment.Exit(0);

        }

        

        static XmlAttributeCollection getAttr()
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

            wait.Until(ExpectedConditions.ElementExists(By.Name("nickname")));
            driver.FindElement(By.Name("nickname")).SendKeys(Resources.nickname);


            wait.Until(ExpectedConditions.ElementExists(By.Name("password")));
            driver.FindElement(By.Name("password")).SendKeys(Resources.psw);

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(15);

            try
            {

                logger.Debug("Пытаемся логиниться под {0}", Resources.nickname);
                wait.Until(ExpectedConditions.ElementExists(By.Id("sysForm_submit")));
                driver.FindElement(By.Id("sysForm_submit")).Click();
            }
            catch (Exception ex){}

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(5);
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
            string att1, att2;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(AppDomain.CurrentDomain.BaseDirectory + Resources.xml);

            XmlNodeList nodes = xDoc.GetElementsByTagName("pic");

            

            StringBuilder xmlStr = new StringBuilder(@"<?xml version='1.0' encoding='windows-1251'?><root>");

            foreach (XmlNode node in nodes)
            {

                att1 = node.Attributes.GetNamedItem("urlpic").Value.ToString();

                att2 = node.Attributes.GetNamedItem("desc").Value.ToString();

                if (!Regex.IsMatch(att1, @"\p{IsCyrillic}") & !Regex.IsMatch(att1, @"%") & !Regex.IsMatch(att2, @"2011") & !Regex.IsMatch(att2, @"2012") & !Regex.IsMatch(att2, @"2013") & !Regex.IsMatch(att2, @"2014") & !Regex.IsMatch(att2, @"2015"))
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
}