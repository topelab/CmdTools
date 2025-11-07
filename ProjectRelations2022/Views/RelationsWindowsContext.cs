namespace ProjectRelations2022.Views
{
    using ProjectRelations2022.DTO;
    using System.Runtime.Serialization;

    [DataContract]
    public class RelationsWindowsContext
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string UserFolder { get; set; }
        [DataMember]
        public string MermaidFile { get; set; }
        [DataMember]
        public UserSettings UserSettings { get; set; }
    }
}
