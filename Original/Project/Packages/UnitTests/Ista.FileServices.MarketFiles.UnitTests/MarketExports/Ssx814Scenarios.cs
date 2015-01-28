using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Models;
using Ista.FileServices.MarketFiles.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketExports
{
    [TestClass]
    public class Ssx814Scenarios
    {
        private IClientDataAccess clientDataAccess;
        private IMarketDataAccess marketDataAccess;
        private IMarket814Export exportDataAccess;
        private ILogger logger;

        private Export814Ssx exporter;

        [TestInitialize]
        public void SetUp()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            exportDataAccess = MockRepository.GenerateMock<IMarket814Export>();
            marketDataAccess = MockRepository.GenerateMock<IMarketDataAccess>();
            logger = MockRepository.GenerateStub<ILogger>();

            exporter = new Export814Ssx(clientDataAccess, marketDataAccess, exportDataAccess, logger);
        }

        [TestMethod]
        public void When_Exporting_814_For_Multiple_ActionCodes()
        {
            var cspDunsPorts = new[]
            {
                new CspDunsPortModel
                {
                    CspDunsId = 1,
                    CspDunsPortId = 1,
                    LdcDuns = "MockTdspDuns_One",
                    LdcShortName = "MockTdspDuns_One",
                    Duns = "MockDuns_One",
                    TradingPartnerId = "Mock_Trade_{DUNS}",
                    ProviderId = 3,
                    LdcId = 122
                }
            };

            var partner = new CspDunsTradingPartnerModel
                              {
                                  CspDuns = "MockDuns_One",
                                  CspDunsTradingPartnerId = 1,
                                  CspName = "MockDuns_One Csp",
                                  CspShortName = "",
                                  CspTradingPartnerId = 2,
                                  TradingPartnerDuns = "MockTdspDuns_One",
                                  TradingPartnerId = 1,
                                  TradingPartnerName = "Mock Trade",
                                  TradingPartnerShortName = "MT"
                                  
                              };



            var headers = new[]
            {
                new Type814Header {HeaderKey = 1, ActionCode = "01", TransactionNbr="H1TransNbr", TransactionDate="12202013", },
                new Type814Header {HeaderKey = 2, ActionCode = "E", TransactionNbr="H2TransNbr", TransactionDate="12212013", },
                new Type814Header {HeaderKey = 3, ActionCode = "D", TransactionNbr="H3TransNbr", TransactionDate="12222013", }
            };

            
            #region Set Names

            var namesHeader1 = new[]
                                   {
                                       new Type814Name
                                           {
                                               CustType = "R",
                                               EntityName = "EntityNameH1N1",
                                               Address1 = "Address1H1N1",
                                               Address2 = "Address2H1N1",
                                               City = "CityH1N1",
                                               State = "StateH1N1",
                                               PostalCode = "PostalCodeH1N1",
                                               ContactName = "ContactNameH1N1",
                                               ContactPhoneNbr1 = "ContactPhoneNbrH1N1",
                                               EntityFirstName = "EntityH1N1",
                                               EntityLastName = "LastNameH1N1",
                                               EntityIdType = "8R"
                                           }
                                       , new Type814Name
                                             {
                                                 CustType = "R",
                                                 EntityName = "EntityNameH1N2",
                                                 Address1 = "Address1H1N2",
                                                 Address2 = "Address2H1N2",
                                                 City = "CityH1N2",
                                                 State = "StateH1N2",
                                                 PostalCode = "PostalCodeH1N2",
                                                 ContactName = "ContactNameH1N2",
                                                 ContactPhoneNbr1 = "ContactPhoneNbrH1N2",
                                                 EntityFirstName = "EntityH1N2",
                                                 EntityLastName = "LastNameH1N2",
                                                 EntityIdType = "N1"
                                             }
                                   };


            
            var namesHeader2 = new[]
                                   {
                                       new Type814Name
                                        {
                                            CustType = "R",
                                            EntityName = "EntityNameH2N1",
                                            Address1 = "Address1H2N1",
                                            Address2 = "Address2H2N1",
                                            City = "CityH2N1",
                                            State = "StateH2N1",
                                            PostalCode = "PostalCodeH2N1",
                                            ContactName = "ContactNameH2N1",
                                            ContactPhoneNbr1 = "ContactPhoneNbrH2N1",
                                            EntityFirstName = "EntityH2N1",
                                            EntityLastName = "LastNameH2N1",
                                            EntityIdType = "8R"
                                        },new Type814Name
                                        {
                                            CustType = "R",
                                            EntityName = "EntityNameH2N2",
                                            Address1 = "Address1H2N2",
                                            Address2 = "Address2H2N2",
                                            City = "CityH2N2",
                                            State = "StateH2N2",
                                            PostalCode = "PostalCodeH2N2",
                                            ContactName = "ContactNameH2N2",
                                            ContactPhoneNbr1 = "ContactPhoneNbrH2N2",
                                            EntityFirstName = "EntityH2N2",
                                            EntityLastName = "LastNameH2N2",
                                            EntityIdType = "N1"
                                        }
                                   };


            var namesHeader3 = new[]
                                   {
                                       new Type814Name
                                        {
                                            CustType = "R",
                                            EntityName = "EntityNameH3N1",
                                            Address1 = "Address1H3N1",
                                            Address2 = "Address2H3N1",
                                            City = "CityH3N1",
                                            State = "StateH3N1",
                                            PostalCode = "PostalCodeH3N1",
                                            ContactName = "ContactNameH3N1",
                                            ContactPhoneNbr1 = "ContactPhoneNbrH3N1",
                                            EntityFirstName = "EntityH3N1",
                                            EntityLastName = "LastNameH3N1",
                                            EntityIdType = "8R"
                                        },
                                        new Type814Name
                                        {
                                            CustType = "R",
                                            EntityName = "EntityNameH3N2",
                                            Address1 = "Address1H3N2",
                                            Address2 = "Address2H3N2",
                                            City = "CityH3N2",
                                            State = "StateH3N2",
                                            PostalCode = "PostalCodeH3N2",
                                            ContactName = "ContactNameH3N2",
                                            ContactPhoneNbr1 = "ContactPhoneNbrH3N2",
                                            EntityFirstName = "EntityH3N2",
                                            EntityLastName = "LastNameH3N2",
                                            EntityIdType = "N1"
                                        }   
                                    }; 
            #endregion
            
            #region Set Services and ServiceMeters

            var servicesHeader1 = new []
                                      {
                                          new Type814Service
                                              {
                                                  ServiceKey = 10,
                                                  ServiceType1 = "ServiceType1H1S1",
                                                  EsiId = "EsiIdH1S1",
                                                  EspAccountNumber = "EspAccountNumberH1S1"

                                              }
                                      };

            var serviceMetersHeader1 = new [] {new Type814ServiceMeter { MeterNumber = "MeterNumberH1S1" }};


            var servicesHeader2 = new[]
                                      {
                                          new Type814Service
                                            {
                                                ServiceKey = 11,
                                                ServiceType1 = "ServiceType1H2S1",
                                                EsiId = "EsiIdH2S1",
                                                EspAccountNumber = "EspAccountNumberH2S1"

                                            }
                                      };

           
            var serviceMetersHeader2 = new [] {new Type814ServiceMeter { MeterNumber = "MeterNumberH2S1" }};


            var servicesHeader3 = new[]
                                      {
                                          new Type814Service
                                            {
                                                ServiceKey = 12,
                                                ServiceType1 = "ServiceType1H3S1",
                                                EsiId = "EsiIdH3S1",
                                                EspAccountNumber = "EspAccountNumberH3S1"

                                            }
                                      };


            var serviceMetersHeader3 = new[] { new Type814ServiceMeter { MeterNumber = "MeterNumberH3S1" } };

          

            #endregion


            var cspDunsTradingPartnerModel = new CspDunsTradingPartnerModel
                                                 {
                                                     CspDunsTradingPartnerId = 10
                                                 };

            
            // arrange

            clientDataAccess.Stub(x => x.LoadLdcById(Arg<int>.Is.Anything)).Return(new LdcModel
                                                                                       {
                                                                                           LdcName = "Mock Tdsp One",
                                                                                           LdcId = 122
                                                                                       });

            clientDataAccess.Stub(x => x.ListCspDunsPort())
                .Return(cspDunsPorts);



            marketDataAccess.Stub(x => x.LoadCspDunsTradingPartner(Arg<string>.Is.Anything, Arg<string>.Is.Anything)).Return(partner);

          
            marketDataAccess.Stub(x => x.LoadCspDunsTradingPartnerConfig(Arg<CspDunsTradingPartnerModel>.Is.Anything)).
                WhenCalled(x => cspDunsTradingPartnerModel.AddConfig("Name", "Value"));


            exportDataAccess.Stub(x => x.ListUnprocessed(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg.Is(3)))
                .Return(headers);


            //Stub out ListServices
            exportDataAccess.Stub(x => x.ListServices(1)).Return(servicesHeader1);
            exportDataAccess.Stub(x => x.ListServices(2)).Return(servicesHeader2);
            exportDataAccess.Stub(x => x.ListServices(3)).Return(servicesHeader3);


            //Stub out ListServiceMeters
            exportDataAccess.Stub(x => x.ListServiceMeters(10)).Return(serviceMetersHeader1);
            exportDataAccess.Stub(x => x.ListServiceMeters(11)).Return(serviceMetersHeader2);
            exportDataAccess.Stub(x => x.ListServiceMeters(12)).Return(serviceMetersHeader3);


            //Stub out ListNames
            exportDataAccess.Stub(x => x.ListNames(1)).Return(namesHeader1);
            exportDataAccess.Stub(x => x.ListNames(2)).Return(namesHeader2);
            exportDataAccess.Stub(x => x.ListNames(3)).Return(namesHeader3);
            

            // act
            var results = exporter.Export(CancellationToken.None);

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(3, results.Length);

            var firstResult = results[0];
            Assert.IsNotNull(firstResult);
            Assert.AreEqual("Mock_Trade_{DUNS}", firstResult.TradingPartnerId);
            Assert.AreEqual("MockTdspDuns_One", firstResult.LdcShortName);

            Assert.AreEqual(1, firstResult.HeaderCount);
            CollectionAssert.Contains(firstResult.HeaderKeys, 1);

            Assert.AreEqual(1, results[1].HeaderCount);
            CollectionAssert.Contains(results[1].HeaderKeys, 2);


            Assert.AreEqual(1, results[2].HeaderCount);
            CollectionAssert.Contains(results[2].HeaderKeys, 3);


        }

       


    }
}
