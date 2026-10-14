# Pac-Man

Problemas que afectan directamente la IA

  ✅1. El fantasma no sabe cuándo está centrado en una casilla
  La lógica de "estoy alineado con la casilla y puedo girar" solo existe en Player.UpdateDirection (Player.cs:67-69, centerX/centerY). Ahora el fantasma cambia de dirección en cualquier punto, y
  eso funciona porque solo va de izquierda a derecha. Cuando empiece a girar en las intersecciones, si gira a media casilla se va a desalinear y a rozar las paredes. Ese cálculo debería poder
  usarlo cualquier Entity, no solo el jugador.

  ✅2. La puerta siempre está abierta para los fantasmas
  BaseMove llama a Move(maze, true, ...) siempre (Ghost.cs:65). Blinky empieza en (11,14), justo encima de la puerta (12,13-14). Si el jugador está más abajo, la IA va a decidir que "abajo" lo
  acerca y Blinky se va a meter a la casa. La puerta solo debería poder cruzarse al salir (y después, al regresar cuando se lo coman).

  ✅3. Algunos giros no alinean la posición
  LeaveRoom cambia a Up y luego a InitialDirection sin ajustar el eje perpendicular (Ghost.cs:43-50). Además, UpdateColRow solo actualiza el eje de la dirección actual (Entity.cs:44). El jugador
  sí se alinea al girar con SetPositionX/SetPositionY. El fantasma necesita lo mismo, porque toda la IA depende de que MazeRow/MazeCol sean exactos.

  ✅Detalle menor: Ghost.cs todavía no está agregado en git (sale como ??). Acuérdate de agregarlo al hacer commit.

  ---

  Cómo funciona Blinky

  Blinky no "busca un camino" hacia Pac-Man. No usa BFS ni A*: es una decisión voraz (greedy) que toma en cada casilla.

  1. Casilla objetivo (target): en modo persecución, el objetivo de Blinky es la casilla donde está Pac-Man en ese momento. Eso es lo único que lo distingue de los demás fantasmas, porque cada
     uno calcula su objetivo de otra forma.
  2. Cuándo decide: solo cuando llega al centro de una casilla, y una sola vez por casilla. Si decide en cada frame mientras está "casi centrado", puede cambiar de opinión varias veces y quedarse
     vibrando. Te conviene recordar en qué casilla tomó la última decisión.
  3. Qué opciones tiene: revisa las 4 direcciones y descarta:
     - las que tienen pared (o la puerta, si ya salió);
     - la dirección contraria a la que lleva. Los fantasmas originales nunca se dan la vuelta por su cuenta. Esto es lo que hace que su movimiento se vea natural y no tiemble.
  4. Cómo elige: para cada opción válida, mira la casilla vecina en esa dirección y mide la distancia en línea recta de esa casilla al objetivo. Se queda con la más corta. Puedes comparar la
     distancia al cuadrado y te ahorras la raíz.
  5. Empates: en el original se resuelven con un orden fijo de prioridad: Arriba, Izquierda, Abajo, Derecha. Te recomiendo usar un orden fijo para que el comportamiento sea predecible.

  Qué vas a ver al probarlo

  - Como solo mira la distancia en línea recta, a veces toma rutas tontas: se va hacia una pared que lo separa de Pac-Man y tiene que dar la vuelta larga. Eso no es un bug, así funciona el
    original.
  - El túnel no cuenta para la distancia. Blinky no "sabe" que cruzando el túnel llega más rápido.
  - Si en algún caso la única salida es regresar, tienes que permitir la reversa como último recurso o se queda atorado. En tu laberinto casi no pasa, pero tenlo en cuenta.

  Para más adelante

  - Scatter / Chase: en el original los fantasmas alternan por tiempo entre perseguir y "dispersarse". Al dispersarse, el objetivo de Blinky es una esquina fija (arriba a la derecha) y por eso da
    vueltas ahí. Lo interesante es que es exactamente el mismo algoritmo, solo cambia el objetivo.
  - Cambio de modo: cuando cambia el modo, los fantasmas sí hacen una reversa forzada. Es la única excepción a la regla 3.
  - Frightened (asustado): en vez de calcular distancias, elige una dirección válida al azar.

  Una consideración de diseño

  Ahora cada fantasma tiene su propio MoveBlinky/MoveInky, que además están duplicados. Con lo de arriba verás que los 4 fantasmas comparten el mismo algoritmo de movimiento y solo difieren en
  cómo calculan su objetivo. Piensa en separar "calcular objetivo" (distinto para cada fantasma) de "elegir dirección hacia un objetivo" (común a todos). Así Pinky, Inky y Clyde te van a costar
  casi nada.
##########################################################################################################################################
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