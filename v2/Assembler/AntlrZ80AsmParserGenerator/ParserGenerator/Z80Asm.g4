grammar Z80Asm;

options {
    superClass=Z80AsmBaseParser;
}

/*
 * Parser Rules
 */

compileUnit	
	:	EOF
	|	NEWLINE* asmline (NEWLINE+ asmline)* NEWLINE* EOF
	;

asmline
	:	label? lineBody? comment?
	|	directive comment?
	;

lineBody
	:	pragma 
	|	operation 
	|	macroParam 
	|	macroOrStructInvocation 
	|	statement
	|	fieldAssignment
	;

label
	:	IDENTIFIER COLON
	|	IDENTIFIER{!this.exprStart()}?
	;

comment
	:	COMMENT
	;

pragma
	:	orgPragma
	|	bankPragma
	|	xorgPragma
	|	entPragma
	|	xentPragma
	|	dispPragma
	|	equPragma
	|	varPragma
	|	defbPragma
	|	defwPragma
	|	defmPragma
	|	defcPragma
	|	defnPragma
	|	defhPragma
	|	skipPragma
	|	externPragma
	|	defsPragma
	|	fillbPragma
	|	fillwPragma
	|	modelPragma
	|	alignPragma
	|	tracePragma
	|	rndSeedPragma
	|   defgxPragma
	|	defgPragma
	|   errorPragma
	|	incBinPragma
	|	compareBinPragma
	|	zxBasicPragma
	|   injectOptPragma
	;

directive
	:	(IFDEF|IFNDEF|DEFINE|UNDEF|IFMOD|IFNMOD) IDENTIFIER
	|	ENDIF
	|	ELSE
	|	IF expr
	|	INCLUDE (STRING)
	|	LINEDIR expr (COMMA? STRING)?
	;	

statement
	:	iterationTest
	|	macroStatement
	|	macroEndMarker
	|	loopEndMarker
	|	whileEndMarker
	|	procStatement
	|	procEndMarker
	|	repeatStatement
	|	ifStatement
	|	elseStatement
	|	endifStatement
	|	forStatement
	|	nextStatement
	|	breakStatement
	|	continueStatement
	|	moduleStatement
	|	moduleEndMarker
	|	structStatement
	|	structEndMarker
	|	localStatement
	;

iterationTest: (LOOP | WHILE | UNTIL | ELIF | IDENTIFIER
	{this.p("loop", "while", "until", "elif")}?
	) expr;
macroStatement: MACRO LPAR (IDENTIFIER (COMMA IDENTIFIER)*)? RPAR	;
macroEndMarker: ENDMACRO ;
procStatement: PROC;
procEndMarker: ENDPROC ;
loopEndMarker: ENDLOOP ;
repeatStatement: REPEAT ;
whileEndMarker: ENDWHILE ;
ifStatement: IFSTMT expr | IFUSED symbol | IFNUSED symbol ;
elseStatement: ELSESTMT ;
endifStatement: ENDIFSTMT ;
forStatement: FOR IDENTIFIER ASSIGN expr TO expr (STEP expr)? ;
nextStatement: (NEXT | FORNEXT) ;
breakStatement: BREAK ;
continueStatement: CONTINUE ;
moduleStatement: MODULE IDENTIFIER? ;
moduleEndMarker: ENDMOD ;
structStatement: STRUCT ;
structEndMarker: ENDST ;
localStatement: LOCAL IDENTIFIER (COMMA IDENTIFIER)* ;

macroOrStructInvocation: IDENTIFIER LPAR macroArgument (COMMA macroArgument)* RPAR	;
macroArgument: operand? ;

fieldAssignment: GOESTO byteEmPragma ;

