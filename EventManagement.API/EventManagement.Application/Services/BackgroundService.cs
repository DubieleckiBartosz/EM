using System;
using System.Collections.Generic;
using System.Linq;
using EventManagement.Application.Contracts;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Features.EventFeatures.Commands.MarkAsRealizedEvent;
using EventManagement.Application.Strings;
using Hangfire;
using Hangfire.Storage.Monitoring;
using MediatR;

namespace EventManagement.Application.Services
{
    public class BackgroundService : IBackgroundService
    {
        private readonly IBackgroundJobClient _jobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IUserRepository _userRepository;
        private readonly IPerformanceProposalRepository _performanceProposalRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerManager<BackgroundService> _loggerManager;

        public BackgroundService(IBackgroundJobClient jobClient, IRecurringJobManager recurringJobManager,
            IUserRepository userRepository, IPerformanceProposalRepository performanceProposalRepository,
            IMediator mediator, ILoggerManager<BackgroundService> loggerManager)
        {
            this._jobClient = jobClient ?? throw new ArgumentNullException(nameof(jobClient));
            this._recurringJobManager =
                recurringJobManager ?? throw new ArgumentNullException(nameof(recurringJobManager));
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._performanceProposalRepository = performanceProposalRepository ??
                                                  throw new ArgumentNullException(
                                                      nameof(performanceProposalRepository));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
        }

        public void StartJobs()
        {
            this.ClearTokensRecurringJob();
            this.MarkAsDeletedExpiredProposals();
        }

        private void ClearTokensRecurringJob()
        {
            this._recurringJobManager.AddOrUpdate(Constants.ClearTokensRecurringJob,
                () => this._userRepository.ClearTokens(),
                "0 */12 * * *", TimeZoneInfo.Local);
        }

        private void MarkAsDeletedExpiredProposals()
        {
            this._recurringJobManager.AddOrUpdate(Constants.MarkAsDeletedExpiredProposalsRecurringJob,
                () => this._performanceProposalRepository.RemoveExpiredProposalsAsync(),
                "0 */3 * * *", TimeZoneInfo.Local);
        }


        public void DeleteChangeStatusToRealizedScheduleJob(int eventId)
        {
            var jobsProcessing = this.GetScheduledJob(Constants.MethodSend);
            foreach (var job in jobsProcessing)
            {
                var param = (MarkAsRealizedCommand) job.Value.Job.Args[0];
                if (param.EventId == eventId)
                {
                    BackgroundJob.Delete(job.Key);
                    this._loggerManager.LogInformation(new
                    {
                        Message = $"Job has been removed.",
                        Method = "DeleteChangeStatusToRealizedScheduleJob",
                        JobKey = job.Key,
                        EventId = eventId
                    });
                }
            }
        }

        public void DeleteCancelEventScheduleJob(int eventId)
        {
            var jobsProcessing = this.GetScheduledJob(Constants.MethodSend);
            foreach (var job in jobsProcessing)
            {
                var value = job.Value.Job.Args[0];
                if (value is MarkAsRealizedCommand)
                {
                    continue;
                }

                var param = (CancelEventCommand) value;
                if (param.EventId != eventId)
                {
                    continue;
                }

                BackgroundJob.Delete(job.Key);

                this._loggerManager.LogInformation(new
                {
                    Message = $"Job has been removed.",
                    Method = "DeleteCancelEventScheduleJob",
                    JobKey = job.Key,
                    EventId = eventId
                });
            }
        }


        public void ChangeStatusToRealizedScheduleJob(MarkAsRealizedCommand command, TimeSpan delay)
        {
            this._jobClient.Schedule(() => this._mediator.Send(command, default), delay);

            this._loggerManager.LogInformation(new
            {
                Message = $"New job has been set which will take place in {delay}.",
                Method = "ChangeStatusToRealizedScheduleJob",
                Param = command
            });
        }

        public void CancelEventWhenSuspendedScheduleJob(CancelEventCommand command, TimeSpan delay)
        {
            this._jobClient.Schedule(() => this._mediator.Send(command, default), delay);

            this._loggerManager.LogInformation(new
            {
                Message = $"New job has been set which will take place in {delay}f.",
                Method = "CancelEventWhenSuspendedScheduleJob",
                Param = command
            });
        }

        private List<KeyValuePair<string, ScheduledJobDto>> GetScheduledJob(string methodName)
        {
            var monitor = JobStorage.Current.GetMonitoringApi();
            var jobsProcessing = monitor.ScheduledJobs(0, int.MaxValue)
                .Where(x => x.Value?.Job?.Method?.Name == methodName)?.ToList();

            return jobsProcessing;
        }
    }
}