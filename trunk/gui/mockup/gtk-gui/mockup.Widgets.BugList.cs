// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace mockup.Widgets {
    
    
    public partial class BugList {
        
        private Gtk.VBox vbox1;
        
        private Gtk.ScrolledWindow scrolledwindow3;
        
        private Gtk.HButtonBox hbuttonbox3;
        
        private Gtk.Button cmdRefresh;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget mockup.Widgets.BugList
            Stetic.BinContainer.Attach(this);
            this.Name = "mockup.Widgets.BugList";
            // Container child mockup.Widgets.BugList.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.scrolledwindow3 = new Gtk.ScrolledWindow();
            this.scrolledwindow3.CanFocus = true;
            this.scrolledwindow3.Name = "scrolledwindow3";
            this.scrolledwindow3.ShadowType = ((Gtk.ShadowType)(1));
            this.vbox1.Add(this.scrolledwindow3);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.vbox1[this.scrolledwindow3]));
            w1.Position = 0;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbuttonbox3 = new Gtk.HButtonBox();
            this.hbuttonbox3.Name = "hbuttonbox3";
            this.hbuttonbox3.Spacing = 6;
            this.hbuttonbox3.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child hbuttonbox3.Gtk.ButtonBox+ButtonBoxChild
            this.cmdRefresh = new Gtk.Button();
            this.cmdRefresh.CanFocus = true;
            this.cmdRefresh.Name = "cmdRefresh";
            this.cmdRefresh.UseUnderline = true;
            this.cmdRefresh.Label = Mono.Unix.Catalog.GetString("Refresh");
            this.hbuttonbox3.Add(this.cmdRefresh);
            Gtk.ButtonBox.ButtonBoxChild w2 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox3[this.cmdRefresh]));
            w2.Expand = false;
            w2.Fill = false;
            this.vbox1.Add(this.hbuttonbox3);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbuttonbox3]));
            w3.Position = 1;
            w3.Expand = false;
            w3.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.cmdRefresh.Clicked += new System.EventHandler(this.OnClicked);
        }
    }
}