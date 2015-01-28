using System.Linq;
using Ista.FileServices.Infrastructure.Interfaces;
using Ista.FileServices.MarketFiles.Interfaces;
using Ista.FileServices.MarketFiles.MarketFileModels;
using Ista.FileServices.MarketFiles.Parsers;
using Ista.FileServices.MarketFiles.Parsers.ImportContexts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace Ista.FileServices.MarketFiles.UnitTests.CustomerInfoScenarios
{
    [TestClass]
    public class CustomerInfoImportScenario
    {
        private IClientDataAccess clientDataAccess;
        private ILogger logger;
        private ImportCustomerInfo concern;

        [TestInitialize]
        public void SetUp()
        {
            clientDataAccess = MockRepository.GenerateMock<IClientDataAccess>();
            logger = MockRepository.GenerateStub<ILogger>();

            concern = new ImportCustomerInfo(clientDataAccess, logger);
        }

        [TestMethod]
        public void When_Parsing_Customer_Info_Line_With_Field_Zero_Equal_To_HDR_Then_A_Header_Is_Created()
        {
            // arrange
            var context = new PrismCustomerInfoContext();
            var fields = new[] { "HDR", "MOCK", "", "" };

            // act
            concern.ParseLine(context, string.Join("|", fields));

            // assert
            Assert.IsNotNull(context.Current);

            var model = context.Current as TypeCustomerInfoFile;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void When_Parsing_Customer_Info_Header_With_Field_One_Indicating_Response_Then_File_Type_Id_Is_Set_To_Two()
        {
            // arrange
            const int expected = 2;

            var context = new PrismCustomerInfoContext();
            var fields = new[] {"", "MTCRCUSTOMERINFORMATIONERCOTRESPONSE", "", ""};

            // act
            concern.ParseHeader(context, fields);
            context.ResolveToFile();

            // assert
            var results = context.Results;
            var collection = results.TypeHeaders;
            Assert.AreEqual(1, collection.Length);

            var model = collection.First();
            Assert.AreEqual(expected, model.FileTypeId);
        }

        [TestMethod]
        public void When_Parsing_Customer_Info_Error_With_Field_Zero_Equal_To_ER1_Then_Record_Type_Id_Is_Set_To_One()
        {
            // arrange
            const int expected = 1;

            var context = new PrismCustomerInfoContext();
            var fileModel = new TypeCustomerInfoFile();
            context.PushModel(fileModel);

            var fields = new[] { "ER1", "", "PREMNO", "" };

            // act
            concern.ParseError(context, fields);
            context.ResolveToFile();

            // assert
            var results = context.Results;
            var collection = results.TypeHeaders;
            var model = collection.First();

            var errors = model.ErrorRecords;
            Assert.AreEqual(1, errors.Length);

            var error = errors[0];
            Assert.AreEqual(expected, error.RecordTypeId);
            Assert.IsNull(error.FieldName);
        }
    }
}
