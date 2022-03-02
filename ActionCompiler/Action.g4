grammar Action;

options {
    language=CSharp;
}

file                : map_or_section*;

map_or_section      : (map | section) SEMICOLON;

map                 : MAP IDENTIFIER OPEN_BRACE (section_properties|section_statements)* CLOSE_BRACE;
section             : SECTION IDENTIFIER? POINT_LIT? OPEN_BRACE 
                        (section_properties|section_statements)* CLOSE_BRACE;
colour              : COLOUR OPEN_BRACE (colour_properties)* CLOSE_BRACE;
image               : IMAGE OPEN_BRACE (image_properties)* CLOSE_BRACE;
box                 : BOX OPEN_BRACE (box_properties)* CLOSE_BRACE;
line                : LINE OPEN_BRACE (point_statements)* CLOSE_BRACE;
coordinates         : COORDINATES OPEN_BRACE (point_statements)* CLOSE_BRACE;
point_shape         : POINT;
reference_section   : REFERENCE SECTION IDENTIFIER POINT_LIT;

section_properties  : (background_property | shape_property) SEMICOLON;
box_properties      : (height_property | width_property) SEMICOLON;
colour_properties   : hex_property SEMICOLON;
image_properties    : path_property SEMICOLON;

background_property : BACKGROUND COLON background_values;
background_values   : colour | image;

shape_property      : SHAPE COLON shape_values;
shape_values        : box | line | coordinates | point_shape;

height_property     : HEIGHT COLON INTEGER;
width_property      : WIDTH COLON INTEGER;
hex_property        : HEX COLON COLOUR_LIT;
path_property       : PATH COLON STRING;

point_statements    : POINT_LIT SEMICOLON;
section_statements  : (section | reference_section) SEMICOLON;

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
SEMICOLON           : ';';
COLON               : ':';

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
POINT               : 'point';
SHAPE               : 'shape';

STRING              : DQ_STRING | SQ_STRING;
POINT_LIT           : INTEGER WS*? ',' WS*? INTEGER;
IDENTIFIER          : LETTER ALPHANUM*;
INTEGER             : '-'? ('0' | NATURAL_NUMBER);
NATURAL_NUMBER      : ('0' | NZ_DIGIT DIGIT*);
// this is the only way to match exactly 6 hexes
COLOUR_LIT          : '#' HEX_LIT HEX_LIT HEX_LIT HEX_LIT HEX_LIT HEX_LIT;