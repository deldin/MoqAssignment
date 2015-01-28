using System;
using System.Linq;
using Ista.FileServices.MarketFiles.Enumerations;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles
{
    public abstract class AbstractErcotFileTask
    {
        protected const string MissingCspDunsTexasPort = @"The CSPDUNS for this file does not have a Texas Port associated with it.";

        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketFile marketFileDataAccess;

        protected AbstractErcotFileTask(IClientDataAccess clientDataAccess, IMarketFile marketFileDataAccess)
        {
            this.clientDataAccess = clientDataAccess;
            this.marketFileDataAccess = marketFileDataAccess;
        }

        public CspDunsPortModel IdentifyCspDunsPort(CspDunsPortModel[] cspDunsPorts, int cspDunsId)
        {
            var port = cspDunsPorts
                .Where(x => x.CspDunsId.Equals(cspDunsId))
                .FirstOrDefault(x => !x.LdcId.HasValue || x.LdcId == 0);

            if (port != null)
                return port;

            var ldcIdentifiers = cspDunsPorts
                .Where(x => x.LdcId.HasValue)
                .Select(x => x.LdcId ?? 0)
                .Distinct();

            foreach (var ldcIdentifier in ldcIdentifiers)
            {
                var ldcModel = clientDataAccess.LoadLdcById(ldcIdentifier);
                if (ldcModel == null)
                    continue;

                var market = (MarketOptions)ldcModel.MarketId;
                if (market != MarketOptions.Texas)
                    continue;

                port = cspDunsPorts.FirstOrDefault(x => x.LdcId.Equals(ldcIdentifier));
                break;
            }

            return port;
        }

        public void UpdateMarketFilesToError(MarketFileModel[] models, string errorMessage)
        {
            foreach (var marketFile in models)
            {
                marketFile.ProcessError = errorMessage;
                marketFile.ProcessDate = DateTime.Now;
                marketFile.Status = MarketFileStatusOptions.Error;
                marketFileDataAccess.UpdateMarketFile(marketFile);
            }
        }
    }
}
