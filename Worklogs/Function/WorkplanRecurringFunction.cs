using Cronos;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using TMS.UserManager.Business;

namespace Worklogs
{
    public class WorkplanRecurringFunction
    {
        private readonly ITimeLogService _timeLogService;

        public WorkplanRecurringFunction(ITimeLogService TimeLogService)
        {
            _timeLogService = TimeLogService;
        }

        [FunctionName("WorkplanRecurringFunction")]
        public void Run([TimerTrigger("0 0 1 * * *")] TimerInfo myTimer, ILogger log)
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }

            log.LogInformation($"WorkplanRecurringFunction Timer trigger function executed at: {DateTime.UtcNow}");
            try
            {
                var workPlanList = _timeLogService.GetAllWorplanRecurringByDate();

                if (workPlanList != null)
                {
                    foreach (var item in workPlanList)
                    {
                        CronExpression expression = CronExpression.Parse(item.RecurringCronExp);

                        DateTime? nextUtc = expression.GetNextOccurrence(DateTime.UtcNow.Date,TimeZoneInfo.Utc, true);//including today

                        if (nextUtc.HasValue)
                        {
                            var check = nextUtc.Value.Date.CompareTo(DateTime.UtcNow.Date);

                            if (check == 0) // value = 0 date matches
                            {
                                var updatedWorkPlan = _timeLogService.WorkplanRecurringOperations(item.WorkPlanId, DateTime.UtcNow);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"{ex.Message}");
            }

        }
    }
    }
