using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.PerformerFeatures.Commands.UpdatePerformer
{
    public class UpdatePerformerCommand : IRequest<Response<string>>
    {
        public int? NumberOfPeople { get; set; }
        public string PerformerMail { get; set; }

        [JsonConstructor]
        public UpdatePerformerCommand(int? numberOfPeople = null, string performerMail = null)
        {
            this.NumberOfPeople = numberOfPeople;
            this.PerformerMail = performerMail;
        }
    }
}
