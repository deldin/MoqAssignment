using System.Collections.Generic;
using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public class ExecutionElement : AbstractElement
    {
        private readonly List<ExecutionTypeElement> collection;

        public override ElementTypes ElementType
        {
            get { return ElementTypes.Execution; }
        }

        public ExecutionTypeElement[] ExecutionTypes
        {
            get { return collection.ToArray(); }
        }

        public ExecutionElement()
        {
            collection = new List<ExecutionTypeElement>();
        }

        public void AddExecutionType(AbstractElement element)
        {
            var executionType = element as ExecutionTypeElement;
            if (executionType == null)
                return;

            collection.Add(executionType);
        }
    }
}
