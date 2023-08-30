using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TMS.UserManager.Business;

namespace Reports.Function
{
    public class ManagementReportFunction
    {
        
        private readonly IReportService _reportService;

        public ManagementReportFunction(IReportService reportService)
        {
            _reportService = reportService;
        }

        [FunctionName("ManagementReportFunction")]
        public void Run([TimerTrigger("0 0 3 * * *")] TimerInfo myTimer, ILogger log)
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            try
            {
                var result = _reportService.SendManagementReportEmail();
            }
            catch (Exception ex)
            {
                log.LogInformation($"{ex.Message}");
            }
        }
    }
}
