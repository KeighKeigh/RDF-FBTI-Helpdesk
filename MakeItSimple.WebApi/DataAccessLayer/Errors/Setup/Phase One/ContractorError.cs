using MakeItSimple.WebApi.Common;

namespace MakeItSimple.WebApi.DataAccessLayer.Errors.Setup.Phase_One
{
    public class ContractorError
    {
        public static Error ContractorAlreadyExist(string Contractor) =>
         new Error("Contractor.ContractorAlreadyExist", $"Contractor {Contractor} already exist!");
        public static Error ContractorNotExist() =>
        new Error("Contractor.CategoryConcerNotExist", $"Contractor not exist!");

        public static Error ContractorNochanges() =>
        new Error("Contractor.CategoryConcerNochanges", "No changes has made!");

        public static Error ContractorIsUse(string Contractor) =>
        new Error("Contractor.CategoryConcerIsUse", $"Category {Contractor} is use!");
    }
}
