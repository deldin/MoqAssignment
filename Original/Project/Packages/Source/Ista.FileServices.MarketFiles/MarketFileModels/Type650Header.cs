using System;
using System.Collections.Generic;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;

namespace Ista.FileServices.MarketFiles.MarketFileModels
{
    public class Type650Header : IType650Model, IMarketHeaderModel
    {
        private readonly List<Type650Name> names;
        private readonly List<Type650Service> services;

        public Type650Types ModelType
        {
            get { return Type650Types.Header; }
        }

        public int? HeaderKey { get; set; }
        public int MarketFileId { get; set; }
        public string TransactionSetId { get; set; }
        public string TransactionSetControlNbr { get; set; }
        public string TransactionSetPurposeCode { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionNbr { get; set; }
        public string ReferenceNbr { get; set; }
        public string TransactionType { get; set; }
        public string ActionCode { get; set; }
        public string TdspName { get; set; }
        public string TdspDuns { get; set; }
        public string CrName { get; set; }
        public string CrDuns { get; set; }
        public string ProcessedReceivedDateTime { get; set; }
        public short ProcessFlag { get; set; }
        public DateTime ProcessDate { get; set; }
        public bool Direction { get; set; }
        public int TransactionTypeId { get; set; }
        public int MarketId { get; set; }
        public int ProviderId { get; set; }

        public Type650Name[] Names
        {
            get { return names.ToArray(); }
        }

        public Type650Service[] Services
        {
            get { return services.ToArray(); }
        }

        public Type650Header()
        {
            names = new List<Type650Name>();
            services = new List<Type650Service>();
        }

        public void AddName(Type650Name item)
        {
            names.Add(item);
        }

        public void AddService(Type650Service item)
        {
            services.Add(item);
        }
    }
}