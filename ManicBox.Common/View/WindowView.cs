using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ManicBox.Common.ViewModel;
using ReactiveUI;

namespace ManicBox.Common.View;

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
		this.WhenActivated( d =>
		{
			this.WhenAnyValue( view => view.ViewModel )
				.BindTo( this, view => view.DataContext )
				.DisposeWith( d );

			this.WhenAnyValue( view => view.ViewModel!.Title )
				.Where( title => !string.IsNullOrEmpty( title ) )
				.BindTo( this, view => view.Title )
				.DisposeWith( d );
		} );
	}
}