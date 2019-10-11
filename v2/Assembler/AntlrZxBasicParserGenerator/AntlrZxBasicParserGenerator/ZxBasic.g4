grammar ZxBasic;

// ============================================================================
// Common parser rules for ZX BASIC and embedded Z80 ASM
compileUnit
	:	(asm_section | label? line)* EOF
	;

// ============================================================================
// ZX BASIC specific parser rules
label
	:	DECNUM 
	|	IDENTIFIER COLON
	;

line
	: line_item* LINE_END
	| line_item+
	;

line_item
	:	keyword 
	|	function
	|	operator
	|	special
	|	number
	|	identifier
	|	string
	|	comment
	|	ErrorCharacter
	;

asm_section
	:	ASM_SECTION
	;

// --- ZX BASIC keywords
keyword
	:	AT
	|	BEEP
	|	BOLD
	|	BORDER
	|	BRIGHT
	|	BYREF
	|	BYVAL
	|	CIRCLE
	|	CLS
	|	CONST
	|	CONTINUE
	|	DECLARE
	|	DIM
	|	DO
	|	DATA
	|	DRAW
	|	ELSE
	|	ELSEIF
	|	END
	|	EXIT
	|	FASTCALL
	|	FLASH
	|	FOR
	|	FUNCTION
	|	GOTO
	|	GOSUB
	|	IF
	|	INK
	|	INVERSE
	|	ITALIC
	|	LET
	|	LOAD
	|	LOOP
	|	NEXT
	|	OVER
	|	OUT
	|	PAPER
	|	PAUSE
	|	PI
	|	PLOT
	|	POKE
	|	PRINT
	|	RANDOMIZE
	|	READ
	|	REM
	|	RESTORE
	|	RETURN
	|	SAVE
	|	STDCALL
	|	STEP
	|	STOP
	|	SUB
	|	THEN
	|	TO
	|	UNTIL
	|	VERIFY
	|	WEND
	|	WHILE
	;

// --- ZX BASIC functions
function
	:	ABS
	|	ACS
	|	ASC
	|	ASN
	|	ATN
	|	ATTR
	|	CAST
	|	CHR
	|	CODE
	|	COS
	|	CSRLIN
	|	EXP
	|	GETKEY
	|	GETKEYSCANCODE
	|	HEX
	|	HEX16
	|	IN
	|	INKEY
	|	INPUT
	|	INT
	|	LBOUND
	|	LCASE
	|	LEN
	|	LN
	|	MULTIKEYS
	|	PEEK
	|	POINT
	|	POS
	|	PRINT42
	|	PRINTAT42
	|	PRINT64
	|	PRINTAT64
	|	RND
	|	SCREEN
	|	SGN
	|	STR
	|	TAN
	|	UBOUND
	|	UCASE
	|	VAL
	;

// --- ZX BASIC operators
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
	|	SIN
	|	SQR
	|	XOR
	|	ASSIGN
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
	|	DIVOP
	|	LSHOP
	|	RSHOP
	;

// --- ZX BASIC special keywords
special
	:	ALIGN
	;
	
// --- ZX BASIC numbers
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
	:	STRING
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

// ============================================================================
// Common lexer rules for ZX BASIC and embedded Z80 ASM
WS
	:	('\u0020' | '\t')+ -> channel(HIDDEN)
	;
ASM_SECTION
	:	A S M WS* NewLine+ (InputCharacter* NewLine+)* NewLine* A S M WS* E N D
	;

COLON: ':' ;
UNDERSCORE: '_' ;
SINGLE_QUOTE: '\'' ;
SLASH: '/' ;
DOLLAR: '$' ;
SCOLON	: ';' ;
COMSEP	: '//' ;
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
EQOP	: '==' ;
CIEQOP	: '===' ;
NEQOP	: '!=' ;
CINEQOP	: '!==' ;
LTOP	: '<' ;
LTEOP	: '<=' ;
GTOP	: '>' ;
GTEOP	: '>=' ;
LSHOP	: '<<' ;
RSHOP	: '>>' ;
MULOP	: '*' ;
DIVOP	: '/' ;
DOT		: '.' ;

// --- Z80 Asm and ZX BASIC decimal number
DECNUM	: Digit Digit? Digit? Digit? Digit?;

// --- Z80 Asm and ZX BASIC real number
REALNUM	: [0-9]* '.' [0-9]+ ExponentPart? 
		| [0-9]+ ExponentPart;

// ============================================================================
// ZX BASIC specifix lexer rules

// --- ZXB Comments
BLOCK_COMMENT
	:	SLASH SINGLE_QUOTE .*? SINGLE_QUOTE SLASH
	;

LINE_COMMENT
	:	(REM | SINGLE_QUOTE) InputCharacter*
	;

NOTEQ: '<>';

// --- ZX BASIC keywords
ABS: A B S;
ACS: A C S;
AND: A N D;
ALIGN: A L I G N;
ASC: A S C;
ASN: A S N;
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
BYVAL: B Y V A L;
CAST: C A S T;
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
FLASH: F L A S H;
FOR: F O R;
FUNCTION: F U N C T I O N;
GETKEY: G E T K E Y;
GETKEYSCANCODE: G E T K E Y S C A N C O D E;
GO: G O;
GOTO: G O T O | GO TO;
GOSUB: G O S U B | GO SUB;
HEX: H E X;
HEX16: H E X '16';
IF: I F;
IN: I N;
INK: I N K;
INKEY: I N K E Y DOLLAR?;
INPUT: I N P U T;
INT: I N T;
INVERSE: I N V E R S E;
ITALIC: I T A L I C;
LBOUND: L B O U N D;
LCASE: L C A S E;
LET: L E T;
LEN: L E N;
LN: L N;
LOAD: L O A D;
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
SUB: S U B;
TAN: T A N;
THEN: T H E N;
TO: T O;
UBOUND: U B O U N D;
UCASE: U C A S E;
UNTIL: U N T I L;
VAL: V A L;
VERIFY: V E R I F Y;
WEND: W E N D;
WHILE: W H I L E;
XOR: X O R;

// --- ZX BASIC hexadecimal number
HEXNUM	: '$' HexDigit HexDigit? HexDigit? HexDigit?
		| Digit HexDigit? HexDigit? HexDigit? HexDigit? ('H' | 'h') ;

// --- ZX BASIC binary number
BINNUM	: '%' BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		| BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit? ('B' | 'b') ;

// --- ZX BASIC identifier
IDENTIFIER
	:	IDSTART IDCONT*
	;
IDSTART
	: 'A'..'Z' | 'a'..'z'
	;
IDCONT
	:	'0'..'9' | 'A'..'Z' | 'a'..'z'
	;

// --- ZX BASIC String
STRING
	:	'"' (~["\\\r\n\u0085\u2028\u2029])* '"'
	;

// --- This is how a ZX BASIC line can end
LINE_END
	: UNDERSCORE? NewLine
	;

// --- Any invalid character should be converted into an ErrorCharacter token.
ErrorCharacter
    :   .
    ;

// ============================================================================
// ZX BASIC and Z80 fragments
// --- Any parsed input character
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

// --- Reserved words are case-insensitive
fragment A : [aA]; // match either an 'a' or 'A'
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

// --- Digits for different radixes
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
