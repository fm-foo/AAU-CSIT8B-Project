
1. # The <insert name here> language.
The <insert name here> language is designed to programmatically create video game maps and entities that can perform actions and interact with other entities and with the map itself.

The map creator can create maps by defining subsections and maps, and assembling them in the desired way. This approach encourages the reuse of already defined components.

The size of the map (height and with) is defined by the programmer. Maps may contain other maps and sections. The final output is assembled from different map and section definitions.

- First iteration: the first iteration of the <insert name here> language is similar to markup languages (such as XML), in the sense that it only describes data, not actions. It can be used to describe a map on a square grid.
1. # Definitions
OPEN\_BRACE:			'{';

CLOSE\_BRACE          		'}'

SEMICOLON            		';'

COLON               		':'



MAP                 			'map'

SECTION              		'section'

REFERENCE            		'reference'

BACKGROUND          		'background'

COLOUR              		'colour'

BOX                 			'box'

IMAGE               		'image'

COORDINATES         		'coordinates'

LINE                			'line'

HEX                 			'hex'

HEIGHT              		'height'

WIDTH               		'width'

PATH                			'path'

POINT               		'point'

SHAPE               		'shape'

STRING               		DOUBLE\_QUOTED\_STRING | SINGLE\_QUOTED\_STRING

DOUBLE\_QUOTED\_STRING 	'"' CHARACTER\*? '"'

SINGLE\_QUOTED\_STRING 	'\'' CHARACTER\*? '\''

POINT\_LIT            		INTEGER WS\*? ',' WS\*? INTEGER

IDENTIFIER          		LETTER ALPHANUM\*

INTEGER             		'-'? NATURAL\_NUMBER

NATURAL\_NUMBER      	DIGIT+

COLOUR\_LIT          	 	'#' HEX\_LIT HEX\_LIT HEX\_LIT HEX\_LIT HEX\_LIT HEX\_LIT


1. # Programs in the <insert name here> language.
*file			: map\_or\_section\**

*map\_or\_section	: (map | section) SEMICOLON*

A valid file in the <insert name here> language consists of any number of map or section definitions. Each map or section definition must be terminated with a semicolon. 

The entire file defines a grid map, built up using one or more map or section definitions.

Map and section are mostly the same type, however only maps can be used as an output.

*map                		 : MAP IDENTIFIER OPEN\_BRACE* 

`  	   `*(section\_properties|section\_statements)\* CLOSE\_BRACE*



`	`A map definition is declared using the **MAP** reserved word followed by the required 

`  	`**IDENTIFIER** of the map, followed by an **OPEN\_BRACE**, then any number of section 

properties or section statements, and terminated with a **CLOSE\_BRACE**.



`	`*section\_properties 	 : (background\_property | shape\_property) SEMICOLON*

`	`*section\_statements	: (section | reference\_section) SEMICOLON*

`	`**section\_property** is used to define the look of the resulting map (segment). It can 

`	`define a background color or a background image.

`	`**shape\_property** is used to define the shape of the map (or section). See <reference>.

*section             	: SECTION IDENTIFIER? POINT\_LIT? OPEN\_BRACE* 

`                        	   	  `*(section\_properties|section\_statements)\* CLOSE\_BRACE*



`	`A section definition consists a **SECTION** keyword, an optional **IDENTIFIER,**  an optional 

`	`pair of integer literals **(*POINT\_LIT*)**, an **OPEN\_BRACE**, any number of section 

`	`properties or section statements (<reference>), and terminated with a **CLOSE\_BRACE**.

A section definition can be used inside a map definition in order to define a portion of the 

Map. 



`	`The two integers (**POINT\_LIT**) define the top left coordinates of the section. They are 

zero indexed, and begin from the top left (0,0) position of the outer map. 
1. # Background
`	`*background\_property		: BACKGROUND COLON background\_values*

`	`*background\_values		: colour | image*

*colour             			: COLOUR OPEN\_BRACE (colour\_properties)\** 

`  `*CLOSE\_BRACE;*

*image              	 	: IMAGE OPEN\_BRACE (image\_properties)\** 

` `*CLOSE\_BRACE;*
\*


