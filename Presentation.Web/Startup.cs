using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Core.ApplicationServices;
using Core.ApplicationServices.Quartz;
using Microsoft.Owin;
using Ninject;
using Owin;
using Quartz;
using Quartz.Impl;

[assembly: OwinStartup(typeof(OS2Indberetning.Startup))]

namespace OS2Indberetning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
            ConfigureAuth(app);
            ScheduleQuartzTask();
        }

        private void ScheduleQuartzTask()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            var job = JobBuilder.Create<QuartzMailJob>().Build();

            var trigger =
                TriggerBuilder.Create().WithIdentity("mailTrigger")
                    .WithDailyTimeIntervalSchedule(
                        s =>
                            s.WithIntervalInHours(24).OnEveryDay().StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(6, 0)))
                    .Build();

           

            scheduler.ScheduleJob(job, trigger);

        }


    }
}
