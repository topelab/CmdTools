using AvaloniaProjectRelations.Browser;
using CmdTools.Shared;
using ReactiveUI;
using RelationsShared.DTO;
using RelationsShared.Services;
using System.Collections.ObjectModel;
using Topelab.Core.Avalonia.Base;

namespace AvaloniaProjectRelations.MainControl
{
    internal class MainControlVM : BaseVM<MainControlVM, ProjectOptions>
    {
        private string url;
        private string selectedItem;
        private bool usedByMe;
        private bool usingMe;
        private bool includePackages;
        private bool showListOnly;
        private HtmlViewerVM htmlViewerVM;
        private string solutionPath;

        public MainControlVM()
        {
            IsMain = true;
        }

        public MainControlVM(ProjectOptions model) : this()
        {
            this.model = model;
        }

        public string Url
        {
            get => url;
            set => this.RaiseAndSetIfChanged(ref url, value);
        }

        public string MermaidFile { get; set; }

        public UserSettings UserSettings { get; set; }

        public ObservableCollection<string> Projects { get; set; } = [];

        public IProjectsService ProjectData { get; set; }

        public string SelectedItem
        {
            get => selectedItem;
            set => this.RaiseAndSetIfChanged(ref selectedItem, value);
        }

        public bool IncludePackages
        {
            get { return includePackages; }
            set { this.RaiseAndSetIfChanged(ref includePackages, value); }
        }

        public bool ShowListOnly
        {
            get => showListOnly;
            set => this.RaiseAndSetIfChanged(ref showListOnly, value);
        }

        public bool IncludePackagesVisibility => true;


        public bool UsedByMe
        {
            get { return usedByMe; }
            set
            {
                usingMe = !value;
                if (this.RaiseAndSetIfChanged(ref usedByMe, value))
                {
                    this.RaisePropertyChanged(nameof(IncludePackagesVisibility));
                    this.RaisePropertyChanged(nameof(RelationType));
                }
            }
        }

        public bool UsingMe
        {
            get { return usingMe; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref usingMe, value))
                {
                    this.RaisePropertyChanged(nameof(IncludePackagesVisibility));
                    this.RaisePropertyChanged(nameof(RelationType));
                }
            }
        }

        public HtmlViewerVM HtmlViewerVM
        {
            get { return htmlViewerVM; }
            set { this.RaiseAndSetIfChanged(ref htmlViewerVM, value); }
        }

        public string SolutionPath
        {
            get => solutionPath;
            set => this.RaiseAndSetIfChanged(ref solutionPath, value);
        }

        public RelationType RelationType => UsedByMe ? RelationType.UsedBy : RelationType.Using;
    }
}
