using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public class ContinuousElement : ExecutionTypeElement
    {
        public override ElementTypes ElementType
        {
            get { return ElementTypes.Continuous; }
        }

        public override ExecutionTypes Type
        {
            get { return ExecutionTypes.Continuous; }
        }

        public int IdlePeriod { get; set; }
        public string IdlePeriodType { get; set; }   
    }
}
