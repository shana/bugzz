html2xmldir=$(prefix)/tools/html2xml
bin_SCRIPTS = 
html2xml_DATA = html2xml.exe
PROGRAM = html2xml.exe
CLEANFILES = html2xml.exe html2xml.exe.mdb

sources = @html2xml.exe.sources

REWRITE = sed \
	-e 's,@''prefix@,$(prefix),g'	\
	-e 's,@''PACKAGE@,Bugzz,g'	\
	-e 's,@''exe_file@,$@.exe,g'				\
	-e 's,@''RUNTIME@,@RUNTIME@ --debug,g'

#html2xml: html2xml.in
# 	$(REWRITE) < $(srcdir)/html2xml.in > html2xml

EXTRA_DIST = $(sources) $(resources) html2xml.in html2xml.mdp

assemblies =  \
	-r:../../3rdparty/HtmlAgilityPack/HtmlAgilityPack.dll \
	-r:../../library/Bugzz.dll \
	-r:System


html2xml.exe: $(resources) $(shell cat html2xml.exe.sources)
	$(GMCS) -noconfig -codepage:utf8 -warn:4 -optimize+ -debug -define:DEBUG -out:$(PROGRAM) $(sources) $(ress) $(packages) $(assemblies)


# build/rules.make


