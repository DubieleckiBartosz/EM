using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;

namespace EventManagement.Application.Features.EventApplicationFeatures.Queries.GetEventApplicationsBySearch
{
    public class GetEventApplicationsQuery : IRequest<Response<List<EventApplicationDto>>>
    {
        public int? Status { get; set; }
        public string EventName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? PerformanceType { get; set; }
        public int? DurationInMinutesMin { get; set; }
        public int? DurationInMinutesMax { get; set; }
        public bool? LastModifiedByApplicant { get; set; }
        public SortModelQuery SortModel { get; set; }

        public GetEventApplicationsQuery()
        {
        }

        [JsonConstructor]
        public GetEventApplicationsQuery(int? status = null, string eventName = null, DateTime? from = null,
            DateTime? to = null, int? performanceType = null,
            int? durationInMinutesMin = null, int? durationInMinutesMax = null, bool? lastModifiedByApplicant = null,
            SortModelQuery sortModel = null)
        {
            this.PerformanceType = performanceType;
            this.Status = status;
            this.EventName = eventName;
            this.From = from;
            this.To = to;
            this.DurationInMinutesMin = durationInMinutesMin;
            this.DurationInMinutesMax = durationInMinutesMax;
            this.LastModifiedByApplicant = lastModifiedByApplicant;
            this.SortModel = sortModel;
        }
    }
}