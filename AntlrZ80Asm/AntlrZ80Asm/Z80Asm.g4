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
	|	DI
	|	EI
	|	NEG
	|	RETN
	|	RETI
	|	RLD
	|	RRD
	|	LDI
	|	CPI
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
DI		: 'di' | 'DI';
EI		: 'ei' | 'EI';
NEG		: 'neg' | 'NEG';
RETN	: 'retn' | 'RETN';
RETI	: 'reti' | 'RETI';
RLD		: 'rld' | 'RLD';
RRD		: 'rrd' | 'RRD';
LDI		: 'ldi'	| 'LDI';
CPI		: 'cpi' | 'CPI';

IDENTIFIER
	:	IDSTART IDCONT*
	;

IDSTART
	:	'_' | 'A'..'Z' | 'a'..'z'
	;

IDCONT
	:	'_' | '0'..'9' | 'A'..'Z' | 'a'..'z'
	;

