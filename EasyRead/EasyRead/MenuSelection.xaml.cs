﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasyRead
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuSelection : ContentPage
    {
        public MenuSelection()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            App.Current.MainPage = new MainPage();
            return true;
        }

        private void Text_Detection_Clicked(object sender, EventArgs e)
        {
            Settings.mSelection = Settings.TEXT_DETECTION;
            LoadMainApplication();
        }

        private void LoadMainApplication()
        {
            App.Current.MainPage = new AppPage();
        }

        private void Safe_Detection_Clicked(object sender, EventArgs e)
        {
            Settings.mSelection = Settings.SAFE_SEARCH_DETECTION;
            LoadMainApplication();
        }

        private void Logo_Detection_Clicked(object sender, EventArgs e)
        {
            Settings.mSelection = Settings.LOGO_DETECTION;
            LoadMainApplication();
        }

        private void Label_Detection_Clicked(object sender, EventArgs e)
        {
            Settings.mSelection = Settings.LABEL_DETECTION;
            LoadMainApplication();
        }

        private void Landmark_Detection_Clicked(object sender, EventArgs e)
        {
            Settings.mSelection = Settings.LANDMARK_DETECTION;
            LoadMainApplication();
        }
    }
}