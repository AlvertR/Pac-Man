# Pac-Man
Bugs

  1. El túnel no suma OffsetX (Entity.cs:83 y Entity.cs:94). SetPositionX(Board.Pixel * (Cols-1)) y SetPositionX(0) ignoran el offset. Con 800×1000 vale 0 y no se nota, pero si cambias el tamaño
     de la ventana el jugador aparecerá fuera del laberinto. Deberían ser Board.OffsetX + (Cols-1) * Board.Pixel y Board.OffsetX.
  2. Blinky se mete un poco en la pared al rebotar (Blinky.cs:31-43). Como cambia de dirección en UpdateDirection, Move no alcanza a ajustarlo a la celda. Avanza hasta un paso dentro de la pared
     y luego regresa. Además, revisa izquierda y derecha sin importar hacia dónde va. Al decidir la dirección, ajusta también la X a la celda (SetPositionX(OffsetX + MazeCol * Pixel)).
  3. GetInitialPosition devuelve [0,0] si no encuentra el carácter (Maze.cs:186). Esa celda es una pared, así que un error en el Grid no avisa: la entidad simplemente aparece en la esquina. Mejor
     lanza una excepción.
  4. SetCells convierte cualquier carácter desconocido en Path (Maze.cs:94). Es el mismo problema de antes, solo que ahora sale un hueco en vez de una pared invisible. Deja 'b','i','r','c' como
     Path y que el default lance una excepción.
  5. El jugador y Blinky son del mismo color. Los dos son Color.Red (Player.cs:28 y Blinky.cs:20). Pac-Man debería ser amarillo.
  6. La puerta de la casa no se dibuja (Maze.cs:50). La casa se ve abierta. Normalmente se dibuja como una línea rosa delgada.

  Mejoras de diseño

  1. Mover la creación de Bounds a Entity. Player y Blinky repiten el mismo bloque de x0/y0/x1/y1. Un constructor protected Entity(char key, float speed, Board board) quitaría la duplicación y
     también el aviso CS8618. Así cada fantasma nuevo sería una línea.
  2. Sacar un helper CellX(col) / CellY(row). La expresión Board.OffsetX + MazeCol * Board.Pixel se repite unas 12 veces entre Entity, Player, Blinky y Maze. Ponla en Board.
  3. Hacer Entity abstracta. El Draw base dibuja un cuadro negro que nadie usa. También puedes subir UpdateDirection como método abstracto: así Game puede recorrer una lista List<Entity> en lugar
     de tener MovementPlayer y MoveBlinky por separado.
  4. Pasar dt como parámetro de Move. Hoy usas SetDeltaTime y una propiedad DeltaTime. Pasarlo directo es más simple y no puedes olvidar actualizarlo.
  5. Hacer que la lógica de giro sirva también para los fantasmas. Player.UpdateDirection ya resuelve "girar solo cuando estoy centrado en la celda". Los fantasmas necesitan lo mismo. Si lo subes
     a Entity (algo como TryTurn(Direction, maze, canUseDoor)), la IA solo tiene que elegir la dirección.

  Lo que falta para terminar

  En orden aproximado:


  1. IA de los fantasmas. En cada intersección, el fantasma elige la casilla vecina más cercana a un objetivo, y no puede dar media vuelta. El objetivo cambia según el fantasma:
     - Blinky: la casilla del jugador.
     - Pinky ('r'): 4 casillas delante del jugador.
     - Inky ('i'): un vector calculado desde Blinky.
     - Clyde ('c'): persigue al jugador, pero si está a menos de 8 casillas se va a su esquina.
  2. Salida de la casa. Pinky, Inky y Clyde salen por la puerta con un temporizador. Una vez fuera, ya no deben poder volver a entrar (solo cuando se los comen).
  3. Colisión jugador–fantasma, comparando celdas o con CheckCollisionRecs.
  4. Vidas y reinicio de posiciones cuando un fantasma te atrapa.
  En orden aproximado:

  1. IA de los fantasmas. En cada intersección, el fantasma elige la casilla vecina más cercana a un objetivo, y no puede dar media vuelta. El objetivo
     cambia según el fantasma:
     - Blinky: la casilla del jugador.
     - Pinky ('r'): 4 casillas delante del jugador.
     - Inky ('i'): un vector calculado desde Blinky.
     - Clyde ('c'): persigue al jugador, pero si está a menos de 8 casillas se va a su esquina.
  2. Salida de la casa. Pinky, Inky y Clyde salen por la puerta con un temporizador. Una vez fuera, ya no deben poder volver a entrar (solo cuando se los
     comen).
  3. Colisión jugador–fantasma, comparando celdas o con CheckCollisionRecs.

  1. IA de los fantasmas. En cada intersección, el fantasma elige la casilla vecina más cercana a un objetivo, y no puede dar media vuelta. El objetivo cambia según el fantasma:
     - Blinky: la casilla del jugador.
     - Pinky ('r'): 4 casillas delante del jugador.
     - Inky ('i'): un vector calculado desde Blinky.
     - Clyde ('c'): persigue al jugador, pero si está a menos de 8 casillas se va a su esquina.
  2. Salida de la casa. Pinky, Inky y Clyde salen por la puerta con un temporizador. Una vez fuera, ya no deben poder volver a entrar (solo cuando se los comen).
  3. Colisión jugador–fantasma, comparando celdas o con CheckCollisionRecs.
  4. Vidas y reinicio de posiciones cuando un fantasma te atrapa.
  5. Modo asustado con el power pellet: los fantasmas se ponen azules y huyen, puedes comértelos (200, 400, 800, 1600 puntos) y regresan a la casa como ojos.
  6. Alternancia Scatter/Chase por tiempo: cada fantasma va a su esquina un rato y luego persigue.
  7. Condición de victoria: cuando no queden pellets, pasas al siguiente nivel.
  8. Estados del juego: Ready / Playing / Muerte / Game Over / Victoria, para no mezclarlos con ifs dentro de Update.

  Lo que más te conviene ahora es el punto 1 de diseño (constructor en Entity) y el punto 5 (giro reutilizable). Con eso, los otros tres fantasmas y la IA salen casi solos.