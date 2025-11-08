namespace ProjectRelations2022.Views
{
    using ProjectRelations2022.DTO;
    using System.Runtime.Serialization;

    [DataContract]
    public class RelationsUserControlContext
    {
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
    }
}
