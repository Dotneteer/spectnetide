---
layout: documents
categories: 
  - "Spectrum48 BASIC"
title:  "Appendix E: Binary and hexadecimal"
alias: basic-appe
seqno: 310
selector: spectrum
permalink: "spectrum/basic-appe"
---

This appendix describes how computers count, using the binary system.

Most European languages count using a more or less regular pattern of tens - in
English, for example, although it starts off a bit erratically, it soon settles
down into regular groups:

**twenty, twenty one, twenty two, . . . twenty nine**

**thirty, thirty one, thirty two, . . . thirty nine**

**forty, forty one, forty two, . . . forty nine**

and so on, and this is made even more systematic with the Arabic numerals that
we use. However, the only reason for using ten is that we happen to have ten
fingers and thumbs.

Instead of using the decimal system, with ten as its base, computers use a form
of binary called hexadecimal (or hex, for short), based on sixteen. As there are
only ten digits available in our number system we need six extra digits to do
the counting. So we use A, B, C, D, E and F. And what comes after F? Just as we,
with ten fingers, write 10 for ten, so computers write 10 for sixteen. Their
number system starts off:


|Hex | English|
|---|---|
0  | nought
|1 | one|
|2   |two|
|  : |  :   |
|  : |  :   |
|9 |nine|


just as ours does, but then it carries on

|Hex | English|
|---|---|
|A |ten|
|B |eleven|
|C |twelve|
|D |thirteen|
|E |fourteen|
|F |fifteen|
|10 |sixteen|
|11 |seventeen
|: |:|
|: |:|
|19 |twenty five|
|1A |twenty six|
|1B |twenty seven|
|: |:|
|: |:|
|1F |thirty one|
|20 |thirty two|
|21 |thirty three|
|: |:|
|: |:|
|9E |one hundred and fifty eight|
|9F |one hundred and fifty nine|
|A0 |one hundred and sixty|
|A1 |one hundred and sixty one|
|: |:|
|B4 |one hundred and eighty|
|: |:|
|FE |two hundred and fifty four|
|FF |two hundred and fifty five|
|100 |two hundred and fifty six|

If you are using hex notation and you want to make the fact quite plain, then
write 'h' at the end of the number, and say 'hex'. For instance, for one hundred
and fifty eight, write '9Eh' and say 'nine E hex'.

You will be wondering what all this has to do with computers. In fact, computers
behave as though they had only two digits, represented by a low voltage, or off
(0), and a high voltage, or on (1). This is called the binary system, and the
two binary digits are called bits: so a bit is either 0 or 1.

In the various systems, counting starts off

|English |Decimal |Hexadecimal |Binary|
|---|---|---|---|
|nought |0 |0 |0 or 0000|
|one |1 |1 |1 or 0001|
|two |2 |2 |10 or 0010|
|three |3 |3 |11 or 0011|
|four |4 |4 |100 or 0100|
|five |5 |5 |101 or 0101|
|six |6 |6 |110 or 0110|
|seven |7 |7 |111 or 0111|
|eight |8 |8 |1000|
|nine |9 |9 |1001|
|ten |10 |A |1010|
|eleven |11 |B |1011|
|twelve |12 |C |1100|
|thirteen |13 |D |1101|
|fourteen |14 |E |1110|
|fifteen |15 |F |1111|
|sixteen |16 |10 |10000|

The important point is that sixteen is equal to two raised to the fourth power,
and this makes converting between hex and binary very easy.

To convert hex to binary, change each hex digit into four bits, using the table
above.

To convert binary to hex, divide the binary number into groups of four bits,
starting on the right, and then change each group into the corresponding hex
digit.

For this reason, although strictly speaking computers use a pure binary system,
humans often write the numbers stored inside a computer using hex notation.

The bits inside the computer are mostly grouped into sets of eight, or bytes. A
single byte can represent any number from nought to two hundred and fifty five
(11111111 binary or FF hex), or alternatively any character in the ZX Spectrum
character set. Its value can be written with two hex digits.

Two bytes can be grouped together to make what is technically called a word. A
word can be written using sixteen bits or four hex digits, and represents a
number from 0 to (in decimal) 2l6-l=65535.

A byte is always eight bits, but words vary in length from computer to computer.

The **BIN** notation in Chapter 14 provides a means of writing numbers in binary on
the ZX Spectrum: '**BIN** 0' represents nought, '**BIN** 1' represents one, '**BIN** 10'
represents two, and so on.

You can only use 0's and 1 's for this, so the number must be a non negative
whole number; for instance you can't write '**BIN** -11' for minus three - you must
write '-**BIN** 11' instead. The number must also be no greater than decimal 65535 -
i.e. it can't have more than sixteen bits.

**ATTR** really was binary. If you convert the result from **ATTR** into binary, you can
write it in eight bits.

The first is 1 for flashing, 0 for steady.

The second is 1 for bright, 0 for normal.

The next three are the code for the paper colour, written in binary.

The last three are the code for the ink colour, written in binary.

The colour codes also use binary: each code written in binary can be written in
three bits, the first for green, the second for red and the third for blue.

Black has no light at all, so all the bits are 0 (off). Therefore the code for
black is 000 in binary, or nought.

The pure colours, green, red and blue have just one bit 1 (on) out of the three.
Their codes are 100, 010 and 001 in binary, or four, two and one.

The other colours are mixtures of these, so their codes in binary have two or
more bits 1.