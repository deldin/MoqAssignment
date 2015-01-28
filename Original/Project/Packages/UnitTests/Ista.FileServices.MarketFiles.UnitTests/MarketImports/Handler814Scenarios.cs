using System.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Handlers;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ista.FileServices.MarketFiles.UnitTests.MarketImports
{
    [TestClass]
    public class Handler814Scenarios
    {
        private IMarket814Import dataAccess;
        private ILogger logger;
        
        private Import814Handler concern;

        [TestInitialize]
        public void SetUp()
        {
            dataAccess = MockRepository.GenerateMock<IMarket814Import>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new Import814Handler(dataAccess, logger);
        }

        [TestMethod]
        public void When_Saving_Header_Then_Insert_Header_Is_Called()
        {
            // arrange
            var header = new Type814Header();
            
            dataAccess.Expect(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Names_Then_Insert_Name_Is_Called()
        {
            // arrange
            const int headerKey = 1;

            var name = new Type814Name { EntityIdCode = "mock", };

            var header = new Type814Header();
            header.AddName(name);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(headerKey);

            dataAccess.Expect(x => x.InsertName(Arg.Is(name)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(headerKey, name.HeaderKey);
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Names_Then_Insert_Name_Is_Called_For_Each_Name()
        {
            // arrange
            const int headerKey = 1;

            var header = new Type814Header();
            header.AddName(new Type814Name { EntityIdCode = "mock_one", });
            header.AddName(new Type814Name { EntityIdCode = "mock_two", });
            header.AddName(new Type814Name { EntityIdCode = "mock_three", });
            
            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(headerKey);

            dataAccess.Expect(x => x.InsertName(Arg<Type814Name>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(header.Names.All(x => x.HeaderKey.Equals(headerKey)));

            dataAccess.AssertWasCalled(x => x.InsertName(null), x => x.IgnoreArguments().Repeat.Times(3, 3));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Names_Then_Insert_Name_Is_Not_Called()
        {
            // arrange
            var header = new Type814Header();

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertName(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Services_Then_Insert_Service_Is_Called()
        {
            // arrange
            const int headerKey = 1;

            var service = new Type814Service {ServiceType1 = "mock"};

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(headerKey);

            dataAccess.Expect(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(headerKey, service.HeaderKey);
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Services_Then_Insert_Service_Is_Called_For_Each_Service()
        {
            // arrange
            const int headerKey = 1;

            var header = new Type814Header();
            header.AddService(new Type814Service { ServiceType1 = "mock_one", });
            header.AddService(new Type814Service { ServiceType1 = "mock_two", });
            header.AddService(new Type814Service { ServiceType1 = "mock_three", });

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(headerKey);

            dataAccess.Expect(x => x.InsertService(Arg<Type814Service>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(header.Services.All(x => x.HeaderKey.Equals(headerKey)));

            dataAccess.AssertWasCalled(x => x.InsertService(null), x => x.IgnoreArguments().Repeat.Times(3, 3));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Services_Then_Insert_Service_Is_Not_Called()
        {
            // arrange
            var header = new Type814Header();

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertService(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Account_Changes_Then_Insert_Service_Account_Change_Is_Called()
        {
            // arrange
            const int serviceKey = 2;

            var change = new Type814ServiceAccountChange { ChangeReason = "mock" };

            var service = new Type814Service();
            service.AddChange(change);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceAccountChange(Arg.Is(change)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceKey, change.ServiceKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Account_Changes_Then_Insert_Service_Account_Change_Is_Called_For_Each_Service_Account_Change()
        {
            // arrange
            const int serviceKey = 2;

            var service = new Type814Service();
            service.AddChange(new Type814ServiceAccountChange { ChangeReason = "mock_one" });
            service.AddChange(new Type814ServiceAccountChange { ChangeReason = "mock_two" });

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceAccountChange(Arg<Type814ServiceAccountChange>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(service.Changes.All(x => x.ServiceKey.Equals(serviceKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceAccountChange(null), x => x.IgnoreArguments().Repeat.Times(2, 2));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Account_Changes_Then_Insert_Service_Account_Change_Is_Not_Called()
        {
            // arrange
            var service = new Type814Service();
            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceAccountChange(null), x => x.IgnoreArguments());            
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Dates_Then_Insert_Service_Date_Is_Called()
        {
            // arrange
            const int serviceKey = 2;

            var date = new Type814ServiceDate { Date = "mock" };

            var service = new Type814Service();
            service.AddDate(date);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceDate(Arg.Is(date)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceKey, date.ServiceKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Dates_Then_Insert_Service_Date_Is_Called_For_Each_Service_Date()
        {
            // arrange
            const int serviceKey = 2;

            var service = new Type814Service();
            service.AddDate(new Type814ServiceDate { Date = "mock_date_one" });
            service.AddDate(new Type814ServiceDate { Date = "mock_date_two" });

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceDate(Arg<Type814ServiceDate>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(service.Dates.All(x => x.ServiceKey.Equals(serviceKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceDate(null), x => x.IgnoreArguments().Repeat.Times(2, 2));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Dates_Then_Insert_Service_Date_Is_Not_Called()
        {
            // arrange
            var service = new Type814Service();
            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceDate(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Rejects_Then_Insert_Service_Reject_Is_Called()
        {
            // arrange
            const int serviceKey = 2;

            var reject = new Type814ServiceReject { RejectCode = "mock" };

            var service = new Type814Service();
            service.AddReject(reject);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceReject(Arg.Is(reject)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceKey, reject.ServiceKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Rejects_Then_Insert_Service_Reject_Is_Called_For_Each_Service_Reject()
        {
            // arrange
            const int serviceKey = 2;

            var service = new Type814Service();
            service.AddReject(new Type814ServiceReject { RejectCode = "mock_one" });
            service.AddReject(new Type814ServiceReject { RejectCode = "mock_two" });
            service.AddReject(new Type814ServiceReject { RejectCode = "mock_four" });

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceReject(Arg<Type814ServiceReject>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(service.Rejects.All(x => x.ServiceKey.Equals(serviceKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceReject(null), x => x.IgnoreArguments().Repeat.Times(3, 3));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Rejects_Then_Insert_Service_Reject_Is_Not_Called()
        {
            // arrange
            var service = new Type814Service();
            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceReject(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Statuses_Then_Insert_Service_Status_Is_Called()
        {
            // arrange
            const int serviceKey = 2;

            var status = new Type814ServiceStatus { StatusCode = "mock" };

            var service = new Type814Service();
            service.AddStatus(status);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceStatus(Arg.Is(status)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceKey, status.ServiceKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Statuses_Then_Insert_Service_Status_Is_Called_For_Each_Service_Status()
        {
            // arrange
            const int serviceKey = 2;

            var service = new Type814Service();
            service.AddStatus(new Type814ServiceStatus { StatusCode = "mock_one" });
            service.AddStatus(new Type814ServiceStatus { StatusCode = "mock_two" });
            service.AddStatus(new Type814ServiceStatus { StatusCode = "mock_three" });

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceStatus(Arg<Type814ServiceStatus>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(service.Statuses.All(x => x.ServiceKey.Equals(serviceKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceStatus(null), x => x.IgnoreArguments().Repeat.Times(3, 3));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Status_Then_Insert_Service_Status_Is_Not_Called()
        {
            // arrange
            var service = new Type814Service();
            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceStatus(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Meters_Then_Insert_Service_Meter_Is_Called()
        {
            // arrange
            const int serviceKey = 2;

            var meter = new Type814ServiceMeter { MeterNumber = "mock" };

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceKey, meter.ServiceKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Meters_Then_Insert_Service_Meter_Is_Called_For_Each_Service_Meter()
        {
            // arrange
            const int serviceKey = 2;

            var service = new Type814Service();
            service.AddMeter(new Type814ServiceMeter { MeterNumber = "mock_num_one" });
            service.AddMeter(new Type814ServiceMeter { MeterNumber = "mock_num_two" });
            
            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(serviceKey);

            dataAccess.Expect(x => x.InsertServiceMeter(Arg<Type814ServiceMeter>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(service.Meters.All(x => x.ServiceKey.Equals(serviceKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceMeter(null), x => x.IgnoreArguments().Repeat.Times(2, 2));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Meters_Then_Insert_Service_Meter_Is_Not_Called()
        {
            // arrange
            var service = new Type814Service();
            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceMeter(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Meter_Changes_Then_Insert_Service_Meter_Change_Is_Called()
        {
            // arrange
            const int serviceMeterKey = 2;

            var change = new Type814ServiceMeterChange { ChangeReason = "mock" };

            var meter = new Type814ServiceMeter();
            meter.AddChange(change);

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(serviceMeterKey);

            dataAccess.Expect(x => x.InsertServiceMeterChange(Arg.Is(change)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceMeterKey, change.MeterKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Meter_Changes_Then_Insert_Service_Meter_Change_Is_Called_For_Each_Service_Meter_Change()
        {
            // arrange
            const int serviceMeterKey = 2;

            var meter = new Type814ServiceMeter();
            meter.AddChange(new Type814ServiceMeterChange { ChangeReason = "mock_one" });
            meter.AddChange(new Type814ServiceMeterChange { ChangeReason = "mock_two" });
            meter.AddChange(new Type814ServiceMeterChange { ChangeReason = "mock_six" });
            meter.AddChange(new Type814ServiceMeterChange { ChangeReason = "mock_ten" });

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(serviceMeterKey);

            dataAccess.Expect(x => x.InsertServiceMeterChange(Arg<Type814ServiceMeterChange>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(meter.Changes.All(x => x.MeterKey.Equals(serviceMeterKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceMeterChange(null), x => x.IgnoreArguments().Repeat.Times(4, 4));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Meter_Changes_Then_Insert_Service_Meter_Change_Is_Not_Called()
        {
            // arrange
            var meter = new Type814ServiceMeter();
            var service = new Type814Service();
            var header = new Type814Header();
            service.AddMeter(meter);
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceMeterChange(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Meter_TOUs_Then_Insert_Service_Meter_Tou_Is_Called()
        {
            // arrange
            const int serviceMeterKey = 2;

            var tou = new Type814ServiceMeterTou { TouCode = "mock" };

            var meter = new Type814ServiceMeter();
            meter.AddTou(tou);

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(serviceMeterKey);

            dataAccess.Expect(x => x.InsertServiceMeterTou(Arg.Is(tou)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceMeterKey, tou.MeterKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Meter_TOUs_Then_Insert_Service_Meter_Tou_Is_Called_For_Each_Service_Meter_TOU()
        {
            // arrange
            const int serviceMeterKey = 2;

            var meter = new Type814ServiceMeter();
            meter.AddTou(new Type814ServiceMeterTou { TouCode = "mock_here" });
            meter.AddTou(new Type814ServiceMeterTou { TouCode = "mock_now" });

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(serviceMeterKey);

            dataAccess.Expect(x => x.InsertServiceMeterTou(Arg<Type814ServiceMeterTou>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(meter.Tous.All(x => x.MeterKey.Equals(serviceMeterKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceMeterTou(null), x => x.IgnoreArguments().Repeat.Times(2, 2));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Meter_TOUs_Then_Insert_Service_Meter_Tou_Is_Not_Called()
        {
            // arrange
            var meter = new Type814ServiceMeter();
            var service = new Type814Service();
            var header = new Type814Header();
            service.AddMeter(meter);
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceMeterTou(null), x => x.IgnoreArguments());
        }

        [TestMethod]
        public void When_Saving_Header_With_Any_Service_Meter_Types_Then_Insert_Service_Meter_Type_Is_Called()
        {
            // arrange
            const int serviceMeterKey = 2;

            var type = new Type814ServiceMeterType { ChangeReason = "mock" };

            var meter = new Type814ServiceMeter();
            meter.AddType(type);

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(serviceMeterKey);

            dataAccess.Expect(x => x.InsertServiceMeterType(Arg.Is(type)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.AreEqual(serviceMeterKey, type.MeterKey);

            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_Multiple_Service_Meter_Types_Then_Insert_Service_Meter_Type_Is_Called_For_Each_Service_Meter_Type()
        {
            // arrange
            const int serviceMeterKey = 2;

            var meter = new Type814ServiceMeter();
            meter.AddType(new Type814ServiceMeterType { ChangeReason = "mock_type_one" });
            meter.AddType(new Type814ServiceMeterType { ChangeReason = "mock_type_two" });

            var service = new Type814Service();
            service.AddMeter(meter);

            var header = new Type814Header();
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(serviceMeterKey);

            dataAccess.Expect(x => x.InsertServiceMeterType(Arg<Type814ServiceMeterType>.Is.TypeOf))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            Assert.IsTrue(meter.Types.All(x => x.MeterKey.Equals(serviceMeterKey)));

            dataAccess.AssertWasCalled(x => x.InsertServiceMeterType(null), x => x.IgnoreArguments().Repeat.Times(2, 2));
            dataAccess.VerifyAllExpectations();
        }

        [TestMethod]
        public void When_Saving_Header_With_No_Service_Meter_Types_Then_Insert_Service_Meter_Type_Is_Not_Called()
        {
            // arrange
            var meter = new Type814ServiceMeter();
            var service = new Type814Service();
            var header = new Type814Header();
            service.AddMeter(meter);
            header.AddService(service);

            dataAccess.Stub(x => x.InsertHeader(Arg.Is(header)))
                .Return(1);

            dataAccess.Stub(x => x.InsertService(Arg.Is(service)))
                .Return(1);

            dataAccess.Stub(x => x.InsertServiceMeter(Arg.Is(meter)))
                .Return(1);

            // act
            concern.SaveHeader(header);

            // assert
            dataAccess.AssertWasNotCalled(x => x.InsertServiceMeterType(null), x => x.IgnoreArguments());
        }
    }
}
