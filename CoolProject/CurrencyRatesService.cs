using System;
using System.ServiceProcess;
using System.Timers;
using System.Threading.Tasks;
using CheckingPaymentService;
using IniParser;
using System.IO;
using IniParser.Model;


namespace CoolProject
{
    public partial class CurrencyRatesService : ServiceBase
    {
        private Timer timer;
        private HttpRequestHandler requestHandler = new HttpRequestHandler();
        private ServiceConfiguration configData = new ServiceConfiguration();
        private static readonly string configPath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";

        private readonly FileIniDataParser parser = new FileIniDataParser();

        public CurrencyRatesService()
        {
            InitializeComponent();
        }

        //When starting the service, we start the timer with the execution period
        protected override void OnStart(string[] args)
        {
            LogHelper.WriteEvent("Service Started!", EventType.Info);
            timer = new Timer(configData.execInterval);
            timer.Elapsed += async (sender, e) => await OnTimer();
            timer.Start();
        }

        protected override void OnStop()
        {
            LogHelper.WriteEvent("Service Stoped!", EventType.Info);
            timer.Stop();
        }


        //pull all the parameters from the configuration file
        private void LoadConfiguration()
        {
            try
            {
                var data = parser.ReadFile(configPath);
                if (data == null)
                {
                    LogHelper.WriteEvent("Can`t parse config.ini file", EventType.Error);
                    return;
                }

                /*foreach (var section in data.Sections)
                {
                    LogHelper.WriteEvent($"Section: {section.SectionName}", EventType.Error);

                    foreach (var key in section.Keys)
                    {
                        LogHelper.WriteEvent($"{key.KeyName} = {key.Value}", EventType.Error);
                    }

                }*/


                //-------------------------------------------Section `Settings`------------------------------------------------------------------------
                if (data.Sections.ContainsSection("Settings"))
                {
                    if (data["Settings"].ContainsKey("filePath"))
                    {
                        if (!string.IsNullOrEmpty(data["Settings"]["filePath"]))
                        {
                            configData.filePath = data["Settings"]["filePath"];
                            LogHelper.WriteEvent($"File Path: {configData.filePath}", EventType.Info);
                        }
                    }
                    else
                    {
                        LogHelper.WriteEvent("filePath not found in [Settings] section.", EventType.Error);
                    }


                    if (data["Settings"].ContainsKey("fileType"))
                    {
                        if (!string.IsNullOrEmpty(data["Settings"]["fileType"]))
                        {
                            configData.fileType = data["Settings"]["fileType"].ToLower();
                            LogHelper.WriteEvent($"File Type: {configData.fileType}", EventType.Info);
                        }
                    }
                    else
                    {
                        LogHelper.WriteEvent("fileType not found in [Settings] section.", EventType.Error);
                    }

                    if (data["Settings"].ContainsKey("execInterval"))
                    {
                        if (!string.IsNullOrEmpty(data["Settings"]["execInterval"]))
                        {
                            configData.execInterval = int.Parse(data["Settings"]["execInterval"].ToString()) *1000;
                            LogHelper.WriteEvent($"Execution interval: {configData.execInterval}", EventType.Info);
                            timer.Interval = configData.execInterval;
                        }
                    }
                    else
                    {
                        LogHelper.WriteEvent("fileType not found in [Settings] section.", EventType.Error);
                    }
                }
                else
                {

                    LogHelper.WriteEvent("There is no [Settings] section in the ini file", EventType.Info);
                }


                //-------------------------------------------Section `RequestData`------------------------------------------------------------------------
                if (data.Sections.ContainsSection("RequestData"))
                {
                    if (data["RequestData"].ContainsKey("urlNBUByPeriod"))
                    {
                        if (!string.IsNullOrEmpty(data["RequestData"]["urlNBUByPeriod"]))
                        {
                            configData.urlNBUByPeriod = data["RequestData"]["urlNBUByPeriod"];
                            LogHelper.WriteEvent($"URL NBU By Period: {configData.urlNBUByPeriod}", EventType.Info);
                        }
                    }
                    else
                    {
                        LogHelper.WriteEvent("urlNBUByPeriod not found in [RequestData] section.", EventType.Error);
                    }

                    if (data["RequestData"].ContainsKey("urlNBUByDate"))
                    {
                        if (!string.IsNullOrEmpty(data["RequestData"]["urlNBUByDate"]))
                        {
                            configData.urlNBUByDate = data["RequestData"]["urlNBUByDate"];
                            LogHelper.WriteEvent($"URL NBU By Date: {configData.urlNBUByDate}",  EventType.Info);
                        }
                    }
                    else
                    {
                        LogHelper.WriteEvent("urlNBUByDate not found in [RequestData] section.", EventType.Error);
                    }

                    if (data["RequestData"].ContainsKey("startDate"))
                    {
                        configData.startDate = DateTime.ParseExact(data["RequestData"]["startDate"], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        LogHelper.WriteEvent($"Start Date: {configData.startDate.Date}", EventType.Info);
                    }
                    else
                    {
                        configData.startDate = DateTime.Now;
                        LogHelper.WriteEvent("startDate not found in [RequestData] section.", EventType.Error);
                    }

                    if (data["RequestData"].ContainsKey("valcode"))
                    {
                        configData.valcode = data["RequestData"]["valcode"];
                        LogHelper.WriteEvent($"Valcode: {configData.valcode}", EventType.Info);
                    }
                    else
                    {
                        configData.valcode = string.Empty;
                        LogHelper.WriteEvent("valcode not found in [RequestData] section.", EventType.Error);
                    }



                    if (data["RequestData"].ContainsKey("endDate"))
                    {
                        configData.endDate = DateTime.ParseExact(data["RequestData"]["endDate"], "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                        LogHelper.WriteEvent($"End Date: {configData.endDate.Date}", EventType.Info);
                    }
                    else
                    {
                        configData.endDate = DateTime.Now;
                        LogHelper.WriteEvent("endDate not found in [RequestData] section.", EventType.Error);
                    }
                }
                else
                {
                    LogHelper.WriteEvent("There is no [RequestData] section in the ini file", EventType.Info);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteEvent($"Config file parse ERROR: {e.Message}", EventType.Error);
            }
        }


        public static void UpdateParameter(string section, string key, string newValue)
        {
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(configPath);

                //Adding a section if it doesn't exist
                if (!data.Sections.ContainsSection(section))
                {
                    data.Sections.AddSection(section);
                    LogHelper.WriteEvent($"Section '{section}' was not found and has been added.", EventType.Info);
                }

                // Adding a key if it doesn't exist
                if (!data[section].ContainsKey(key))
                {
                    data[section].AddKey(key, newValue);
                    LogHelper.WriteEvent($"Key '{key}' was not found in section '{section}' and has been added with value '{newValue}'.", EventType.Info);
                }
                else
                {
                    data[section][key] = newValue;
                    LogHelper.WriteEvent($"Successfully updated '{key}' in section '{section}' to '{newValue}'.", EventType.Info);
                }

                parser.WriteFile(configPath, data);
            }
            catch (Exception ex)
            {
                LogHelper.WriteEvent($"Error updating INI file: {ex.Message}", EventType.Error);
            }
        }


        //On a timer, we start reading and saving currency rates to a file
        private async Task OnTimer()
        { 
            LoadConfiguration();
            string response = await requestHandler.SendGetRequestAsync(GenerateUrl());
            
            if (response != null)
            {
                if (!Directory.Exists(configData.filePath))
                {
                    Directory.CreateDirectory(configData.filePath);
                }
                switch (configData.fileType)
                {
                    case "xml":
                        Utils.SaveResponseAsXml(response, configData.filePath + "CurrencyRates.xml");
                        break;
                    case "json":
                        Utils.SaveResponseAsJson(response, configData.filePath + "CurrencyRates.json");
                        break;
                    case "csv":
                        Utils.SaveResponseAsCsv(response, configData.filePath + "CurrencyRates.csv");
                        break;

                }
            }
        }

        //generate links depending on the parameters in the configuration file
        private string GenerateUrl()
        {
            string geberatedUrl = configData.urlNBUByDate;
            if(configData.startDate!=null)
            {
                if(configData.endDate != null)
                {
                    if (configData.startDate.Date < configData.endDate.Date)
                    {
                        geberatedUrl = configData.urlNBUByPeriod + "?start=" + configData.startDate.ToString("yyyyMMdd") + "&end=" + configData.endDate.ToString("yyyyMMdd") + "&sort=exchangedate&order=desc";
                    }
                }
                else
                {
                    geberatedUrl = configData.urlNBUByDate + "?date=" + configData.startDate.ToString("yyyyMMdd");
                }
            }


            if (!string.IsNullOrEmpty(configData.fileType) && (configData.fileType.Equals("json") || configData.fileType.Equals("csv")))
            {
                if (geberatedUrl.Contains("?"))
                {
                    geberatedUrl += "&json";
                }
                else
                {
                    geberatedUrl += "?json";
                }
            }


            if (!string.IsNullOrEmpty(configData.valcode))
            {
                if (geberatedUrl.Contains("?"))
                {
                    geberatedUrl += "&valcode=" + configData.valcode;
                }
                else
                {
                    geberatedUrl += "?valcode=" + configData.valcode;
                }
            }


            LogHelper.WriteEvent($"Generated url:{geberatedUrl}", EventType.Info);
            return geberatedUrl;
        }
    }
}
