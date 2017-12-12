
|Statement|Explanation|
|---|---|
|**BEEP** |x, y Sounds a note through the loudspeaker for x seconds ata pitch y semitones above middle C (or below if y is negative).
|**BORDER** m |Sets the colour of the border of the screen and also the paper colour for the lower part of the screen. Error K is m not in the range 0 to 7.
|**BRIGHT** |Sets brightness of characters subsequently printed. n=0 for normal, 1 for bright. 8 for transparent. Error K if n not 0, 1 or 8
|**CAT** |Does not work without Microdrive, etc
|**CIRCLE** x, y, z |Draws an arc of a circle, centre (x,y), radius z
|**CLEAR** |Deletes all variables, freeing the space they occupied. Does **RESTORE** and **CLS**, resets the **PLOT** position to the bottom left-hand corner and clears the **GO SUB** stack
|**CLEAR n** |Like **CLEAR**. but if possible changes the system variable RAMTOP to n and puts the new **GO SUB** stack there
|**CLOSE \#** |Does not work without Microdrive, etc
|**CLS** |(Clear Screen). Clears the display file
|**CONTINUE** |Continues the program, starting where it left off last time it stopped with report other than 0. If the report was 9 or L, then continues with the following statement (taking jumps into account); otherwise repeats the one where the error occurred. If the last report was in a command line then **CONTINUE** will attempt to continue the command line and will either go into a loop if the error was in 0:1, give report 0 if it was in 0: 2, or give error N if it was 0: 3 or greater. **CONTINUE** appears as CONT on the keyboard
|**COPY** |Sends a copy of the top 22 lines of display to the printer, if attached; otherwise does nothing. Note that **COPY** can not be used to print the automatic listings that appear on the screen. Report D if BREAK pressed
|**DATA** e1 , e2 , e3 , ... |Part of the **DATA** list. Must be in a program
|**DEF FN** &alpha;(&alpha;<sub>1</sub> . . . &alpha;<sub>k</sub> )=e |User-defined function definition; must be in a program. Each of &alpha; and &alpha;<sub>1</sub> to &alpha;<sub>k</sub> is either a single letter or a single letter followed by '$' for string argument or result. Takes the form **DEF FN** ()=e if no arguments
|**DELETE** |f Does not work without Microdrive, etc
|**DIM** &alpha;( n1 , . . . ,nk ) |Deletes any array with the name &alpha;, and sets up an array &alpha;of numbers with k dimensions n1 , ... ,nk. Initialises all the values to 0
|**DIM** &alpha;$( n1 , . . . ,nk ) |Deletes any array or string with the name &alpha;$, and sets up an array of characters with k dimensions nl ,...,nk. Initialises all the values to " ". This can be considered as an array of strings of fixed length nk , with k-l dimensions nl,...,nk-l . Error 4 occurs if there is no room to fit the array in. An array is undefined until it is dimensioned in a **DIM** statement
|**DRAW** x,y |**DRAW** x,y,0|
|**DRAW** x,y,z |Draws a line from the current plot position moving x horizontally and y vertically relative to it while turning through an angle z. Error B if it runs off the screen|
|**ERASE** |Does not work without Microdrive, etc.|
|**FLASH** |Defines whether characters will be flashing or steady. n=0 for steady, n=l for flash, n=8 for no change. |
|**FOR** &alpha;=x **TO** y |**FOR** &alpha;=x TO y **STEP** 1 |
|**FOR** &alpha;=x **TO** y **STEP** z |Deletes any simple variable &alpha; and sets up a control variable with value x, limit y, step z, and looping address referring to the statement after the **FOR** statement. Checks if the initial value is greater (if step\>=0) or less (if step<0) than the limit, and if so then skips to statement **NEXT** &alpha;, giving error 1 if there is none. See **NEXT**. Error 4 occurs if there is no room for the control variable.
|**FORMAT** |f Does not work without the Microdrive, etc|
|**GOSUB** |n Pushes the line number of the GOSUB statement onto a stack; then as **GO TO** n. Error 4 can occur if there are not enough RETURNs|
|**GO TO** n |Jumps to line n (or, if there is none, the first line after that)|
|**IF** x **THEN** s |If x true (non-zero) then s is executed. Note that s comprises all the statements to the end of the line. The form '**IF** x **THEN** line number' is not allowed.|
|**INK** n |Sets the ink (foreground) colour of characters subsequently printed. n is in the range 0 to 7 for a colour, n=8 for transparent or 9 for contrast. See *The television screen* - Appendix B. Error K if n not in the range 0 to 9.|
|**INPUT** |The ' . . . ' is a sequence of INPUT items, separated as in a **PRINT** statement by commas, semicolons or apostrophes. An **INPUT** item can be <BR>(i) Any **PRINT** item not beginning with a letter <BR>(ii) A variable name, or <BR>(iii) LINE, then a string type variable name. The **PRINT** items and separators in (i) are treated exactly as in **PRINT**, except that everything is printed in the lower part of the screen. For (ii) the computer stops and waits for input of an expression from the keyboard; the value of this is assigned to the variable. The input is echoed in the usual way and syntax errors give the flashing **?**. For string type expressions, the input buffer is initialised to contain two string quotes (which can be erased if necessary). If the first character in the input is **STOP**, the program stops with error H. (iii) is like (ii) except that the input is treated as a string literal without quotes, and the STOP mechanism doesn't work; to stop it you must type ![Crsrdown](./Figures/crsrdown.gif) instead.|
|**INVERSE** n |Controls inversion of characters subsequently printed. If n=0, characters are printed in normal video, as ink colour on paper colour. If n=1, characters are printed in inverse video, i.e. paper colour on ink colour. See *The television screen* - Appendix B. Error K if n is not 0 or 1|
|**LET** v=e |Assigns the value of e to the variable v. **LET** cannot be omitted. A simple variable is undefined until it is assigned to in a **LET**, **READ** or **INPUT** statement. If v is a subscripted string variable, or a sliced string variable (substring), then the assignment is Procrustean (fixed length): the string value of e is either truncated or filled out with spaces on the right, to make it the same length as the variable v|
|**LIST** |**LIST 0**|
|**LIST** n |Lists the program to the upper part of the screen, starting at the first line whose number is at least n, and makes n the current line|
|**LLIST** |**LLIST 0**|
|**LLIST** n |Like **LIST**, but using the printer|
|**LOAD** f |Loads program and variables|
|**LOAD** f **DATA** () |Loads a numeric array|
|**LOAD** f **DATA** $() |Loads character array $|
|**LOAD** f **CODE** m,n |Loads at most n bytes, starting at address m|
|**LOAD** f **CODE** m |Loads bytes starting at address m|
|**LOAD** f **CODE** |Loads bytes back to the address they were saved from.|
|**LOAD** f **SCREEN$** |**LOAD** f **CODE** 16384,6912. Searches for file of the right sort on cassette tape and loads it, deleting previous versions in memory. See Chapter 20|
|**LPRINT** |Like **PRINT** but using the printer|
|**MERGE** f |Like LOAD f. but does not delete old program lines and variables except to make way for new ones with the same line number or name.|
|**MOVE** |f1,f2 |Does not work without the Microdrive, etc||
|**NEW** |Starts the BASIC system off anew, deleting program and variables, and using the memory up to and including the byte whose address is in the system variable RAMBOT and preserves the system variables UDG, P RAMT, RASP and PIP|
|**NEXT**  |(i) Finds the control variable <BR> (ii) Adds its step to its value <BR> (iii) If the step\>=0 and the value\>the limit; or if the step\<0 and the value\<the limit, then jumps to the looping statement.| <BR> Error 2 if there is no variable  |<BR> Error 1 if there is one, but it's not  control variable|
|**OPEN \#** |Does not work without the Microdrive, etc|
|**OUT** m,n |Outputs byte n at port m at the processor level. (Loads the bc register pair with m, the a register with n, and does the assembly language instruction: out (c),a.) 0\<=m\<=65535, -255\<=n\<=255, else error B|
|**OVER** n |Controls overprinting for characters subsequently printed . If n=0, characters obliterate previous characters at that position. If n=l, then new characters are mixed in with old characters to give ink colour wherever either (but not both) had ink colour, and paper colour if they were both|paper or both ink colour. See *The television screen* - Appendix B. Error K if n not 0 or 1|
|**PAPER** n |Like INK, but controlling the paper (background) colour| 
|**PAUSE** n |Stops computing and displays the display file for n frames (at 50 frames per second or 60 frames per second in North America) or until a key is pressed. 0\<=nc=65535, else error B. If n=0 then the pause is not timed, but lasts until a key is pressed. |
|**PLOT** c;m,n |Prints an ink spot (subject to **OVER** and **INVERSE**) at the pixel (\|m\|, \|n\|); moves the **PLOT** position. Unless the colour items c specify otherwise, the ink colour at the character position containing the pixel is changed to the current permanent ink colour, and the other (paper colour, flashing and brightness) are left unchanged. 0\<=\|m\|\<=255, 0\<=\|n\|\<=175, else error B |
|**POKE** m,n |Writes the value n to the byte in store with address m. 0\<=m\<=65535, -255\<=n\<=255, else error B|
|**PRINT** |The ' . . . ' is a sequence of **PRINT** items, separated by commas **,** , semicolons **;** or apostrophes **'** and they are written to the display file for output to the television A semicolon **;** between two items has no effect: it is used purely to separate the items. A comma **,** outputs the comma control character, and an apostrophe **'** outputs the ENTER character. At the end of the **PRINT** statement, if it does not end in a semicolon, or comma, or apostrophe, an ENTER character is output. A **PRINT** item can be <BR> (i) empty, i.e. nothing. <br. (ii) a numerical expression First a minus sign is printed if the value is negative. Now let x be the modulus of value. If x\<=10-5 or x\>=1013, then it is printed using scientific notation. The mantissa part has up to eight digits (with no trailing zeros), and the decimal point (absent if only one digit) is after the first. The exponent part is E, followed by + or -, followed by one or two digits. Otherwise x is printed in ordinary decimal notation with up to eight significant digits, and no trailing zeros after the decimal point. A decimal point right at the beginning is always followed by a zero, so for instance .03 and 0.3 are printed as such. 0 is printed as a single digit 0. <BR>(iii) a string expression The tokens in the string are expanded, possibly with a space before or after. Control characters have their control effect. Unrecognized characters print as ?. <BR>(iv) **AT** m,n Outputs an **AT** control character followed by a byte for m (the line number) and a byte for n (the column number). <BR>(v) **TAB** n  Outputs a **TAB** control character followed by two bytes for n (less significant byte first), the **TAB** stop. (vi) A colour item, which takes the form of a **PAPER**, **INK**, **FLASH**, **BRIGHT**, **INVERSE** or **OVER** statement **RANDOMIZE** **RANDOMIZE** 0|
|**RANDOMIZE** n |Sets the system variable (called SEED) used to generate the next value of **RND**. If n\<\>0, SEED is given the value n; if n=0 then it is given the value of another system variable (called FRAMES) that counts the frames so far displayed on the television, and so should be fairly random.
|**RANDOMIZE** |appears as RAND on the keyboard. Error B occurs if n is not in the range 0 to 65535|
|**READ** vl , v2 , . . . vk |Assigns to the variables using successive expressions in the **DATA** list. Error C if an expression is the wrong type. Error E if there are variables left to be read when the **DATA** list is exhausted| 
|**REM** . . . |No effect. ' . . . ' can be any sequence of characters except ENTER. This can include : , so no statements are possible after the **REM** statement on the same line |
|**RESTORE** |**RESTORE** 0 **RESTORE** n Restores the **DATA** pointer to the first **DATA**  statement in a line with number at least n: the next|
|**READ** |statement will start reading there| 
|**RETURN** |Takes a reference to a statement off the **GO SUB** stack, and jumps to the line after it. Error 7 occurs when there is no statement reference on the stack. There is some mistake in your program **GO SUB**s are not properly balanced by RETURNs
|**RUN** |**RUN 0**|
|**RUN** n |**CLEAR**, and then **GO TO** n|
|**SAVE** f |Saves the program and variables|
|**SAVE** f **LINE** m |Saves the program and variables so that if they are loaded there is an automatic jump to line m|
|**SAVE** f **DATA** () |Saves the numeric array|
|**SAVE** f **DATA** $() |Saves the character array $|
|**SAVE** f **CODE** m,n |Saves n bytes starting at address m|
|**SAVE** f **SCREEN$**<BR>**SAVE** f **CODE** 16384,6912. |Saves information on cassette, giving it the name f Error F if f is empty or has length eleven or more. See Chapter 20|
|**STOP** |Stops the program with report 9. **CONTINUE** will resume with the following statement|
|**VERIFY** |The same as **LOAD** except that the data is not loaded into RAM, but compared against what is already there. Error R if one of the comparisons shows different bytes.

 
