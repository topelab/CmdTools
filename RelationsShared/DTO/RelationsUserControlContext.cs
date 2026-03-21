namespace RelationsShared.DTO
{
    using ReactiveUI;
    using System.Runtime.Serialization;

    [DataContract]
    public class RelationsUserControlContext : ReactiveObject
    {
        private string selectedItem;
        private bool isUsedBy;
        private bool isUsing;
        private bool includePackages;
        private string url;
        private bool showListOnly;
        private double windowWidth = 1920;
        private double windowHeight = 1080;

        [DataMember]
        public string Url { get => url; set => this.RaiseAndSetIfChanged(ref url, value); }

        [DataMember]
        public string UserDataFolder { get; set; }
        [DataMember]
        public string MermaidFile { get; set; }
        [DataMember]
        public UserSettings UserSettings { get; set; }
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public List<string> Items { get; set; } = [];

        [DataMember]
        public string SelectedItem { get => selectedItem; set => this.RaiseAndSetIfChanged(ref selectedItem, value); }


        [DataMember]
        public bool IncludePackages
        {
            get { return includePackages; }
            set { this.RaiseAndSetIfChanged(ref includePackages, value); }
        }

        [DataMember]
        public bool ShowListOnly
        {
            get => showListOnly;
            set => this.RaiseAndSetIfChanged(ref showListOnly, value);
        }

        [DataMember]
        public bool IncludePackagesVisibility => IsUsedBy;


        [DataMember]
        public bool IsUsedBy
        {
            get { return isUsedBy; }
            set
            {
                if (isUsedBy == value) return;
                isUsedBy = value;
                this.RaisePropertyChanged(nameof(IsUsedBy));
                this.RaisePropertyChanged(nameof(IncludePackagesVisibility));
                this.RaisePropertyChanged(nameof(RelationType));
            }
        }

        [DataMember]
        public bool IsUsing
        {
            get { return isUsing; }
            set { this.RaiseAndSetIfChanged(ref isUsing, value); }
        }

        [DataMember]
        public RelationType RelationType => IsUsedBy ? RelationType.UsedBy : RelationType.Using;

        /// <summary>
        /// Ancho de la ventana del diálogo.
        /// </summary>
        [DataMember]
        public double WindowWidth
        {
            get => windowWidth;
            set => this.RaiseAndSetIfChanged(ref windowWidth, value);
        }

        /// <summary>
        /// Alto de la ventana del diálogo.
        /// </summary>
        [DataMember]
        public double WindowHeight
        {
            get => windowHeight;
            set => this.RaiseAndSetIfChanged(ref windowHeight, value);
        }
    }
}
