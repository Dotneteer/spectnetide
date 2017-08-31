grammar Z80Asm;

/*
 * Parser Rules
 */

compileUnit	
	:	asmline? (NEWLINE asmline)* EOF 
	;

asmline
	:	label? (pragma | operation)
	|	directive
	|	NEWLINE
	;

label
	:	IDENTIFIER ':'?
	;

pragma
	:	orgPragma
	|	entPragma
	|	dispPragma
	|	equPragma
	|	defbPragma
	|	defwPragma
	|	defmPragma
	|	skipPragma
	;

directive
	:	(IFDEF|IFNDEF|DEFINE|UNDEF) IDENTIFIER
	|	ENDIF
	|	ELSE
	;	

orgPragma	: ORGPRAG expr ;
entPragma	: ENTPRAG expr ;
dispPragma	: DISPRAG expr ;
equPragma	: EQUPRAG expr ;
defbPragma	: DBPRAG expr (',' expr)* ;
defwPragma	: DWPRAG expr (',' expr)* ;
defmPragma	: DMPRAG STRING ;
skipPragma	: SKIPRAG expr ;

operation
	:	trivialOperation
	|	loadOperation
	|	incDecOperation
	|	exchangeOperation
	|	aluOperation
	|	controlFlowOperation
	|	stackOperation
	|	ioOperation
	|	interruptOperation
	|	bitOperation
	;

trivialOperation
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

loadOperation
	:	LD ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|'xl'|'XL'|'xh'|'XH'|'yl'|'YL'|'yh'|'YH'
			|('(' HL ')')) ',' 
			('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|'xl'|'XL'|'xh'|'XH'|'yl'|'YL'|'yh'|'YH'
			| ('(' HL ')'))
	|	LD ('i'|'I'|'r'|'R') ',' ('a'|'A')
	|   LD ('a'|'A') ',' ('i'|'I'|'r'|'R')
	|	LD SP ',' (HL|REGIDX)
	|	LD ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|'xl'|'XL'|'xh'|'XH'|'yl'|'YL'|'yh'|'YH'
			| ('(' HL ')')) ',' expr
	|	LD (BC|DE|HL|SP|REGIDX) ',' expr
	|	LD '(' (BC|DE) ')' ',' ('a'|'A')
	|	LD indexedAddr ',' ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L')
	|	LD ('a'|'A') ',' '(' (BC|DE) ')'
	|	LD ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L') ',' indexedAddr
	|	LD '(' expr ')' ',' ('a'|'A'|BC|DE|HL|REGIDX)
	|	LD ('a'|'A'|BC|DE|HL|REGIDX) ',' '(' expr ')'
	;

incDecOperation
	:	(INC|DEC) ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|'xl'|'XL'|'xh'|'XH'|'yl'|'YL'|'yh'|'YH'
			|('(' HL ')'))
	|	(INC|DEC) (BC|DE|HL|SP|REGIDX)
	|	(INC|DEC) indexedAddr
	;

exchangeOperation
	:	EX AF ',' AFX
	|	EX DE ',' HL
	|	EX '(' SP ')' ',' (HL|REGIDX)
	;

aluOperation
	:	(ADD|ADC|SBC) ('a'|'A') ',' ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|'xl'|'XL'|'xh'|'XH'|'yl'|'YL'|'yh'|'YH'
			|('(' HL ')'))
	|	(ADD|ADC|SBC) (HL|REGIDX) ',' 
			(BC|DE|HL|SP|REGIDX)
	|	(ADD|ADC|SBC) ('a'|'A') ',' indexedAddr
	|	(ADD|ADC|SBC) ('a'|'A') ',' expr
	|	(SUB|AND|XOR|OR|CP) ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|'xl'|'XL'|'xh'|'XH'|'yl'|'YL'|'yh'|'YH'
			|('(' HL ')'))
	|	(SUB|AND|XOR|OR|CP) indexedAddr
	|	(SUB|AND|XOR|OR|CP)	expr
	;

