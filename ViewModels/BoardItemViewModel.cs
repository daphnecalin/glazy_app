using ReactiveUI;

namespace ASTEM_DB.ViewModels
{
    public class BoardItemViewModel : ViewModelBase
    {
        private int _id;
        private Avalonia.Media.Imaging.Bitmap _image = null!;

        private int _tileCount;

        private string _description = string.Empty;
        private string _createdBy = string.Empty;
        private string _imagePath = string.Empty;
        public int Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }

        public int TileCount
        {
            get => _tileCount;
            set => this.RaiseAndSetIfChanged(ref _tileCount, value);
        }
        public string ImagePath
        {
            get => _imagePath;
            set => this.RaiseAndSetIfChanged(ref _imagePath, value);
        }

        public string Description
        {
            get => _description;
            set => this.RaiseAndSetIfChanged(ref _description, value);
        }

        public string CreatedBy
        {
            get => _createdBy;
            set => this.RaiseAndSetIfChanged(ref _createdBy, value);
        }

        public Avalonia.Media.Imaging.Bitmap Image
        {
            get => _image;
            set => this.RaiseAndSetIfChanged(ref _image, value);
        }


    }
}