`	`*colour\_properties   		: hex\_property SEMICOLON;*

*image\_properties    		: path\_property SEMICOLON;*

*hex\_property        		: HEX COLON COLOUR\_LIT;*

*path\_property       		: PATH COLON STRING;*

\*	The background property can be used to define the background color or image of maps 	

`	`and sections.

`	`An image background can be specified using the **image** keyword followed by the **path** to 

`	`the image.

`	`Similarly, the color of the background can be specified with the **color** keyword, and a hexadecimal number. 

`	`For more information, see: [Texture properties](#_37if3ojoziiu)

1. # Shapes
*shape\_property      : SHAPE COLON shape\_values*

In <insert name here> you can define some shapes to draw on the map. For defining a new shape you have to use the **SHAPE** keyword followed by a **COLON** followed by a shape value.

*shape\_values        : box | line | coordinates | point\_shape*

1. Shapes values

Here are the shape values you can use for defining a new shape.

*box                 	      : BOX OPEN\_BRACE (box\_properties)\* CLOSE\_BRACE*

A box is used to draw a rectangular or square shape. You can create one by using the **BOX** keyword followed by an **OPEN\_BRACE**, then the properties you need, to define your box, and finishing by a **CLOSE\_BRACE**.

1. Box properties

`	`*box\_properties      : (height\_property | width\_property) SEMICOLON*



For creating a box you can define its height and/or its width. If you just define its height or its width your box will be a square. For defining its height you have to use the height\_property. For defining its width you have to use the width\_property. You need to use a **SEMICOLON** after each definition.



*line                : LINE OPEN\_BRACE (point\_statements)\* CLOSE\_BRACE*

A line is used to draw a vertical, horizontal or diagonal line. You can create one by using the **LINE** keyword followed by an **OPEN\_BRACE**, then the list of points\_statements you need, to define your line, and finishing with a **CLOSE\_BRACE**. You can define a nonlinear line by giving every point where the line changes its direction. For example if you want to draw the outline of a cube you need to give four point\_statements.

1. Point\_statements

`	`*point\_statements    : POINT\_LIT SEMICOLON*



A point\_statements corresponds to a **POINT\_LIT** followed by a **SEMICOLON**. So for defining a line you can have a list of point\_statements


*coordinates         : COORDINATES OPEN\_BRACE (point\_statements)\* CLOSE\_BRACE*

Coordinates is used to define the color or texture of a given list of point\_statements. You can create one by using the **COORDINATES** keyword followed by an **OPEN\_BRACE**, then a list of point\_statements, and finishing with a **CLOSE\_BRACE**.

**point\_statements** see: <reference point\_statement>




*point\_shape         : POINT*

A point is used to define a 1x1 map. It has to be used in a section. You can create one by using the **POINT** keyword.


1. # Size properties
*height\_property     : HEIGHT COLON INTEGER*

The height\_property is used to define the height of an element, like a box shape. The height corresponds to the number of lines occupied by an element. You can define the height by using the **HEIGHT** keyword followed by a **COLON** and an **INTEGER** corresponding to the height you want for your element.


*width\_property      : WIDTH COLON INTEGER*

The width\_property is used to define the width of an element, like a box shape. The width corresponds to the number of columns occupied by an element. You can define the width by using the **WIDTH** keyword followed by a **COLON** and an **INTEGER** corresponding to the width you want for your element.


1. # Texture properties
*hex\_property        : HEX COLON COLOUR\_LIT*

The hex\_property is used to define an hexadecimal colour. You can define a new one by using the **HEX** keyword followed by a **COLON** and a **COLOUR\_LIT** corresponding to the hexadecimal code of the colour you want for your element.


*path\_property       : PATH COLON STRING*

The path\_property is used to indicate which file you want to use for defining the texture of an element. To define a path\_property you have to use the **PATH** keyword followed by a **COLON** and a **STRING** corresponding to the path to the texture file. This path is relative to the program file’s folder



*section\_statements  : (section | reference\_section) SEMICOLON*

A section\_statements is used to create a new section or a new reference section. To create one of those you have to use a section to create a section or a reference\_section to create a new reference section. Both of them have to be followed by a **SEMICOLON.**
