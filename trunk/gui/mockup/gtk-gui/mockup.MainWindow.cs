// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace mockup {
    
    
    public partial class MainWindow {
        
        private Gtk.Action File;
        
        private Gtk.Action yes;
        
        private Gtk.ToggleAction actSettings;
        
        private Gtk.ToggleAction actSearch;
        
        private Gtk.ToggleAction actBugList;
        
        private Gtk.ToggleAction actBugDetail;
        
        private Gtk.Action actOnline;
        
        private Gtk.VBox vbox1;
        
        private Gtk.Toolbar toolbar1;
        
        private Gtk.VPaned vpane;
        
        private Gtk.ProgressBar progressbar1;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget mockup.MainWindow
            Gtk.UIManager w1 = new Gtk.UIManager();
            Gtk.ActionGroup w2 = new Gtk.ActionGroup("Default");
            this.File = new Gtk.Action("File", Mono.Unix.Catalog.GetString("File"), null, null);
            this.File.ShortLabel = Mono.Unix.Catalog.GetString("File");
            w2.Add(this.File, null);
            this.yes = new Gtk.Action("yes", null, null, "gtk-yes");
            w2.Add(this.yes, null);
            this.actSettings = new Gtk.ToggleAction("actSettings", Mono.Unix.Catalog.GetString("Settings"), null, null);
            this.actSettings.ShortLabel = Mono.Unix.Catalog.GetString("Settings");
            w2.Add(this.actSettings, null);
            this.actSearch = new Gtk.ToggleAction("actSearch", Mono.Unix.Catalog.GetString("Search Bugs"), null, null);
            this.actSearch.ShortLabel = Mono.Unix.Catalog.GetString("Search Bugs");
            w2.Add(this.actSearch, null);
            this.actBugList = new Gtk.ToggleAction("actBugList", Mono.Unix.Catalog.GetString("Bug List"), null, null);
            this.actBugList.ShortLabel = Mono.Unix.Catalog.GetString("Bug List");
            w2.Add(this.actBugList, null);
            this.actBugDetail = new Gtk.ToggleAction("actBugDetail", Mono.Unix.Catalog.GetString("Bug Detail"), null, null);
            this.actBugDetail.ShortLabel = Mono.Unix.Catalog.GetString("Bug Detail");
            w2.Add(this.actBugDetail, null);
            this.actOnline = new Gtk.Action("actOnline", null, null, null);
            w2.Add(this.actOnline, null);
            w1.InsertActionGroup(w2, 0);
            this.AddAccelGroup(w1.AccelGroup);
            this.Name = "mockup.MainWindow";
            this.Title = Mono.Unix.Catalog.GetString("MainWindow");
            this.WindowPosition = ((Gtk.WindowPosition)(4));
            // Container child mockup.MainWindow.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            w1.AddUiFromString("<ui><toolbar name='toolbar1'><toolitem action='actSettings'/><toolitem action='actSearch'/><toolitem action='actBugList'/><toolitem action='actBugDetail'/><toolitem action='actOnline'/></toolbar></ui>");
            this.toolbar1 = ((Gtk.Toolbar)(w1.GetWidget("/toolbar1")));
            this.toolbar1.Name = "toolbar1";
            this.toolbar1.ShowArrow = false;
            this.toolbar1.ToolbarStyle = ((Gtk.ToolbarStyle)(0));
            this.vbox1.Add(this.toolbar1);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox1[this.toolbar1]));
            w3.Position = 0;
            w3.Expand = false;
            w3.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.vpane = new Gtk.VPaned();
            this.vpane.CanFocus = true;
            this.vpane.Name = "vpane";
            this.vpane.Position = 20;
            this.vbox1.Add(this.vpane);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox1[this.vpane]));
            w4.Position = 1;
            // Container child vbox1.Gtk.Box+BoxChild
            this.progressbar1 = new Gtk.ProgressBar();
            this.progressbar1.Name = "progressbar1";
            this.vbox1.Add(this.progressbar1);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox1[this.progressbar1]));
            w5.Position = 2;
            w5.Expand = false;
            w5.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 763;
            this.DefaultHeight = 483;
            this.Show();
            this.DeleteEvent += new Gtk.DeleteEventHandler(this.OnDeleteEvent);
            this.actSettings.Toggled += new System.EventHandler(this.OnToggleSettings);
            this.actSearch.Toggled += new System.EventHandler(this.OnToggleSearch);
            this.actBugList.Toggled += new System.EventHandler(this.OnToggleList);
            this.actBugDetail.Toggled += new System.EventHandler(this.OnToggleDetail);
            this.actOnline.Activated += new System.EventHandler(this.OnToggleOnline);
        }
    }
}
