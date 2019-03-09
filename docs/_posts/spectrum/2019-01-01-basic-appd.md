---
layout: documents
categories: 
  - "Spectrum48 BASIC"
title:  "Appendix D: Example programs"
alias: basic-appd
seqno: 300
selector: spectrum
permalink: "spectrum/basic-appd"
---

This appendix contains some example programs to demonstrate the abilities of the ZX Spectrum.
 
The first of these programs requires a date to be input and gives the day of the week which corresponds to this date.

```
10 REM convert date to day
20 DIM d$(7,6): REM days of week
30 FOR n=1 TO 7: READ d$(n): NEXT n
40 DIM m(12): REM lengths of months
50 FOR n=1 TO 12: READ m(n): NEXT n
100 REM input date
110 INPUT "day?";day
120 INPUT "month?";month
130 INPUT "year (20th century only)?";year
140 IF year\<1901 THEN PRINT "20th century starts at 1901": GO TO 100
150 IF year\>2000 THEN PRINT "20th century ends at 2000": GO TO 100
160 IF month\<1 THEN GO TO 210
170 IF month\>12 THEN GO TO 210
180 IF year/4-INT(year/4)=0 THEN LET m(2)=29: REM leap year
190 IF day\>m(month) THEN PRINT "This month has only "; m(month);" days.": GO TO 500
200 IF day\>0 THEN GO TO 300
210 PRINT "Stuff and nonsense. Give me a real date."
220 GO TO 500
300 REM convert date to number of days since start of century
310 LET y=year-1901
320 LET b=365\*y+INT (y/4): REM number of days to start of year
330 FOR n=1 TO month 1: REM add on previous months
340 LET b=b+m(n): NEXT n
350 LET b=b+day
400 REM convert to day of week
410 LET b=b-7\*INT (b/7)+1
420 PRINT day;"/";month;"/";year
430 FOR n=6 TO 3 STEP -1: REM remove trailing spaces
440 IF d$(b,n) \<\> " " THEN GO TO 460
450 NEXT n
460 LET e$=d$(b, TO n)
470 PRINT" is a "; e$; "day"
500 LET m(2)=28: REM restore February
510 INPUT "again?", a$
520 IF a$="n" THEN GO TO 540
530 IF a$ \<\> "N" THEN GO TO 100
1000 REM days of week
1010 DATA "Mon", "Tues", "Wednes"
1020 DATA "Thurs", "Fri", "Satur", "Sun"
1100 REM lengths of months
1110 DATA 31, 28, 31, 30, 31, 30
1120 DATA 31, 31, 30, 31, 30, 31
```
 

This program handles yards, feet and inches.
```
10 INPUT "yards?",yd,"feet?",ft, "inches?",in
40 GO SUB 2000: REM print the values
50 PRINT '" = ";
70 GO SUB 1000: REM the adjustment
80 GO SUB 2000: REM print the adjusted values
90 PRINT
100 GOTO 10
1000 REM subroutine to adjust yd, ft, in to the normal form for yards,feet and inches
1010 LET in=36\*yd+12\*ft+in: REM now everything is in inches
1030 LET s=SGN in: LET in=ABS in: REM we work with in positive, holding its sign in s
1060 LET ft=INT (in/12): LET in=(in-12\*ft)\*s: REM now in is ok
1080 LET yd=INT (ft/3)\*s: LET ft=ft\*s-3\*yd: RETURN
2000 REM subroutine to print yd, ft and in
2010 PRINT yd;"yd";ft;"ft";in;"in";: RETURN
```

 

Here is a program to throw coins for the I Ching. (Unfortunately it produces the
patterns upside down. but you might not worry about this.)

