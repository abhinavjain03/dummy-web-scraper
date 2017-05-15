using ScrapySharp.Network;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;

namespace MyScraper
{
    public class Program
    {
        public enum Language
        {
            Select, Arunachali, Assamese, Bhojpuri, Bengali, Chhatisgarhi, Dogri, English, Gojri, Gujarati, Hindi, Kannada,
            Kashmiri, Konkani, Ladakhi, Marathi, Malyalam, Malayalam, Manipuri, Nagamese, Nepali, Odia, Oriya, Punjabi, Rajasthani,
            Sanskrit, Sindhi, Tamil, Telugu, Urdu, Mizo, Lepcha, Bhutia, Maithili
        };

        private static string ConvertToRequiredFormat(string date)
        {
            return date.Replace('-', '/');
        }

        private static string GetLinkFromHtml(string html, bool whatToDo)
        {
            string link="";

            if (whatToDo)
            {
                int indexOfDataAttribute = html.IndexOf("data");
                int firstIndex = indexOfDataAttribute + 24;
                int indexOfWidthAttribute = html.IndexOf("width");
                int lastIndex = indexOfWidthAttribute - 2;
                link = html.Substring(firstIndex, lastIndex - firstIndex);
            }
            else
            {
                //Do for text
                html = html.Trim();
                int indexOfHrefAttribute = html.IndexOf("href");
                int firstIndex = indexOfHrefAttribute + 6;
                int indexOfTargetAttribute = html.IndexOf("target");
                int lastIndex = indexOfTargetAttribute - 2;
                link = "http://newsonair.com/" + html.Substring(firstIndex, lastIndex - firstIndex);
            }

            
            return link;
        }


        public int doTheWork(int nextPage, Language lang, bool whatToDo, out bool flagForTermination)
        {
            //int count = 0;
            flagForTermination = true;
            List<string> previous = null;
            List<string> current;

            int pageNum;
            //string __VIEWSTATE;
            string ago = ConvertToRequiredFormat(DateTime.Today.AddMonths(-3).AddDays(2).ToString("MM/dd/yyyy"));
            string now = ConvertToRequiredFormat(DateTime.Today.ToString("MM/dd/yyyy"));

            ScrapingBrowser virtualBrowser = new ScrapingBrowser();
            virtualBrowser.UserAgent = new FakeUserAgent("Mozilla", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:50.0) Gecko/20100101 Firefox/50.0");
            virtualBrowser.AllowAutoRedirect = true;
            virtualBrowser.AllowMetaRedirect = true;
            WebPage pageResult;
            PageWebForm form;
            if (whatToDo)
            {
                pageResult = virtualBrowser.NavigateToPage(new Uri("http://newsonair.com/Regional_NSD_Search_MP3.aspx"));
            }
            else
            {
                pageResult = virtualBrowser.NavigateToPage(new Uri("http://newsonair.com/Regional_NSD_Search_Text.aspx"));
            }


            pageNum = 1;
            while (flagForTermination)
                {
                    //__VIEWSTATE = pageResult.Html.CssSelect("#__VIEWSTATE").First().GetAttributeValue("value");
                    //Console.WriteLine(__VIEWSTATE.Length);
                    form = pageResult.FindFormById("aspnetForm");
                    // assign values to the form fields

                    Console.WriteLine("PageNo.:" + pageNum);

                    //form["ctl00$ContentPlaceHolder1$btn_submit"] = "";
                    if (pageNum == 1)
                    {
                        form["ctl00$ContentPlaceHolder1$dropdown_Bulletin"] = Convert.ToInt32(lang).ToString();
                        form["ctl00$ContentPlaceHolder1$txtDate"] = ago;
                        form["ctl00$ContentPlaceHolder1$txtDate1"] = now;
                        form["ctl00$ContentPlaceHolder1$btn_submit"] = "Search";
                        pageNum = nextPage-1;
                        //Console.WriteLine("From the first request:");
                    }

                    else
                    {
                        //form.FormFields.Remove(new FormField() { Name = "ctl00$ContentPlaceHolder1$GridView1$ctl02$imageButtonDelete"});
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$btn_submit"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl02$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl03$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl04$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl05$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl06$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl07$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl08$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl09$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl10$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$GridView1$ctl11$imageButtonDelete"));
                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$btn_reset"));

                        form.FormFields.Remove(form.FormFields.Find(x => x.Name == "ctl00$ContentPlaceHolder1$btn_submit"));
                        //form["ctl00$ContentPlaceHolder1$btn_submit"] = "";
                        form["__EVENTTARGET"] = "ctl00$ContentPlaceHolder1$GridView1";
                        form["__EVENTARGUMENT"] = "Page$" + pageNum.ToString();
                        //form["__VIEWSTATE"] = __VIEWSTATE;
                        form["ctl00_ContentPlaceHolder1_toolkit1_HiddenField"] = ";;AjaxControlToolkit,+Version=3.5.40412.0,+Culture=neutral,+PublicKeyToken=28f01b0e84b6d53e:en-US:1547e793-5b7e-48fe-8490-03a375b13a33:de1feab2:fcf0e993:f2c8e708:720a52bf:f9cec9bc:589eaa30:698129cf:fb9b4c57:ccb96cf9";
                        //Console.WriteLine("From pagination Request:");
                    }
                    form.Method = HttpVerb.Post;




