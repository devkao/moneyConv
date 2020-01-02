using MoneyConv.Common.Interfaces;
using System.ServiceModel;

namespace MoneyConv
{
    /// <summary>
    /// custom implementation if needed
    /// </summary>
    public class SampleClient : ClientBase<IMoneyConvV1>, IMoneyConvV1
    {
        public SampleClient()
        {
        }

        public SampleClient(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public SampleClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
        }

        public SampleClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
        }

        public SampleClient(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
        }

        public ApiResponse<string> ConvertNumberToWords(string currencyValue)
        {
            return Channel.ConvertNumberToWords(currencyValue);
        }
    }
}