orgPragma	: ORGPRAG expr ;
bankPragma	: BANKPRAG expr (COMMA expr)?;
xorgPragma	: XORGPR expr ;
entPragma	: ENTPRAG expr ;
xentPragma	: XENTPRAG expr ;
dispPragma	: DISPRAG expr ;
equPragma	: EQUPRAG expr ;
varPragma	: (VARPRAG | ASSIGN) expr ;
defbPragma	: DBPRAG expr (COMMA expr)* ;
defwPragma	: DWPRAG expr (COMMA expr)* ;
defcPragma	: DCPRAG expr ;
defmPragma	: DMPRAG expr ;
defnPragma	: DNPRAG expr ;
defhPragma	: DHPRAG expr ;
skipPragma	: SKIPRAG expr (COMMA expr)?;
externPragma: EXTPRAG ;
defsPragma	: DSPRAG expr (COMMA expr)?;
fillbPragma	: FBPRAG expr COMMA expr ;
fillwPragma : FWPRAG expr COMMA expr ;
modelPragma : MODPRAG (IDENTIFIER | NEXT) ;
alignPragma : ALGPRAG expr? ;
tracePragma : (TRACE | TRACEHEX) expr ( ',' expr)* ;
rndSeedPragma: RNDSEED expr? ;
defgxPragma	: DGXPRAG expr ;
defgPragma	: DGPRAG ;
errorPragma : ERRORPR expr ;
incBinPragma: INCBIN expr ( ',' expr ( ',' expr)? )? ;
compareBinPragma: COMPAREBIN expr ( ',' expr ( ',' expr)? )? ;
zxBasicPragma: ZXBPRAG ;
injectOptPragma: IOPTPRAG IDENTIFIER;

byteEmPragma
	:  defbPragma 
	| defwPragma 
	| defcPragma 
	| defmPragma
	| defnPragma
	| defhPragma
    | defsPragma
	| fillbPragma
	| fillwPragma
	| defgxPragma
    | defgPragma
	;


operation
	:	trivialOperation
	|	compoundOperation
	|	trivialNextOperation
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
	:	LD operand COMMA operand
	|	INC operand
	|	DEC operand
	|	EX operand COMMA operand
	|	ADD operand COMMA operand
	|	ADC operand COMMA operand
	|	SUB (operand COMMA)? operand
	|	SBC operand COMMA operand
	|	AND (operand COMMA)? operand
	|	XOR (operand COMMA)? operand
	|	OR (operand COMMA)? operand
	|	CP (operand COMMA)? operand
	|	DJNZ operand
	|	JR (operand COMMA)? operand
	|	JP (operand COMMA)? operand
	|	CALL (operand COMMA)? operand
	|	RET operand?
	|	RST operand
	|	PUSH operand
	|	POP operand
	|	IN (operand COMMA)? operand
	|	OUT (operand COMMA)? operand
	|	IM operand
	|	RLC (operand COMMA)? operand
	|	RRC (operand COMMA)? operand
	|	RL (operand COMMA)? operand
	|	RR (operand COMMA)? operand
	|	SLA (operand COMMA)? operand
	|	SRA (operand COMMA)? operand
	|	SLL (operand COMMA)? operand
	|	SRL (operand COMMA)? operand
	|	BIT operand COMMA operand
	|	RES operand COMMA (operand COMMA)? operand
	|	SET operand COMMA (operand COMMA)? operand
	// --- Next operation
	|	MIRROR operand
	|	TEST operand
	|	NEXTREG operand COMMA operand
	;

trivialNextOperation
	:	SWAPNIB
	|	MUL
	|	OUTINB
	|	LDIX
	|	LDIRX
	|	LDDX
	|	LDDRX
	|	PIXELDN
	|	PIXELAD
	|	SETAE
	|	LDPIRX
	|	LDIRSCALE
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
	|	condition
	|	(HREG | LREG) LPAR (reg16Std | macroParam | NONEARG) RPAR
	|	NONEARG
	;

