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

	// [Reactive] public Rectangle SourceRect { get; set; }
	[Reactive] public Margins DestinationRect { get; set; }

	[Reactive] public HWND DestinationWindow { get; set; }
	[Reactive] public HWND SourceWindow { get; set; }

	[ObservableAsProperty] public bool HasThumbnail { get; }
	[ObservableAsProperty] private DwmApi.Thumbnail? Thumbnail { get; }

	public ThumbnailViewModel()
	{
		this.WhenAnyValue( viewModel => viewModel.Thumbnail, viewModel => viewModel.Opacity )
			.Where( tuple => tuple.Item1 is not null )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Subscribe( SetOpacity! );
		this.WhenAnyValue( viewModel => viewModel.Thumbnail, viewModel => viewModel.Visible )
			.Where( tuple => tuple.Item1 is not null )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Subscribe( SetVisible! );
		this.WhenAnyValue( viewModel => viewModel.Thumbnail, viewModel => viewModel.SourceClientAreaOnly )
			.Where( tuple => tuple.Item1 is not null )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Subscribe( SetSourceClientAreaOnly! );
		// this.WhenAnyValue( viewModel => viewModel.Thumbnail, viewModel => viewModel.SourceRect )
		// 	.Where( tuple => tuple.Item1 is not null )
		// 	.ObserveOn( RxApp.MainThreadScheduler )
		// 	.Subscribe( SetSourceRect! );
		this.WhenAnyValue( viewModel => viewModel.Thumbnail, viewModel => viewModel.DestinationRect )
			.Where( tuple => tuple.Item1 is not null )
			.ObserveOn( RxApp.MainThreadScheduler )
			.Subscribe( SetDestinationRect! );
		this.WhenAnyValue( viewModel => viewModel.Thumbnail )
			.Select( thumbnail => thumbnail is not null )
			.ObserveOn( RxApp.MainThreadScheduler )
			.ToPropertyEx( this, viewModel => viewModel.HasThumbnail );

		this.WhenActivated( d =>
		{
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

	private static void SetOpacity( (DwmApi.Thumbnail Thumbnail, byte Value) tuple )
	{
		tuple.Thumbnail.SetProperties( props => props.SetOpacity( tuple.Value ) );
	}

	private static void SetVisible( (DwmApi.Thumbnail Thumbnail, bool Value) tuple )
	{
		tuple.Thumbnail.SetProperties( props => props.SetVisible( tuple.Value ) );
	}

	private static void SetSourceClientAreaOnly( (DwmApi.Thumbnail Thumbnail, bool Value) tuple )
	{
		tuple.Thumbnail.SetProperties( props => props.SetSourceClientAreaOnly( tuple.Value ) );
	}

	// private static void SetSourceRect( (DwmApi.Thumbnail Thumbnail, Rectangle Value) tuple )
	// {
	// 	tuple.Thumbnail.SetProperties( props => props.SetSourceRect( tuple.Value ) );
	// }

	private static void SetDestinationRect( (DwmApi.Thumbnail Thumbnail, Margins Value) tuple )
	{
		tuple.Thumbnail.SetProperties( props => props.SetDestinationRect( tuple.Value ) );
	}
}