grammar Z80Asm;

/*
 * Parser Rules
 */

compileUnit
	:	(asmline NEWLINE?)* EOF
	;

asmline
	:	label? (pragma | instruction) | NEWLINE
	;

label
	:	IDENTIFIER ':'
	;

pragma
	:	'.db' | '.dw' | '.skip' | '.equ'
	;

instruction
	:	trivialInstruction
	|	loadInstruction
	;

trivialInstruction
	:	NOP
	|	RLCA
	|	RRCA
	|	RLA
	|	RRA
	|	DAA
	|	CPL
	|	SCF
	|	CCF
	|	RET
	|	EXX
	|	DI
	|	EI
	|	NEG
	|	RETN
	|	RETI
	|	RLD
	|	RRD
	|	LDI
	|	CPI
	|	INI
	|	OUTI
	|	LDD
	|	CPD
	|	IND
	|	OUTD
	|	LDIR
	|	CPIR
	|	INIR
	|	OTIR
	|	LDDR
	|	CPDR
	|	INDR
	|	OTDR
	;

loadInstruction
	:	load8BitInstruction
	|	load8BitWithValueInstruction
	;

load8BitInstruction
	:	LD (REG8 | HLIND) ',' (REG8 | HLIND)
	;

load8BitWithValueInstruction
	:	LD (REG8 | HLIND) ',' expr
	;

expr
	: xorExpr ('|' xorExpr)*
	;

xorExpr
	: andExpr ('^' andExpr)*
	;

andExpr
	: shiftExpr ('&' shiftExpr)*
	;

shiftExpr
	: addExpr (('<<' | '>>' ) addExpr)*
	;

addExpr
	: multExpr (('+' | '-' ) multExpr)*
	;

multExpr
	: unExpr (('*' | '/' | '%') unExpr)*
	;

unExpr
	: '+' unExpr
	| '-' unExpr
	| '[' expr ']'
	| CONST
	;

/*
 * Lexer Rules
 */

COMMENT
	:	';' ~('\r' | '\n')* -> channel(HIDDEN)
	;

WS
	:	(' ' | '\t') -> channel(HIDDEN)
	;

NEWLINE
	:	('\r'? '\n' | '\r')+
	;

// --- Trivial instruction tokens

NOP		: 'nop' | 'NOP';
RLCA	: 'rlca' | 'RLCA';
RRCA	: 'rrca' | 'RRCA';
RLA		: 'rla' | 'RLA';
RRA		: 'rra' | 'RRA';
DAA		: 'daa' | 'DAA';
CPL		: 'cpl' | 'CPL';
SCF		: 'scf' | 'SCF';
CCF		: 'ccf' | 'CCF';
RET		: 'ret' | 'RET';
EXX		: 'exx' | 'EXX';
DI		: 'di' | 'DI';
EI		: 'ei' | 'EI';
NEG		: 'neg' | 'NEG';
RETN	: 'retn' | 'RETN';
RETI	: 'reti' | 'RETI';
RLD		: 'rld' | 'RLD';
RRD		: 'rrd' | 'RRD';
LDI		: 'ldi'	| 'LDI';
CPI		: 'cpi' | 'CPI';
INI		: 'ini' | 'INI';
OUTI	: 'outi' | 'OUTI';
LDD		: 'ldd'	| 'LDD';
CPD		: 'cpd' | 'CPD';
IND		: 'ind' | 'IND';
OUTD	: 'outd' | 'OUTD';
LDIR	: 'ldir'| 'LDIR';
CPIR	: 'cpir' | 'CPIR';
INIR	: 'inir' | 'INIR';
OTIR	: 'otir' | 'OTIR';
LDDR	: 'lddr'| 'LDDR';
CPDR	: 'cpdr' | 'CPDR';
INDR	: 'indr' | 'INDR';
OTDR	: 'otdr' | 'OTDR';

// --- Other instruction tokens
LD		: 'ld' | 'LD';

// --- 8-bit registers
REG8	: 'a' | 'A' | 'b' | 'B' | 'c' | 'C' | 'd' | 'D' | 'e' | 'E'
		| 'h' | 'H' | 'l' | 'L' | 'r' | 'R' | 'i' | 'I'
		| 'xl' | 'XL' | 'xh' | 'XH' | 'yl' | 'YL' | 'yh' | 'YH';

HLIND	: '(' WS* ('hl' | 'HL') WS* ')';

REG16	: 'af' | 'AF' | 'bc' | 'BC' | 'de' | 'DE' | 'hl' | 'HL' | 'sp' | 'SP' | IDXREG;

IDXREG	: 'ix' | 'IX' | 'iy' | 'IY';

CONST	: DECNUM | HEXNUM | CHAR;

DECNUM	: DIGIT DIGIT? DIGIT? DIGIT? DIGIT?;
DIGIT	: '0'..'9';

HEXNUM	: '#' HDIGIT HDIGIT? HDIGIT? HDIGIT?
		| HDIGIT HDIGIT? HDIGIT? HDIGIT? 'H';

HDIGIT	: '0'..'9' | 'a'..'f' | 'A'..'F';

CHAR	: '"' ( '\\"' | . ) '"';

STRING	: '"' ( '\\"' | . )*? '"';

IDENTIFIER
	:	IDSTART IDCONT*
	;

IDSTART
	:	'_' | 'A'..'Z' | 'a'..'z'
	;

IDCONT
	:	'_' | '0'..'9' | 'A'..'Z' | 'a'..'z'
	;
