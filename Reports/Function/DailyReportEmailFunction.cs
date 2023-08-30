using Cronos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using TMS.UserManager.Business;

namespace Reports
{
    public class DailyReportEmailFunction
    {

        private readonly IReportService _reportService;

        public DailyReportEmailFunction(IReportService reportService)
        {
            _reportService = reportService;
        }

        [FunctionName("DailyReportEmailFunction")]
        public void Run([TimerTrigger("0 30 2 * * *")]TimerInfo myTimer, ILogger log)
        {
            if(myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            try
            {
                var result = _reportService.SendDailyReportEmail();
            }
            catch (Exception ex)
            {
                log.LogInformation($"{ex.Message}");
            }
        }
    }
}
