using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace ManicBox.Reactive.ViewBase;

public class UserControlView<TViewModel> : UserControl, IViewFor<TViewModel>
	where TViewModel : class
{
	/// <summary>
	/// The view model dependency property.
	/// </summary>
	public static readonly DependencyProperty ViewModelProperty =
		DependencyProperty.Register(
			nameof(ViewModel),
			typeof(TViewModel),
			typeof(UserControlView<TViewModel>),
			new PropertyMetadata( null ) );

	/// <inheritdoc/>
	public TViewModel? ViewModel
	{
		get => (TViewModel)GetValue( ViewModelProperty );
		set => SetValue( ViewModelProperty, value );
	}

	/// <inheritdoc/>
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
				.BindTo( this, view => view.DataContext );
		} );
	}
}