controlFlowOperation
	:	DJNZ expr
	|	JR ( ('z' | 'Z' | 'nz' | 'NZ' | 'c' | 'C' | 'nc' | 'NC') ',' )? expr
	|	JP ( ('z' | 'Z' | 'nz' | 'NZ' | 'c' | 'C' | 'nc' | 'NC' 
		| 'po' | 'PO' | 'pe' | 'PE' | 'p' | 'P' | 'm' | 'M') ',' )? expr
	|	JP '(' (HL|REGIDX) ')'
	|	RET ( 'z' | 'Z' | 'nz' | 'NZ' | 'c' | 'C' | 'nc' | 'NC' 
		| 'po' | 'PO' | 'pe' | 'PE' | 'p' | 'P' | 'm' | 'M' )
	|	CALL ( ('z' | 'Z' | 'nz' | 'NZ' | 'c' | 'C' | 'nc' | 'NC' 
		| 'po' | 'PO' | 'pe' | 'PE' | 'p' | 'P' | 'm' | 'M') ',' )? expr
	|	RST expr
	;

stackOperation
	:	(PUSH|POP) (BC|DE|HL|AF|REGIDX)
	;

ioOperation
	:	IN ('a'|'A') ',' '(' expr ')'
	|	IN (('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L') ',')? '(' ('c'|'C') ')'
	|	OUT '(' expr ')' ',' ('a'|'A')
	|	OUT '(' ('c'|'C') ')' ',' ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L')
	|	OUT '(' ('c'|'C') ')' (',' DECNUM)?
	;

interruptOperation
	:	IM DECNUM
	;

bitOperation
	:	(RLC|RRC|RL|RR|SLA|SRA|SLL|SRL) ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|('(' HL ')'))
	|	(RLC|RRC|RL|RR|SLA|SRA|SLL|SRL) indexedAddr (',' ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'))?
	|	(BIT|RES|SET) expr ',' ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'
			|('(' HL ')'))
	|	(BIT) expr ',' indexedAddr
	|	(RES|SET) expr ',' indexedAddr (',' ('a'|'A'|'b'|'B'|'c'|'C'|'d'|'D'|'e'|'E'|'h'|'H'|'l'|'L'))?
	;

// --- Addressing
indexedAddr
	:	'(' REGIDX (('+' | '-') (literalExpr | symbolExpr | '[' expr ']'))? ')'
	;

// --- Expressions
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
	: unaryExpr (('*' | '/' | '%') unaryExpr)*
	;

unaryExpr
	: '+' unaryExpr
	| '-' unaryExpr
	| '[' expr ']'
	| literalExpr
	| symbolExpr
	;

literalExpr
	: DECNUM 
	| HEXNUM 
	| CHAR
	| '$'
	;

symbolExpr
	: IDENTIFIER
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

NOP		: 'nop'|'NOP' ;
RLCA	: 'rlca'|'RLCA' ;
RRCA	: 'rrca'|'RRCA' ;
RLA		: 'rla'|'RLA' ;
RRA		: 'rra'|'RRA' ;
DAA		: 'daa'|'DAA' ;
CPL		: 'cpl'|'CPL' ;
SCF		: 'scf'|'SCF' ;
CCF		: 'ccf'|'CCF' ;
RET		: 'ret'|'RET' ;
EXX		: 'exx'|'EXX' ;
DI		: 'di'|'DI' ;
EI		: 'ei'|'EI' ;
NEG		: 'neg'|'NEG' ;
RETN	: 'retn'|'RETN' ;
RETI	: 'reti'|'RETI' ;
RLD		: 'rld'|'RLD' ;
RRD		: 'rrd'|'RRD' ;
LDI		: 'ldi'|'LDI' ;
CPI		: 'cpi'|'CPI' ;
INI		: 'ini'|'INI' ;
OUTI	: 'outi'|'OUTI' ;
LDD		: 'ldd'|'LDD' ;
CPD		: 'cpd'|'CPD' ;
IND		: 'ind'|'IND' ;
OUTD	: 'outd'|'OUTD' ;
LDIR	: 'ldir'|'LDIR' ;
CPIR	: 'cpir'|'CPIR' ;
INIR	: 'inir'|'INIR' ;
OTIR	: 'otir'|'OTIR' ;
LDDR	: 'lddr'|'LDDR' ;
CPDR	: 'cpdr'|'CPDR' ;
INDR	: 'indr'|'INDR' ;
OTDR	: 'otdr'|'OTDR' ;

