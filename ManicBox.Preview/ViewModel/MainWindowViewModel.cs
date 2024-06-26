﻿using System.Reactive.Disposables;
using ManicBox.Common.ViewModel;
using ManicBox.Interop;
using ManicBox.Services.Interface;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ManicBox.Preview.ViewModel;

public class MainWindowViewModel : WindowViewModel
{
	[Reactive] public HWND OwningWindowHandle { get; set; }

	public WindowListViewModel WindowListViewModel { get; }

	public ThumbnailViewModel ThumbnailViewModel { get; }

	public MainWindowViewModel( IWindowMonitorService windowMonitorService )
	{
		WindowListViewModel = new WindowListViewModel( windowMonitorService );

		ThumbnailViewModel = new ThumbnailViewModel();

		this.WhenActivated( d =>
		{
			this.WhenAnyValue( viewModel => viewModel.OwningWindowHandle )
				.BindTo( ThumbnailViewModel, viewModel => viewModel.DestinationWindow )
				.DisposeWith( d );

			this.WhenAnyValue( viewModel => viewModel.WindowListViewModel.SelectedItem )
				.BindTo( ThumbnailViewModel, viewModel => viewModel.SourceWindow )
				.DisposeWith( d );
		} );
	}
}