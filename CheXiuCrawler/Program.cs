using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POPO.Http.Helper;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace CheXiuCrawler
{
    class Program
    {
        static string sessionkey_value = string.Empty;
        // 匹配url路径的正则表达式
        static string reg = @"(/[A-Za-z]+/[A-Za-z]+.[A-Za-z]+)(\??[A-Za-z0-9]+)?";
        static void Main(string[] args)
        {
            Console.WriteLine("starting crawler");

            sessionkey_value = "8sbuph93pbkbstqk00u4nvokg1";
            // 存储url的列表
            List<string> urlList = new List<string>();
            List<string> otherList = new List<string>();

            //首页
            string homeUrl = @"http://zhidian.chexiu.cn/";

            Console.WriteLine("geting the home page");
            string homePage = GetPage(homeUrl);

            Console.WriteLine("matching the url");
            // 获取url列表
            urlList.AddRange(GetUrl(homePage));

            System.Threading.Thread.Sleep(3000);
            foreach (var item in urlList)
            {
                Console.WriteLine("current url is " + item);
                if (item.Contains("css")|| item.Contains("js")|| item.Contains("assets"))
                {
                    continue;
                }
                string page = GetPage("http://zhidian.chexiu.cn"+item);
                System.Threading.Thread.Sleep(3000);
                foreach (var li in GetUrl(page))
                {
                    if (!urlList.Contains(li))
                    {
                        otherList.Add(li);
                    }
                }

                WritePage(page, item);
            }
        }

        static string GetPage(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Console.WriteLine("The url is null or empty, break from this fun");
                return null;
            }

            CookieContainer cc = new CookieContainer();

            Cookie c = new Cookie();
            c.Domain = "zhidian.chexiu.cn";
            c.Path = "/";
            c.HttpOnly = true;
            c.Name = "market_session";
            c.Value = sessionkey_value;

            cc.Add(c);

            string page = HttpHelper.AccessURL_GET(url,cc);

            return page;
        }

        static List<string> GetUrl(string page)
        {
            List<string> urlList = new List<string>();
            if (string.IsNullOrEmpty(page))
            {
                Console.WriteLine("the page is null or empty, and break from this fun");
                return null;
            }

            Regex regex = new Regex(reg);
            var matchs = regex.Matches(page);
            foreach (Match item in matchs)
            {
                urlList.Add(item.Groups[1].ToString());
            }

            return urlList;
        }

        static void WritePage(string page, string url)
        {
            string dir = "D://Page/";

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string[] array = url.Split('/');
            dir += array[1];

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string path = dir + "/" + array[2].Replace("?","_").Replace("=","_")+".html";

            FileStream fs = new FileStream(path,FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);

            Console.WriteLine("writing "+path);

            sw.WriteLine(page);
            sw.Flush();
            sw.Close();

            Console.WriteLine("finishing " + path);
        }

    }
}
