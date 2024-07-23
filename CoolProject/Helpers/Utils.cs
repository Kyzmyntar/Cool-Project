using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using CheckingPaymentService;
using System.Text;
using System.Security.Principal;

namespace CoolProject
{
    public static class Utils
    {

        //A method to save the response in XML format
        public static void SaveResponseAsXml(string response, string filePath)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(response);
                doc.Save(filePath);
            }
            catch (Exception e)
            {
                LogHelper.WriteEvent($"Error saving XML: {e.Message}", EventType.Error);
            }
        }

        //A method to save the response in JSON format 
        public static void SaveResponseAsJson(string response, string filePath)
        {
            try
            {
                File.WriteAllText(filePath, response);
            }
            catch (Exception e)
            {
                LogHelper.WriteEvent($"Error saving JSON: {e.Message}", EventType.Error);
            }
        }

        //A method to save the response in CSV format
        public static void SaveResponseAsCsv(string response, string filePath)
        {
            try
            {
                var records = JsonConvert.DeserializeObject<List<ExchangeRate>>(response);

                using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(true)))
                {
                    writer.WriteLine("r030;txt;rate;cc;exchangedate");

                    foreach (var record in records)
                    {
                        writer.WriteLine($"{record.r030};{record.txt};{record.rate.ToString(CultureInfo.InvariantCulture)};{record.cc};{record.exchangedate}");
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteEvent($"Error saving CSV: {e.Message}", EventType.Error);
            }
        }


        //Check for administrator access
        public static bool IsRunAsAdmin()
        {
            var wi = WindowsIdentity.GetCurrent();
            var wp = new WindowsPrincipal(wi);
            return wp.IsInRole(WindowsBuiltInRole.Administrator);
        }

        //Parse passed arguments in
        public static Dictionary<string, string> ParseArguments(string[] args)
        {
            var commands = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    string key = args[i];
                    string value = null;

                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        value = args[i + 1];
                        i++;
                    }

                    commands[key] = value;
                }
            }
            return commands;
        }

    }
}
