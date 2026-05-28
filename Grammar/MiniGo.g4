/* ============================================================================
 *  MiniGo.g4 - Gramática ANTLR4 para Mini-GO 
 *  Target: C# / .NET 10
 * ==========================================================================*/

grammar MiniGo;

/* REGLA RAIZ - Punto de entrada del parser */
root
    : 'package' IDENTIFIER ';' topDeclarationList EOF
    ;

topDeclarationList
    : ( variableDecl | typeDecl | funcDecl )*
    ;

/* ============ DECLARACIONES ============ */

variableDecl
    : 'var' singleVarDecl ';'
    | 'var' '(' innerVarDecls? ')' ';'
    ;

innerVarDecls
    : singleVarDecl ';' ( singleVarDecl ';' )*
    ;

singleVarDecl
    : identifierList declType '=' expressionList
    | identifierList '=' expressionList
    | singleVarDeclNoExps
    ;

singleVarDeclNoExps
    : identifierList declType
    ;

typeDecl
    : 'type' singleTypeDecl ';'
    | 'type' '(' innerTypeDecls? ')' ';'
    ;

innerTypeDecls
    : singleTypeDecl ';' ( singleTypeDecl ';' )*
    ;

singleTypeDecl
    : IDENTIFIER declType
    ;

funcDecl
    : funcFrontDecl block ';'
    ;

funcFrontDecl
    : 'func' IDENTIFIER '(' funcArgDecls? ')' declType?
    ;

funcArgDecls
    : singleVarDeclNoExps ( ',' singleVarDeclNoExps )*
    ;

/* ============ TIPOS ============ */

declType
    : '(' declType ')'
    | IDENTIFIER
    | sliceDeclType
    | arrayDeclType
    | structDeclType
    ;

sliceDeclType
    : '[' ']' declType
    ;

arrayDeclType
    : '[' INTLITERAL ']' declType
    ;

structDeclType
    : 'struct' '{' structMemDecls? '}'
    ;

structMemDecls
    : singleVarDeclNoExps ';' ( singleVarDeclNoExps ';' )*
    ;

identifierList
    : IDENTIFIER ( ',' IDENTIFIER )*
    ;

/* ============ EXPRESIONES (Precedencia: mayor -> menor) ============
 *  1) Unarios: + - ! ^
 *  2) Multiplicativos: * / % << >> & &^
 *  3) Aditivos: + - | ^
 *  4) Comparación: == != < <= > >=
 *  5) AND lógico: &&
 *  6) OR lógico: ||
 */

expression
    : primaryExpression                                            # PrimaryExpr
    | ('+' | '-' | '!' | '^') expression                           # UnaryExpr
    | expression ('*' | '/' | '%' | '<<' | '>>' | '&' | '&^') expression  # MulExpr
    | expression ('+' | '-' | '|' | '^') expression                # AddExpr
    | expression ('==' | '!=' | '<' | '<=' | '>' | '>=') expression # RelExpr
    | expression '&&' expression                                   # AndExpr
    | expression '||' expression                                   # OrExpr
    ;

expressionList
    : expression ( ',' expression )*
    ;

primaryExpression
    : operand
    | primaryExpression selector
    | primaryExpression index
    | primaryExpression arguments
    | appendExpression
    | lengthExpression
    | capExpression
    ;

operand
    : literal
    | IDENTIFIER
    | '(' expression ')'
    ;

literal
    : INTLITERAL
    | FLOATLITERAL
    | RUNELITERAL
    | RAWSTRINGLITERAL
    | INTERPRETEDSTRINGLITERAL
    ;

index
    : '[' expression ']'
    ;

arguments
    : '(' expressionList? ')'
    ;

selector
    : '.' IDENTIFIER
    ;

appendExpression
    : 'append' '(' expression ',' expression ')'
    ;

lengthExpression
    : 'len' '(' expression ')'
    ;

capExpression
    : 'cap' '(' expression ')'
    ;

