namespace ProjectRelations2022.Views
{
    using Microsoft.VisualStudio.Extensibility.UI;
    using ProjectRelations2022.DTO;
    using System.Runtime.Serialization;
    using System.Windows;

    [DataContract]
    public class RelationsUserControlContext : NotifyPropertyChangedObject
    {
        private string selectedItem;
        private bool isUsedBy;
        private bool isUsing;

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

        private bool includePackages;
        private string url;

        [DataMember]
        public bool IncludePackages
        {
            get { return includePackages; }
            set { SetProperty(ref includePackages, value); }
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
    }
}
