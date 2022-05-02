using System;
using System.IO;
using System.Threading.Tasks;
using Felicity.Configs;
using Felicity.Services;
using Quartz;
using Quartz.Impl;

// ReSharper disable ClassNeverInstantiated.Global

namespace Felicity;

internal static class Jobs
{
    public static async Task StartJobs()
    {
        var scheduler = await new StdSchedulerFactory().GetScheduler();
        await scheduler.Start();

        var registrationJob = JobBuilder.Create<RegistrationJob>().WithIdentity(Guid.NewGuid().ToString()).Build();
        var registrationTrigger =
            TriggerBuilder.Create().WithIdentity("RegistrationTrigger").WithCronSchedule("0 * * ? * * *").Build();

        var dailyJob = JobBuilder.Create<DailyResetJob>().WithIdentity(Guid.NewGuid().ToString()).Build();
        var dailyTrigger = TriggerBuilder.Create().WithIdentity("DailyResetTrigger").WithCronSchedule("0 15 19 ? * * *").Build();

        var weeklyJob = JobBuilder.Create<WeeklyResetJob>().WithIdentity(Guid.NewGuid().ToString()).Build();
        var weeklyTrigger = TriggerBuilder.Create().WithIdentity("WeeklyResetTrigger").WithCronSchedule("0 15 19 ? * TUE *").Build();

        await scheduler.ScheduleJob(registrationJob, registrationTrigger);
        await scheduler.ScheduleJob(dailyJob, dailyTrigger);
        await scheduler.ScheduleJob(weeklyJob, weeklyTrigger);
    }
}

internal class RegistrationJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var time = DateTime.Now;
        Console.WriteLine($"Registration Job - {time:T}");

        if (time.Minute is 0 or 15 or 30 or 45) 
            StatusService.ChangeGame();

        foreach (var enumerateFile in Directory.EnumerateFiles("Users"))
        {
            var user = OAuthConfig.FromJson(await File.ReadAllTextAsync(enumerateFile));
            if (user.DestinyMembership != null)
                continue;

            if (user.AccessToken != null)
                await OAuthService.PopulateDestinyMembership(enumerateFile, user);
        }
        
        await Task.CompletedTask;
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