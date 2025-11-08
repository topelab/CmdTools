namespace ProjectRelations2022.Views
{
    using Microsoft.VisualStudio.Extensibility.UI;
    using ProjectRelations2022.DTO;
    using System.Runtime.Serialization;

    [DataContract]
    public class RelationsUserControlContext : NotifyPropertyChangedObject
    {
        private string selectedItem;

        [DataMember]
        public string Url { get; set; }
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
    }
}
