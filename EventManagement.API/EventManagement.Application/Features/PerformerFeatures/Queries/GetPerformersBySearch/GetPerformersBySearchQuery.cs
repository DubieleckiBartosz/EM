using System.Collections.Generic;
using EventManagement.Application.Features.Search;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.PerformerFeatures.Queries.GetPerformersBySearch
{
    public class GetPerformersBySearchQuery : BaseSearchQuery, IRequest<Response<List<PerformerWithNumberOfPerformancesDto>>>
    {
        public string PerformerName { get; set; }
        public bool? OnlyVip { get; set; }
        public SortModelQuery SortModel { get; set; }

        [JsonConstructor]
        public GetPerformersBySearchQuery(string performerName = null, bool? onlyVip = null, SortModelQuery sortModel = null)
        {
            this.PerformerName = performerName;
            this.SortModel = sortModel;
            this.OnlyVip = onlyVip;
        }
    }
}