
EXTRA_DIST =  expansions.m4

#Warning: This is an automatically generated file, do not edit!
if ENABLE_DEBUG
 SUBDIRS =  . 
endif
if ENABLE_RELEASE
 SUBDIRS =  . 
endif

# Include project specific makefile
if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/mockup.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/mockup.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release


endif

AL=al
SATELLITE_ASSEMBLY_NAME=.resources.dll

BINARIES = \
	$(MOCKUP)  


	
all: $(ASSEMBLY) $(BINARIES) 

FILES =  \
	AssemblyInfo.cs \
	gtk-gui/generated.cs \
	gtk-gui/mockup.MainWindow.cs \
	gtk-gui/mockup.Widgets.BugList.cs \
	gtk-gui/mockup.Widgets.Detail.cs \
	gtk-gui/mockup.Widgets.Search.cs \
	gtk-gui/mockup.Widgets.Settings.cs \
	Main.cs \
	MainWindow.cs \
	Settings.cs \
	Utils/Loader.cs \
	Widgets/BugList.cs \
	Widgets/Detail.cs \
	Widgets/Search.cs \
	Widgets/Settings.cs 

DATA_FILES = 

RESOURCES =  \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	mockup.in 

REFERENCES =  \
	../../3rdparty/HtmlAgilityPack/HtmlAgilityPack.dll \
	../../library/Bugzz.dll \
	Mono.Posix \
	-pkg:glade-sharp-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0 \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(BINARIES) 

include $(top_srcdir)/Makefile.include

MOCKUP = $(BUILD_DIR)/mockup

$(eval $(call emit-deploy-wrapper,MOCKUP,mockup,x))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

run:
	mono $(ASSEMBLY)