reg8: A | B | C | D | E | H | L ;
reg8Idx: XL | XH | YL | YH ;
reg8Spec: I | R ;
reg16: BC | DE | HL | SP ;
reg16Idx: IX | IY ;
reg16Std: BC | DE | HL | IX | IY ;
reg16Spec: AF | AF_ ;
regIndirect: LPAR (reg16) RPAR ;
cPort: LPAR C RPAR ;
memIndirect: LPAR expr RPAR ;
indexedAddr: LPAR reg16Idx ((PLUS | MINUS) expr)? RPAR ;
condition: Z | NZ |	C | NC | PO | PE | P | M ;

// --- Expressions
expr
	: builtinFunctionInvocation                            #BuiltInFunctionExpr
	| functionInvocation                                   #FunctionInvocationExpr
	| macroParam                                           #MacroParamExpr
	| PLUS expr                                            #UnaryPlusExpr
	| MINUS expr                                           #UnaryMinusExpr
	| TILDE expr                                           #BinaryNotExpr
	| EXCLM expr                                           #LogicalNotExpr
	| LSBRAC expr RSBRAC                                   #BracketedExpr
	| LPAR expr RPAR                                       #ParenthesizedExpr
	| literal                                              #LiteralExpr
	| symbol                                               #SymbolExpr
	| expr op=(MINOP | MAXOP) expr                         #MinMaxExpr
	| expr op=(MULOP | DIVOP | MODOP | MINOP | MAXOP) expr #MultExpr
	| expr op=(PLUS | MINUS ) expr                         #AddExpr
	| expr op=(LSHOP | RSHOP) expr                         #ShiftExpr
	| expr op=(LTOP | LTEOP | GTOP | GTEOP) expr           #RelExpr
	| expr op=(EQOP | NEQOP | CIEQOP | CINEQOP) expr       #EquExpr
	| expr AMP expr                                        #AndExpr
	| expr UPARR expr                                      #XorExpr
	| expr VBAR expr                                       #OrExpr
	| expr QMARK expr COLON expr                           #TernaryExpr
	;

functionInvocation
	: IDENTIFIER LPAR RPAR
	| IDENTIFIER LPAR expr (COMMA expr)* RPAR
	;

builtinFunctionInvocation
	: (TEXTOF | LTEXTOF) LPAR 
		(mnemonic | regsAndConds | macroParam) RPAR        #TextOfInvoke
	| DEF LPAR operand? RPAR                               #DefInvoke
	| ISREG8 LPAR operand? RPAR                            #IsReg8Invoke
	| ISREG8STD LPAR operand? RPAR                         #IsReg8StdInvoke
	| ISREG8SPEC LPAR operand? RPAR                        #IsReg8StdSpecInvoke
	| ISREG8IDX LPAR operand? RPAR                         #IsReg8IdxInvoke
	| ISREG16 LPAR operand? RPAR                           #IsReg16Invoke
	| ISREG16STD LPAR operand? RPAR                        #IsReg16StdInvoke
	| ISREG16IDX LPAR operand? RPAR                        #IsReg16IdxInvoke
	| ISREGINDIRECT LPAR operand? RPAR                     #IsRegIndirectInvoke
	| ISCPORT LPAR operand? RPAR                           #IsCportInvoke
	| ISINDEXEDADDR LPAR operand? RPAR                     #IsIndexedAddrInvoke
	| ISCONDITION LPAR operand? RPAR                       #IsConditionInvoke
	| ISEXPR LPAR operand? RPAR                            #IsExprInvoke
	| ISREGA LPAR operand? RPAR                            #IsRegAInvoke
	| ISREGAF LPAR operand? RPAR                           #IsRegAfInvoke
	| ISREGB LPAR operand? RPAR                            #IsRegBInvoke
	| ISREGC LPAR operand? RPAR                            #IsRegCInvoke
	| ISREGBC LPAR operand? RPAR                           #IsRegBCInvoke
	| ISREGD LPAR operand? RPAR                            #IsRegDInvoke
	| ISREGE LPAR operand? RPAR                            #IsRegEInvoke
	| ISREGDE LPAR operand? RPAR                           #IsRegDEInvoke
	| ISREGH LPAR operand? RPAR                            #IsRegHInvoke
	| ISREGL LPAR operand? RPAR                            #IsRegLInvoke
	| ISREGHL LPAR operand? RPAR                           #IsRegHLInvoke
	| ISREGI LPAR operand? RPAR                            #IsRegIInvoke
	| ISREGR LPAR operand? RPAR                            #IsRegRInvoke
	| ISREGXH LPAR operand? RPAR                           #IsRegXHInvoke
	| ISREGXL LPAR operand? RPAR                           #IsRegXLInvoke
	| ISREGIX LPAR operand? RPAR                           #IsRegIXInvoke
	| ISREGYH LPAR operand? RPAR                           #IsRegYHInvoke
	| ISREGYL LPAR operand? RPAR                           #IsRegYLInvoke
	| ISREGIY LPAR operand? RPAR                           #IsRegIYInvoke
	| ISREGSP LPAR operand? RPAR                           #IsRegSPInvoke
	;

