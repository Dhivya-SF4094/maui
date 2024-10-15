﻿#nullable enable
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Platform;
using System.Reflection;

namespace Maui.Controls.Sample.Issues
{
    [Issue(IssueTracker.Github, 20706, "Stepper doesn't change increment value when being bound to a double in MVVM context (Windows)")]
    public partial class Issue20706 : ContentPage
    {
        public Issue20706()
        {
            InitializeComponent();
        }

		private void AddItemsButton_Clicked(object sender, EventArgs e)
		{
			stepperValue.Increment = 10;
		}
	}
	internal class ViewModelClass : INotifyPropertyChanged
    {
        private double _increment;

        public double Increment
        {
			get 
			{
				return _increment; 
			}
			
			set
			{  _increment = value;
				OnPropertyChanged("Increment");
			}
        }

		public ViewModelClass()
		{
			Increment = 2;
		}
		private void OnPropertyChanged(string v)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(v));
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}

