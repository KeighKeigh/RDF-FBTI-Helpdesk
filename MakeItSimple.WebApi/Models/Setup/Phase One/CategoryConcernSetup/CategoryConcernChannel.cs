using MakeItSimple.WebApi.Models.Setup.ChannelSetup;
using MakeItSimple.WebApi.Models.Setup.Phase_One.ServiceProviderSetup;

namespace MakeItSimple.WebApi.Models.Setup.Phase_One.CategoryConcernSetup
{
    public class CategoryConcernChannel
    {
        public int Id { get; set; }
        public bool? IsActive { get; set; } = true;
        public int? ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
        public int CategoryConcernId { get; set; }
        public virtual CategoryConcern CategoryConcern { get; set; }
    }
}
