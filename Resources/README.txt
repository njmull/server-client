TITLE: PS6 - Spreadsheet GUI
AUTHOR: Emerson Ford (u0407846) and Gabe Brodbeck (u0847035)
DATE: October 27th, 2017
CLASS: CS 3500

This projects connects together all of the previous projects we have been working on, and wraps it together
in a GUI.

This project relies on the following DLLs:
- Formula.dll
- Spreadsheet.dll
- SpreadsheetPanel.dll
- SpreadsheetUtilities.dll


I ran into a weird bug with PS4. For some reason, whenever a FormulaError was thrown, it would not pass up through the Formula
dependencies. I'm not sure what fixed it but when I ran it a couple of times it fixed itself...?

The extra features we implemented:
- Keyboard controls (arrow keys, enter)
- Save function
- Open program through clicking associated file