literal
	: HEXNUM                                               #HexLiteral
	| DECNUM                                               #DecimalLiteral
	| OCTNUM                                               #OctalLiteral
	| CHAR                                                 #CharLiteral
	| BINNUM                                               #BinLiteral
	| REALNUM                                              #RealLiteral
	| BOOLLIT                                              #BoolLiteral
	| STRING                                               #StringLiteral
	| (CURADDR | DOT | MULOP)                              #CurAddrLiteral
	| CURCNT                                               #CurCounterLiteral
	;

symbol
	: DCOLON? IDENTIFIER
	;

macroParam
	: LDBRAC IDENTIFIER RDBRAC
	;

regs
	: reg8
	| reg8Idx
	| reg8Spec
	| reg16
	| reg16Idx
	| reg16Spec
	;

regsAndConds
	: regs
	| regIndirect
	| cPort
	| condition
	;

mnemonic
	: NOP | RLCA | RRCA | RLA | RRA | DAA | CPL | SCF | CCF | HALT | RET
	| EXX | DI | EI | NEG | RETN | RETI | RLD | RRD | LDI | CPI | INI
	| OUTI | LDD | CPD | IND | OUTD | LDIR | CPIR | INIR | OTIR | LDDR
	| CPDR | INDR | OTDR | LD | INC | DEC | EX | ADD | ADC | SUB | SBC
	| AND | XOR | OR | CP | DJNZ | JR | JP | CALL | RST | PUSH | POP
	| IN | OUT | IM | RLC | RRC | RL | RR | SLA | SRA | SLL | SRL | BIT
	| RES | SET | SWAPNIB | MUL | POPX | MIRROR	| TEST | NEXTREG 
	| OUTINB | LDIX | LDIRX | LDDX | LDDRX | PIXELDN | PIXELAD | SETAE
	| LDPIRX
	;

/*
 * Lexer Rules
 */

WS
	:	('\u0020' | '\t')+ -> channel(HIDDEN)
	;

BLCOMMENT
	:	'/*'  ~('\r' | '\n')*? '*/' -> channel(HIDDEN)
	;

COMMENT
	:	(SCOLON | COMSEP) ~('\r' | '\n')*
	;

NEWLINE
	:	('\r'? '\n' | '\r')+
	;

// --- Common tokens
// --- Other tokens
COLON	: ':' ;
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

// --- Register and flag tokens
A	: 'a'|'A' ;
B	: 'b'|'B' ;
C	: 'c'|'C' ;
D	: 'd'|'D' ;
E	: 'e'|'E' ;
H	: 'h'|'H' ;
L	: 'l'|'L' ;
I	: 'i'|'I' ;
R	: 'r'|'R' ;
XL	: 'xl'|'XL'|'ixl'|'IXL'|'IXl' ;
XH	: 'xh'|'XH'|'ixh'|'IXH'|'IXh' ;
YL	: 'yl'|'YL'|'iyl'|'IYL'|'IYl' ;
YH	: 'yh'|'YH'|'iyh'|'IYH'|'IYh' ;
BC	: 'bc'|'BC' ;
DE	: 'de'|'DE' ;
HL	: 'hl'|'HL' ;
SP	: 'sp'|'SP' ;
IX	: 'ix'|'IX' ;
IY	: 'iy'|'IY' ;
AF	: 'af'|'AF' ;
AF_	: 'af\''|'AF\'' ;
Z	: 'z'|'Z' ;
NZ	: 'nz'|'NZ' ;
NC	: 'nc'|'NC' ;
PO	: 'po'|'PO' ;
PE	: 'pe'|'PE' ;
P	: 'p'|'P' ;
M	: 'm'|'M' ;

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

