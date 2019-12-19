using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace BitcoinGeneration
{
    public class WebCrawler
    {
        public static void CrawlSite(string url)
        {
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    // Or you can get the file content without saving it
                    string htmlCode = client.DownloadString(url);
                    Console.WriteLine(htmlCode);
                    if (htmlCode != "0")
                    {
                        Console.WriteLine("FOUND");
                        Console.ReadLine();
                    }
                    /*
                    // Or you can get the file content without saving it
                    string htmlCode = client.DownloadString(url);
                    //Console.WriteLine(htmlCode);
                    dynamic stuff = JsonConvert.DeserializeObject(htmlCode);
                    Console.WriteLine("Address: " + stuff.address);
                    Console.WriteLine("Total Received: " + stuff.total_received);
                    Console.WriteLine("Unconfirmed Balance: " + stuff.unconfirmed_balance);
                    if (stuff.total_received != 0)
                    {
                        Console.WriteLine("FOUND");
                        Console.ReadLine();
                    }*/
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                System.Threading.Thread.Sleep(50000);
                /*
                if (e.Message.Contains("(429)"))
                {
                    try
                    {
                        System.Threading.Thread.Sleep(5000);
                        using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                        {
                            // Or you can get the file content without saving it
                            string htmlCode = client.DownloadString(url);
                            //Console.WriteLine(htmlCode);
                            dynamic stuff = JsonConvert.DeserializeObject(htmlCode);
                            Console.WriteLine("Address: " + stuff.address);
                            Console.WriteLine("Total Received: " + stuff.total_received);
                            Console.WriteLine("Unconfirmed Balance: " + stuff.unconfirmed_balance);
                            if (stuff.total_received != 0)
                            {
                                Console.WriteLine("FOUND");
                                Console.ReadLine();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + e.Message);
                        if (e.Message.Contains("(429)"))
                        {
                            System.Threading.Thread.Sleep(50000);
                            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                            {
                                // Or you can get the file content without saving it
                                string htmlCode = client.DownloadString(url);
                                //Console.WriteLine(htmlCode);
                                dynamic stuff = JsonConvert.DeserializeObject(htmlCode);
                                Console.WriteLine("Address: " + stuff.address);
                                Console.WriteLine("Total Received: " + stuff.total_received);
                                Console.WriteLine("Unconfirmed Balance: " + stuff.unconfirmed_balance);
                                if (stuff.total_received != 0)
                                {
                                    Console.WriteLine("FOUND");
                                    Console.ReadLine();
                                }
                            }
                        }
                    }
                }
                */
            }
            
        }
    }
}