```
5 RANDOMIZE
10 FOR m=1 TO 6: REM for 6 throws
20 LET c=0: REM initialize coin total to 0
30 FOR n=1 TO 3: REM for 3 coins
40 LET c=c+2+INT (2\*RND)
50 NEXT n 60: PRINT " ";
70 FOR n=1 TO 2: REM 1st for the thrown hexagram, 2nd for the changes
80 PRINT "___";
90 IF c=7 THEN PRINT " ";
100 IF c=8 THEN PRINT " ";
110 IF c=6 THEN PRINT "X";: LET c=7
120 IF c=9 THEN PRINT "0";: LET c=8
130 PRINT "__\_ ";
140 NEXT n
150 PRINT
160 INPUT a$
170 NEXT m: NEW
```

To use this, type it in and run it, and then press ENTER five times to get the
two hexagrams. Look these up in a copy of the Chinese Book of Changes. The text
will describe a situation and the courses of action appropriate to it, and you
must ponder deeply to discover the parallels between that and your own life.
Press ENTER a sixth time, and the program will erase itself this is to
discourage you from using it frivolously.

Many people find the texts are always more apt than they would expect on grounds
of chance; this may or may not be the case with your ZX Spectrum In general,
computers are pretty godless creatures.

Here is a program to play 'Pangolins'. You think up an animal, and the computer
tries to guess what it is, by asking you questions that can be answered 'yes' or
'no'. If it's never heard of your animal before, it asks you to give it some
question that it can use next time to find out whether someone's given it your
new animal.

``` 
5 REM Pangolins
10 LET nq=100: REM number of questions and animals
15 DIM q$(nq,50): DIM a(nq,2): DIM r$(1)
20 LET qf=8
30 FOR n=1 TO qf/2 1
40 READ q$(n): READ a(n,1): READ a(n,2)
50 NEXT n
60 FOR n=n TO qf-1
70 READ q$(n): NEXT n
100 REM start playing
110 PRINT "Think of an animal.","Press any key to continue."
120 PAUSE 0
130 LET c=1: REM start with 1st question
140 IF a(c,1)=0 THEN GO TO 300
150 LET p$=q$(c): GO SUB 910
160 PRINT "?": GO SUB 1000
170 LET in=1: IF r$="y" THEN GO TO 210
180 IF r$="Y" THEN GO TO 210
190 LET in=2: IF r$="n" THEN GO TO 210
200 IF r$\<\>"N" THEN GO TO 150
210 LET c=a(c,in): GO TO 140
300 REM animal
310 PRINT "Are you thinking of"
320 LET P$=q$(c): GO SUB 900: PRINT "?"
330 GO SUB 1000
340 IF r$="y" THEN GO TO 400
350 IF r$="Y" THEN GO TO 400
360 IF r$="n" THEN GO TO 500
370 IF r$="N" THEN GO TO 500
380 PRINT "Answer me properly when I'm","talking to you.": GO TO 300
400 REM guessed it
410 PRINT "I thought as much.": GO TO 800
500 REM new animal
510 IF qf\>nq-1 THEN PRINT "I'm sure your animal is very", 
"interesting,but I don't have","room for it just now.": GO TO 800
520 LET q$(qf)=q$(c): REM move old animal
530 PRINT "What is it, then?": INPUT q$(qf+1)
540 PRINT "Tell me a question which dist ","inguishes between "
550 LET p$=q$(qf): GO SUB 900: PRINT " and"
560 LET p$=q$(qf+1): GO SUB 900: PRINT " "
570 INPUT s$: LET b=LEN s$
580 IF s$(b)="?" THEN LET b=b-1
590 LET q$(c)=s$(TO b): REM insert question
600 PRINT "What is the answer for"
610 LET p$=q$(qf+1): GO SUB 900: PRINT "?"
620 GO SUB 1000
630 LET in=1: LET io=2: REM answers for new and old animals
640 IF r$="y" THEN GO T0 700
650 IF r$="Y" THEN GO TO 700
660 LET in=2: LET io=1
670 IF r$="n" THEN GO TO 700
680 IF r$="N" THEN GO TO 700
690 PRINT "That's no good. ": GO TO 600
700 REM update answers
710 LET a(c,in)=qf+1: LET a(c,io)=qf
720 LET qf=qf+2: REM next free animal space
730 PRINT "That fooled me."
800 REM again?
810 PRINT "Do you want another go?": GO SUB 1000
820 IF r$="y" THEN GO TO 100
830 IF r$="Y" THEN GO TO 100
840 STOP
900 REM print without trailing spaces
905 PRINT " ";
910 FOR n=50 TO 1 STEP -1
920 IF p$(n)\<\>" " THEN GO TO 940
930 NEXT n
940 PRINT p$(TO n);: RETURN
1000 REM get reply
1010 INPUT r$: IF r$="" THEN RETURN
1020 LET r$=r$(1): RETURN
2000 REM initial animals
2010 DATA "Does it live in the sea",4,2
2020 DATA "Is it scaly",3,5
2030 DATA "Does it eat ants",6,7
2040 DATA "a whale", "a blancmange", "a pangolin", "an ant"
``` 