/* ============ STATEMENTS ============ */

statementList
    : statement*
    ;

block
    : '{' statementList '}'
    ;

statement
    : 'print'   '(' expressionList? ')' ';'
    | 'println' '(' expressionList? ')' ';'
    | 'return' expression? ';'
    | 'break' ';'
    | 'continue' ';'
    | simpleStatement ';'
    | block ';'
    | switchStatement ';'
    | ifStatement ';'
    | loopStatement ';'
    | typeDecl
    | variableDecl
    ;

simpleStatement
    : /* vacío */                           #emptyStmt
    | expressionList ':=' expressionList    #shortVarDeclStmt
    | assignmentStatement                   #assignStmt
    | expression ( '++' | '--' )            #incDecStmt
    | expression                            #exprStmt 
    ;

assignmentStatement
    : expressionList '=' expressionList                # assignSimple
    | expression ( '+=' | '-=' | '*=' | '/=' | '%=' 
                 | '&=' | '|=' | '^=' | '<<=' | '>>=' | '&^=' ) expression # assignCompound
    ;

/* IF - Resuelve dangling-else por greedy matching del 'else' al 'if' más cercano */
ifStatement
    : 'if' ( simpleStatement ';' )? expression block elseClause?
    ;

elseClause
    : 'else' ( ifStatement | block )
    ;

/* LOOP (palabra reservada 'for' de Go) */
loopStatement
    : 'for' block
    | 'for' expression block
    | 'for' simpleStatement ';' expression? ';' simpleStatement block
    ;

/* SWITCH */
switchStatement
    : 'switch' ( simpleStatement ';' )? expression? '{' expressionCaseClauseList '}'
    ;

expressionCaseClauseList
    : expressionCaseClause*
    ;

expressionCaseClause
    : expressionSwitchCase ':' statementList
    ;

expressionSwitchCase
    : 'case' expressionList
    | 'default'
    ;

/* ============ LEXER ============ */

/* Identificador: letra o '_' seguido de letras, dígitos o '_' */
IDENTIFIER
    : LETTER ( LETTER | DIGIT )*
    ;

/* Literales numéricos (FLOAT antes que INT para maximal munch) */
FLOATLITERAL
    : DIGIT+ '.' DIGIT* EXPONENT?
    | '.' DIGIT+ EXPONENT?
    | DIGIT+ EXPONENT
    ;

INTLITERAL
    : DIGIT+                                  // decimal
    | '0' [xX] HEXDIGIT+                       // hexadecimal
    | '0' [oO] [0-7]+                          // octal
    | '0' [bB] [01]+                           // binario
    ;

/* Literal de carácter (entre comillas simples) */
RUNELITERAL
    : '\'' ( ESCAPE_SEQ | ~['\\\r\n] ) '\''
    ;

/* Strings: formato raw (backticks) e interpretado (comillas dobles) */
RAWSTRINGLITERAL
    : '`' ~[`]* '`'
    ;

INTERPRETEDSTRINGLITERAL
    : '"' ( ESCAPE_SEQ | ~["\\\r\n] )* '"'
    ;

/* Comentarios (descartados por el parser) */
LINE_COMMENT
    : '//' ~[\r\n]* -> skip
    ;

BLOCK_COMMENT
    : '/*' ( ~[*] | '*' ~[/] )* '*/' -> skip
    ; 

/* Espacios en blanco (ignorados) */
WS
    : [ \t\r\n]+ -> skip
    ;

/* Fragmentos (auxiliares, no generan tokens) */
fragment LETTER     : [a-zA-Z_] ;
fragment DIGIT      : [0-9] ;
fragment HEXDIGIT   : [0-9a-fA-F] ;
fragment EXPONENT   : [eE] [+-]? DIGIT+ ;
fragment ESCAPE_SEQ : '\\' [abfnrtv\\'"`0] ;

/* Token de error genérico (captura caracteres no reconocidos) */
ERROR_CHAR
    : .
    ;