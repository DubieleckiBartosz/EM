using AutoMapper;
using EventManagement.Application.Mappings.Converters;
using EventManagement.Application.Models.Dao;
using EventManagement.Application.Models.Dao.EventApplicationDAOs;
using EventManagement.Application.Models.Dao.EventDAOs;
using EventManagement.Application.Models.Dao.PerformerDAOs;
using EventManagement.Application.Models.Dao.ProposalDAOs;
using EventManagement.Application.Models.Dto;
using EventManagement.Application.Models.Dto.EventDTOs;
using EventManagement.Domain.Entities;

namespace EventManagement.Application.Mappings
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            this.CreateMap<EventWithImagesDao, Event>()
                .ConvertUsing(new EventImageDaoToEventTypeConverter());
            this.CreateMap<EventImageDao, EventImage>()
                .ConvertUsing(new EventImageDaoToEventImageTypeConverter());
            this.CreateMap<EventBaseDao, Event>()
                .ConvertUsing(new EventBaseDaoToEventTypeConverter());
            this.CreateMap<EventOpinionDao, EventOpinion>()
                .ConvertUsing(new EventOpinionDaoToEventOpinionTypeConverter());
            this.CreateMap<EventWithOpinionsDao, Event>()
                .ConvertUsing(new EventOpinionToEventTypeConverter());
            this.CreateMap<EventApplicationDao, EventApplication>()
                .ConvertUsing(new EventApplicationDaoToEventApplicationTypeConverter());
            this.CreateMap<EventWithApplicationsDao, Event>()
                .ConvertUsing(new EventApplicationDaoToEventTypeConverter());
            this.CreateMap<PerformerDao, Performer>()
                .ConvertUsing(new PerformerDaoToPerformerTypeConverter());
            this.CreateMap<ProposalDao, PerformanceProposal>()
                .ConvertUsing(new ProposalDaoToPerformanceProposalTypeConverter());
            this.CreateMap<PerformerWithProposalsDao, Performer>()
                .ConvertUsing(new PerformerWithProposalsDaoToPerformerTypeConverter());


            //DAO -> DTO
            this.CreateMap<EventDetailsDao, EventDetailsDto>();
            this.CreateMap<EventImageDao, EventImageDto>();
            this.CreateMap<EventOpinionDao, EventOpinionDto>();
            this.CreateMap<EventsWithCountDao, EventBaseDto>();
            this.CreateMap<EventWithOpinionsDao, EventWithOpinionsDto>();
            this.CreateMap<EventApplicationDao, EventApplicationDto>();
            this.CreateMap<EventWithApplicationsDao, EventWithApplicationsDto>();
            this.CreateMap<ProposalDao, PerformanceProposalDto>();
            this.CreateMap<PerformerWithNumberOfPerformancesDao, PerformerWithNumberOfPerformancesDto>();
        }
    }
}