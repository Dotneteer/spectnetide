grammar CommandTool;

/*
 * Parser Rules
 */

compileUnit
	:	toolCommand EOF
	;

toolCommand
	: gotoCommand
	| gotoSymbolCommand
	| romPageCommand
	| bankPageCommand
	| memModeCommand
	| labelCommand
	| commentCommand
	| prefixCommentCommand
	| setBreakpointCommand
	| toggleBreakpointCommand
	| removeBreakpointCommand
	| updateBreakpointCommand
	| eraseAllBreakpointsCommand
	| retrieveCommand
	| literalCommand
	| disassemblyTypeCommand
	| reDisassemblyCommand
	| jumpCommand
	;

gotoCommand: G WS (HEXNUM | IDENTIFIER) ;
gotoSymbolCommand: GS WS IDENTIFIER ;
romPageCommand: R WS HEXNUM ;
bankPageCommand: B WS HEXNUM ;
memModeCommand: M ;
labelCommand: L WS HEXNUM (WS IDENTIFIER)? ;
commentCommand: C WS HEXNUM WS? .*? ;
prefixCommentCommand: P WS HEXNUM WS? .*? ;
setBreakpointCommand: SB WS HEXNUM
	(WS H WS? (LESS|LESSEQ|GREAT|GREATEQ|EQ|MULT) WS? HEXNUM)? 
	(WS C WS .*?)? ;
toggleBreakpointCommand: TB WS HEXNUM ;
removeBreakpointCommand: RB WS HEXNUM ;
updateBreakpointCommand: UB WS HEXNUM ;
eraseAllBreakpointsCommand: EB ;
retrieveCommand: RETRIEVE WS HEXNUM ;
literalCommand: D WS HEXNUM (WS (HASH|IDENTIFIER)) ;
disassemblyTypeCommand: T WS .*? ;
reDisassemblyCommand: RD ;
jumpCommand: J WS (HEXNUM | IDENTIFIER) ;

/*
 * Lexer Rules
 */

WS:	(' '|'\t')+	;

LESS: '<' ;
LESSEQ: '<=' ;
GREAT: '>' ;
GREATEQ: '>=' ;
EQ: '=' ;
MULT: '*' ;
HASH: '#';

B: 'b'|'B' ;
C: 'c'|'C' ;
D: 'd'|'D' ;
G: 'g'|'G' ;
GS: 'gs'|'gS'|'Gs'|'GS' ;
J: 'j'|'J' ;
L: 'l'|'L' ;
M: 'm'|'M' ;
P: 'p'|'P' ;
R: 'r'|'R' ;
RD: 'rd'|'rD'|'Rd'|'RD' ;
SB: 'sb'|'sB'|'Sb'|'SB' ;
H: 'h'|'H' ;
T: 't'|'T' ;
TB: 'tb'|'tB'|'Tb'|'TB' ;
RB: 'rb'|'rB'|'Rb'|'RB' ;
UB: 'ub'|'uB'|'Ub'|'UB' ;
EB: 'eb'|'eB'|'Eb'|'EB' ;
RETRIEVE: R (L|C|P|B) ;

HEXNUM: HEXSTART HexDigit? HexDigit? HexDigit? ;
HEXSTART: DecimalDigit | '0' ('A'|'a'|B|C|D|'e'|'E'|'f'|'F') ;

IDENTIFIER: IDSTART IDCONT*	;
IDSTART	: '_' | 'A'..'Z' | 'a'..'z'	;
IDCONT	: '_' | '0'..'9' | 'A'..'Z' | 'a'..'z' ;

OTHER: . ;

fragment DecimalDigit: [0-9] ;
fragment HexDigit: DecimalDigit|'A'|'a'|B|C|D|'e'|'E'|'f'|'F' ;
