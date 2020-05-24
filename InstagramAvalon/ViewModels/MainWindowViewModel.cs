namespace InstagramAvalon.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia.Controls;
    using Commands;
    using ReactiveUI;
    using Views;
    using static InstagramUploader;

    public class MainWindowViewModel : ReactiveObject //ViewModelBase
    {
        private readonly Window _appWindow;

        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _videoPath = string.Empty;
        private string _imagePath = string.Empty;
        private string _comments = string.Empty;

        private bool _uploadEnabled = false;

        public MainWindowViewModel(Window appWindow)
        {
            _appWindow = appWindow;
            SelectVideoPath = new RelayCommand(new Action<object>(GetVideoPath), () => true);
            SelectImagePath = new RelayCommand(new Action<object>(GetImagePath), () => true);
            UploadVideo = new RelayCommand(new Action<object>(UploadToInstagram), () => true);
        }

        public ICommand SelectVideoPath { get; set; }
        public ICommand SelectImagePath { get; set; }
        public ICommand UploadVideo { get; set; }

        public string Username
        {
            get => _userName;
            set
            {
                RunChangeState();
                this.RaiseAndSetIfChanged(ref _userName, value);
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                RunChangeState();
                this.RaiseAndSetIfChanged(ref _password, value);
            }
        }

        public string VideoPath
        {
            get => _videoPath;
            set
            {
                RunChangeState();
                this.RaiseAndSetIfChanged(ref _videoPath, value);
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                RunChangeState();
                this.RaiseAndSetIfChanged(ref _imagePath, value);
            }
        }

        public string Comments
        {
            get => _comments;
            set
            {
                RunChangeState();
                this.RaiseAndSetIfChanged(ref _comments, value);
            }
        }

        public bool UploadEnabled
        {
            get => _uploadEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref _uploadEnabled, value);
            }
        }

        public async void GetVideoPath(object obj)
        {
            var fileDialog = getDialog("movies", "mp4", "mov");

            var result = await fileDialog.ShowAsync(_appWindow);
            var first = result.FirstOrDefault();

            VideoPath = first;
        }

        public async void GetImagePath(object obj)
        {
            var fileDialog = getDialog("images", "jpeg", "jpg", "png", "gif", "bmp");

            var result = await fileDialog.ShowAsync(_appWindow);
            var first = result.FirstOrDefault();
            ImagePath = first;
        }

        public async void UploadToInstagram(object obj)
        {
            var data = new InstagramData
            {
                Username = Username,
                Comments = Comments,
                ImagePath = ImagePath,
                Password = Password,
                VideoPath = VideoPath
            };

            _appWindow.IsEnabled = false;
            
            var resultMessage = await UploadVideo(data);
            
            await MessageBox.Show(_appWindow, resultMessage, "Video Upload", MessageBox.MessageBoxButtons.Ok);
            _appWindow.IsEnabled = true;
        }

        private void RunChangeState()
        {
            UploadEnabled = !(string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password)) &&
                            File.Exists(VideoPath) && File.Exists(ImagePath);
        }


        private OpenFileDialog getDialog(string name, params string[] array) =>
            new OpenFileDialog()
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>(new[]
                {
                    new FileDialogFilter
                    {
                        Name = name,
                        Extensions = new List<string>(array)
                    }
                }),
            };
    }
}
