using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DLToolkit.Forms.Controls;
using Xamarin.Forms;

namespace YogiBearX.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            FlowListView.Init();
        }
    }
}
