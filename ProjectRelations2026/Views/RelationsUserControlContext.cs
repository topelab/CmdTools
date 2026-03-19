namespace ProjectRelations2026.Views
{
    using Microsoft.VisualStudio.Extensibility.UI;
    using ProjectRelations2026.DTO;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Forms;

    [DataContract]
    public class RelationsUserControlContext : NotifyPropertyChangedObject
    {
        private string selectedItem;
        private bool isUsedBy;
        private bool isUsing;
        private bool includePackages;
        private string url;
        private bool showListOnly;
        private double windowWidth;
        private double windowHeight;

        public RelationsUserControlContext()
        {
            // Inicializar con el tamaño de la pantalla actual
            var screenWidth = Screen.PrimaryScreen.Bounds.Width;
            var screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Usar el tamaño de la pantalla menos un margen para dejar espacio a VS
            windowWidth = screenWidth - 50;
            windowHeight = screenHeight - 50;
        }

        [DataMember]
        public string Url { get => url; set => SetProperty(ref url, value); }

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
        public string SelectedItem { get => selectedItem; set => SetProperty(ref selectedItem, value); }


        [DataMember]
        public bool IncludePackages
        {
            get { return includePackages; }
            set { SetProperty(ref includePackages, value); }
        }

        [DataMember]
        public bool ShowListOnly
        {
            get => showListOnly;
            set => SetProperty(ref showListOnly, value);
        }

        [DataMember]
        public Visibility IncludePackagesVisibility => IsUsedBy ? Visibility.Visible : Visibility.Collapsed;


        [DataMember]
        public bool IsUsedBy
        {
            get { return isUsedBy; }
            set
            {
                if (SetProperty(ref isUsedBy, value))
                {
                    RaiseNotifyPropertyChangedEvent(nameof(IncludePackagesVisibility));
                    RaiseNotifyPropertyChangedEvent(nameof(RelationType));
                }
            }
        }

        [DataMember]
        public bool IsUsing
        {
            get { return isUsing; }
            set { SetProperty(ref isUsing, value); }
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
            set => SetProperty(ref windowWidth, value);
        }

        /// <summary>
        /// Alto de la ventana del diálogo.
        /// </summary>
        [DataMember]
        public double WindowHeight
        {
            get => windowHeight;
            set => SetProperty(ref windowHeight, value);
        }
    }
}
