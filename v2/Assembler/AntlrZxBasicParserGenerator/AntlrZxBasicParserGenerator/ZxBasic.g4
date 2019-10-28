grammar ZxBasic;

compileUnit
	:	(label? (asm_section | line | NEWL))* EOF
	;

label
	:	DECNUM
	|	IDENTIFIER NEWL
	|	IDENTIFIER COLON NEWL?
	;

line
	: line_item* LINE_END
	| line_item+
	;

line_item
	:	console
	|	preproc
	|	control_flow
	|	statement
	|	function
	|	operator
	|	type
	|	number
	|	identifier
	|	string
	|	comment
	|	other
	|	ErrorCharacter
	;

asm_section
	:	asm_start asm_body asm_end
	;

asm_start
	:	ASM
	;

asm_body
	:	(asm_token | NEWL+)*
	;

asm_end
	:	END_ASM
	;

console
	:	AT
	|	BOLD
	|	BORDER
	|	BRIGHT
	|	CLS
	|	FLASH
	|	INK
	|	INVERSE
	|	ITALIC
	|	OVER
	|	PAPER
	|	TAB
	;

preproc
	:	P_DEFINE
	|	P_ELSE
	|	P_ELSEIF
	|	P_ENDIF
	|	P_IF
	|	P_IFDEF
	|	P_IFNDEF
	|	P_INCLIB
	|	P_INCLUDE
	|	P_LINE
	|	P_PRAGMA
	|	P_REQUIRE
	|	P_UNDEF
	|	P_DINCLUDE
	;

statement
	:	ALIGN
	|	BEEP
	|	CIRCLE
	|	CLEAR
	|	CODE
	|	DATA
	|	DIM
	|	DRAW
	|	INPUT
	|	LET
	|	LOAD
	|	OUT
	|	PAUSE
	|	PLOT
	|	POINT
	|	POKE
	|	PRINT
	|	RANDOMIZE
	|	READ
	|	REM
	|	RESTORE
	|	SAVE
	|	VERIFY
	;

control_flow
	:	CONTINUE
	|	DO
	|	ELSE
	|	ELSEIF
	|	END
	|	EXIT
	|	FASTCALL
	|	FOR
	|	FUNCTION
	|	GOSUB
	|	GOTO
	|	IF
	|	LOOP
	|	NEXT
	|	RETURN
	|	STDCALL
	|	STEP
	|	STOP
	|	SUB
	|	THEN
	|	TO
	|	UNTIL
	|	WEND
	|	WHILE
	;

function
	:	ABS
	|	ACS
	|	ASC
	|	ASN
	|	ATN
	|	ATTR
	|	CAST
	|	CHR
	|	COS
	|	CSRLIN
	|	EXP
	|	GETKEY
	|	GETKEYSCANCODE
	|	HEX
	|	HEX16
	|	IN
	|	INKEY
	|	INT
	|	LBOUND
	|	LCASE
	|	LEN
	|	LN
	|	MULTIKEYS
	|	PEEK
	|	POS
	|	PRINT42
	|	PRINTAT42
	|	PRINT64
	|	PRINTAT64
	|	RND
	|	SCREEN
	|	SGN
	|	SIN
	|	SQR
	|	STR
	|	TAN
	|	UBOUND
	|	UCASE
	|	VAL
	;

operator
	:	AND
	|	BAND
	|	BNOT
	|	BOR
	|	BXOR
	|	MOD
	|	NOT
	|	OR
	|	SHL
	|	SHR
	|	XOR
	|	PLUS
	|	MINUS
	|	UPARR
	|	AMP
	|	NOTEQ
	|	LTOP
	|	LTEOP
	|	GTOP
	|	GTEOP
	|	MULOP
	|	SLASH
	|	LSHOP
	|	RSHOP
	;

other
	:	COLON
	|	UNDERSCORE
	|	DOLLAR
	|	SCOLON
	|	COMMA
	|	ASSIGN
	|	LPAR
	|	RPAR
	|	LSBRAC
	|	RSBRAC
	|	QMARK
	|	DOT
	|	HMARK
	;

number
	:	BINNUM
	|	DECNUM
	|	HEXNUM
	|	REALNUM
	;

identifier
	:	IDENTIFIER
	;

string
	:	ZXB_STRING
	|	ZXB_FSTRING
	;

type
	:	AS
	|	BYREF
	|	BYTE
	|	BYVAL
	|	CONST
	|	FIXED
	|	FLOAT
	|	INTEGER
	|	LONG
	|	STRING
	|	UBYTE
	|	UINTEGER
	|	ULONG
	;

