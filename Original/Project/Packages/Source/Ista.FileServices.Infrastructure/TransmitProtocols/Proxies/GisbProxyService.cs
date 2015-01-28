using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Web.Services2;

namespace Ista.FileServices.Infrastructure.TransmitProtocols.Proxies
{
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [WebServiceBinding(Name = "GISBSoap", Namespace = "https://services.ista-billing.com/api")]
    public class GisbProxyService : WebServicesClientProtocol
    {
        public GisbProxyService()
        {
            Url = "https://services.ista-billing.com/api/GISB.asmx";
        }

        public IAsyncResult BeginReportError(string errorCode, string errorMessage, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("ReportError", new object[] { errorCode, errorMessage }, callback, asyncState);
        }

        public IAsyncResult BeginSendEDI(Byte[] ediTransaction, Byte[] hash, string transaction, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("SendEDI", new object[] { ediTransaction, hash, transaction }, callback, asyncState);
        }

        public void EndReportError(IAsyncResult asyncResult)
        {
            EndInvoke(asyncResult);
        }

        public ApiResponse EndSendEDI(IAsyncResult asyncResult)
        {
            var results = EndInvoke(asyncResult);
            return ((ApiResponse)(results[0]));
        }

        [SoapDocumentMethod("https://services.ista-billing.com/api/ReportError", RequestNamespace = "https://services.ista-billing.com/api", ResponseNamespace = "https://services.ista-billing.com/api", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public void ReportError(string errorCode, string errorMessage)
        {
            Invoke("ReportError", new object[] {errorCode, errorMessage});
        }

        [SoapDocumentMethod("https://services.ista-billing.com/api/SendEDI", RequestNamespace = "https://services.ista-billing.com/api", ResponseNamespace = "https://services.ista-billing.com/api", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ApiResponse SendEDI([XmlElementAttribute(DataType = "base64Binary")] Byte[] ediTransaction, [XmlElementAttribute(DataType = "base64Binary")] Byte[] hash, string transaction)
        {
            var results = Invoke("SendEDI", new object[] {ediTransaction, hash, transaction});
            return ((ApiResponse)(results[0]));
        }
    }

    [XmlType(Namespace = "https://services.ista-billing.com/api")]
    public class ApiResponse
    {
        public bool SuccessFlag;
        public string ErrorCode;
        public string ErrorMessage;
    }
}
