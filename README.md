# Pac-Man

  Mejoras antes de seguir

 ✅ - Esquinas de scatter: el siguiente paso, scatter, necesita una esquina para cada fantasma. Tienes dos opciones:
    - Pasarla como parámetro del constructor de Ghost, por ejemplo una propiedad ScatterCorner.
    - Usar marcadores 1–4 en el mapa, todos consistentes.

    En el original las esquinas de scatter están fuera del laberinto, para que el fantasma dé vueltas alrededor de un
    bloque. Con una casilla dentro también funciona.
  - Lo pendiente de la revisión anterior cobra importancia ahora:
    - Mover el cálculo de objetivos a Ghost, con un GetChaseTarget() o un Func. Scatter y frightened solo cambian el
      objetivo, así que MoveGhost quedaría en un foreach.
    - Compactar ChooseDirection en un ciclo.
  - Opcional, para ser fiel al original: cuando Pac-Man mira hacia arriba, el original tiene un bug y el objetivo de
    Pinky (4 adelante) y el pivote de Inky (2 adelante) también se mueven la misma cantidad a la izquierda. Solo si
    quieres replicarlo.

  Lo que sigue

  1. Simplificar Clyde como en la propuesta (es corto).
  2. Colisión Pac-Man–fantasma, vidas y Reset() en Ghost y Player.
  3. Alternar scatter/chase por tiempo, con reversa forzada al cambiar de modo.
  4. Frightened, y las pantallas de victoria y Game Over.


  Mejoras (no son urgentes)

  - Mueve el cálculo de objetivos fuera de Game. GetTarget y GetTargetInky son lógica de los fantasmas. Una opción es
    darle a Ghost un Func<int[]> (o un método virtual GetChaseTarget()) para que Update sea solo foreach (var g in
    ghosts) g.BaseMove(...). Te va a servir mucho para scatter y frightened, porque ahí solo cambia el objetivo.
  ✅- Usa (int row, int col) o un struct en vez de int[2]. target[0]/target[1], LastCell y DoorPosition son fáciles de
    confundir (fila o columna), y cada llamada a GetTarget crea un arreglo nuevo en cada frame.
  - Puedes compactar ChooseDirection. Los cuatro bloques casi idénticos caben en un ciclo sobre {Up, Left, Down, Right}:
    ese orden ya da el desempate correcto, con un helper Neighbor(dir) y GetReverse(Direction) para descartar la
    reversa. Lo que tienes ahora funciona bien, esto solo es limpieza.
  - Todos tienen velocidad 100, el jugador y los fantasmas. En el original Pac-Man es un poco más rápido, y los
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