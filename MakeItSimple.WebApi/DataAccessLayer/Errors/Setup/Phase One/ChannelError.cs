using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup
{
    public class ChannelError
    {

      public static Error ChannelNameAlreadyExist(string ChannelName) =>
      new Error("Channel.ChannelNameAlreadyExist", $"Channel name {ChannelName} already exist!");

        public static Error ProjectNameAlreadyExist(string ProjectName) =>
        new Error("Channel.ProjectNameAlreadyExist", $"Project name {ProjectName} already exist!");
      public static Error ProjectNotExist() =>
      new Error("Channel.ProjectNotExist", $"Project not exist!");
      public static Error SubUnitNotExist() =>
      new Error("Channel.SubUnitNotExist", "SubUnit doesn't exist!");

      public static Error UserNotExist() =>
      new Error("Channel.UserNotExist", "User doesn't exist!");

      public static Error ChannelNotExist() =>
      new Error("Channel.ChannelNotExist", "Channel doesn't exist!");

      public static Error ChannelUserNotExist() =>
      new Error("Channel.ChannelUserNotExist", "Member doesn't exist!");

      public static Error UserAlreadyAdd() =>
      new Error("Channel.UserAlreadyAdd", "Member already been add!");

      public static Error ChannelInUse(string channel) =>
      new Error("Channel.ChannelInUse", $"Channel {channel} is use!");

      public static Error ChannelNoChanges() =>
      new Error("Channel.ChannelNoChanges", "No changes has made!");
     
      

    }

    public class ServiceError
    {
        public static Error ServiceProviderNameAlreadyExist(string ServiceProvider) =>
      new Error("ServiceProvider.ChannelNameAlreadyExist", $"Service Provider name {ServiceProvider} already exist!");

        public static Error ProjectNameAlreadyExist(string ServiceProvider) =>
        new Error("ServiceProvider.ProjectNameAlreadyExist", $"Project name {ServiceProvider} already exist!");
        public static Error ProjectNotExist() =>
        new Error("ServiceProvider.ProjectNotExist", $"Project not exist!");
        public static Error SubUnitNotExist() =>
        new Error("ServiceProvider.SubUnitNotExist", "SubUnit doesn't exist!");

        public static Error ChannelNotExist() =>
        new Error("ServiceProvider.ChannelNotExist", "Channel doesn't exist!");

        public static Error ServiceProviderNotExist() =>
        new Error("ServiceProvider.ServiceProviderNotExist", "ServiceProvider doesn't exist!");

        public static Error ServiceProviderUserNotExist() =>
        new Error("ServiceProvider.ServiceProviderUserNotExist", "Member doesn't exist!");

        public static Error UserAlreadyAdd() =>
        new Error("ServiceProvider.UserAlreadyAdd", "Member already been add!");

        public static Error ServiceProviderInUse(string service) =>
        new Error("ServiceProvider.ServiceProviderInUse", $"Channel {service} is use!");

        public static Error ServiceProviderNoChanges() =>
        new Error("ServiceProvider.ChannelNoChanges", "No changes has made!");

        public static Error CategoryConcernAlreadyExists() =>
        new Error("ServiceProvider.CategoryConcernAlreadyExists", "Category Concern Already Exists!");

        public static Error ContractorAlreadyExists() =>
        new Error("ServiceProvider.ContractorAlreadyExists", "Contractor Already Exists!");
    }
}
