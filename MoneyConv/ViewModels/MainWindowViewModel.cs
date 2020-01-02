using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace MoneyConv.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// string which shall be converted
        /// </summary>
        public string CurrencyInput { get; set; }
        
        /// <summary>
        /// response from the server
        /// </summary>
        public string ServerResponse { get; set; }
        
        /// <summary>
        /// Command to send <see cref="CurrencyInput"/> to the server and display its <see cref="ServerResponse"/>
        /// </summary>
        public ICommand ConvertInput { get; set; }


        public MainWindowViewModel()
        {
            ConvertInput = new ActionCommand(OnConvertInputExecute, OnConvertInputCanExecute);
        }


        private bool OnConvertInputCanExecute(object parameter)
        {
            //an existing input value is required to be processed at all
            return !string.IsNullOrEmpty(CurrencyInput);
        }

        private void OnConvertInputExecute(object parameter)
        {
        //todo: backgroundworker/thread/async to prevent UI freezes
            try
            {
                // get serviceclient and send input
                using (var client = new MoneyConvSrv.MoneyConvV1Client())
                {
                    var resp = client.ConvertNumberToWords(CurrencyInput);
                    ServerResponse = resp.Success ? resp.Value : resp.Message;

                    OnPropertyChanged(nameof(ServerResponse));
                }
            }
            catch (Exception e)
            {
                ServerResponse = e.Message;

                OnPropertyChanged(nameof(ServerResponse));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
