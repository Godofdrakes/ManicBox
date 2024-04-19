using System.Drawing;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ManicBox.Interop;
using ManicBox.Interop.Common;
using ManicBox.Preview.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Preview.ViewModel;

public sealed class ThumbnailViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	[Reactive] public bool Visible { get; set; } = true;
	[Reactive] public byte Opacity { get; set; } = byte.MaxValue;

	[Reactive] public bool SourceClientAreaOnly { get; set; } = true;

	// [Reactive] public Margins SourceRect { get; set; } = Margins.Fill;
	[Reactive] public Margins DestinationRect { get; set; } = Margins.Fill;

	[Reactive] public HWND DestinationWindow { get; set; }
	[Reactive] public HWND SourceWindow { get; set; }

	[ObservableAsProperty] public bool HasThumbnail { get; }
	[ObservableAsProperty] public Size SourceSize { get; }
	[ObservableAsProperty] private DwmApi.Thumbnail? Thumbnail { get; }

	public ThumbnailViewModel()
	{
		// Update thumbnail properties
		this.WhenAnyValue(
				viewModel => viewModel.Thumbnail,
				viewModel => viewModel.Visible,
				viewModel => viewModel.Opacity,
				viewModel => viewModel.DestinationRect,
				viewModel => viewModel.SourceClientAreaOnly,
				( thumbnail, visible, opacity, destinationRect, clientAreaOnly ) =>
				{
					var properties = new DwmApi.ThumbnailProperties();
					properties.SetVisible( visible );
					properties.SetOpacity( opacity );
					properties.SetDestinationRect( destinationRect );
					properties.SetSourceClientAreaOnly( clientAreaOnly );
					return (thumbnail, properties);
				} )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Subscribe( tuple => tuple.thumbnail?.SetProperties( ref tuple.properties ) );

		// Maintain a bool of whether the thumbnail is valid or not
		this.WhenAnyValue( viewModel => viewModel.Thumbnail )
			.Select( thumbnail => thumbnail is not null )
			.ObserveOn( RxApp.MainThreadScheduler )
			.ToPropertyEx( this, viewModel => viewModel.HasThumbnail );

		// Maintain a read-only property of the window's actual size
		this.WhenAnyValue( viewModel => viewModel.SourceWindow )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Select( window => window.IsNull ? Observable.Never<Margins>() : User32.OnWindowMoveSize( window ) )
			.Switch()
			.Select( margins => new Size( margins.Right - margins.Left, margins.Bottom - margins.Top ) )
			.ToPropertyEx( this, viewModel => viewModel.SourceSize );

		this.WhenActivated( d =>
		{
			// Create a thumbnail when the window properties are properly set
			this.WhenAnyValue( viewModel => viewModel.DestinationWindow, viewModel => viewModel.SourceWindow )
				.ObserveOn( RxApp.MainThreadScheduler )
				.Select( tuple =>
				{
					if ( tuple.Item1.IsNull )
					{
						return null;
					}

					if ( tuple.Item2.IsNull )
					{
						return null;
					}

					if ( tuple.Item1 == tuple.Item2 )
					{
						return null;
					}

					return new DwmApi.Thumbnail( tuple.Item1, tuple.Item2 );
				} )
				.DisposeEach()
				.ToPropertyEx( this, viewModel => viewModel.Thumbnail )
				.DisposeWith( d );
		} );
	}
}