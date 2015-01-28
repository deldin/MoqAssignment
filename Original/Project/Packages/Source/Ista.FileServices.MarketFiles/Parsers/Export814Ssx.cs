using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Ista.FileServices.Infrastructure.Extensions;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;

namespace Ista.FileServices.MarketFiles.Parsers
{
    public class Export814Ssx : IMarketFileExporter
    {
        private readonly IClientDataAccess clientDataAccess;
        private readonly IMarketDataAccess marketDataAccess;
        private readonly IMarket814Export exportDataAccess;
        private readonly ILogger logger;

        public Export814Ssx(IClientDataAccess clientDataAccess, IMarketDataAccess marketDataAccess, IMarket814Export exportDataAccess, ILogger logger)
        {
            this.clientDataAccess = clientDataAccess;
            this.marketDataAccess = marketDataAccess;
            this.exportDataAccess = exportDataAccess;
            this.logger = logger;
        }

        public IMarketFileExportResult[] Export(CancellationToken token)
        {
            var cspDunsPorts = clientDataAccess.ListCspDunsPort();
            var cspDunsPort = cspDunsPorts
                .Where(x => x.ProviderId == 3)
                .ToArray();

            var collection = new List<IMarketFileExportResult>();
            foreach (var port in cspDunsPort)
            {
                if (token.IsCancellationRequested)
                    break;


                var partner = marketDataAccess.LoadCspDunsTradingPartner(port.Duns, port.LdcDuns);
                if (partner == null)
                {
                    logger.ErrorFormat(
                        "No CSP DUNS Trading Partner record exists between CR DUNS \"{0}\" and LDC DUNS \"{1}\". 814 Transactions will not be exported.",
                        port.Duns, port.LdcDuns);

                    continue;
                }

                marketDataAccess.LoadCspDunsTradingPartnerConfig(partner);

                var headers = exportDataAccess.ListUnprocessed(port.LdcDuns, port.Duns, 3);
                if (headers.Length == 0)
                {
                    logger.TraceFormat("Zero 814 records found to export for TDSP Duns \"{0}\" and CR Duns \"{1}\".",
                        port.LdcDuns, port.Duns);
                    continue;
                }
                
                

                var distinctActionCodes = headers.Select(x => new { x.ActionCode }).Distinct().ToList();


                //we need to loop for each action code and based on that generate a file
                foreach (var uniqueValue in distinctActionCodes)
                {
                    var value = uniqueValue;

                    var headerList = new List<Type814Header>();

                    foreach (var header in headers)
                    {

                        if (header.ActionCode.Trim().ToUpper() == value.ActionCode.Trim().ToUpper())
                        {
                            headerList.Add(header);
                        }
                    }

                    if (headerList.Count <= 0) continue;

                    var model = new Export814Model
                    {
                        CspDuns = port.Duns,
                        LdcDuns = port.LdcDuns,
                        LdcShortName = port.LdcShortName,
                        TradingPartnerId = port.TradingPartnerId,
                        ForSsxContent = true,
                        UniqueFileNameFiller = string.Format("{0}_SSX", value.ActionCode.Trim())
                    };

                    model.CspDunsTradingPartnerId = partner.CspDunsTradingPartnerId;
                    
                    var fileContentBuilder = BuildFileContent(headerList, port);

                    model.Content = fileContentBuilder.ToString();
                    model.AddHeaderKeys(headerList.Where(x => x.HeaderKey.HasValue).Select(x => x.HeaderKey.Value).ToList().ToArray());

                    collection.Add(model);


                }//loop for each Action Code combination

           }

            return collection.ToArray();
        }

