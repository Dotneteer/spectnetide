grammar ZxBasic;

// ============================================================================
// Common parser rules for ZX BASIC and embedded Z80 ASM
compileUnit
	:	(zxb_label? zxb_line | zxb_asm_section)* EOF
	;

// ============================================================================
// ZX BASIC specific parser rules
zxb_label
	:	DECNUM 
	|	ZXB_IDENTIFIER COLON
	;

zxb_line
	: zxb_line_item* ZXB_LINE_END
	| zxb_line_item+
	;

zxb_line_item
	:	zxb_keyword 
	|	zxb_function
	|	zxb_operator
	|	zxb_special
	|	zxb_number
	|	zxb_identifier
	|	zxb_string
	|	zxb_comment
	|	ErrorCharacter
	;

zxb_asm_section
	:	zxb_asm_start NEWLINE asm_section NEWLINE+ zxb_asm_end
	;

zxb_asm_start
	:	ZXB_ASM
	;

zxb_asm_end
	:	ZXB_ASM ZXB_END
	;

// --- ZX BASIC keywords
zxb_keyword
	:	ZXB_AT
	|	ZXB_BEEP
	|	ZXB_BOLD
	|	ZXB_BORDER
	|	ZXB_BRIGHT
	|	ZXB_BYREF
	|	ZXB_BYVAL
	|	ZXB_CIRCLE
	|	ZXB_CLS
	|	ZXB_CONST
	|	ZXB_CONTINUE
	|	ZXB_DECLARE
	|	ZXB_DIM
	|	ZXB_DO
	|	ZXB_DATA
	|	ZXB_DRAW
	|	ZXB_ELSE
	|	ZXB_ELSEIF
	|	ZXB_END
	|	ZXB_EXIT
	|	ZXB_FASTCALL
	|	ZXB_FLASH
	|	ZXB_FOR
	|	ZXB_FUNCTION
	|	ZXB_GOTO
	|	ZXB_GOSUB
	|	ZXB_IF
	|	ZXB_INK
	|	ZXB_INVERSE
	|	ZXB_ITALIC
	|	ZXB_LET
	|	ZXB_LOAD
	|	ZXB_LOOP
	|	ZXB_NEXT
	|	ZXB_OVER
	|	ZXB_OUT
	|	ZXB_PAPER
	|	ZXB_PAUSE
	|	ZXB_PI
	|	ZXB_PLOT
	|	ZXB_POKE
	|	ZXB_PRINT
	|	ZXB_RANDOMIZE
	|	ZXB_READ
	|	ZXB_REM
	|	ZXB_RESTORE
	|	ZXB_RETURN
	|	ZXB_SAVE
	|	ZXB_STDCALL
	|	ZXB_STEP
	|	ZXB_STOP
	|	ZXB_SUB
	|	ZXB_THEN
	|	ZXB_TO
	|	ZXB_UNTIL
	|	ZXB_VERIFY
	|	ZXB_WEND
	|	ZXB_WHILE
	;

// --- ZX BASIC functions
zxb_function
	:	ZXB_ABS
	|	ZXB_ACS
	|	ZXB_ASC
	|	ZXB_ASN
	|	ZXB_ATN
	|	ZXB_ATTR
	|	ZXB_CAST
	|	ZXB_CHR
	|	ZXB_CODE
	|	ZXB_COS
	|	ZXB_CSRLIN
	|	ZXB_EXP
	|	ZXB_GETKEY
	|	ZXB_GETKEYSCANCODE
	|	ZXB_HEX
	|	ZXB_HEX16
	|	ZXB_IN
	|	ZXB_INKEY
	|	ZXB_INPUT
	|	ZXB_INT
	|	ZXB_LBOUND
	|	ZXB_LCASE
	|	ZXB_LEN
	|	ZXB_LN
	|	ZXB_MULTIKEYS
	|	ZXB_PEEK
	|	ZXB_POINT
	|	ZXB_POS
	|	ZXB_PRINT42
	|	ZXB_PRINTAT42
	|	ZXB_PRINT64
	|	ZXB_PRINTAT64
	|	ZXB_RND
	|	ZXB_SCREEN
	|	ZXB_SGN
	|	ZXB_STR
	|	ZXB_TAN
	|	ZXB_UBOUND
	|	ZXB_UCASE
	|	ZXB_VAL
	;

// --- ZX BASIC operators
zxb_operator
	:	ZXB_AND
	|	ZXB_BAND
	|	ZXB_BNOT
	|	ZXB_BOR
	|	ZXB_BXOR
	|	ZXB_MOD
	|	ZXB_NOT
	|	ZXB_OR
	|	ZXB_SHL
	|	ZXB_SHR
	|	ZXB_SIN
	|	ZXB_SQR
	|	ZXB_XOR
	|	ASSIGN
	|	PLUS
	|	MINUS
	|	UPARR
	|	AMP
	|	ZXB_NOTEQ
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
zxb_special
	:	ZXB_ALIGN
	|	ZXB_ASM
	;
	
