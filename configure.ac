AC_INIT([Bugzz], [0.0.1])
AC_PREREQ([0.61])
AC_CANONICAL_SYSTEM
AC_CONFIG_SRCDIR(README)
AM_INIT_AUTOMAKE([foreign tar-ustar])

AM_MAINTAINER_MODE

AC_PROG_INSTALL

dnl C# compiler
AC_PATH_PROG(MCS, mcs, no)
AC_PATH_PROG(GMCS, gmcs, no)
AC_PATH_PROG(RUNTIME, mono, no)

CS="C#"
if test "x$MCS" = "xno"  ; then
	dnl AC_MSG_ERROR([You need to install a C# compiler])
	AC_MSG_ERROR([No $CS compiler found])
fi

if test "x$RUNTIME" = "xno" ; then
	AC_MSG_ERROR([No runtime found])
fi

# dnl resgen
# AC_PATH_PROG(RESGEN, resgen, no)
# if test "x$RESGEN" = "xno"  ; then
# 	AC_MSG_ERROR([resgen not found in $PATH])
# fi

AC_SUBST(MCS)
AC_SUBST(RUNTIME)
# AC_SUBST(RESGEN)

# PKG_CHECK_MODULES(NUNIT, mono-nunit)
# AC_SUBST(NUNIT_LIBS)

# PKG_CHECK_MODULES(MONODOC, monodoc)
# AC_SUBST(MONODOC_LIBS)

# PKG_CHECK_MODULES(GTK_SHARP, gtk-sharp-2.0 glade-sharp-2.0 gnome-sharp-2.0 gconf-sharp-2.0)

# AC_SUBST(GTK_SHARP_LIBS)

# PKG_CHECK_MODULES(GTKHTML_SHARP, gtkhtml-sharp-3.14, enable_gtkhtml=yes, enable_gtkhtml=no)
# if test "x$enable_gtkhtml" = "xyes"; then
# CSHARP_FLAGS="-d:GTKHTML_SHARP_3_14"
# else
# PKG_CHECK_MODULES(GTKHTML_SHARP, gtkhtml-sharp-2.0, enable_gtkhtml=yes, enable_gtkhtml=no)
# CSHARP_FLAGS=
# fi

# AC_SUBST(GTKHTML_SHARP_LIBS)
AC_SUBST(CSHARP_FLAGS)
# AM_CONDITIONAL(ENABLE_GTKHTML, test "x$enable_gtkhtml" = "xyes")

# PKG_CHECK_MODULES(GECKO_SHARP, gecko-sharp-2.0, enable_gecko=yes, enable_gecko=no)
# AC_SUBST(GECKO_SHARP_LIBS)
# AM_CONDITIONAL(ENABLE_GECKO, test "x$enable_gecko" = "xyes")

# if test "x$enable_gecko" = "xyes"; then
#    # get the mozilla home directory
#    MOZILLA_HOME="`$PKG_CONFIG --variable=libdir mozilla-gtkmozembed`"
#    AC_SUBST(MOZILLA_HOME)
# fi


# if test "x$enable_gtkhtml$enable_gecko" = "xnono" ; then
#    AC_MSG_ERROR([no HTML display found. You need either gtkhtml or gecko])
# fi

# dnl
# dnl I hate PKG_CONFIG_MODULES, that is for the weak
# dnl
# if pkg-config --modversion cecil>/dev/null; then
# AC_MSG_RESULT("Found cecil.pc")
# else
# AC_MSG_ERROR([You are missing Cecil (cecil.pc for packageconfig) required for gui-compare])
# fi

dnl Intl
GETTEXT_PACKAGE=Bugzz
AC_SUBST(PKGPREFIX)
AC_SUBST(GETTEXT_PACKAGE)
AC_DEFINE_UNQUOTED(GETTEXT_PACKAGE,"$GETTEXT_PACKAGE", [Gettext package])
# ALL_LINGUAS="da es fr ca pt_BR"
AM_GLIB_GNU_GETTEXT

# GNUNIT_VERSION=0.5
# AC_SUBST(GNUNIT_VERSION)

AC_PROG_AWK
export VERSION
# [TOOLS_VERSION=$($AWK 'BEGIN {
#   split (ENVIRON["VERSION"] ".0.0.0", vsplit, ".")
#   if(length(vsplit [1]) > 4) {
#     split (substr(ENVIRON["VERSION"], 0, 4) "." substr(ENVIRON["VERSION"], 5) ".0.0", vsplit, ".")
#   }
#   print vsplit [1] "." vsplit [2] "." vsplit [3] "." vsplit [4]
# }')]

# AC_SUBST(TOOLS_VERSION)

AC_OUTPUT([
Makefile
po/Makefile.in
library/Makefile
3rdparty/Makefile
3rdparty/HtmlAgilityPack/Makefile
tools/html2xml/Makefile
common/Constants.cs
])

echo "---"
echo "Configuration summary"
echo ""
echo "   * Installation prefix = $prefix"
# echo "   * gecko-sharp.dll = $enable_gecko"
# echo ""
# echo "      NOTE: if any of the above say 'no' you may install the"
# echo "            corresponding development packages for them, rerun"
# echo "            autogen.sh to include them in the build."
echo " Please type make to, erm, make. "
echo ""
