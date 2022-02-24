grammar Action;

options {
    language=CSharp;
}

map                 : MAP OPEN_BRACE (map_property|section)* CLOSE_BRACE SEMICOLON;
map_property        : background_property | shape_property;
background_property : BACKGROUND COLON background_value;
background_value    : bg_colour | bg_image;
bg_colour           : KW_COLOUR OPEN_BRACE hex_property CLOSE_BRACE


shape_property      :;

property    : IDENTIFIER COLON value;
section     :;
value       :;


fragment NEWLINE    : '\r\n' | '\n';
fragment CHARACTER  : ~[ \t\r\n];
fragment WS         : [ \t\r\n];
fragment LETTER     : [a-zA-Z];
fragment DIGIT      : [0-9];
fragment ALPHANUM   : [a-zA-Z0-9];
fragment HEX        : [0-9a-fA-F];

WHITESPACE          : WS+ -> skip;
SINGLELINE_COMMENT  : '//' (CHARACTER|[\t ])* NEWLINE -> skip;
MULTILINE_COMMENT   : '/*' .*? '*/' -> skip;
PRIMITIVE           : KEYWORD |
                        IDENTIFIER |
                        STRING |
                        COORDINATES | 
                        INTEGER | 
                        COLOUR |
                        OPEN_BRACE |
                        CLOSE_BRACE |
                        SEMICOLON |
                        COLON;


OPEN_BRACE          : '{';
CLOSE_BRACE         : '}';
SEMICOLON           : ';';
COLON               : ':';
KEYWORD             : MAP | SECTION | REFERENCE;
MAP                 : 'map';
SECTION             : 'section';
REFERENCE           : 'reference';
BACKGROUND          : 'background';
STRING              : DOUBLE_QUOTED_STRING | SINGLE_QUOTED_STRING;
DOUBLE_QUOTED_STRING: '"' CHARACTER*? '"';
SINGLE_QUOTED_STRING: '\'' CHARACTER*? '\'';
COORDINATES         : INTEGER WS*? ',' WS*? INTEGER;
IDENTIFIER          : LETTER ALPHANUM*;
INTEGER             : '-'? NATURAL_NUMBER;
NATURAL_NUMBER      : DIGIT+;
// this is the only way to match exactly 6 hexes
// we're not an angry witch screaming at someone
COLOUR              : '#' HEX HEX HEX HEX HEX HEX;