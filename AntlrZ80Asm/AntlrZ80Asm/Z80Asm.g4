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
skipPragma	: SKIPRAG expr (',' expr)?;

operation
	:	trivialOperation
	|	compoundOperation
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
	|	HALT
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

compoundOperation
	:	LD operand ',' operand
	|	INC operand
	|	DEC operand
	|	EX operand ',' operand
	|	ADD operand ',' operand
	|	ADC operand ',' operand
	|	SUB operand
	|	SBC operand ',' operand
	|	AND operand
	|	XOR operand
	|	OR operand
	|	CP operand
	|	DJNZ operand
	|	JR (condition ',')? operand
	|	JP (condition ',')? operand
	|	CALL (condition ',')? operand
	|	RET condition?
	|	RST operand
	|	PUSH operand
	|	POP operand
	|	IN (operand ',')? operand
	|	OUT (operand ',')? operand
	|	IM operand
	|	RLC (operand ',')? operand
	|	RRC (operand ',')? operand
	|	RL (operand ',')? operand
	|	RR (operand ',')? operand
	|	SLA (operand ',')? operand
	|	SRA (operand ',')? operand
	|	SLL (operand ',')? operand
	|	SRL (operand ',')? operand
	|	BIT expr ',' operand
	|	RES expr ',' (operand ',')? operand
	|	SET expr ',' (operand ',')? operand
	;

// --- Operands
operand
	:	reg8
	|	reg8Idx
	|	reg8Spec
	|	reg16
	|	reg16Idx
	|	reg16Spec
	|	regIndirect
	|	cPort
	|	memIndirect
	|	indexedAddr
	|	expr
	;

reg8
	:	'a'|'A'
	|	'b'|'B'
	|	'c'|'C'
	|	'd'|'D'
	|	'e'|'E'
	|	'h'|'H'
	|	'l'|'L'
	;

reg8Idx
	:	'xl'|'XL'
	|	'xh'|'XH'
	|	'yl'|'YL'
	|	'yh'|'YH'
	;

reg8Spec
	:	'i'|'I'
	|	'r'|'R'
	;

reg16
	:	'bc'|'BC'
	|	'de'|'DE'
	|	'hl'|'HL'
	|	'sp'|'SP'
	;

reg16Idx
	:	'ix'|'IX'
	|	'iy'|'IY'
	;

reg16Spec
	:	'af\''|'AF\''
	|	'af'|'AF'
	;

regIndirect
	:	'(' (reg16) ')'
	;

cPort
	:	'(' ('c'|'C') ')'
	;

memIndirect
	:	'(' expr ')'
	;

indexedAddr
	:	'(' reg16Idx (('+' | '-') (literalExpr | symbolExpr | '[' expr ']'))? ')'
	;

condition
	:	'z'|'Z'
	|	'nz'|'NZ'
	|	'c'|'C'
	|	'nc'|'NC'
	|	'po'|'PO'
	|	'pe'|'PE'
	|	'p'|'P'
	|	'm'|'M'
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
HALT	: 'halt'|'HALT' ;
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
DIGIT	: '0'..'9';
CHAR	: '"' ( '\"' | . ) '"' ;
STRING	: '"' ( '\"' | . )* '"' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;
