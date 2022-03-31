using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Felicity;

internal class Jobs
{
    public static async Task StartJobs()
    {
        var scheduler = await new StdSchedulerFactory().GetScheduler();
        await scheduler.Start();

        var dailyJob = JobBuilder.Create<DailyResetJob>().WithIdentity(Guid.NewGuid().ToString()).Build();
        var dailyTrigger = TriggerBuilder.Create().WithIdentity("DailyResetTrigger").WithCronSchedule("0 15 19 ? * * *").Build();

        var weeklyJob = JobBuilder.Create<WeeklyResetJob>().WithIdentity(Guid.NewGuid().ToString()).Build();
        var weeklyTrigger = TriggerBuilder.Create().WithIdentity("WeeklyResetTrigger").WithCronSchedule("0 15 19 ? * TUE *").Build();

        await scheduler.ScheduleJob(dailyJob, dailyTrigger);
        await scheduler.ScheduleJob(weeklyJob, weeklyTrigger);
    }
}

internal class DailyResetJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Daily Reset Job - " + DateTime.Now.ToString("T"));
        await Task.CompletedTask;
    }
}

internal class WeeklyResetJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Weekly Reset Job - " + DateTime.Now.ToString("T"));
        await Task.CompletedTask;
    }
}