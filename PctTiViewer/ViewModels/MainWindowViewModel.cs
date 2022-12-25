using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using PctTiViewer.Models;
using PctTiViewer.Views;
using ReactiveUI;

namespace PctTiViewer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ReadOnlyObservableCollection<string> FileAtributes { get; }

        private MainWindowModel _model;
        
        public MainWindowViewModel()
        {
            _model = new();
            FileAtributes = _model.FileAttributes;
            
            LoadFile = ReactiveCommand.Create<Window>(async x =>
            {
                var dialog = new OpenFileDialog();
                dialog.AllowMultiple = false;

                var result = await dialog.ShowAsync(x);

                if (result != null && result.Length != 0)
                {
                    await _model.LoadFile(result[0]);
                }
            });
        }
        
        public ReactiveCommand<Window, Unit> LoadFile { get; } 
    }
}