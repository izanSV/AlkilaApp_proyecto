

namespace AlkilaApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Login());

        }

        public static void Main(string[] args) { }

    }
}
