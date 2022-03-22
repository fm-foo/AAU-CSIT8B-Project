grammar Action;

options {
    language=CSharp;
}

file                : map_or_section*;

map_or_section      : (map | section | entity | game) SEMICOLON;

map                 : MAP IDENTIFIER OPEN_BRACE (section_properties|section_statements)* CLOSE_BRACE;
section             : SECTION IDENTIFIER? POINT_LIT? OPEN_BRACE 
                        (section_properties|section_statements)* CLOSE_BRACE;
colour              : COLOUR OPEN_BRACE (colour_properties)* CLOSE_BRACE;
image               : IMAGE OPEN_BRACE (image_properties)* CLOSE_BRACE;
box                 : BOX OPEN_BRACE (box_properties)* CLOSE_BRACE;
line                : LINE OPEN_BRACE (coord_statements)* CLOSE_BRACE;
coordinates         : COORDINATES OPEN_BRACE (coord_statements)* CLOSE_BRACE;
unit_shape          : UNIT;
reference_section   : REFERENCE SECTION IDENTIFIER POINT_LIT;

section_properties  : (background_property | shape_property) SEMICOLON;
box_properties      : (height_property | width_property) SEMICOLON;
colour_properties   : hex_property SEMICOLON;
image_properties    : path_property SEMICOLON;

background_property : BACKGROUND COLON background_values;
background_values   : colour | image;

shape_property      : SHAPE COLON shape_values;
shape_values        : box | line | coordinates | unit_shape;

height_property     : HEIGHT COLON INTEGER;
width_property      : WIDTH COLON INTEGER;
hex_property        : HEX COLON COLOUR_LIT;
path_property       : PATH COLON STRING;

coord_statements    : POINT_LIT SEMICOLON;
section_statements  : (section | reference_section) SEMICOLON;

entity              : ENTITY IDENTIFIER OPEN_BRACE (func_def | field_dec)* CLOSE_BRACE;
game                : GAME IDENTIFIER OPEN_BRACE (func_def | field_dec)* CLOSE_BRACE;

field_dec           : type IDENTIFIER (EQUALS expr)? SEMICOLON;

func_def            : IDENTIFIER COLON FUNCTION OPEN_PAREN args? CLOSE_PAREN block;
args                : arg | arg COMMA args;
arg                 : type IDENTIFIER;

statement           : block | if | while | for | foreach | semicolon_statement;
semicolon_statement : (declaration | assignment | expr) SEMICOLON;

block               : OPEN_BRACE statement* CLOSE_BRACE;
declaration         : type IDENTIFIER (EQUALS expr)?;
assignment          : expr EQUALS expr;
if                  : IF OPEN_PAREN expr CLOSE_PAREN statement (ELSE statement)?;
while               : WHILE OPEN_PAREN expr CLOSE_PAREN statement;
for                 : FOR OPEN_PAREN (assignment | declaration)? SEMICOLON expr? SEMICOLON expr? statement;
foreach             : FOREACH OPEN_PAREN type IDENTIFIER IN expr CLOSE_PAREN statement;


literal             : STRING | POINT_LIT | INTEGER | FLOAT_LIT | BOOL_LIT;

type                : INT | BOOL | STRING_KW | FLOAT | COORD | IDENTIFIER;


expr                : boolean_expr; // parens expr

boolean_expr        : equality_expr
                    | boolean_expr ANDAND equality_expr
                    | boolean_expr OROR equality_expr;

equality_expr       : relational_expr
                    | equality_expr EQUALSEQUALS relational_expr
                    | equality_expr NOTEQUALS relational_expr;

relational_expr     : additive_expr
                    | relational_expr LESSTHAN additive_expr
                    | relational_expr GREATERTHAN additive_expr
                    | relational_expr LESSTHANEQUAL additive_expr
                    | relational_expr GREATERTHANEQUAL additive_expr;


additive_expr       : multiplicative_expr
                    | additive_expr PLUS multiplicative_expr
                    | additive_expr MINUS multiplicative_expr;

multiplicative_expr : unary_expr
                    | multiplicative_expr TIMES unary_expr
                    | multiplicative_expr DIVIDE unary_expr;

