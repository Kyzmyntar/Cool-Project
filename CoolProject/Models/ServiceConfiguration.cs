using System;

namespace CoolProject
{
    public class ServiceConfiguration
    {
        public string filePath { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public string fileType { get; set; } = "xml";
        public string urlNBUByPeriod { get; set; } = "https://bank.gov.ua/NBU_Exchange/exchange_site";
        public string urlNBUByDate { get; set; } = "https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange";
        public DateTime startDate { get; set; } = DateTime.Now;
        public DateTime endDate { get; set; } = DateTime.Now;
        public string valcode { get; set; }
        public int execInterval { get; set; } = 20000;

    }
}
