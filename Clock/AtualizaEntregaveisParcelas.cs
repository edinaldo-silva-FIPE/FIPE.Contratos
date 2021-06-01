using ApiFipe.Models;
using ApiFipe.Models.Context;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace ApiFipe.Clock
{
    public class AtualizaEntregaveisParcelas
    {
        public static async Task Run()
        {
            try
            {
                // Grab the Scheduler instance from the Factory 
                IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;

                // and start it off
                scheduler.Start();

                // define the job and tie it to our InserirPlanoJob class
                IJobDetail job = JobBuilder.Create<AtualizaEntregaveisParcelasJob>()
                    .Build();

                // Trigger the job to run now, and then repeat every day
                ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                        (s =>
                            s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(1,0))
                        ).Build();

                // Tell quartz to schedule the job using our trigger
                scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
            }
        }

    }

    public class AtualizaEntregaveisParcelasJob : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            using (var db = new FIPEContratosContext())
            {
                var strategy = db.Database.CreateExecutionStrategy();

                strategy.Execute(() =>
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var listaEntregaveis = new bEntregavel(db).ObterTodosEntregaiveis();
                            var listaParcelas = new bParcela(db).ObterTodasParcelas();

                            foreach (var entregavel in listaEntregaveis)
                            {
                                if (DateTime.Now.Date > entregavel.DtProduto)
                                {
                                    if (entregavel.IdSituacao != 89 && entregavel.IdSituacao != 90 && entregavel.IdSituacao != 91)
                                    {
                                        entregavel.IcAtraso = true;
                                        if (entregavel.IdSituacao == 56)
                                        {
                                            entregavel.IdSituacao = 68;
                                        }
                                    }
                                }
                            }

                            foreach (var parcela in listaParcelas)
                            {
                                if (DateTime.Now.Date > parcela.DtFaturamento && parcela.IdSituacao == 92)
                                {
                                    parcela.IdSituacao = 93;
                                    parcela.IcAtraso = true;
                                }
                            }

                            db.SaveChanges();
                            db.Database.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                        }

                    }

                    return Task.CompletedTask;
                });

                return Task.CompletedTask;
            }
        }
    }
}