comment
	:	block_comment
	|	line_comment
	;

block_comment
	:	BLOCK_COMMENT
	;

line_comment
	:	LINE_COMMENT
	;

asm_token
	:	console
	|	preproc
	|	control_flow
	|	statement
	|	function
	|	operator
	|	type
	|	number
	|	identifier
	|	string
	|	comment
	|	other
	|	COLON
	|	UNDERSCORE
	|	SINGLE_QUOTE
	|	SLASH
	|	DOLLAR
	|	SCOLON
	|	COMMA
	|	ASSIGN
	|	LPAR
	|	RPAR
	|	LSBRAC
	|	RSBRAC
	|	QMARK
	|	PLUS
	|	MINUS
	|	VBAR
	|	UPARR
	|	AMP
	|	LTOP
	|	GTOP
	|	MULOP
	|	DOT
	|	HMARK
	|	LBRAC
	|	RBRAC
	|	EXCLM
	|	GO
	|	LINE_END
	|	ErrorCharacter
	;

// ============================================================================
// Lexer rules for ZX BASIC
WS
	:	('\u0020' | '\t')+ -> channel(HIDDEN)
	;

NEWL:	NewLine;

COLON: ':' ;
UNDERSCORE: '_' ;
SINGLE_QUOTE: '\'' ;
SLASH: '/' ;
DOLLAR: '$' ;
SCOLON	: ';' ;
COMMA	: ',' ;
ASSIGN	: '=' ;
LPAR	: '(' ;
RPAR	: ')' ;
LSBRAC	: '[' ;
RSBRAC	: ']' ;
QMARK	: '?' ;
PLUS	: '+' ;
MINUS	: '-' ;
VBAR	: '|' ;
UPARR	: '^' ;
AMP		: '&' ;
LTOP	: '<' ;
LTEOP	: '<=' ;
GTOP	: '>' ;
GTEOP	: '>=' ;
LSHOP	: '<<' ;
RSHOP	: '>>' ;
MULOP	: '*' ;
DOT		: '.' ;
HMARK	: '#' ;
LBRAC	: '{' ;
RBRAC	: '}' ;
EXCLM	: '!' ;

DECNUM	: Digit Digit*;

REALNUM	: [0-9]* '.' [0-9]+ ExponentPart? 
		| [0-9]+ ExponentPart;

BLOCK_COMMENT
	:	SLASH SINGLE_QUOTE .*? SINGLE_QUOTE SLASH
	;

LINE_COMMENT
	:	(REM | SINGLE_QUOTE) InputCharacter*
	;

NOTEQ: '<>';

P_DEFINE: HMARK D E F I N E;
P_ELSE: HMARK E L S E;
P_ELSEIF: HMARK E L S E I F;
P_ENDIF: HMARK E N D I F;
P_IF: HMARK I F;
P_IFDEF: HMARK I F D E F;
P_IFNDEF: HMARK I F N D E F;
P_INCLIB: HMARK I N C L I B;
P_INCLUDE: HMARK I N C L U D E;
P_LINE: HMARK L I N E;
P_PRAGMA: HMARK P R A G M A;
P_REQUIRE: HMARK R E Q U I R E;
P_UNDEF: HMARK U N D E F;
P_DINCLUDE: DOLLAR I N C L U D E;

END_ASM: E N D WS* A S M;

