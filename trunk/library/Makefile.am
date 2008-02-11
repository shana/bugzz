bugzzdir=$(prefix)/library
bin_SCRIPTS = 
bugzz_DATA = Bugzz.dll

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

EXTRA_DIST = $(sources) $(resources) 
# bugzz.in bugzz.mds bugzz.mdp

packages = 
assemblies = -r:System -r:System.Xml
ress =
# ress= $(foreach res,$(resources), $(addprefix -resource:,$(res)),$(notdir $(res)))

# Mono.Cecil.dll: 
# 	cp `pkg-config --variable=Libraries cecil` .

Bugzz.dll: $(resources)
	$(GMCS) -noconfig -codepage:utf8 -warn:4 -optimize+ -debug -define:DEBUG -target:library -out:Bugzz.dll $(sources) $(ress) $(packages) $(assemblies)