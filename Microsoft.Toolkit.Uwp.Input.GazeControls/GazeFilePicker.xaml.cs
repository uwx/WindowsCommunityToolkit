// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.Toolkit.Uwp.Input.GazeControls
{
    public sealed partial class GazeFilePicker : ContentDialog, INotifyPropertyChanged
    {
        private Grid _commandSpaceGrid;
        private Button _newFolderButton;
        private Button _enterFilenameButton;
        private Button _selectButton;
        private Button _cancelButton;
        private DispatcherTimer _initializationTimer;

        private PathPart[] _currentFolderParts;
        private ObservableCollection<StorageItem> _currentFolderItems;

        private StorageItem _curSelectedItem;

        private StorageItem _selectedItem;

        public bool SaveMode = false;

        public IStorageItem SelectedItem
        {
            get
            {
                return _selectedItem != null ? _selectedItem.Item : null;
            }
        }

        private StorageFolder _currentFolder;

        public StorageFolder CurrentFolder
        {
            get
            {
                return _currentFolder;
            }

            set
            {
                RefreshContents(value.Path);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GazeFilePicker"/> class.
        /// </summary>
        public GazeFilePicker()
        {
            this.InitializeComponent();

            this.Opened += OnGazeFilePickerOpened;

            _initializationTimer = new DispatcherTimer();
            _initializationTimer.Interval = TimeSpan.FromMilliseconds(125);
            _initializationTimer.Tick += OnInitializationTimerTick;
        }

        private void OnInitializationTimerTick(object sender, object e)
        {
            _initializationTimer.Stop();
            CreateDialogButtons();
            GazeKeyboard.Target = FilenameTextbox;
            SetFileListingsLayout();
        }

        private void CreateDialogButtons()
        {
            _commandSpaceGrid = this.FindControl<Grid>("CommandSpace");
            Debug.Assert(_commandSpaceGrid != null, "CommandSpaceGrid not found");

            _commandSpaceGrid.Children.Clear();
            _commandSpaceGrid.RowDefinitions.Clear();
            _commandSpaceGrid.ColumnDefinitions.Clear();

            _commandSpaceGrid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < 4; i++)
            {
                _commandSpaceGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            var style = (Style)this.Resources["PickerButtonStyles"];
            this.PrimaryButtonStyle = style;
            this.SecondaryButtonStyle = style;
            this.CloseButtonStyle = style;

            _newFolderButton = new Button();
            _newFolderButton.Content = "New Folder...";
            _newFolderButton.Style = style;
            _newFolderButton.Click += OnNewFolderClick;

            _enterFilenameButton = new Button();
            _enterFilenameButton.Content = "Enter file name...";
            _enterFilenameButton.Style = style;
            _enterFilenameButton.Click += OnNewFolderClick;

            _selectButton = _commandSpaceGrid.FindName("PrimaryButton") as Button;
            _selectButton.Click += OnSelectButtonClick;
            _selectButton.Content = "Select";

            _cancelButton = _commandSpaceGrid.FindName("CloseButton") as Button;
            _cancelButton.Click += OnCancelButtonClick;
            _cancelButton.Content = "Cancel";

            _commandSpaceGrid.Children.Add(_newFolderButton);
            _commandSpaceGrid.Children.Add(_enterFilenameButton);
            _commandSpaceGrid.Children.Add(_selectButton);
            _commandSpaceGrid.Children.Add(_cancelButton);

            _enterFilenameButton.Content = "Enter Filename...";
            _enterFilenameButton.Click += OnEnterFilenameButtonClick;

            Grid.SetRow(_newFolderButton, 0);
            Grid.SetRow(_enterFilenameButton, 0);
            Grid.SetRow(_selectButton, 0);
            Grid.SetRow(_cancelButton, 0);

            Grid.SetColumnSpan(_newFolderButton, 1);
            Grid.SetColumnSpan(_enterFilenameButton, 1);
            Grid.SetColumnSpan(_selectButton, 1);
            Grid.SetColumnSpan(_cancelButton, 1);

            Grid.SetColumn(_newFolderButton, 0);
            Grid.SetColumn(_enterFilenameButton, 1);
            Grid.SetColumn(_selectButton, 2);
            Grid.SetColumn(_cancelButton, 3);

            SetFileListingsLayout();
        }

        private void OnSelectButtonClick(object sender, RoutedEventArgs e)
        {
            if (!SaveMode)
            {
                _selectedItem = _curSelectedItem;
            }
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            SetFileListingsLayout();
        }

        private void SetFileListingsLayout()
        {
            FileListingGrid.Visibility = Visibility.Visible;
            FilenameEntryGrid.Visibility = Visibility.Collapsed;

            var vis = SaveMode ? Visibility.Visible : Visibility.Collapsed;
            _newFolderButton.Visibility = vis;
            _enterFilenameButton.Visibility = vis;
        }

        private void SetKeyboardInputLayout()
        {
            FileListingGrid.Visibility = Visibility.Collapsed;
            FilenameEntryGrid.Visibility = Visibility.Visible;

            _newFolderButton.Visibility = Visibility.Collapsed;
            _enterFilenameButton.Visibility = Visibility.Collapsed;
        }

        private void OnEnterFilenameButtonClick(object sender, RoutedEventArgs e)
        {
            SetKeyboardInputLayout();
        }

        private async void OnNewFolderClick(object sender, RoutedEventArgs e)
        {
            SetKeyboardInputLayout();
            await GazeKeyboard.LoadLayout("FilenameEntry.xaml");
        }

        private void OnGazeFilePickerOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            _initializationTimer.Start();
        }

        private Task[] GetThumbnailsAsync(ObservableCollection<StorageItem> storageItems)
        {
            Task[] tasks = new Task[storageItems.Count];
            for (int i = 0; i < storageItems.Count; i++)
            {
                tasks[i] = storageItems[i].GetThumbnailAsync();
            }

            return tasks;
        }

        private async void RefreshContents(string path)
        {
            _currentFolder = await StorageFolder.GetFolderFromPathAsync(path);
            var items = await _currentFolder.GetItemsAsync();
            _currentFolderItems = new ObservableCollection<StorageItem>(items.Select(item => new StorageItem(item)));
            OnPropertyChanged("_currentFolderItems");

            var tasks = GetThumbnailsAsync(_currentFolderItems);
            await Task.WhenAll(tasks);
            foreach (var item in _currentFolderItems)
            {
                item.OnPropertyChanged("Thumbnail");
            }

            var parts = _currentFolder.Path.Split('\\');
            _currentFolderParts = parts.Select((part, index) => new PathPart { Index = index, Name = part }).ToArray();

            OnPropertyChanged("_currentFolderParts");
        }

        private void OnCurrentFolderContentsItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = e.ClickedItem as StorageItem;
            var selectedItem = CurrentFolderContents.SelectedItem as StorageItem;

            if ((clickedItem == selectedItem) && clickedItem.IsFolder)
            {
                RefreshContents(clickedItem.Path);
            }

            _curSelectedItem = clickedItem;

            OnPropertyChanged("_curSelectedItem");
        }

        private void OnPathPartClick(object sender, RoutedEventArgs e)
        {
            var buttonIndex = int.Parse((sender as Button).Tag.ToString());
            int selectedIndex = CurrentFolderPartsList.SelectedIndex;
            if (buttonIndex != selectedIndex)
            {
                CurrentFolderPartsList.SelectedIndex = buttonIndex;
                return;
            }

            var newFolder = Path.Combine(_currentFolderParts.Select(part => part.Name).Take(buttonIndex + 1).ToArray());
            RefreshContents(newFolder);
        }

        private void OnFilePickerClosing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if ((args.Result == ContentDialogResult.None) && (FileListingGrid.Visibility == Visibility.Collapsed))
            {
                args.Cancel = true;
                return;
            }
        }
    }
}