// --- Next instruction tokens
SWAPNIB	: 'swapnib'|'SWAPNIB' ;
MUL		: 'mul'|'MUL' ;
POPX	: 'popx'|'POPX' ;
MIRROR	: 'mirror'|'MIRROR' ;
TEST	: 'test'|'TEST' ;
NEXTREG	: 'nextreg'|'NEXTREG' ;
OUTINB	: 'outinb'|'OUTINB' ;
LDIX	: 'ldix'|'LDIX' ;
LDIRX	: 'ldirx'|'LDIRX' ;
LDDX	: 'lddx'|'LDDX' ;
LDDRX	: 'lddrx'|'LDDRX' ;
PIXELDN	: 'pixeldn'|'PIXELDN' ;
PIXELAD	: 'pixelad'|'PIXELAD' ;
SETAE	: 'setae'|'SETAE' ;
LDPIRX	: 'ldpirx'|'LDPIRX' ;
LDIRSCALE: 'ldirscale'|'LDIRSCALE' ;

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
LINEDIR	: '#line';

// --- Pragma tokens
ORGPRAG	: '.org' | '.ORG' | 'org' | 'ORG' ;
BANKPRAG: '.bank' | '.BANK' | 'bank' | 'BANK' ;
XORGPR	: '.xorg' | '.XORG' | 'xorg' | 'XORG' ;
ENTPRAG	: '.ent' | '.ENT' | 'ent' | 'ENT' ;
XENTPRAG: '.xent' | '.XENT' | 'xent' | 'XENT' ;
EQUPRAG	: '.equ' | '.EQU' | 'equ' | 'EQU' ;
VARPRAG	: '.var' | '.VAR' | 'var' | 'VAR' | ':=' ;
DISPRAG	: '.disp' | '.DISP' | 'disp' | 'DISP' ;
DBPRAG	: '.defb' | '.DEFB' | 'defb' | 'DEFB' | 'db' | '.db' | 'DB' | '.DB' ;
DWPRAG	: '.defw' | '.DEFW' | 'defw' | 'DEFW' | 'dw' | '.dw' | 'DW' | '.DW' ;
DMPRAG	: '.defm' | '.DEFM' | 'defm' | 'DEFM' | 'dm' | '.dm' | 'DM' | '.DM' ;
DNPRAG	: '.defn' | '.DEFN' | 'defn' | 'DEFN' | 'dn' | '.dn' | 'DN' | '.DN' ;
DHPRAG	: '.defh' | '.DEFH' | 'defh' | 'DEFH' | 'dh' | '.dh' | 'DH' | '.DH' ;
DGXPRAG	: '.defgx' | '.DEFGX' | 'defgx' | 'DEFGX' | 'dgx' | '.dgx' | 'DGX' | '.DGX' ;
DGPRAG	: ( '.defg' | '.DEFG' | 'defg' | 'DEFG' | 'dg' | '.dg' | 'DG' | '.DG' ) WS+ ~('\r' | '\n')+;
DCPRAG	: '.defc' | '.DEFC' | 'defc' | 'DEFC' | 'dc' | '.dc' | 'DC' | '.DC' ;
SKIPRAG	: '.skip' | '.SKIP' | 'skip' | 'SKIP' ;
EXTPRAG : '.extern'|'.EXTERN'|'extern'|'EXTERN' ;
DSPRAG	: '.defs' | '.DEFS' | 'defs' | 'DEFS' | '.ds' | '.DS' | 'ds' | 'DS' ;
FBPRAG	: '.fillb' | '.FILLB' | 'fillb' | 'FILLB' ;
FWPRAG	: '.fillw' | '.FILLW' | 'fillw' | 'FILLW' ;
MODPRAG : '.model' | '.MODEL' | 'model' | 'MODEL' ;
IOPTPRAG: '.injectopt' | '.INJECTOPT' | 'injectopt' | 'INJECTOPT';
ALGPRAG	: '.align' | '.ALIGN' | 'align' | 'ALIGN' ;
TRACE	: '.trace' | '.TRACE' | 'trace' | 'TRACE' ;
TRACEHEX: '.tracehex' | '.TRACEHEX' | 'tracehex' | 'TRACEHEX' ;
RNDSEED	: '.rndseed' | 'rndseed' | '.RNDSEED' | 'RNDSEED' ;
ERRORPR	: '.error' | '.ERROR' | 'error' | 'ERROR' ;
INCBIN	: '.includebin' | 'includebin' | '.INCLUDEBIN' | 'INCLUDEBIN' 
		  | '.include_bin' | 'include_bin' | '.INCLUDE_BIN' | 'INCLUDE_BIN' 
		  | '.incbin' | 'incbin' | '.INCBIN' | 'INCBIN' ;
