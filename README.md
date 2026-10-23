# Pac-Man
  Bugs

 ✅ 1. La velocidad se acumula en setScared (Game.cs:217, 223, 229, 235)
  Speed = Speed * 0.8f multiplica sobre la velocidad que el fantasma tenga en ese momento:
  - Si comes un segundo power pellet mientras ya están asustados, queda en 80 → 64 → 51.2, y baja más con cada pellet.
  - Al comerlo, Speed * 4 (Game.cs:184…) parte de ese valor ya reducido, así que la velocidad de los ojos cambia según
    cuántos pellets comiste.

  Usa constantes fijas, por ejemplo NormalSpeed = 80, FrightSpeed = 50 y EyesSpeed = 200, y asígnalas en lugar de
  multiplicar. Hoy el 80 está repetido en Ghost.cs:84, Ghost.cs:101 y Ghost.cs:161.

  ✅2. Un power pellet revive a los ojos (Game.cs:213-236)
  setScared solo revisa OutRoom, y los ojos que regresan a la casa tienen OutRoom = true. Si comes un pellet en ese
  momento, los ojos pasan a scared: vuelven a ser fantasma azul a mitad del camino, a velocidad 256 × 0.8 ≈ 205, y otra
  vez se pueden comer. Agrega la condición Status != GhostStatus.dead.

  ⚠️3. Los fantasmas asustados siguen persiguiendo al jugador (Ghost.cs:166)
  En BaseMove, el caso scared cae en el else y llama a ChooseDirection(maze, targetRow, targetCol) con el objetivo de
  chase. Van más lento, pero se siguen acercando a ti. En el original:
  - Al asustarse dan media vuelta de inmediato: Direction = GetReverse(Direction) y LastCell se invalida.
  - En cada intersección eligen una dirección válida al azar, sin reversa.

  Como tu ChooseDirection ya junta las opciones válidas, puedes sacar ese filtro a un helper y usarlo con Random.

  ✅4. ResetPositions reinicia todo en cada frame (Game.cs:247-253)
  Durante los 5 s de TimeToLife llama a Player.ResetPosition y a los cuatro Reset en cada frame. Ahora no se nota, pero
  es frágil: cualquier estado que esos métodos inicialicen se vuelve a pisar unas 300 veces. Además el jugador ve 5 s
  congelado + 5 s esperando, 10 s en total. Mejor:
  - Hacer el reset una sola vez, al terminar TimeInDead.
  - Agregar un estado Ready con su timer, de unos 2 s, que muestre "READY!".

  ✅5. No se puede salir de Game Over (Game.cs:132)
  GameOver no tiene input. La única forma de volver a jugar es cerrar la ventana. Necesitas un NewGame() que reinicie el
  maze (Maze.SetCells()), los puntos, las vidas y las posiciones, y que se active con una tecla.

  ??6. Blinky al morir no entra a la casa (Game.cs:185)
  Su CanUseDoor = false y su base 'b' es (11,14), afuera de la puerta. Funciona, porque reaparece y vuelve a salir, pero
  es inconsistente con los otros tres (que usan true) y con el original, donde todos los ojos entran. Si lo hiciste a
  propósito, ignora este punto. Si no, dale a Blinky como objetivo de muerte una casilla dentro de la casa, por ejemplo
  la de Pinky.

  Diseño y mejoras

  ✅- CheckCollision está repetido 8 veces. Con una List<Ghost> Ghosts y un helper Collides(Entity a, Entity b) queda así:
  foreach (var g in Ghosts) {
      if (!Collides(Player, g)) continue;
      if (g.Status == GhostStatus.normal) { KillPlayer(); return; }
      if (g.Status == GhostStatus.scared) g.SetDead();
  }
    Lo mismo aplica a setScared y ResetPositions. Este cambio también simplifica casi todo lo que sigue.
  ⚠️- Mueve las transiciones de estado a métodos de Ghost. Ahora Game asigna Status, Speed y CanUseDoor directamente. Con
    SetDead(), SetScared() y SetNormal(), cada uno con su velocidad y sus banderas, los bugs 1 y 2 no habrían pasado.
  ✅- Guarda el key en Entity. Hoy pasas 'b'/'r'/'i'/'c' a BaseMove y a Reset en cada llamada, y un typo ahí no lo detecta
    el compilador.
  - Puntos por fantasma en cadena: 200, 400, 800, 1600 dentro del mismo power pellet, en vez de 100 fijo.
  - Avisa que se acaba el susto: que el fantasma parpadee en blanco cuando TimeScared < 2.
  - Radio de colisión: con Pixel/2 + Pixel/2, basta con que los círculos se toquen para morir, y en las esquinas se
    siente injusto. Prueba con Pixel/3 o compara la casilla (MazeRow/MazeCol).

  Lo que falta

  1. Victoria y siguiente nivel: guarda un contador PelletsLeft en Maze. Cuando llegue a 0, reinicia el maze y las
     posiciones, y sube la velocidad.
  2. Alternar scatter/chase por tiempo, con reversa forzada al cambiar de modo. Ya tienes ScatterCorner en los cuatro
     fantasmas.
  3. Terminar frightened: movimiento aleatorio y reversa (bug 3).
  4. Reiniciar desde Game Over (bug 5).

  Te sugiero empezar por el refactor a List<Ghost> con los métodos de estado en Ghost: arregla los bugs 1 y 2 de paso y
  deja listo el punto 2 de lo que falta.
  Mejoras antes de seguir



    En el original las esquinas de scatter están fuera del laberinto, para que el fantasma dé vueltas alrededor de un
    bloque. Con una casilla dentro también funciona.
  - Lo pendiente de la revisión anterior cobra importancia ahora:
    - Mover el cálculo de objetivos a Ghost, con un GetChaseTarget() o un Func. Scatter y frightened solo cambian el
      objetivo, así que MoveGhost quedaría en un foreach.
    - Compactar ChooseDirection en un ciclo.


  Lo que sigue

  1. Simplificar Clyde como en la propuesta (es corto).
  ✅2. Colisión Pac-Man–fantasma, vidas y Reset() en Ghost y Player.
  3. Alternar scatter/chase por tiempo, con reversa forzada al cambiar de modo.
  ⚠️4. Frightened, y las pantallas de victoria y Game Over.


  Mejoras (no son urgentes)

  ??- Mueve el cálculo de objetivos fuera de Game. GetTarget y GetTargetInky son lógica de los fantasmas. Una opción es
    darle a Ghost un Func<int[]> (o un método virtual GetChaseTarget()) para que Update sea solo foreach (var g in
    ghosts) g.BaseMove(...). Te va a servir mucho para scatter y frightened, porque ahí solo cambia el objetivo.
  ✅- Usa (int row, int col) o un struct en vez de int[2]. target[0]/target[1], LastCell y DoorPosition son fáciles de
    confundir (fila o columna), y cada llamada a GetTarget crea un arreglo nuevo en cada frame.
  ??- Puedes compactar ChooseDirection. Los cuatro bloques casi idénticos caben en un ciclo sobre {Up, Left, Down, Right}:
    ese orden ya da el desempate correcto, con un helper Neighbor(dir) y GetReverse(Direction) para descartar la
    reversa. Lo que tienes ahora funciona bien, esto solo es limpieza.
  ⚠️- Todos tienen velocidad 100, el jugador y los fantasmas. En el original Pac-Man es un poco más rápido, y los
    fantasmas van más lento en el túnel y cuando están asustados. Cuando llegues a frightened vas a necesitar cambiar
    Speed de forma dinámica.
  - El túnel funciona por el orden de las llamadas. Probablemente el teletransporte de Move ocurre antes de que el
    fantasma llegue al centro de la celda del borde. Si un día cambias ese orden, el fantasma podría encontrarse sin
    salidas en la columna 0 y darse la vuelta.

  IA de los fantasmas. En cada intersección, el fantasma elige la casilla vecina más cercana a un objetivo, y no puede dar media vuelta. El objetivo cambia según el fantasma:
     - Blinky: la casilla del jugador.
     - Pinky ('r'): 4 casillas delante del jugador.
     - Inky ('i'): un vector calculado desde Blinky.
     - Clyde ('c'): persigue al jugador, pero si está a menos de 8 casillas se va a su esquina.