Here is a program to draw a Union Flag.

``` 
5 REM union flag
10 LET r=2: LET w=7: LET b=1
20 BORDER 0: PAPER b: INK w: CLS
30 REM black in bottom of screen
40 INVERSE 1
50 FOR n=40 TO 0 STEP -8
60 PLOT PAPER 0;7,n: DRAW PAPER 0;241,0
70 NEXT n: INVERSE 0
100 REM draw in white parts
105 REM St. George
110 FOR n=0 TO 7
120 PLOT 104+n,175: DRAW 0,-35
130 PLOT 151-n,175: DRAW 0,-35
140 PLOT 151-n,48: DRAW 0,35
150 PLOT 104+n,48: DRAW 0,35
160 NEXT n
200 FOR n=0 TO 11
210 PLOT 0,139-n: DRAW 111,0
220 PLOT 255,139-n: DRAW 111,0
230 PLOT 255,84+n: DRAW -111,0
240 PLOT 0,84+n: DRAW 111,0
250 NEXT n
300 REM St. Andrew
310 FOR n=0 TO 35
320 PLOT 1+2\*n,175-n: DRAW 32,0
330 PLOT 224-2\*n,175-n: DRAW 16,0
340 PLOT 254-2\*n,48+n: DRAW-32,0
350 PLOT 17+2\*n,48+n: DRAW 16,0
360 NEXT n
370 FOR n=0 TO 19
380 PLOT 185+2\*n,140+n: DRAW 32,0
390 PLOT 200+2\*n,83-n: DRAW 16,0
400 PLOT 39-2\*n,83-n: DRAW 32,0
410 PLOT 54-2\*n,140+n: DRAW -16,0
420 NEXT n
425 REM fill in extra bits
430 FOR n=0 TO 15
440 PLOT 255,160+n: DRAW 2\*n-30,0
450 PLOT 0,63-n DRAW 31-2\*n,0
460 NEXT n
470 FOR n=0 TO 7
480 PLOT 0,160+n: DRAW 14 2\*n,0
485 PLOT 255,63-n: DRAW 2\*n-15,0
490 NEXT n
500 REM red stripes
510 INVERSE 1
520 REM St George
530 FOR n=96 TO 120 STEP 8
540 PLOT PAPER r;7,n: DRAW PAPER r;241,0
550 NEXT n
560 FOR n=112 TO 136 STEP 8
570 PLOT PAPER r;n,168: DRAW PAPER r;0,-113
580 NEXT n
600 REM St Patrick
610 PLOT PAPER r;170,140: DRAW PAPER r;70,35
620 PLOT PAPER r;179,140: DRAW PAPER r;70,35
630 PLOT PAPER r;199,83: DRAW PAPER r;56,-28
640 PLOT PAPER r;184,83: DRAW PAPER r;70,-35
650 PLOT PAPER r;86,83: DRAW PAPER r;-70,-35
660 PLOT PAPER r;72,83: DRAW PAPER r;-70,-35
670 PLOT PAPER r;56,140: DRAW PAPER r;-56,28
680 PLOT PAPER r;71,140: DRAW PAPER r;-70,35
690 INVERSE 0: PAPER 0: INK 7
``` 
 