ABS: A B S;
ACS: A C S;
AND: A N D;
ALIGN: A L I G N;
ASC: A S C;
ASM: A S M;
ASN: A S N;
AS: A S;
AT: A T;
ATN: A T N;
ATTR: A T T R;
BAND: B A N D;
BNOT: B N O T;
BOR: B O R;
BXOR: B X O R;
BEEP: B E E P;
BOLD: B O L D;
BORDER: B O R D E R;
BRIGHT: B R I G H T;
BYREF: B Y R E F;
BYTE: B Y T E;
BYVAL: B Y V A L;
CAST: C A S T;
CLEAR: C L E A R;
CHR: C H R DOLLAR?;
CIRCLE: C I R C L E;
CLS: C L S;
CODE: C O D E;
CONST: C O N S T;
CONTINUE: C O N T I N U E;
COS: C O S;
CSRLIN: C S R L I N;
DECLARE: D E C L A R E;
DIM: D I M;
DO: D O;
DATA: D A T A;
DRAW: D R A W;
ELSE: E L S E;
ELSEIF: E L S E I F;
END: E N D;
EXIT: E X I T;
EXP: E X P;
FASTCALL: F A S T C A L L;
FIXED: F I X E D;
FLASH: F L A S H;
FLOAT: F L O A T;
FOR: F O R;
FUNCTION: F U N C T I O N;
GETKEY: G E T K E Y;
GETKEYSCANCODE: G E T K E Y S C A N C O D E;
GO: G O;
GOTO: G O T O | GO WS* TO;
GOSUB: G O S U B | GO WS* SUB;
HEX: H E X;
HEX16: H E X '16';
IF: I F;
IN: I N;
INK: I N K;
INKEY: I N K E Y DOLLAR?;
INPUT: I N P U T;
INT: I N T;
INTEGER: I N T E G E R;
INVERSE: I N V E R S E;
ITALIC: I T A L I C;
LBOUND: L B O U N D;
LCASE: L C A S E;
LET: L E T;
LEN: L E N;
LN: L N;
LOAD: L O A D;
LONG: L O N G;
LOOP: L O O P;
MOD: M O D;
MULTIKEYS: M U L T I K E Y S;
NEXT: N E X T;
NOT: N O T;
OR: O R;
OVER: O V E R;
OUT: O U T;
PAPER: P A P E R;
PAUSE: P A U S E;
PEEK: P E E K;
PI: P I;
PLOT: P L O T;
POINT: P O I N T;
POKE: P O K E;
POS: P O S;
PRINT: P R I N T;
PRINT42: P R I N T '42';
PRINTAT42: P R I N T A T '42';
PRINT64: P R I N T '64';
PRINTAT64: P R I N T A T '64';
RANDOMIZE: R A N D O M I Z E;
READ: R E A D;
REM: R E M;
RESTORE: R E S T O R E;
RETURN: R E T U R N;
RND: R N D;
SAVE: S A V E;
SCREEN: S C R E E N;
SGN: S G N;
SHL: S H L | LSHOP;
SHR: S H R | RSHOP;
SIN: S I N;
SQR: S Q R;
STDCALL: S T D C A L L;
STEP: S T E P;
STOP: S T O P;
STR: S T R DOLLAR?;
STRING: S T R I N G;
SUB: S U B;
TAB: T A B;
TAN: T A N;
THEN: T H E N;
TO: T O;
UBOUND: U B O U N D;
UBYTE: U B Y T E;
UCASE: U C A S E;
UINTEGER: U I N T E G E R;
ULONG: U L O N G;
UNTIL: U N T I L;
VAL: V A L;
VERIFY: V E R I F Y;
WEND: W E N D;
WHILE: W H I L E;
XOR: X O R;

HEXNUM	: '$' HexDigit HexDigit? HexDigit? HexDigit?
		| Digit HexDigit? HexDigit? HexDigit? HexDigit? ('H' | 'h') ;

BINNUM	: '%' BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		| BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit? ('B' | 'b') ;

IDENTIFIER
	:	IDSTART IDCONT*
	;
IDSTART
	: 'A'..'Z' | 'a'..'z' | '_'
	;
IDCONT
	:	'0'..'9' | 'A'..'Z' | 'a'..'z' | '_' | '$'
	;

ZXB_STRING
	:	'"' (~["\\\r\n\u0085\u2028\u2029])* '"'
	;
ZXB_FSTRING
	:	'<' (~["\\\r\n\u0085\u2028\u2029])* '>'
	;

LINE_END
	: UNDERSCORE? NewLine
	;

ErrorCharacter
    :   .
    ;

// ============================================================================
// Fragments
fragment InputCharacter
	:	~[\r\n\u0085\u2028\u2029]
	;

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
	| '\\'
	;

fragment HexEscapeSequence
	: '\\x' HexDigit
	| '\\x' HexDigit HexDigit
	;

fragment A : [aA];
fragment B : [bB];
fragment C : [cC];
fragment D : [dD];
fragment E : [eE];
fragment F : [fF];
fragment G : [gG];
fragment H : [hH];
fragment I : [iI];
fragment J : [jJ];
fragment K : [kK];
fragment L : [lL];
fragment M : [mM];
fragment N : [nN];
fragment O : [oO];
fragment P : [pP];
fragment Q : [qQ];
fragment R : [rR];
fragment S : [sS];
fragment T : [tT];
fragment U : [uU];
fragment V : [vV];
fragment W : [wW];
fragment X : [xX];
fragment Y : [yY];
fragment Z : [zZ];

fragment HexDigit 
	: [0-9] 
	| [A-F] 
	| [a-f]
	;

fragment OctDigit
	: [0-7]
	;

fragment Digit 
	: OctDigit | '8' | '9'
	;

fragment BinDigit
	: ('0'|'1') '_'?
	;

fragment ExponentPart
	: [eE] ('+' | '-')? [0-9]+
	;

fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;
