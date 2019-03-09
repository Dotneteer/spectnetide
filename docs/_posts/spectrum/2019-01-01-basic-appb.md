---
layout: documents
categories: 
  - "Spectrum48 BASIC"
title:  "Appendix B: Reports"
alias: basic-appb
seqno: 290
selector: spectrum
permalink: "spectrum/basic-appb"
---

These appear at the bottom of the screen whenever the computer stops executing some BASIC, and explain why it stopped, whether for a natural reason, or because
an error occurred.

The report has a code number or letter so that you can refer to the table here, a brief message explaining what happened and the line number and statement
number within that line where it stopped. (A command is shown as line 0. Within a line, statement 1 is at the beginning, statement 2 comes after the first colon
or **THEN**, and so on.)

The behaviour of **CONTINUE** depends very much on the reports. Normally, **CONTINUE** goes to the line and statement specified in the last report, but there are
exceptions with reports 0, 9 and D (also see Appendix C).

Here is a table showing all the reports. It also tells you in what circumstances the report can occur, and this refers you to Appendix C. For instance error A
Invalid argument can occur with **SQR**, **IN**, **ACS** and **ASN** and the entries for these in Appendix C tell you exactly what arguments are invalid.

|Code | Meaning | Situations|
|---|---|---|
|0 |0K <BR> Successful completion, or jump to a line number bigger than any existing. This report does not change the line and statement jumped to by **CONTINUE**.|Any |
|1| NEXT without FOR NEXT <BR>The control variable does not exist (it has not been set up by a **FOR** statement), but there is an ordinary variable with the same name.|**NEXT**|
|2| Variable not found <BR>  For a simple variable this will happen if the variable is used before it has been assigned to in a **LET**, **READ** or **INPUT** statement or loaded from tape or set up in a **FOR** statement. For a subscripted variable it will happen if the variable is used before it has been dimensioned in a **DIM** statement or loaded from tape.|Any|
|3|Subscript wrong<BR> A subscript is beyond the dimension of the array, or there are the wrong number of subscripts. If the subscript is negative or bigger than 65535, then error B will result. |Subscripted variables, Substrings|
|4|Out of memory <BR>There is not enough room in the computer for what you are trying to do. If the computer really seems to be stuck in this state, you may have to clear out the command line using **DELETE** and then delete a program line or two (with the intention of putting them back afterwards) to give yourself room to manoeuvre with - say - **CLEAR**.|**LET**, **INPUT**, **FOR**, **DIM**, **GO SUB**, **LOAD**, **MERGE**.Sometimes during expression evaluation. |
|5|lines in the lower half of the screen. Also occurs with **PRINT AT 22**, . . . |**INPUT**, **PRINT AT**
|6|Number too big <BR>Calculations have led to a number greater than about 10^38.|Any arithmetic|
|7|RETURN without GO SUB <BR>There has been one more **RETURN** than there were **GO SUB**s.|RETURN
|8|End of file |Microdrive, etc, operations|
|9|STOP statement <BR> After this, **CONTINUE** will not repeat the STOP, but carries on with the statement after.|**STOP**
|A|Invalid argument <BR> The argument for a function is no good for some reason.|** SQR, LN, ASN, ACS, USR (with string argument)|
|B|Integer out of range<BR>When an integer is required, the floating point argument is rounded to the nearest integer.* If this is outside a suitable range then error B results. For array access, see also Error 3.*|**RUN**, **RANDOMIZE**, **POKE**, **DIM**, **GO TO**, **GO SUB**, **LIST**, **LLIST**, **PAUSE**, **PLOT**, **CHR$**, **PEEK**, **USR** (with numeric argument), Array access|
|C|Nonsense in BASIC <BR> The text of the (string) argument does not form a valid expression.|**VAL**, **VAL$**|
|D|BREAK - CONT repeats  <BR> Also when the computer asks scroll? and you type N, SPACE or STOP. BREAK was pressed during some peripheral operation. The behaviour of **CONTINUE*** after this report is normal in that it repeats the statement. Compare with report L.|**LOAD**, **SAVE**, **VERIFY**, **MERGE**, **LPRINT**, LLIST, **COPY**.|
|E|Out of DATA <BR>You have tried to READ past the end of the DATA list.|**READ**|
|F|Invalid file name <BR> **SAVE** with name empty or longer than 10 characters.|**SAVE**|
|G| No room for line <BR> There is not enough room left in memory to accommodate the new program line.|Entering a line into the program|
|H|STOP in INPUT <BR> Some **INPUT** data started with STOP, or - for **INPUT** LINE - was pressed. Unlike the case with report 9, after report H **CONTINUE** will behave normally, by repeating the **INPUT** statement.|**INPUT**|
|I|FOR without NEXT <BR> There was a **FOR** loop to be executed no times (e.g. **FOR** n= 1 **TO** 0) and the corresponding **NEXT** statement could not be found.* |**FOR**|
|J|Invalid I/O device <BR>[undocumented] |Microdrive, etc, operations|
|K|Invalid colour <BR>The number specified is not an appropriate value.| **INK**, **PAPER**, **BORDER**, **FLASH**, **BRIGHT**, **INVERSE**, **OVER**; also after one of the corresponding control characters. |
|L|BREAK into program <BR> BREAK pressed, this is detected between two statements. The line and statement number in the report refer to the statement before BREAK was pressed, but **CONTINUE** goes to the* statement after (allowing for any jumps to be done), so it does not repeat any statements.|Any|
|M|RAMTOP no good <BR>The number specified for RAMTOP is either too big or too small.|**CLEAR**; possibly in **RUN**|
|N|Statement lost <BR> Jump to a statement that no longer exists.| **RETURN**, **NEXT**, **CONTINUE**|
|O|Invalid stream <BR>[undocumented] |Microdrive,etc, operations|
|P|FN without DEF <BR> *User-defined function* |FN|
|Q|Parameter error <BR>Wrong number of arguments, or one of them is the wrong type (string instead of number or vice versa).|FN|
|R|Tape loading error <BR> A file on tape was found but for some reason could not be read in, or would not verify.|**VERIFY** **LOAD** or **MERGE**|
