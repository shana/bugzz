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
    
    
    public partial class Settings {
        
        private Gtk.VBox vbox2;
        
        private Gtk.Alignment alignment1;
        
        private Gtk.Table table1;
        
        private Gtk.Label label1;
        
        private Gtk.Entry txtUrl;
        
        private Gtk.HBox hbox2;
        
        private Gtk.HButtonBox hbuttonbox1;
        
        private Gtk.Button button3;
        
        private Gtk.Button button4;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget mockup.Widgets.Settings
            Stetic.BinContainer.Attach(this);
            this.Name = "mockup.Widgets.Settings";
            // Container child mockup.Widgets.Settings.Gtk.Container+ContainerChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.alignment1 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment1.Name = "alignment1";
            // Container child alignment1.Gtk.Container+ContainerChild
            this.table1 = new Gtk.Table(((uint)(1)), ((uint)(2)), false);
            this.table1.Name = "table1";
            // Container child table1.Gtk.Table+TableChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Url");
            this.table1.Add(this.label1);
            Gtk.Table.TableChild w1 = ((Gtk.Table.TableChild)(this.table1[this.label1]));
            w1.XOptions = ((Gtk.AttachOptions)(4));
            w1.YOptions = ((Gtk.AttachOptions)(4));
            // Container child table1.Gtk.Table+TableChild
            this.txtUrl = new Gtk.Entry();
            this.txtUrl.CanFocus = true;
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.IsEditable = true;
            this.txtUrl.InvisibleChar = '●';
            this.table1.Add(this.txtUrl);
            Gtk.Table.TableChild w2 = ((Gtk.Table.TableChild)(this.table1[this.txtUrl]));
            w2.LeftAttach = ((uint)(1));
            w2.RightAttach = ((uint)(2));
            w2.YOptions = ((Gtk.AttachOptions)(4));
            this.alignment1.Add(this.table1);
            this.vbox2.Add(this.alignment1);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox2[this.alignment1]));
            w4.Position = 0;
            w4.Expand = false;
            w4.Fill = false;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox2 = new Gtk.HBox();
            this.hbox2.Name = "hbox2";
            this.hbox2.Homogeneous = true;
            this.hbox2.Spacing = 6;
            // Container child hbox2.Gtk.Box+BoxChild
            this.hbuttonbox1 = new Gtk.HButtonBox();
            this.hbuttonbox1.Name = "hbuttonbox1";
            this.hbuttonbox1.Spacing = 6;
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.button3 = new Gtk.Button();
            this.button3.CanFocus = true;
            this.button3.Name = "button3";
            this.button3.UseUnderline = true;
            this.button3.Label = Mono.Unix.Catalog.GetString("Cancel");
            this.hbuttonbox1.Add(this.button3);
            Gtk.ButtonBox.ButtonBoxChild w5 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.button3]));
            w5.Expand = false;
            w5.Fill = false;
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.button4 = new Gtk.Button();
            this.button4.CanFocus = true;
            this.button4.Name = "button4";
            this.button4.UseUnderline = true;
            this.button4.Label = Mono.Unix.Catalog.GetString("Save");
            this.hbuttonbox1.Add(this.button4);
            Gtk.ButtonBox.ButtonBoxChild w6 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.button4]));
            w6.Position = 1;
            w6.Expand = false;
            w6.Fill = false;
            this.hbox2.Add(this.hbuttonbox1);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.hbox2[this.hbuttonbox1]));
            w7.Position = 1;
            this.vbox2.Add(this.hbox2);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox2]));
            w8.Position = 1;
            w8.Expand = false;
            w8.Fill = false;
            this.Add(this.vbox2);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.button3.Activated += new System.EventHandler(this.OnCancel);
            this.button4.Activated += new System.EventHandler(this.OnSave);
        }
    }
}
