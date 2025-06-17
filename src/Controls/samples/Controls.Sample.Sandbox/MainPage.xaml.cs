using System.ComponentModel;

namespace Maui.Controls.Sample;

public partial class MainPage : ContentPage
{
	public class ImageViewModel : INotifyPropertyChanged
	{
		private Aspect _aspect = Aspect.AspectFit;
		public Aspect Aspect
		{
			get => _aspect;
			set
			{
				if (_aspect != value)
				{
					_aspect = value;
					OnPropertyChanged();
				}
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	private readonly ImageViewModel _viewModel = new ImageViewModel();

	public MainPage()
	{
		InitializeComponent();
		BindingContext = _viewModel;
	}

	private void AspectRadio_CheckedChanged(object sender, CheckedChangedEventArgs e)
	{
		if (!(sender is RadioButton rb) || !rb.IsChecked)
			return;
		switch (rb.Content?.ToString())
		{
			case "AspectFit":
				_viewModel.Aspect = Aspect.AspectFit;
				break;
			case "AspectFill":
				_viewModel.Aspect = Aspect.AspectFill;
				break;
			case "Fill":
				_viewModel.Aspect = Aspect.Fill;
				break;
			case "Center":
				_viewModel.Aspect = Aspect.Center;
				break;
		}
	}
}