using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public class ScheduledElement : ExecutionTypeElement
    {
        public override ElementTypes ElementType
        {
            get { return ElementTypes.Scheduled; }
        }

        public override ExecutionTypes Type
        {
            get { return ExecutionTypes.Scheduled; }
        }

        public int[] SecondsConsidered { get; set; }
        public int[] MinutesConsidered { get; set; }
        public int[] HoursConsidered { get; set; }
        public int[] DaysConsidered { get; set; }
    }
}
