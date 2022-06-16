using AutoMapper;
using EventManagement.Application.Models.Dao.PerformerDAOs;
using EventManagement.Domain.Entities;
using EventManagement.Domain.ValueObjects;

namespace EventManagement.Application.Mappings.Converters
{
    public class PerformerDaoToPerformerTypeConverter : ITypeConverter<PerformerDao, Performer>
    {
        public Performer Convert(PerformerDao source, Performer destination, ResolutionContext context)
        {
            var userId = source.UserId;
            var name = PerformerName.Create(source.PerformerName);
            var numberPeople = source.NumberOfPeople;
            var mail = source.PerformerMail;
            return Performer.Load(source.Id, userId, source.VIP, name, numberPeople, mail);
        }
    }
}