unary_expr          : primary_expr 
                    | PLUS unary_expr 
                    | MINUS unary_expr 
                    | BANG unary_expr
                    | PLUSPLUS unary_expr
                    | MINUSMINUS unary_expr;

primary_expr        : literal #lit
                    | IDENTIFIER #identifier
                    | OPEN_PAREN expr CLOSE_PAREN #parens_expr
                    | primary_expr PLUSPLUS #postfix_increment
                    | primary_expr MINUSMINUS #postfix_decrement
                    | primary_expr OPEN_PAREN func_args? CLOSE_PAREN #func_call
                    | primary_expr DOT IDENTIFIER #member_access
                    | TYPEOF OPEN_PAREN IDENTIFIER CLOSE_PAREN #typeof_expr
                    | NEW IDENTIFIER OPEN_PAREN func_args? CLOSE_PAREN #new_object
                    ;

func_args           : expr | expr COMMA func_args;



fragment NEWLINE    : '\r\n' | '\n';
fragment CHARACTER  : ~[ \t\r\n];
fragment WS         : [ \t\r\n];
fragment LETTER     : [a-zA-Z];
fragment DIGIT      : [0-9];
fragment NZ_DIGIT   : [1-9];
fragment ALPHANUM   : [a-zA-Z0-9];
fragment HEX_LIT    : [0-9a-fA-F];
fragment DQ_STRING  : '"' CHARACTER*? '"';
fragment SQ_STRING  : '\'' CHARACTER*? '\'';


WHITESPACE          : WS+ -> skip;
SINGLELINE_COMMENT  : '//' (CHARACTER|[\t ])* NEWLINE -> skip;
MULTILINE_COMMENT   : '/*' .*? '*/' -> skip;


OPEN_BRACE          : '{';
CLOSE_BRACE         : '}';
OPEN_PAREN          : '(';
CLOSE_PAREN         : ')';
SEMICOLON           : ';';
COLON               : ':';
COMMA               : ',';
DOT                 : '.';

PLUS                : '+';
MINUS               : '-';
TIMES               : '*';
DIVIDE              : '/';
EQUALS              : '=';
EQUALSEQUALS        : '==';
NOTEQUALS           : '!=';
BANG                : '!';
PLUSPLUS            : '++';
MINUSMINUS          : '--';
LESSTHAN            : '<';
GREATERTHAN         : '>';
LESSTHANEQUAL       : '<=';
GREATERTHANEQUAL    : '>=';

ANDAND              : '&&';
OROR                : '||';

MAP                 : 'map';
SECTION             : 'section';
REFERENCE           : 'reference';
BACKGROUND          : 'background';
COLOUR              : 'colour';
BOX                 : 'box';
IMAGE               : 'image';
COORDINATES         : 'coordinates';
LINE                : 'line';
HEX                 : 'hex';
HEIGHT              : 'height';
WIDTH               : 'width';
PATH                : 'path';
UNIT                : 'unit';
SHAPE               : 'shape';
ENTITY              : 'entity';
FUNCTION            : 'function';
IF                  : 'if';
ELSE                : 'else';
WHILE               : 'while';
FOR                 : 'for';
FOREACH             : 'foreach';
IN                  : 'in';
INT                 : 'int';
BOOL                : 'bool';
STRING_KW           : 'string';
FLOAT               : 'float';
COORD               : 'coord';
NULL                : 'null';
TYPEOF              : 'typeof';
NEW                 : 'new';
GAME                : 'game';

STRING              : DQ_STRING | SQ_STRING;
POINT_LIT           : INTEGER WS*? ',' WS*? INTEGER;
FLOAT_LIT           : '-'? NATURAL_NUMBER '.' DIGIT+;
IDENTIFIER          : LETTER ALPHANUM*;
INTEGER             : '-'? NATURAL_NUMBER;
NATURAL_NUMBER      : ('0' | NZ_DIGIT DIGIT*);
// this is the only way to match exactly 6 hexes
COLOUR_LIT          : '#' HEX_LIT HEX_LIT HEX_LIT HEX_LIT HEX_LIT HEX_LIT;
BOOL_LIT            : 'true' | 'false';