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

func_def            : IDENTIFIER COLON FUNCTION OPEN_PAREN func_def_args? CLOSE_PAREN block;
func_def_args       : func_def_arg | func_def_arg COMMA func_def_args;
func_def_arg        : type IDENTIFIER;

statement           : block | if | while | for | foreach | semicolon_statement;
semicolon_statement : (declaration | assignment | expr) SEMICOLON;

block               : OPEN_BRACE statement* CLOSE_BRACE;
declaration         : type IDENTIFIER (EQUALS expr)?;
assignment          : left_expr EQUALS right_expr;
left_expr           : expr;
right_expr          : expr;
if                  : IF OPEN_PAREN expr CLOSE_PAREN statement (ELSE else_statement)?;
else_statement      : statement;
while               : WHILE OPEN_PAREN expr CLOSE_PAREN statement;
for                 : FOR OPEN_PAREN initialization? SEMICOLON cond_expr? SEMICOLON control_expr? CLOSE_PAREN statement;
initialization      : (assignment | declaration);
cond_expr           : expr;
control_expr        : expr | assignment;
foreach             : FOREACH OPEN_PAREN type IDENTIFIER IN expr CLOSE_PAREN statement;

literal             : STRING | POINT_LIT | INTEGER | FLOAT_LIT | BOOL_LIT;

type                : INT #int_type 
                    | BOOL #bool_type
                    | STRING_KW #string_type
                    | FLOAT #float_type
                    | COORD #coord_type
                    | IDENTIFIER #simple_type
                    | type OPEN_SQBRACKET CLOSE_SQBRACKET #array_type
                    ;

expr                : bool_expr;

bool_expr           : equality_expr #eq_expr
                    | bool_expr ANDAND equality_expr #andand_expr
                    | bool_expr OROR equality_expr #oror_expr
                    ;

equality_expr       : relational_expr #rel_expr
                    | equality_expr EQUALSEQUALS relational_expr #equalsequals_expr
                    | equality_expr NOTEQUALS relational_expr #notequals_expr
                    ;

relational_expr     : additive_expr #add_expr
                    | relational_expr LESSTHAN additive_expr #lessthan_expr
                    | relational_expr GREATERTHAN additive_expr #greaterthan_expr
                    | relational_expr LESSTHANEQUAL additive_expr #lessthanequal_expr
                    | relational_expr GREATERTHANEQUAL additive_expr #greaterthanequal_expr
                    ;


additive_expr       : multiplicative_expr #mult_expr
                    | additive_expr PLUS multiplicative_expr #plus_expr
                    | additive_expr MINUS multiplicative_expr #minus_expr
                    ;

multiplicative_expr : unary_expr #un_expr
                    | multiplicative_expr TIMES unary_expr #times_expr
                    | multiplicative_expr DIVIDE unary_expr #divide_expr
                    ;

unary_expr          : primary_expr #prim_expr
                    | PLUS unary_expr #plus_unary_expr
                    | MINUS unary_expr #minus_unary_expr
                    | BANG unary_expr #bang_expr
                    | PLUSPLUS unary_expr #plusplus_expr
                    | MINUSMINUS unary_expr #minusminus_expr
                    ;

primary_expr        : literal #lit
                    | IDENTIFIER #identifier
                    | OPEN_PAREN expr CLOSE_PAREN #parens_expr
                    | primary_expr PLUSPLUS #postfix_increment
                    | primary_expr MINUSMINUS #postfix_decrement
                    | primary_expr OPEN_PAREN func_args? CLOSE_PAREN #func_call
                    | primary_expr DOT IDENTIFIER #member_access
                    | TYPEOF OPEN_PAREN IDENTIFIER CLOSE_PAREN #typeof_expr
                    | NEW IDENTIFIER OPEN_PAREN func_args? CLOSE_PAREN #new_object
                    | primary_expr OPEN_SQBRACKET expr CLOSE_SQBRACKET #array_access
                    | OPEN_SQBRACKET array_values CLOSE_SQBRACKET #array_creation
                    ;

array_values        : expr | expr COMMA array_values;

func_args           : expr | expr COMMA func_args;

fragment NEWLINE    : '\r\n' | '\n';
fragment CHARACTER  : ~[ \t\r\n];
fragment WS         : ' ' | '\t';
fragment LETTER     : [a-zA-Z];
fragment DIGIT      : [0-9];
fragment NZ_DIGIT   : [1-9];
fragment ALPHANUM   : [a-zA-Z0-9];
fragment HEX_LIT    : [0-9a-fA-F];
fragment DQ_STRING  : '"' (CHARACTER | WS)*? '"';
fragment SQ_STRING  : '\'' (CHARACTER | WS)*? '\'';

WHITESPACE          : (WS | NEWLINE)+ -> skip;
SINGLELINE_COMMENT  : '//' (CHARACTER | WS)* NEWLINE -> skip;
MULTILINE_COMMENT   : '/*' .*? '*/' -> skip;

OPEN_BRACE          : '{';
CLOSE_BRACE         : '}';
OPEN_SQBRACKET      : '[';
CLOSE_SQBRACKET     : ']';
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
BOOL_LIT            : 'true' | 'false';
IDENTIFIER          : LETTER ALPHANUM*;
INTEGER             : '-'? NATURAL_NUMBER;
NATURAL_NUMBER      : ('0' | NZ_DIGIT DIGIT*);
// this is the only way to match exactly 6 hexes
COLOUR_LIT          : '#' HEX_LIT HEX_LIT HEX_LIT HEX_LIT HEX_LIT HEX_LIT;