COMPAREBIN
		: '.comparebin' | 'comparebin' | '.COMPAREBIN' | 'COMPAREBIN' ;
ZXBPRAG	: 'zxbasic' | 'ZXBASIC' | '.zxbasic' | '.ZXBASIC' ;

// --- Compiler statements
MACRO	: '.macro' | '.MACRO' | 'macro' | 'MACRO' ;
ENDMACRO: '.endm' | '.ENDM' | '.mend' | '.MEND' ;
PROC	: '.proc' | '.PROC' ;
ENDPROC	: '.endp' | '.ENDP' | '.pend' | '.PEND' ;
LOOP	: '.loop' | '.LOOP' ;
ENDLOOP	: '.endl' | '.ENDL' | '.lend' | '.LEND' ;
REPEAT	: '.repeat' | '.REPEAT' ;
UNTIL	: '.until' | '.UNTIL' ;
WHILE	: '.while' | '.WHILE' ;
ENDWHILE: '.endw' | '.ENDW' | '.wend' | '.WEND' ;
IFSTMT	: '.if' | '.IF' | 'if' | 'IF' ;
IFUSED	: '.ifused' | '.IFUSED' | 'ifused' | 'IFUSED' ;
IFNUSED	: '.ifnused' | '.IFNUSED' | 'ifnused' | 'IFNUSED' ;
ELIF	: '.elif' | '.ELIF' ;
ELSESTMT: '.else' | '.ELSE' ;
ENDIFSTMT: '.endif' | '.ENDIF' ;
FOR		: '.for' | '.FOR' | 'for' | 'FOR' ;
TO		: '.to' | '.TO' | 'to' | 'TO' ;
STEP	: '.step' | '.STEP' | 'step' | 'STEP' ;
FORNEXT	: '.next' | '.NEXT' ;
NEXT	: 'next' | 'NEXT' ;
BREAK	: '.break' | '.BREAK' ;
CONTINUE: '.continue' | '.CONTINUE' ;
MODULE	: '.module' | '.MODULE' | 'module' | 'MODULE' | '.scope' | '.SCOPE' | 'scope' | 'SCOPE' ;
ENDMOD	: '.endmodule' | '.ENDMODULE' | 'endmodule' | 'ENDMODULE' 
		  | '.endscope' | '.ENDSCOPE' | 'endscope' | 'ENDSCOPE'
		  | '.moduleend' | '.MODULEEND' | 'moduleend' | 'MODULEEND'
		  | '.scopeend' | '.SCOPEEND' | 'scopeend' | 'SCOPEEND' ;
