using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ManicBox.Interop;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Preview.ViewModel;

public sealed class HWNDViewModel : ReactiveObject, IDisposable
{
	[Reactive] public HWND HWND { get; set; }

	[ObservableAsProperty] public string Title { get; }

	private readonly CompositeDisposable _onDispose = new();

	public HWNDViewModel()
	{
		this.WhenAnyValue( viewModel => viewModel.HWND )
			.Select( hwnd =>
				hwnd.IsValid
					? User32.OnWindowTitleChanged( hwnd )
					: Observable.Never<string>().Prepend( string.Empty ) )
			.Switch()
			.SubscribeOn( RxApp.MainThreadScheduler )
			.ToPropertyEx( this, viewModel => viewModel.Title )
			.DisposeWith( _onDispose );
	}

	public void Dispose() => _onDispose.Dispose();
}

public class WindowListViewModel : ReactiveObject, IActivatableViewModel
{
	public ViewModelActivator Activator { get; } = new();

	public ReadOnlyObservableCollection<HWNDViewModel> Windows => _windows;

	private readonly ReadOnlyObservableCollection<HWNDViewModel> _windows;

	private readonly SourceCache<HWND, HWND> _cache = new( hwnd => hwnd );

	public WindowListViewModel()
	{
		_cache.Connect()
			.Transform( hwnd => new HWNDViewModel()
			{
				HWND = hwnd
			} )
			.DisposeMany()
			.Bind( out _windows )
			.Subscribe();

		this.WhenActivated( d =>
		{
			var changeSet = ObservableChangeSet.Create<HWND, HWND>( cache =>
				{
					var onDispose = new CompositeDisposable();

					User32.OnWindowCreated()
						.SubscribeOn( RxApp.MainThreadScheduler )
						.Subscribe( cache.AddOrUpdate )
						.DisposeWith( onDispose );

					User32.OnWindowDestroyed()
						.SubscribeOn( RxApp.MainThreadScheduler )
						.Subscribe( cache.RemoveKey )
						.DisposeWith( d );

					return onDispose;
				},
				hwnd => hwnd );

			changeSet
				.PopulateInto( _cache )
				.DisposeWith( d );
		} );
	}
}