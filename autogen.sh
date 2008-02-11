#!/bin/sh
#glib-gettextize --force --copy ||
#  { echo "**Error**: glib-gettextize failed."; exit 1; }

#aclocal
#automake -a
#autoconf
autoreconf -f -i
./configure --enable-maintainer-mode $*
