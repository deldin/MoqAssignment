using System.Collections.Generic;
using System.Linq;
using Ista.FileServices.Service.Enumerations;

namespace Ista.FileServices.Service.Elements
{
    public abstract class ExecutionTypeElement : AbstractElement
    {
        private readonly List<ExecutionTypeItemElement> collection;

        public abstract ExecutionTypes Type { get; }
        public bool HasExecutionList
        {
            get { return collection.Any(); }
        }

        public ExecutionTypeItemElement[] ExecutionList
        {
            get
            {
                return collection
                    .OrderBy(x => x.Order)
                    .ToArray();
            }
        }

        public int? ClientId { get; set; }
        public string Id { get; set; }

        protected ExecutionTypeElement()
        {
            collection = new List<ExecutionTypeItemElement>();
        }

        public void AddItem(ExecutionTypeItemElement item)
        {
            collection.Add(item);
        }
    }
}