// --- Other instruction tokens
LD		: 'ld'|'LD' ;
INC		: 'inc'|'INC' ;
DEC		: 'dec'|'DEC' ;
EX		: 'ex'|'EX' ;
ADD		: 'add'|'ADD' ;
ADC		: 'adc'|'ADC' ;
SUB		: 'sub'|'SUB' ;
SBC		: 'sbc'|'SBC' ;
AND		: 'and'|'AND' ;
XOR		: 'xor'|'XOR' ;
OR		: 'or'|'OR' ;
CP		: 'cp'|'CP' ;
DJNZ	: 'djnz'|'DJZN' ;
JR		: 'jr'|'JR' ;
JP		: 'jp'|'JP' ;
CALL	: 'call'|'CALL' ;
RST		: 'rst'|'RST' ;
PUSH	: 'push'|'PUSH' ;
POP		: 'pop'|'POP' ;
IN		: 'in'|'IN' ;
OUT		: 'out'|'OUT' ;
IM		: 'im'|'IM' ;
RLC		: 'rlc'|'RLC' ;
RRC		: 'rrc'|'RRC' ;
RL		: 'rl'|'RL' ;
RR		: 'rr'|'RR' ;
SLA		: 'sla'|'SLA' ;
SRA		: 'sra'|'SRA' ;
SLL		: 'sll'|'SLL' ;
SRL		: 'srl'|'SRL' ;
BIT		: 'bit'|'BIT' ;
RES		: 'res'|'RES' ;
SET		: 'set'|'SET' ;

// --- Pre-processor tokens
IFDEF	: '#ifdef'|'#IFDEF' ;
IFNDEF	: '#ifndef'|'#IFNDEF' ;
ENDIF	: '#endif'|'#ENDIF' ;
ELSE	: '#else'|'#ELSE' ;
DEFINE	: '#define'|'#DEFINE' ;
UNDEF	: '#undef'|'#UNDEF' ;

// --- Pragma tokens
ORGPRAG	: '.org' | '.ORG' | 'org' | 'ORG' ;
ENTPRAG	: '.ent' | '.ENT' | 'ent' | 'ENT' ;
EQUPRAG	: '.equ' | '.EQU' | 'equ' | 'EQU' ;
DISPRAG	: '.disp' | '.DISP' | 'disp' | 'DISP' ;
DBPRAG	: '.defb' | '.DEFB' | 'defb' | 'DEFB' ;
DWPRAG	: '.defw' | '.DEFW' | 'defw' | 'DEFW' ;
DMPRAG	: '.defm' | '.DEFM' | 'defm' | 'DEFM' ;
SKIPRAG	: '.skip' | '.SKIP' | 'skip' | 'SKIP' ;

// --- Basic literals
DECNUM	: DIGIT DIGIT? DIGIT? DIGIT? DIGIT?;
HEXNUM	: '#' HDIGIT HDIGIT? HDIGIT? HDIGIT?
		| HDIGIT HDIGIT? HDIGIT? HDIGIT? ('H' | 'h');
HDIGIT	: DIGIT | 'a'..'f' | 'A'..'F';
DIGIT	: D0|D1|D2|'3'..'9';
D0		: '0' ;
D1		: '1' ;
D2		: '2' ;
CHAR	: '"' ( '\"' | . ) '"' ;
STRING	: '"' ( '\"' | . )* '"' ;

// --- Registers
BC		: 'bc'|'BC' ;
DE		: 'de'|'DE' ;
HL		: 'hl'|'HL'	;
SP		: 'sp'|'SP' ;
AF		: 'af'|'AF' ;
AFX		: 'af\''|'AF\'' ;
REGIDX	: 'ix'|'IX'|'iy'|'IY' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;
