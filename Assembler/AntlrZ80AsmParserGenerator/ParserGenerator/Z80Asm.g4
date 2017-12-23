grammar Z80Asm;

/*
 * Parser Rules
 */

compileUnit	
	:	EOF
	|	NEWLINE* asmline (NEWLINE+ asmline)* NEWLINE* EOF
	;

asmline
	:	label? (pragma | operation) comment?
	|	directive comment?
	|	comment
	|   label comment?
	;

label
	:	IDENTIFIER ':'?
	;

comment
	:	COMMENT
	;

pragma
	:	orgPragma
	|	entPragma
	|	xentPragma
	|	dispPragma
	|	equPragma
	|	varPragma
	|	defbPragma
	|	defwPragma
	|	defmPragma
	|	skipPragma
	|	externPragma
	|	defsPragma
	|	fillbPragma
	|	fillwPragma
	|	modelPragma
	;

directive
	:	(IFDEF|IFNDEF|DEFINE|UNDEF|IFMOD|IFNMOD) IDENTIFIER
	|	ENDIF
	|	ELSE
	|	IF expr
	|	INCLUDE (STRING | FSTRING)
	;	

orgPragma	: ORGPRAG expr ;
entPragma	: ENTPRAG expr ;
xentPragma	: XENTPRAG expr ;
dispPragma	: DISPRAG expr ;
equPragma	: EQUPRAG expr ;
varPragma	: VARPRAG expr ;
defbPragma	: DBPRAG expr (',' expr)* ;
defwPragma	: DWPRAG expr (',' expr)* ;
defmPragma	: DMPRAG STRING ;
skipPragma	: SKIPRAG expr (',' expr)?;
externPragma: EXTPRAG ;
defsPragma	: DSPRAG expr ;
fillbPragma	: FBPRAG expr ',' expr ;
fillwPragma : FWPRAG expr ',' expr ;
modelPragma : MODPRAG IDENTIFIER ;

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
	|	SUB (operand ',')? operand
	|	SBC operand ',' operand
	|	AND (operand ',')? operand
	|	XOR (operand ',')? operand
	|	OR (operand ',')? operand
	|	CP (operand ',')? operand
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
	|	'ixl'|'IXL'|'IXl'
	|	'ixh'|'IXH'|'IXh'
	|	'iyl'|'IYL'|'IYl'
	|	'iyh'|'IYH'|'IYh'
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
	: orExpr ('?' expr ':' expr)?
	; 

orExpr
	: xorExpr ('|' xorExpr)*
	;

xorExpr
	: andExpr ('^' andExpr)*
	;

andExpr
	: equExpr ('&' equExpr)*
	;

equExpr
	: relExpr (('=='|'!=') relExpr)*
	;

relExpr
	: shiftExpr (('<'|'<='|'>'|'>=') shiftExpr)*
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
	| '~' unaryExpr
	| '[' expr ']'
	| literalExpr
	| symbolExpr
	;

literalExpr
	: DECNUM 
	| HEXNUM 
	| CHAR
	| BINNUM
	| '$'
	;

symbolExpr
	: IDENTIFIER
	;

/*
 * Lexer Rules
 */

fragment CommonCharacter
	: SimpleEscapeSequence
	| HexEscapeSequence
	;

fragment SimpleEscapeSequence
	: '\\i'
	| '\\p'
	| '\\f'
	| '\\b'
	| '\\I'
	| '\\o'
	| '\\a'
	| '\\t'
	| '\\P'
	| '\\C'
	| '\\\''
	| '\\"'
	| '\\\\'
	| '\\0'
	;

fragment HexEscapeSequence
	: '\\x' HexDigit
	| '\\x' HexDigit HexDigit
	;

fragment HexDigit 
	: [0-9] 
	| [A-F] 
	| [a-f]
	;

fragment Digit 
	: '0'..'9' 
	;

fragment BinDigit
	: ('0'|'1') '_'?
	;

COMMENT
	:	';' ~('\r' | '\n')*
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
DJNZ	: 'djnz'|'DJNZ' ;
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
IFDEF	: '#ifdef' ;
IFNDEF	: '#ifndef' ;
ENDIF	: '#endif' ;
ELSE	: '#else' ;
DEFINE	: '#define' ;
UNDEF	: '#undef' ;
INCLUDE	: '#include' ;
IF		: '#if' ;
IFMOD	: '#ifmod' ;
IFNMOD	: '#ifnmod' ;

// --- Pragma tokens
ORGPRAG	: '.org' | '.ORG' | 'org' | 'ORG' ;
ENTPRAG	: '.ent' | '.ENT' | 'ent' | 'ENT' ;
XENTPRAG: '.xent' | '.XENT' | 'xent' | 'XENT' ;
EQUPRAG	: '.equ' | '.EQU' | 'equ' | 'EQU' ;
VARPRAG	: '.var' | '.VAR' | 'var' | 'VAR' ;
DISPRAG	: '.disp' | '.DISP' | 'disp' | 'DISP' ;
DBPRAG	: '.defb' | '.DEFB' | 'defb' | 'DEFB' ;
DWPRAG	: '.defw' | '.DEFW' | 'defw' | 'DEFW' ;
DMPRAG	: '.defm' | '.DEFM' | 'defm' | 'DEFM' ;
SKIPRAG	: '.skip' | '.SKIP' | 'skip' | 'SKIP' ;
EXTPRAG : '.extern'|'.EXTERN'|'extern'|'EXTERN' ;
DSPRAG	: '.defs' | '.DEFS' | 'defs' | 'DEFS' ;
FBPRAG	: '.fillb' | '.FILLB' | 'fillb' | 'FILLB' ;
FWPRAG	: '.fillw' | '.FILLW' | 'fillw' | 'FILLW' ;
MODPRAG : '.model' | '.MODEL' | 'model' | 'MODEL' ;

// --- Basic literals
DECNUM	: Digit Digit? Digit? Digit? Digit?;
HEXNUM	: ('#'|'0x') HexDigit HexDigit? HexDigit? HexDigit?
		| HexDigit HexDigit? HexDigit? HexDigit? ('H' | 'h') ;
BINNUM	: ('%'| ('0b' '_'?)) BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		;

CHAR	: '"' (~['\\\r\n\u0085\u2028\u2029] | CommonCharacter) '"' ;
STRING	: '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"' ;
FSTRING	: '<'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '>' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;
