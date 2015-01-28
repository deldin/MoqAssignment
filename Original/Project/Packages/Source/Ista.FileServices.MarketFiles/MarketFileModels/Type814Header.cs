using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type814Header : IType814Model, IMarketHeaderModel
    {
        private readonly List<Type814Name> names;
        private readonly List<Type814Service> services;
 
        public Type814Types ModelType
        {
            get { return Type814Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string TransactionSetId { get; set; }
        public string TransactionSetControlNbr { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string TransactionNbr { get; set; }
        public string TransactionDate { get; set; }
        public string ReferenceNbr { get; set; }
        public string ActionCode { get; set; }
        public string TdspDuns { get; set; }
        public string TdspName { get; set; }
        public string CrDuns { get; set; }
        public string CrName { get; set; }
        public string PolrClass { get; set; }
        public string TransactionTime { get; set; }
        public string TransactionTimeCode { get; set; }
        public string TransactionQualifier { get; set; }
        public int TransactionQueueTypeId { get; set; }
        public int TransactionTypeId { get; set; }
        public int MarketId { get; set; }
        public int ProviderId { get; set; }
        public short ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public bool Direction { get; set; }

        public Type814Name[] Names
        {
            get { return names.ToArray(); }
        }

        public Type814Service[] Services
        {
            get { return services.ToArray(); }
        }

        public Type814Header()
        {
            names = new List<Type814Name>();
            services = new List<Type814Service>();
        }

        public void AddName(Type814Name item)
        {
            names.Add(item);
        }

        public void AddService(Type814Service item)
        {
            services.Add(item);
        }
    }
}
