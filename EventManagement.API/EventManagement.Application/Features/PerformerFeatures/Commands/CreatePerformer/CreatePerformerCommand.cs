using EventManagement.Application.Wrappers;
using MediatR;
using Newtonsoft.Json;

namespace EventManagement.Application.Features.PerformerFeatures.Commands.CreatePerformer
{
    public class CreatePerformerCommand: IRequest<Response<string>>
    {
        public string PerformerName { get; set; }
        public int NumberOfPeople { get; set; }
        public string PerformerMail { get; set; }

        [JsonConstructor]
        public CreatePerformerCommand(string performerName, int numberOfPeople, string performerMail)
        {
            this.PerformerName = performerName;
            this.NumberOfPeople = numberOfPeople;
            this.PerformerMail = performerMail;
        }
    }
}