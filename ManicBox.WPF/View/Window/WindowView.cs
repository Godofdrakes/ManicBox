using System.Windows;
using ManicBox.WPF.ViewModel;
using ReactiveUI;

namespace ManicBox.WPF.View;

public class WindowView<TViewModel> : Window, IViewFor<TViewModel>
	where TViewModel : WindowViewModel
{
	public static readonly DependencyProperty ViewModelProperty =
		DependencyProperty.Register(
			nameof(ViewModel),
			typeof(TViewModel),
			typeof(WindowView<TViewModel>),
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

	public WindowView()
	{
		this.WhenAnyValue( view => view.ViewModel )
			.BindTo( this, view => view.DataContext );
	}
}