        private StringBuilder BuildFileContent(List<Type814Header> headers, CspDunsPortModel cspDunsPort)
        {
            var ldcModel = clientDataAccess.LoadLdcById(cspDunsPort.LdcId.HasValue ? cspDunsPort.LdcId.Value: 0);

            var contentBuilder = new StringBuilder();

            var rootLevelColumns = string.Format("{0},{1},{2}", "LdcDuns", "LdcName", "CspDuns");

            var rootLevelData = string.Format("{0},{1},{2}", cspDunsPort.LdcDuns,
                                              ldcModel.LdcName, cspDunsPort.Duns);

            var headerLines = GetHeaderLines(headers);

            for (var i = 0; i < headerLines.Count; i++)
            {
                //header row
                if (i == 0)
                {
                    //add header row
                    contentBuilder.AppendLine(string.Format("{0},{1}", rootLevelColumns, headerLines[i]));
                }
                else
                {
                    //add data rows
                    contentBuilder.AppendLine(string.Format("{0},{1}", rootLevelData, headerLines[i]));
                }

            }

            return contentBuilder;
        }


        private List<string> GetHeaderLines(List<Type814Header> headers)
        {
            var headerLines = new List<string>();

            var headerLevelColumns = string.Format("{0},{1},{2},{3}",
                                        "CspName",
                                        "TransactionNumber",
                                        "TransactionDate",
                                        "ActionCode");


            bool columnHeadersAdded = false;

            //Loop through each
            foreach (var header in headers)
            {

                var headerLevelData = string.Format("{0},{1},{2},{3}",
                                        header.CrName,
                                        header.TransactionNbr,
                                        header.TransactionDate,
                                        header.ActionCode
                                        );


                var nameLines = GetNameLines(header);
                var serviceLines = GetServiceLines(header);

                //Make the combined Columns Headers 
                if (columnHeadersAdded == false)
                {
                    headerLines.Add(string.Format("{0},{1},{2}", headerLevelColumns, nameLines[0], serviceLines[0]));
                    columnHeadersAdded = true;
                }


                //make a all in row for all header/name/service combinations, skip the column header row which is first one
                for (var i = 1; i < nameLines.Count; i++)
                {
                    var nameLine = nameLines[i];



                    //also combine services
                    for (var j = 1; j < serviceLines.Count; j++)
                    {
                        var serviceLine = serviceLines[j];

                        //Add the data line
                        headerLines.Add(string.Format("{0},{1},{2}", headerLevelData, nameLine, serviceLine));


                    }

                }

            }
            //end loop for each header

            return headerLines;

        }


        private List<string> GetServiceLines(Type814Header header)
        {
            var serviceLines = new List<string>();

            var serviceLevelColumns = string.Format("{0},{1},{2}",
                                  "ServiceType1",
                                  "EsiId",
                                  "ESPAccountNumber");

            var meterLevelColumns = string.Format("{0}",
                                 "MeterNumber");

            //Add column headers
            serviceLines.Add(string.Format("{0},{1}", serviceLevelColumns, meterLevelColumns));


            Type814Service[] services = null;

            if (header.HeaderKey.HasValue)
                services = exportDataAccess.ListServices(header.HeaderKey.Value);
            
            //if there are no services then use blank lines
            //even if services are not there we need to enter empty columns to maintain tabular format for csv
            if (services == null || services.Length == 0)
            {

                var serviceLevelEmpty = string.Format("{0},{1},{2}",
                                  string.Empty, string.Empty, string.Empty);

                var meterLevelEmpty = string.Format("{0}",
                                 string.Empty);



                //Add headers
                serviceLines.Add(string.Format("{0},{1}", serviceLevelEmpty, meterLevelEmpty));


            }


            if (services != null)
            { 

                //loop for Service
                foreach (var service in services)
                {


                    var serviceLevelData = string.Format("{0},{1},{2}",
                                      service.ServiceType1,
                                      service.EsiId,
                                      service.EspAccountNumber);


                    Type814ServiceMeter[] meters = null;

                    if(service.ServiceKey.HasValue)
                        meters = exportDataAccess.ListServiceMeters(service.ServiceKey.Value);

                    //even if meters are not there we need to enter empty columns to maintain tabular format for csv
                    if (meters == null || meters.Length == 0)
                    {
                        var meterEmpty = string.Format("{0}",
                                     string.Empty);

                        //Add ServiceData with EmptyMeterData
                        serviceLines.Add(string.Format("{0},{1}", serviceLevelData, meterEmpty));
                    }

                    if (meters == null) continue;
                    
                    //inner loop for meters
                    foreach (var meter in meters)
                    {

                        var meterLevelData = string.Format("{0}",
                                                           meter.MeterNumber);

                        //Add ServiceData with MeterData
                        serviceLines.Add(string.Format("{0},{1}", serviceLevelData, meterLevelData));

                    } //inner loop for meters in service
                }//end of service loop

            }

            return serviceLines;
        }


