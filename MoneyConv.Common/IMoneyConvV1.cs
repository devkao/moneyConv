using System.Runtime.Serialization;
using System.ServiceModel;

namespace MoneyConv.Common.Interfaces
{
    [ServiceContract]
    public interface IMoneyConvV1
    {
        [OperationContract()]
        ApiResponse<string> ConvertNumberToWords(string currencyValue);
    }

    [DataContract]
    public class ApiResponse<T>
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public T Value { get; set; }
    }
}