// --- ZX BASIC numbers
zxb_number
	:	ZXB_BINNUM
	|	DECNUM
	|	ZXB_HEXNUM
	|	REALNUM
	;

zxb_identifier
	:	ZXB_IDENTIFIER
	;

zxb_string
	:	ZXB_STRING
	;

zxb_comment
	:	zxb_block_comment
	|	zxb_line_comment
	;

zxb_block_comment
	:	ZXB_BLOCK_COMMENT
	;

zxb_line_comment
	:	ZXB_LINE_COMMENT
	;

// ============================================================================
// Z80 ASSEMBLER specific parser rules
asm_section
	:	NEWLINE* asmline (NEWLINE+ asmline)*
	;

asmline
	: DECNUM+ ;

// ============================================================================
// Common lexer rules for ZX BASIC and embedded Z80 ASM
WS
	:	('\u0020' | '\t')+ -> channel(HIDDEN)
	;
// --- Special characters
NEWLINE: ('\r'? '\n' | '\r')+ ;
COLON: ':' ;
UNDERSCORE: '_' ;
SINGLE_QUOTE: '\'' ;
SLASH: '/' ;
DOLLAR: '$' ;
DCOLON	: '::' ;
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
MODOP	: '%' ;
MINOP	: '<?' ;
MAXOP	: '>?' ;
TILDE	: '~';
LDBRAC	: '{{' ;
RDBRAC	: '}}' ;
EXCLM	: '!' ;
DOT		: '.' ;
GOESTO	: '->' ;
ZXB_NOTEQ: '<>';

// ============================================================================
// ZX BASIC specifix lexer rules

// --- ZXB Comments
ZXB_BLOCK_COMMENT
	:	SLASH SINGLE_QUOTE .*? SINGLE_QUOTE SLASH
	;

ZXB_LINE_COMMENT
	:	(ZXB_REM | SINGLE_QUOTE) InputCharacter*
	;

// --- ZX BASIC keywords
ZXB_ABS: A B S;
ZXB_ACS: A C S;
ZXB_AND: A N D;
ZXB_ALIGN: A L I G N;
ZXB_ASC: A S C;
ZXB_ASM: A S M;
ZXB_ASN: A S N;
ZXB_AT: A T;
ZXB_ATN: A T N;
ZXB_ATTR: A T T R;
ZXB_BAND: B A N D;
ZXB_BNOT: B N O T;
ZXB_BOR: B O R;
ZXB_BXOR: B X O R;
ZXB_BEEP: B E E P;
ZXB_BOLD: B O L D;
ZXB_BORDER: B O R D E R;
ZXB_BRIGHT: B R I G H T;
ZXB_BYREF: B Y R E F;
ZXB_BYVAL: B Y V A L;
ZXB_CAST: C A S T;
ZXB_CHR: C H R DOLLAR?;
ZXB_CIRCLE: C I R C L E;
ZXB_CLS: C L S;
ZXB_CODE: C O D E;
ZXB_CONST: C O N S T;
ZXB_CONTINUE: C O N T I N U E;
ZXB_COS: C O S;
ZXB_CSRLIN: C S R L I N;
ZXB_DECLARE: D E C L A R E;
ZXB_DIM: D I M;
ZXB_DO: D O;
ZXB_DATA: D A T A;
ZXB_DRAW: D R A W;
ZXB_ELSE: E L S E;
ZXB_ELSEIF: E L S E I F;
ZXB_END: E N D;
ZXB_EXIT: E X I T;
ZXB_EXP: E X P;
ZXB_FASTCALL: F A S T C A L L;
ZXB_FLASH: F L A S H;
ZXB_FOR: F O R;
ZXB_FUNCTION: F U N C T I O N;
ZXB_GETKEY: G E T K E Y;
ZXB_GETKEYSCANCODE: G E T K E Y S C A N C O D E;
ZXB_GO: G O;
ZXB_GOTO: G O T O | ZXB_GO ZXB_TO;
ZXB_GOSUB: G O S U B | ZXB_GO ZXB_SUB;
ZXB_HEX: H E X;
ZXB_HEX16: H E X '16';
ZXB_IF: I F;
ZXB_IN: I N;
ZXB_INK: I N K;
ZXB_INKEY: I N K E Y DOLLAR?;
ZXB_INPUT: I N P U T;
ZXB_INT: I N T;
ZXB_INVERSE: I N V E R S E;
ZXB_ITALIC: I T A L I C;
ZXB_LBOUND: L B O U N D;
ZXB_LCASE: L C A S E;
ZXB_LET: L E T;
ZXB_LEN: L E N;
ZXB_LN: L N;
ZXB_LOAD: L O A D;
ZXB_LOOP: L O O P;
ZXB_MOD: M O D;
ZXB_MULTIKEYS: M U L T I K E Y S;
ZXB_NEXT: N E X T;
ZXB_NOT: N O T;
ZXB_OR: O R;
ZXB_OVER: O V E R;
ZXB_OUT: O U T;
ZXB_PAPER: P A P E R;
ZXB_PAUSE: P A U S E;
ZXB_PEEK: P E E K;
ZXB_PI: P I;
ZXB_PLOT: P L O T;
ZXB_POINT: P O I N T;
ZXB_POKE: P O K E;
ZXB_POS: P O S;
ZXB_PRINT: P R I N T;
ZXB_PRINT42: P R I N T '42';
ZXB_PRINTAT42: P R I N T A T '42';
ZXB_PRINT64: P R I N T '64';
ZXB_PRINTAT64: P R I N T A T '64';
ZXB_RANDOMIZE: R A N D O M I Z E;
ZXB_READ: R E A D;
ZXB_REM: R E M;
ZXB_RESTORE: R E S T O R E;
ZXB_RETURN: R E T U R N;
ZXB_RND: R N D;
ZXB_SAVE: S A V E;
ZXB_SCREEN: S C R E E N;
ZXB_SGN: S G N;
ZXB_SHL: S H L | LSHOP;
ZXB_SHR: S H R | RSHOP;
ZXB_SIN: S I N;
ZXB_SQR: S Q R;
ZXB_STDCALL: S T D C A L L;
ZXB_STEP: S T E P;
ZXB_STOP: S T O P;
ZXB_STR: S T R DOLLAR?;
ZXB_SUB: S U B;
ZXB_TAN: T A N;
ZXB_THEN: T H E N;
ZXB_TO: T O;
ZXB_UBOUND: U B O U N D;
ZXB_UCASE: U C A S E;
ZXB_UNTIL: U N T I L;
ZXB_VAL: V A L;
ZXB_VERIFY: V E R I F Y;
ZXB_WEND: W E N D;
ZXB_WHILE: W H I L E;
ZXB_XOR: X O R;

