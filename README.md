# Pac-Man

notas 
  1. Al dar reversa, el jugador puede "teletransportarse" casi una celda

  En la ronda pasada te dije que no pasaba nada si MazeCol cambiaba en 1 al dar reversa, y me equivoqué. Sí cambia, pero un
  frame tarde. El orden en Update() es MovementPlayer → UpdateColRow → UpdateDirection, así que después de la reversa el
  siguiente MovementPlayer usa el MazeCol calculado con el redondeo de la dirección anterior.

  Ejemplo: vas a la derecha en la columna 1 pegada a la pared, con X = 1.9 celdas, así que MazeCol = Floor(1.9) = 1. Presionas
  ←:

  1. UpdateDirection cambia a Left y hace return.
  2. En el siguiente frame, MovementPlayer evalúa Check(Left) con MazeCol = 1, que revisa la columna 0. Es pared, así que entra
     al else.
  3. El snap SetPositionX(1 * Pixel) mueve al jugador ~26 px de golpe.

  Debió haber revisado la columna 1, porque con Ceiling(1.9) = 2 el vecino de la izquierda es la 1. Pasa en cualquier reversa
  donde la celda de atrás es pared, en los cuatro sentidos.

  Cómo arreglarlo: recalcula la celda en cuanto cambies la dirección:

  if (isReverse)
  {
      Player.Direction = Player.DesiredDirection;
      UpdateColRow();   // recalcula con el redondeo de la nueva dirección
      return;
  }

  2. Floor/Ceiling sobre un float que "debería" ser entero

  Cuando el jugador choca, el snap lo deja en OffsetY + MazeRow * Pixel. Justo después, UpdateColRow hace el camino inverso: (Y
  - OffsetY) / Pixel. Debería dar el número de fila exacto, pero los float tienen redondeo, y a veces da 22.9999 o 23.0001.

  Simulé tus cálculos en float de 32 bits, como los hace C#, con tu tamaño de ventana:

  - Con Floor (Down/Right) fallan las filas 1, 2, 4 y 23 y la columna 23: dan una celda menos.
  - Con Ceiling (Up) fallan las filas 3, 8, 17, 18, 19, 24, 25 y 26: dan una celda más.

  Ejemplo real: bajas por la columna 1 y chocas en la fila 23, donde está el power pellet de abajo a la izquierda.

  1. El snap deja Y en la fila 23, pero Floor(22.9999) da MazeRow = 22.
  2. Check(Down) revisa la fila 23, que es libre, así que avanza un paso.
  3. Ahora MazeRow = 23, Check(Down) revisa la fila 24, que es pared, y vuelve el snap.
  4. Se repite cada dos frames: el jugador vibra contra la pared.

  Lección de gamedev: nunca confíes en que un float sea exactamente entero. Si redondeas un valor que debería caer justo en un
  borde, dale un margen pequeño:

  const float eps = 0.001f;
  case PlayerDirection.Right: Player.MazeCol = (int)MathF.Floor(x + eps);   break;
  case PlayerDirection.Left:  Player.MazeCol = (int)MathF.Ceiling(x - eps); break;
  case PlayerDirection.Down:  Player.MazeRow = (int)MathF.Floor(y + eps);   break;
  case PlayerDirection.Up:    Player.MazeRow = (int)MathF.Ceiling(y - eps); break;

  Con eps = 0.001 celdas (~0.03 px), el redondeo nunca te lleva a la celda equivocada, y el margen es demasiado pequeño para
  notarse al moverte.

  3. Limpieza y pendientes menores

  - Game.cs:129-136 ya sobra. Esos !Check(...) servían para permitir girar estando detenido contra una pared. Ahora
    MovementPlayer ya alinea al jugador cuando choca, así que centerX/centerY ya dan true en ese caso. Puedes borrar esas 8
    líneas.
  - El warning que queda (Maze.cs:142) viene de Cells?. Pasa lo mismo que con Maze y Player: llama a SetCells() dentro del
    constructor de Maze, quita el ? y la llamada de Run().
  - El #6 sigue pendiente (Player.cs:220-224). Bounds todavía usa Floor, así que el ancho es 28 en lugar de 28.57 y el jugador
    arranca en Y = 714 en lugar de 714.28. Usa MazeCol * Pixel, OffsetY + MazeRow * Pixel y Pixel directamente.

  ---

  Para probar los arreglos:

  - Punto 1: en el pasillo de abajo a la izquierda (fila 29), ve a la derecha desde la pared y da reversa antes de llegar a la
    columna 2.
  - Punto 2: baja por la columna 1 hasta chocar en la fila 23 y fíjate si el jugador vibra.