If you're not British, have a go at drawing your own flag. Tricolours are fairly
easy, although some of the colours - for instance the orange in the Irish flag -
might present difficulties. If you're an American, you might be able to fit the
character \* in.

Here is a program to play hangman. One player enters a word, and the other
guesses
 
```
5 REM hangman
10 REM set up screen
20 INK 0: PAPER 7: CLS
30 LET x=240: GO SUB 1000: REM draw man
40 PLOT 238,128: DRAW 4,0: REM mouth
100 REM set up word
110 INPUT w$: REM word to guess
120 LET b=LEN w$: LET v$=" "
130 FOR n=2 TO b: LET v$=v$+" "
140 NEXT n: REM v$=word guessed so far
150 LET c=0: LET d=0: REM guess & mistake counts
160 FOR n=0 TO b-1
170 PRINT AT 20,n;" ";
180 NEXT n: REM write 's instead of letters
200 INPUT "Guess a letter ";g$
210 IF g$="" THEN GO TO 200
220 LET g$=g$(1): REM 1st letter only
230 PRINT AT 0,c;g$
240 LET c=c+1: LET u$=v$
250 FOR n=1 TO bL REM update guessed word
260 IF w$(n)=g$ THEN LET v$(n)=g$
270 NEXT n
280 PRINT AT l9,0;v$
290 IF v$=w$ THEN GO TO 500: REM word guessed
300 IF v$\<\>u$ THEN GO TO 200: REM guess was right
400 REM draw next part of gallows
410 IF d=8 THEN GO TO 600: REM hanged
420 LET d=d+1
430 READ x0,y0,x,y
440 PLOT x0,y0: DRAW x,y
450 GO TO 200
500 REM free man
510 OVER 1: REM rub out man
520 LET x=240: GO SUB 1000
530 PLOT 238,128: DRAW 4,0: REM mouth
540 OVER 0: REM redraw man
550 LET x=146: GO SUB 1000
560 PLOT 143,129: DRAW 6,0, PI/2: REM smile
570 GO TO 800
600 REM hang man
610 OVER 1: REM rub out floor
620 PLOT 255,65: DRAW -48,0
630 DRAW 0,-48: REM open trapdoor
640 PLOT 238,128: DRAW 4,0: REM rub out mouth
650 REM move limbs
655 REM arms
660 PLOT 255,117: DRAW -15,-15: DRAW -15,15
670 OVER 0
680 PLOT 236,81: DRAW 4,21: DRAW 4, 21
690 OVER 1: REM legs
700 PLOT 255,66: DRAW -15,15: DRAW -15,-15
710 OVER 0
720 PLOT 236,60: DRAW 4,21: DRAW 4,-21
730 PLOT 237,127: DRAW 6,0,-PI/2: REM frown
740 PRINT AT 19,0;w$
800 INPUT "again? ";a$
810 IF a$="" THEN GO TO 850
820 LET a$=a$(1)
830 IF a$="n" THEN STOP
840 IF a$(1)="N" THEN STOP
850 RESTORE: GO TO 5
1000 REM draw man at column x
1010 REM head
1020 CIRCLE x,132,8
1030 PLOT x+4,134: PLOT x-4,134: PLOT x,131
1040 REM body
1050 PLOT x,123: DRAW 0,-20
1055 PLOT x,101: DRAW 0,-19
1060 REM legs
1070 PLOT x-15,66: DRAW 15,15: DRAW 15,-15
1080 REM arms
1090 PLOT x-15,117: DRAW 15,-15: DRAW 15,15
1100 RETURN
2000 DATA 120,65,135,0,184,65,0,91
2010 DATA 168,65,16,16,184,81,16,-16
2020 DATA 184,156,68,0,184,140,16,16
2030 DATA 204,156,-20,-20,240,156,0,-16
```
