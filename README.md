# Pac-Man

  Bugs

  1. Inky se comporta igual que Pinky (Game.cs:95)
  var targetInky = GetTarget(Player, 4);   // mismo objetivo que Pinky
  Nada llama a GetTargetInky(). Además, ese método sigue con el error de la revisión pasada: solo duplica el vector en
  el eje en que se mueve el jugador. El otro eje se queda en la posición del jugador. Y si el jugador está en None,
  ignora a Blinky por completo.

  La fórmula correcta usa los dos ejes siempre:
  - pivote = GetTarget(Player, 2)
  - target = 2 * pivote - Blinky, tanto en filas como en columnas.

  Así GetTargetInky puede reutilizar GetTarget y se reduce a dos líneas.

  2. Hay un UpdateColRow() que sobra en la reversa (Ghost.cs:155)
  Se ejecuta antes de cambiar Direction, así que recalcula la celda con la dirección vieja y no hace nada útil. En el
  mejor caso es inofensivo y en el peor mueve MazeRow/MazeCol en el frame en que das la vuelta. Bórralo: el switch de
  abajo ya alinea la posición.

  3. Clyde todavía persigue igual que Blinky (Game.cs:97). Ya lo tienes como pendiente; lo menciono para que no se te
  olvide.

  Mejoras (no son urgentes)

  - Mueve el cálculo de objetivos fuera de Game. GetTarget y GetTargetInky son lógica de los fantasmas. Una opción es
    darle a Ghost un Func<int[]> (o un método virtual GetChaseTarget()) para que Update sea solo foreach (var g in
    ghosts) g.BaseMove(...). Te va a servir mucho para scatter y frightened, porque ahí solo cambia el objetivo.
  - Usa (int row, int col) o un struct en vez de int[2]. target[0]/target[1], LastCell y DoorPosition son fáciles de
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

  Lo que sigue

  1. Arreglar el objetivo de Inky (bug 1).
  2. Clyde: si la distancia al jugador es mayor a 8 casillas (compara 64 contra la distancia al cuadrado), su objetivo
     es el jugador; si no, su esquina de abajo a la izquierda.
  3. Colisión Pac-Man–fantasma, vidas y reinicio de posiciones. Para el reinicio tendrás que restaurar TimeToStartMove,
     OutRoom, CanUseDoor, LastCell y Direction, así que conviene un método Reset() en Ghost y otro en Player.
  4. Scatter/chase por tiempo, con reversa forzada al cambiar de modo. Vas a tener que saltarte el Direction != reversa
     solo en ese frame.
  5. Frightened, pantallas de victoria y Game Over.

  IA de los fantasmas. En cada intersección, el fantasma elige la casilla vecina más cercana a un objetivo, y no puede dar media vuelta. El objetivo cambia según el fantasma:
     - Blinky: la casilla del jugador.
     - Pinky ('r'): 4 casillas delante del jugador.
     - Inky ('i'): un vector calculado desde Blinky.
     - Clyde ('c'): persigue al jugador, pero si está a menos de 8 casillas se va a su esquina.