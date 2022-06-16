using System.Collections.Generic;
using EventManagement.Application.Models.Dao.ProposalDAOs;

namespace EventManagement.Application.Models.Dao.PerformerDAOs
{
    public class PerformerWithProposalsDao : PerformerDao
    {
        public List<ProposalDao> Proposals { get; set; }
    }
}
