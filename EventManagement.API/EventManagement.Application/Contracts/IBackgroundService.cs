using System;
using EventManagement.Application.Features.EventFeatures.Commands.CancelEvent;
using EventManagement.Application.Features.EventFeatures.Commands.MarkAsRealizedEvent;

namespace EventManagement.Application.Contracts
{
    public interface IBackgroundService
    {
        void StartJobs();
        void ChangeStatusToRealizedScheduleJob(MarkAsRealizedCommand command, TimeSpan delay);
        void DeleteChangeStatusToRealizedScheduleJob(int eventId);
        void CancelEventWhenSuspendedScheduleJob(CancelEventCommand command, TimeSpan delay);
        void DeleteCancelEventScheduleJob(int eventId);
    }
}
