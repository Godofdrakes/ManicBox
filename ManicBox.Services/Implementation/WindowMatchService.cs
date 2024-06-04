using System.Collections.ObjectModel;
using DynamicData;
using DynamicData.Binding;
using ManicBox.Services.Interface;
using ManicBox.Services.ViewModel;

namespace ManicBox.Services.Implementation;

public sealed class WindowMatchService : IWindowMatchService
{
	private readonly SourceCache<IWindowMatchItem, IWindowMatchItem> _sourceCache;

	public WindowMatchService()
	{
		_sourceCache = new SourceCache<IWindowMatchItem, IWindowMatchItem>( item => item );

		// todo
		Add( "ManicBox", "ManicBox.Mock" );
		Add( "RuneLite" );
		Add( "UnrealEditor" );
	}

	public IObservable<IChangeSet<IWindowMatchItem, IWindowMatchItem>> GetItems()
	{
		return _sourceCache.Connect();
	}

	public void Add( IWindowMatchItem item ) => _sourceCache.AddOrUpdate( item );

	public void Remove( IWindowMatchItem item ) => _sourceCache.RemoveKey( item );

	public void Add( string processName, string? windowTitle = default )
	{
		ArgumentException.ThrowIfNullOrEmpty( processName );

		Add( new WindowTitleMatchViewModel()
		{
			ProcessName = processName,
			WindowTitle = windowTitle ?? string.Empty,
		} );
	}

	public IObservable<bool> HasMatch( WindowHandleViewModel viewModel )
	{
		return _sourceCache.Connect()
			.AutoRefreshOnObservable( item =>
				item.WhenAnyPropertyChanged() )
			.QueryWhenChanged( cache =>
				cache.Items.Any( item =>
					item.IsMatch( viewModel ) ) );
	}
}