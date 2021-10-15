If you launch the program in debug mode, you should continue debugging whenever you encounter user-""""unhandled"""" exceptions.
— Because they are not: since VisualStudio incorrectly detects exceptions in invoked targets (which are used a lot) as unhandled.

Or better, launch it in release mode.