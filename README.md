# Game_02


## 1. Concepto

### 1.1. Categoría y objetivos

  (Introduzca nombre de juego) es un party game multijugador online, en el que el objetivo es tirar del mapa a los demás jugadores. El jugador se valdrá de distintos impulsos para esquivar a los demás jugadores y permanecer el mayor tiempo posible sin salirse del mapa, mientras empuja a los demás, jugando con las características especiales de distintos escenarios, como pueden ser columnas o suelo pringoso.

### 1.2. Sinopsis de Jugabilidad y Contenido

  En el jueo, todos los elemtos, incluidos personajes y escenarios, son chuches. Si bien hay varios personajes disponibles, ninguno de ellos tiene una mecánica especial y diferente ddel resto, simplemente permiten escoger al usuario aquel personaje que sea más de su gusto. En cuanto a los mapas, cada uno de ellos tiene una una característica especial, que hará que cada partida se sienta diferente de la anterior.

### 1.3. Mecánica (descripción general)

  La mecánica principal del juego es impulsarse de dos formas diferentes: la primera forma se verá reflejada como un pequeño impulso que estará disponible la mayor parte del tiempo. Este pequeño impulso está pensado para esquivar a los demás jugadores. El otro impulso es otro más potente, pero que tendrá más tiempo de enfriamiento antes de poder usarse. Este gran impulso está pensado para proporcionar un fuerte golpe a otro juegador y tirarlo del mapa. No obstante, también se puede empujar a los demás jugadores simplemente empujándolos sin usar ningún impulso.

### 1.4. Tecnología (incluir software de modelado)

  (Inserte nombre del juego) será desarrollado usando Unity 3D, por lo que se programará en C#. Para editar los efectos de sonido y la música se usará Audacity. Para el diseño de elementos 2D (interfaz y pantallas de menú) se usará Clip Studio Paint.

### 1.5. Estética

  La estética de todo el juego será muy amigable e infantil, siendo los personajes pequeñas chuches, sin haber más violencia más allá del empuje para tirar a otro personaje. Los mapas y los distintos elementos del juego también tendrán forma de dulces o postres. Predominarán los elementos con dinámica de forma circular.

### 1.6. Públco objetivo

  Dada la amigable estética y mecánicas de juego, (inserte nombre del juego) es apto para todo el público, por lo que tendrá pegi 3. No hay elementos que pueda asustar a nadie y en el juego no se encuentra ningún tipo de violencia, solo empujones que no dan pie a ningún tipo de herida.

### 1.7. Desarrolladores

  (Inserte nombre del juego) será desarrollado por el equipo Serendipy Studios, formado por:
  - Sergio López Serrano
  - David González Bella
  - Clara Bartolomé Pereira
  - Julen Justo Neira
  - Alexandra Izquierdo Esteban
  - Javier Morales López


## 2. Monetización y modelo de negocio

### 2.1. Tipo de modelo de negocio

### 2.2. Tabla de productos y precios


## 3. Planificación y costes

### 3.1. Equipo humano

El equipo estará compuesto por los siguientes integrantes con sus respectivos roles:
- Sergio López Serrano: Level designer y game designer.
- David González Bella: Programador
- Clara Bartolomé Pereira: Artista y programadora (auxiliar)
- Julen Justo Neira: Programador y compositor
- Alexandra Izquierdo Esteban: Artista
- Javier Morales López: Gestor, community manager y artista (auxiliar)

### 3.2. Estimación temporal de desarrollo

Dado que es un juego sin historia, el desarrollo del juego se centrará en definir y pulir bien las mecánicas, al mismo tiempo que se ajustan bien para que la dinámica general del juego sea lo más divertida posible. Dada la cantidad de assets a diseñar y programar un multijugador online, el desarrollo del juego llevará 2 meses aproximadamente.
No obstante, se seguirá desarrollando en implementando contenido, como puede ser más personajes jugables o distintos mapas, así como diferentes modos de juego. Pensando en este desarrollo constante, se planea mantener el juego en constante evolución durante al menos 2 años, una vez haya sido desarrollado.

### 3.3. Costes asociados

Se van a estimar costes de cara a tener un equipo trabajando durante dos años, siendo uno de esos años gratis de licencia, por ser alumnos (ya que el año que viene nos graduamos).

Cada programador cobrará 800€ mensuales, por lo que en 2 años será 19200€. Hay 2 programadores, por lo que en total 38400€.

El compositor cobrará 650€ al mes, por lo que en 2 años será 15600€.

Cada artista cobrará 800€ al mes, por lo que en 2 años será 19200€. Hay 2 artistas, por lo que en total 38400€.

El level designer/game designer cobrará 800€ al mes, por lo que en 2 años será 19200€.

El community manager ganará 750€ al mes, por lo que en 2 años será 18000€

El gestor ganará 800€ al mes, por lo que en 2 años será 19200€.

Licencia de Clip Studio Paint Pro: 45€

Licencia de Unity de Empresa: 165€/mes por puesto. Lo usarán los programadores (2 personas), por lo que en total: 3960

Licencia de Visual Studio: 37€/mes. Lo usarán los 2 programadores, por lo que en total: 888€

En total: 154581


## 4. Mecánica

### 4.1. Descripción detallada

El jugador podrá moverse por todo el mapa corriendo. La mecánica principal y lo que distingue al juego son los empujones. Podemos distinguir 2 tipos de empujones:
- Empujón débil, pensado para esquivar de forma rápida. El jugador elegirá la dirección en la que quiera impulsarse y, dando a la tecla o botón que le corresponda, se efectuará el empujón. Este empujón impulsará al jugador en una pequeña distancia. Si choca con otro jugador, será empujando mínimamente. Este empujón podrá efectuarse cada muy pocos segundos.
- Empujón fuerte, pensado para tirar a los rivales fuera del mapa. El jugador elegirá la dirección en la que quiera impulsarse y, dando a la tecla o botón que le corresponda, se efectuará el empujón. Impulsará al jugador en una gran distancia, por lo que se debe de tener en cuenta cuándo usarlo y en qué dirección, ya que si se usa apuntanto a una parte del mapa en el que éste se acaba, el jugador se tirará del mapa. Está pensado para apuntar hacia otro jugador, ya que al culisionar con éste, el jugador que ha sido impulsado se parará, transmitiendo así el impulso al otro jugador, que saldrá disparado del mapa. Al ser una mecánica tan poderosa, tiene un tiempo de enfriamento (tiempo sin poder usarse) algo mayor que el empujón débil.

Asimismo, el usuario podrá

### 4.2. Controles

### 4.3. Cámara

### 4.4. Power Ups

### 4.5. Arquitectura del Software


## 5. Interfaces


## 6. Estados del juego


## 7. Niveles


## 8. Personajes


## 9. Arte

### 9.1. Estética general del juego

### 9.2. Apartado visual

### 9.3. Mapas

### 9.4. Personajes

### 9.5. PowerUps

### 9.6. DLCs

### 9.7. Conceptart y referencias


## 10. Música y sonidos

### 10.1. Sonido de ambiente y música

### 10.2. Efectos sonoros

### 10.3. Lista de sonidos


## 11. Fecha de lanzamiento 