STRUCT	: '.struct' | '.STRUCT' | 'struct' | 'STRUCT' ;
ENDST	: '.ends' | '.ENDS' ;
LOCAL	: '.local' | '.LOCAL' | 'local' | 'LOCAL' | 'Local' ;

// --- Built-in function names
TEXTOF	: 'textof' | 'TEXTOF' ;
LTEXTOF	: 'ltextof' | 'LTEXTOF' ;
HREG	: 'hreg' | 'HREG' ;
LREG	: 'lreg' | 'LREG' ;
DEF		: 'def' | 'DEF' ;
ISREG8	: 'isreg8' | 'ISREG8' ;
ISREG8STD: 'isreg8std' | 'ISREG8STD' ;
ISREG8SPEC: 'isreg8spec' | 'ISREG8SPEC' ;
ISREG8IDX: 'isreg8idx' | 'ISREG8IDX' ;
ISREG16: 'isreg16' | 'ISREG16' ;
ISREG16STD: 'isreg16std' | 'ISREG16STD' ;
ISREG16IDX: 'isreg16idx' | 'ISREG16IDX' ;
ISREGINDIRECT: 'isregindirect' | 'ISREGINDIRECT' ;
ISCPORT: 'iscport' | 'ISCPORT' ;
ISINDEXEDADDR: 'isindexedaddr' | 'ISINDEXEDADDR' ;
ISCONDITION: 'iscondition' | 'ISCONDITION' ;
ISEXPR: 'isexpr' | 'ISEXPR' ;
ISREGA: 'isrega' | 'ISREGA' ;
ISREGAF: 'isregaf' | 'ISREGAF' ;
ISREGB: 'isregb' | 'ISREGB' ;
ISREGC: 'isregc' | 'ISREGC' ;
ISREGBC: 'isregbc' | 'ISREGBC' ;
ISREGD: 'isregd' | 'ISREGD' ;
ISREGE: 'isrege' | 'ISREGE' ;
ISREGDE: 'isregde' | 'ISREGDE' ;
ISREGH: 'isregh' | 'ISREGH' ;
ISREGL: 'isregl' | 'ISREGL' ;
ISREGHL: 'isreghl' | 'ISREGHL' ;
ISREGI: 'isregi' | 'ISREGI' ;
ISREGR: 'isregr' | 'ISREGR' ;
ISREGSP: 'isregsp' | 'ISREGSP' ;
ISREGXH: 'isregxh' | 'ISREGXH' ;
ISREGXL: 'isregxl' | 'ISREGXL' ;
ISREGIX: 'isregix' | 'ISREGIX' ;
ISREGYH: 'isregyh' | 'ISREGYH' ;
ISREGYL: 'isregyl' | 'ISREGYL' ;
ISREGIY: 'isregiy' | 'ISREGIY' ;

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

DECNUM	: Digit Digit? Digit? Digit? Digit?;

CURADDR	: '$' ;

REALNUM	: [0-9]* DOT [0-9]+ ExponentPart? 
		| [0-9]+ ExponentPart;

CHAR	: '\'' (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter) '\'' ;
STRING	: '"'  (~["\\\r\n\u0085\u2028\u2029] | CommonCharacter)* '"' ;

// --- Boolean literals;

BOOLLIT	: TRUE | FALSE ;
TRUE	: 'true' | '.true' | 'TRUE' | '.TRUE' ;
FALSE	: 'false' | '.false' | 'FALSE' | '.FALSE' ;

// --- Semi-identifiers
CURCNT	: '$cnt' | '$CNT' | '.cnt' | '.CNT' ;

// --- Identifiers
IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '.' | '_' | '@' | '`' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '@' | '!' | '?' | '#' | '0'..'9' | 'A'..'Z' | 'a'..'z' | '.' ;

NONEARG	: '$<none>$' ;

// --- Any invalid character should be converted into an ErrorCharacter token.
ErrorCharacter
    :   .
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
	: [eE] (PLUS | MINUS)? [0-9]+
	;
