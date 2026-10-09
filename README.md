# Pac-Man

Bugs

  1. GetInitialPosition devuelve la última coincidencia, no la primera (Maze.cs:177-194)
  El break solo sale del for de columnas; el de filas sigue corriendo. Con 'p' no se nota porque hay una sola, pero con 'g' (que aparece 4 veces) devolvería la fila 15, no la 11. Además, si no encuentra el carácter devuelve [0,0], que es una pared. Te
  conviene usar return dentro del loop y lanzar una excepción si no lo encuentra.

  2. El túnel ('0' / Bridge) no funciona (Maze.cs:111-120)
  CanMove permite pasar por Bridge, pero si el jugador sigue hacia la izquierda desde la columna 0, CanMove(14, -1) devuelve false y el jugador se queda pegado. Falta que salga por el otro lado (columna -1 → Cols-1). Cuando lo agregues, revisa también
  Maze.Cells[Player.MazeRow, Player.MazeCol] en Game.cs:104, porque ahí se puede salir del rango.

  3. Cualquier carácter desconocido se convierte en pared (Enums.cs:5, Maze.cs:82-109)
  Wall es el valor 0 del enum, así que cualquier celda que SetCells no mapea se queda como Wall. Por ejemplo, si escribes mal un carácter en el Grid, no hay error: simplemente aparece una pared invisible. Con un switch que tenga un default: throw lo
  detectarías enseguida.

  4. Bounds con Floor sigue pendiente, y ahora también está en Ghost (Player.cs:13-17, Ghost.cs:12-16)
  Es el punto #6 de tu README. El ancho queda en 28 o 29 en vez de 28.57, y la posición inicial (Y = 714) no coincide con la que luego usan los snaps (OffsetY + MazeRow * Pixel = 714.28). El fantasma copió el mismo patrón.

  5. OffsetX se calcula pero nunca se usa (Maze.cs:131)
  Con 800×1000 vale 0 y no pasa nada. Pero si cambias el tamaño de la ventana para que sobre espacio a los lados, el laberinto quedará pegado a la izquierda mientras que en vertical sí se centra.

  El fantasma

  - Se dibuja bien: (14, 11) es la 'g' arriba de la puerta, y el semicírculo de 180→360 queda arriba. Con segments = 1, raylib calcula los segmentos solo, así que no hay problema.
  - La posición está fija en el código (Game.cs:16). Sería mejor usar GetInitialPosition('g'), ya corregido.
  - Detalle: el fantasma se llama "Inky", no "Ink".

  Mejoras de diseño (importan antes de mover al fantasma)

  1. La lógica de movimiento está amarrada a Player. Check, UpdateColRow, UpdateDirection y MovementPlayer en Game usan Player directamente. Para que el fantasma se mueva tendrías que duplicar todo eso. Te sugiero crear una clase base (por ejemplo
     Entity/Actor) con Bounds, Speed, Direction, MazeRow/Col, SetPositionX/Y y el movimiento por celdas. Player y Ghost heredarían de ella, y solo cambiaría cómo deciden la dirección: el jugador con el teclado, el fantasma con su IA. Hoy Ghost y Player ya
     repiten casi el mismo código.
  2. PlayerDirection → Direction, porque el fantasma también lo usa.
  Pixel = 714.28). El fantasma copió el mismo patrón.

  5. OffsetX se calcula pero nunca se usa (Maze.cs:131)
  Con 800×1000 vale 0 y no pasa nada. Pero si cambias el tamaño de la ventana para que
  sobre espacio a los lados, el laberinto quedará pegado a la izquierda mientras que en
  vertical sí se centra.

  El fantasma

  - Se dibuja bien: (14, 11) es la 'g' arriba de la puerta, y el semicírculo de 180→360
    queda arriba. Con segments = 1, raylib calcula los segmentos solo, así que no hay
    problema.
  - La posición está fija en el código (Game.cs:16). Sería mejor usar
    GetInitialPosition('g'), ya corregido.
  - Detalle: el fantasma se llama "Inky", no "Ink".

  Mejoras de diseño (importan antes de mover al fantasma)

  1. La lógica de movimiento está amarrada a Player. Check, UpdateColRow,
     UpdateDirection y MovementPlayer en Game usan Player directamente. Para que el
     fantasma se mueva tendrías que duplicar todo eso. Te sugiero crear una clase base
     (por ejemplo Entity/Actor) con Bounds, Speed, Direction, MazeRow/Col,
     SetPositionX/Y y el movimiento por celdas. Player y Ghost heredarían de ella, y
     solo cambiaría cómo deciden la dirección: el jugador con el teclado, el fantasma
     con su IA. Hoy Ghost y Player ya repiten casi el mismo código.
  2. PlayerDirection → Direction, porque el fantasma también lo usa.
  3. CanMove bloquea la puerta para todos. Los fantasmas necesitan cruzarla para salir
     de la casa, así que conviene algo como CanMove(row, col, bool canUseDoor).
  4. Maze.Draw lee Sprite.Grid (los caracteres) en vez de Cells. Así tienes el mapeo de
     caracteres en dos lugares. Sería mejor que Draw haga un switch sobre Cells[r, c].
     Lo mismo para las celdas que se repiten en el cálculo de x0/y0/x1/y1: puedes
     sacarlo a un helper CellRect(r, c).
  5. El nombre Sprite confunde. No es un sprite, es el layout y las medidas del tablero.
     Algo como Board o Layout sería más claro.

  Detalles menores
  3. CanMove bloquea la puerta para todos. Los fantasmas necesitan cruzarla para salir de la casa, así que conviene algo como CanMove(row, col, bool canUseDoor).
  4. Maze.Draw lee Sprite.Grid (los caracteres) en vez de Cells. Así tienes el mapeo de caracteres en dos lugares. Sería mejor que Draw haga un switch sobre Cells[r, c]. Lo mismo para las celdas que se repiten en el cálculo de x0/y0/x1/y1: puedes sacarlo
     a un helper CellRect(r, c).
  5. El nombre Sprite confunde. No es un sprite, es el layout y las medidas del tablero. Algo como Board o Layout sería más claro.

  Detalles menores

  - Player.Points es float; debería ser int.
  - CellType.Player no se usa.
  - Hay un typo: GetPlayerInitailPos → GetPlayerInitialPos.
  - El constructor vacío de Program sobra.
  - Los pellets se comen con un frame de retraso, porque MovementPlayer usa el MazeRow/Col del frame anterior. No se nota, pero conviene saberlo.
  - Falta: colisión jugador–fantasma, condición de victoria (contar pellets restantes), vidas y el modo asustado con el power pellet.
  - El README.md son notas de una revisión anterior. Los puntos 1, 2 y el SetCells en el constructor ya están aplicados; los números de línea ya no coinciden; y el punto de Game.cs:129-136 ya no aplica. Ahí solo queda vigente el #6 (el Floor de Bounds,
    que es el bug 4 de arriba).