// --- Z80 Asm and ZX BASIC decimal number
DECNUM	: Digit Digit? Digit? Digit? Digit?;

// --- Z80 Asm and ZX BASIC real number
REALNUM	: [0-9]* '.' [0-9]+ ExponentPart? 
		| [0-9]+ ExponentPart;

// --- ZX BASIC hexadecimal number
ZXB_HEXNUM	: '$' HexDigit HexDigit? HexDigit? HexDigit?
		| Digit HexDigit? HexDigit? HexDigit? HexDigit? ('H' | 'h') ;

// --- ZX BASIC binary number
ZXB_BINNUM	: '%' BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		| BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit? ('B' | 'b') ;

// --- ZX BASIC identifier
ZXB_IDENTIFIER
	:	ZXB_IDSTART ZXB_IDCONT*
	;
ZXB_IDSTART
	: 'A'..'Z' | 'a'..'z'
	;
ZXB_IDCONT
	:	'0'..'9' | 'A'..'Z' | 'a'..'z'
	;

// --- ZX BASIC String
ZXB_STRING
	:	'"' (~["\\\r\n\u0085\u2028\u2029])* '"'
	;

// --- This is how a ZX BASIC line can end
ZXB_LINE_END
	: UNDERSCORE? NewLine
	;

// --- Basic literals
HEXNUM	: ('#'|'0x'|'$') HexDigit HexDigit? HexDigit? HexDigit?
		| Digit HexDigit? HexDigit? HexDigit? HexDigit? ('H' | 'h') ;

BINNUM	: ('%'|'0b') '_'? BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		| '_'? BinDigit BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit?
		  BinDigit? BinDigit? BinDigit? BinDigit? ('B' | 'b')
		;

OCTNUM	: OctDigit OctDigit? OctDigit?
		  OctDigit? OctDigit? OctDigit? ('q' | 'Q' | 'o' | 'O' )
		;

CURADDR	: '$' ;

CHAR	: '\'' (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter) '\'' ;
STRING	: '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"' ;
FSTRING	: '<'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '>' ;

// --- Boolean literals;
BOOLLIT	: TRUE | FALSE ;
TRUE	: 'true' | '.true' | 'TRUE' | '.TRUE' ;
FALSE	: 'false' | '.false' | 'FALSE' | '.FALSE' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | '@' | '`' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '@' | '!' | '?' | '#' | '0'..'9' | 'A'..'Z' | 'a'..'z' | '.' ;

CURCNT	: '$cnt' | '$CNT' | '.cnt' | '.CNT' ;
NONEARG	: '$<none>$' ;

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
