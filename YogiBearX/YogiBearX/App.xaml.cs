using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YogiBearX.Model;
using YogiBearX.Persistence;
using YogiBearX.ViewModel;
using YogiBearX.View;

using Xamarin.Forms;

namespace YogiBearX
{
    public partial class App : Application
    {
        private YogiBearModel model;
        private YogiBearViewModel viewModel;
        private MainPage gamePage;

        public App()
        {
            //       InitializeComponent();

            model = new YogiBearModel();

            viewModel = new YogiBearViewModel(model);
            viewModel.NewGame += new EventHandler(VM_NewGame);
            viewModel.VM_GameOver += new EventHandler<GameOverEventArgs>(ViewModel_GameOver);

            gamePage = new MainPage();
            gamePage.BindingContext = viewModel;

            MainPage = gamePage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
           // model.NewGame();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private async void VM_NewGame(object sender, EventArgs e)
        {
            await model.NewGame();
            viewModel.CopyToFields();
        }

        private async void ViewModel_GameOver(object sender, GameOverEventArgs e)
        {
            if (e.result)
                await MainPage.DisplayAlert("YogiBear", "Gratulálok, győztél!", "OK");
            else
            {
                Device.BeginInvokeOnMainThread(() => {
                    MainPage.DisplayAlert("YogiBear", "Sajnos vesztettél!", "OK");
                });
            }
               
        }
    }
}