                    //foreach (FormField field in form.FormFields)
                    //{
                    //    Console.WriteLine(field.Name + ":" + field.Value);
                    //}
                    //Console.WriteLine();
                    //Console.ReadKey();

                    pageResult = form.Submit();

                    current = new List<string>();
                    HtmlNode table = null;

                bool flag = false;
                try
                {
                    var errorMessage = pageResult.Html.CssSelect("#ctl00_ContentPlaceHolder1_label_error").First();
                }
                catch
                {
                    flag = true;
                }
                if (flag)
                {
                    if (pageResult.Html.InnerHtml.Contains("Please contact your system administrator"))
                    {
                        break;
                    }
                    else
                    {
                        try
                        {
                            table = pageResult.Html.CssSelect("#ctl00_ContentPlaceHolder1_GridView1").First();
                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }                    

                    foreach (var row in table.SelectNodes("tr"))
                    {
                        try
                        {
                            foreach (var cell in row.SelectNodes("td[3]"))
                            {
                                string link = GetLinkFromHtml(cell.InnerHtml, whatToDo);
                                //tableEntries.Add(GetLinkFromHtml(cell.InnerHtml));
                                
                                    Console.WriteLine(link);
                                    //saveToFile(GetLinkFromHtml(cell.InnerHtml));
                                    current.Add(link);
                                    
                                
                            }
                        }
                        catch { continue; }

                    }
                    if (previous != null)
                    {
                        if (previous.ElementAt(previous.Count - 1) == current.ElementAt(current.Count - 1))
                        {
                            flagForTermination = false;
                            continue;
                        }
                        else
                        {
                            foreach (string url in current)
                            {
                                saveToFile(url, lang, whatToDo);
                            }
                        }
                    }
                    else
                    {
                        if (nextPage == 2)
                        {
                            foreach (string url in current)
                            {
                                saveToFile(url, lang, whatToDo);
                            }
                        }
                        
                    }
                    previous = current;
                    current = null;




                    //Thread.Sleep(2000);

                    //pageNum = pageNums[i];
                    //i++;
                    pageNum++;
                }
                else
                {
                    flagForTermination = false;
                }
            }
                    

                //var table = resultPage.Html.CssSelect("#ctl00_ContentPlaceHolder1_GridView1").First();

                //foreach (var row in table.SelectNodes("tr"))
                //{
                //    try
                //    {
                //        foreach (var cell in row.SelectNodes("td[3]"))
                //        {
                //           tableEntries.Add(GetLinkFromHtml(cell.InnerHtml));
                //        }
                //    }
                //    catch(Exception ex)
                //    {
                //        continue;
                //    }

                //}

                //  __EVENTTARGET: "ctl00$ContentPlaceHolder1$GridView1";
                // __EVENTARGUMENT: "Page$1";
                //  __VIEWSTATE: "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"
                //ctl00_ContentPlaceHolder1_toolkit1_HiddenField: ";;AjaxControlToolkit,+Version=3.5.40412.0,+Culture=neutral,+PublicKeyToken=28f01b0e84b6d53e:en-US:1547e793-5b7e-48fe-8490-03a375b13a33:de1feab2:fcf0e993:f2c8e708:720a52bf:f9cec9bc:589eaa30:698129cf:fb9b4c57:ccb96cf9"


                //Console.WriteLine(Convert.ToInt32(lang));
                //Console.ReadKey();


           // }





            return pageNum;
        }



        public void saveToFile(string url, Language lang, bool whatToDo)
        {
            string path;
            //WebRequest request = WebRequest.Create(new Uri(url));
            //request.Method = "HEAD";
            /// try
            //// {
            ///     WebResponse response = request.GetResponse();
            // }
            // catch (WebException ex)
            /// {
            //   Console.WriteLine(ex.Message);
            if (whatToDo)
            {
                path = @"D:\ScrapeData\" + lang.ToString() + ".txt";
            }
            else
            {
                path = @"D:\ScrapeData\Text\" + lang.ToString() + ".txt";
            }
            
                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(url);
                    }
                }

                // This text is always added, making the file longer over time
                // if it is not deleted.
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(url);
                }
                
                ///return;
           // }
            

               // path = @"D:\ScrapeData\Working.txt";
                // This text is added only once to the file.
               // if (!File.Exists(path))
               // {
                   // Create a file to write to.
                //    using (StreamWriter sw = File.CreateText(path))
              //      {
                        //sw.WriteLine(url);
              //      }
              //  }

                // This text is always added, making the file longer over time
                // if it is not deleted.
              //  using (StreamWriter sw = File.AppendText(path))
              //  {
             //       sw.WriteLine(url);
             //   }
 


        }



        static void Main(string[] args)
        {
            int nextPage;

            Program prog = new Program();
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                if (lang == 0)
                    continue;

                bool flagForTermination = true;
                nextPage = 2;
                while (flagForTermination)
                {
                    nextPage = prog.doTheWork(nextPage, lang, true, out flagForTermination);
                    Thread.Sleep(new Random().Next(1000, 20000));
                }

            }


            prog = new Program();
            foreach (Language lang in Enum.GetValues(typeof(Language)))
            {
                if (lang == 0)
                    continue;

                bool flagForTermination = true;
                nextPage = 2;
                while (flagForTermination)
                {
                    nextPage = prog.doTheWork(nextPage, lang, false, out flagForTermination);
                    Thread.Sleep(new Random().Next(1000, 20000));
                }

            }








        }
    }
}
