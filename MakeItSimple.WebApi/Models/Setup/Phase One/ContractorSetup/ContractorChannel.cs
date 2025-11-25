using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup;

namespace MakeItSimple.WebApi.Models.Setup.Phase_One.ContractorSetup
{
    public class ContractorChannel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; } = true;
        public int? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public int ContractorId { get; set; }
        public virtual Contractor Contractor { get; set; }
    }
}