        private List<string> GetNameLines(Type814Header header)
        {

            var nameLines = new List<string>();

            var nameLevelColumns = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20}",
                                   "BillingEntityName",
                                   "BillingAddress1",
                                   "BillingAddress2",
                                   "BillingCity",
                                   "BillingState",
                                   "BillingPostalCode",
                                   "BillingContactName",
                                   "BillingContactPhoneNumber1",
                                   "BillingEntityFirstName",
                                   "BillingEntityLastName",
                                   "ServiceCustType",
                                   "ServiceEntityName",
                                   "ServiceAddress1",
                                   "ServiceAddress2",
                                   "ServiceCity",
                                   "ServiceState",
                                   "ServicePostalCode",
                                   "ServiceContactName",
                                   "ServiceContactPhoneNumber1",
                                   "ServiceEntityFirstName",
                                   "ServiceEntityLastName");

            nameLines.Add(nameLevelColumns);


            var billingName = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", "", "", "", "", "", "", "", "", "", "");
            var serviceName = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "", "", "", "", "", "", "", "", "", "", "");
            var serviceNameFound = false;
            var billingNameFound = false;


            if (header.HeaderKey.HasValue)
            {
                //we have to load the Names here
                var names = exportDataAccess.ListNames(header.HeaderKey.Value);



                //Loop for Names
                foreach (var name in names)
                {

                    if (name.EntityIdType.ToUpper() == "8R" && !serviceNameFound)
                    {
                        serviceName = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                       name.CustType,
                                       name.EntityName,
                                       name.Address1,
                                       name.Address2,
                                       name.City,
                                       name.State,
                                       name.PostalCode,
                                       name.ContactName,
                                       name.ContactPhoneNbr1,
                                       name.EntityFirstName,
                                       name.EntityLastName
                                       );

                        serviceNameFound = true;
                    }


                    if (GetBillingAddressEntityIdTypes(header.ActionCode).Any(match => name.EntityIdType.ToUpper() == match && !billingNameFound))
                    {
                        billingName = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                                                    name.EntityName,
                                                    name.Address1,
                                                    name.Address2,
                                                    name.City,
                                                    name.State,
                                                    name.PostalCode,
                                                    name.ContactName,
                                                    name.ContactPhoneNbr1,
                                                    name.EntityFirstName,
                                                    name.EntityLastName);


                        billingNameFound = true;
                    }


                    if (!serviceNameFound || !billingNameFound) continue;

                    break;

                }//end of name loop

            }

            
            var nameLevelData = string.Format("{0},{1}", billingName, serviceName);

            nameLines.Add(nameLevelData);


            return nameLines;
        }


        private string[] GetBillingAddressEntityIdTypes(string actionCode)
        {
            //based on action code we would know how to get the BillingAddress and in which order

            var entityIdCodes = "N1,BT,FJ";

            switch (actionCode.ToUpper())
            {
                case "01":
                case "16":
                    entityIdCodes = "N1,BT";
                    break;


                case "C":
                case "D":
                case "HU":
                case "E":
                    entityIdCodes = "BT,N1";
                    break;

            }



            return entityIdCodes.Split(new char[] { ',' });



        }
       
    }
}
