bugzzdir=$(prefix)/lib
bin_SCRIPTS = 
bugzz_DATA = Bugzz.dll ../3rdparty/HtmlAgilityPack/HtmlAgilityPack.dll
PROGRAM = Bugzz.dll
sharedir=$(prefix)/share/bugzz
share_DATA = data/bugzilla.xml

CLEANFILES = Bugzz.dll Bugzz.dll.mdb

sources = @Bugzz.dll.sources

resources = 

# REWRITE = sed \
# 	-e 's,@''prefix@,$(prefix),g'	\
# 	-e 's,@''PACKAGE@,Bugzz,g'	\
# 	-e 's,@''dll_file@,$@.dll,g'				\
# 	-e 's,@''RUNTIME@,@RUNTIME@ --debug,g'

# bugzz: bugzz.in
# 	$(REWRITE) < $(srcdir)/bugzz.in > bugzz

EXTRA_DIST = $(sources) $(resources) data/bugzilla.xml ../3rdparty/HtmlAgilityPack/HtmlAgilityPack.dll
# bugzz.in bugzz.mds bugzz.mdp

packages = 
assemblies =  \
	-r:../3rdparty/HtmlAgilityPack/HtmlAgilityPack.dll \
	-r:Mono.C5 \
	-r:System \
	-r:System.Xml
ress = $(foreach res,$(resources), $(addprefix -resource:,$(res)),$(notdir $(res)))

# Mono.Cecil.dll: 
# 	cp `pkg-config --variable=Libraries cecil` .

Bugzz.dll: $(resources) $(shell cat Bugzz.dll.sources)
	$(GMCS) -noconfig -codepage:utf8 -warn:4 -optimize+ -debug -define:DEBUG -target:library -out:$(PROGRAM) $(sources) $(ress) $(packages) $(assemblies)


# build/rules.make

