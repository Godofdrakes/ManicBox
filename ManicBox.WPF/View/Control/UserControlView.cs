using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ManicBox.WPF.ViewModel;
using ReactiveUI;

namespace ManicBox.WPF.View.Control;

public class UserControlView<TViewModel> : UserControl, IViewFor<TViewModel>
	where TViewModel : UserControlViewModel
{
	public static readonly DependencyProperty ViewModelProperty =
		DependencyProperty.Register(
			nameof(ViewModel),
			typeof(TViewModel),
			typeof(UserControlView<TViewModel>),
			new PropertyMetadata( null ) );

	public TViewModel? ViewModel
	{
		get => (TViewModel)GetValue( ViewModelProperty );
		set => SetValue( ViewModelProperty, value );
	}

	object? IViewFor.ViewModel
	{
		get => ViewModel;
		set => ViewModel = (TViewModel?)value;
	}

	public UserControlView()
	{
		this.WhenActivated( d =>
		{
			this.WhenAnyValue( view => view.ViewModel )
				.BindTo( this, view => view.DataContext )
				.DisposeWith( d );
		} );
	}
}