using System.Windows;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;

namespace HelloWorld
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a model instance and check connection status
            var model = new Model();
            if (!model.GetConnectionStatus())
            {
                MessageBox.Show("Tekla Structure not connected");
                return;
            }

            // get model info and send a "Hello world" message to the message box
            var modelInfo = model.GetInfo();
            var name = modelInfo.ModelName;

            MessageBox.Show($"Hello world! your current model is named: {name}");

            // send a hello world message to the Tekla Structures user command prompt
            Operation.DisplayPrompt($"Hello world! your current model is named: {name}");
        }
    }
}