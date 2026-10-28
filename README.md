# Pac-Man
 Bugs

  ✅1. Un fantasma asustado puede quedarse atorado para siempre (Ghost.cs:184-187)
  Al asustarse, el fantasma da media vuelta, pero LastCell no se invalida. El problema aparece cuando acaba de girar en
  un cruce y todavía no sale de esa casilla:
  - Iba hacia la derecha, giró hacia arriba en (r,c) y LastCell = (r,c). Mientras sube, MazeRow sigue siendo r porque
    redondea con Ceiling.
  - Comes un power pellet y su dirección pasa a Down. Si abajo hay pared, Move lo deja fijo en el centro de la celda.
  - En el siguiente frame, ChooseDirection hace return porque isLastCell es true, y Move sigue chocando con la pared.
  - Se queda ahí para siempre, también cuando se le pasa el susto.

  Esto puede pasar durante unos 0.3 s cada vez que un fantasma gira en un cruce en forma de T o en una esquina, así que
  no es raro.

  El startScared de ChooseDirection (Ghost.cs:225-228) parece pensado para cubrir este caso, pero nunca se ejecuta:
  cuando TimeScared == 10f, el código entra a la rama de la reversa y no llama a ChooseDirection.

  La solución es hacer la reversa en SetScared() y quitar el if (TimeScared == 10f):
  Direction = GetReverse(Direction);
  LastCell[0] = -1; LastCell[1] = -1;
  Con eso también puedes borrar startScared.

  ✅2. El objetivo aleatorio provoca overflow (Ghost.cs:190-192)
  Rnd.Next() devuelve hasta unos 2 mil millones. En ChooseDirection, dx * dx pasa de int.MaxValue y da un valor sin
  sentido, que a veces es negativo. Las elecciones parecen aleatorias por casualidad, pero no están distribuidas de
  forma pareja.
  - Solución mínima: Rnd.Next(maze.Cells.GetLength(0)) y Rnd.Next(maze.Cells.GetLength(1)).
  - Solución más fiel al original: juntar las direcciones válidas (con Check y sin la reversa) y elegir una con
    Rnd.Next(count).

  ✅3. TimeScared == 10f funciona como bandera de "recién asustado" (Ghost.cs:184)
  Ahora funciona porque asignas exactamente 10f, pero comparar floats así es frágil. Con el arreglo del bug 1 deja de
  hacer falta.

  ??4. Reset() pone a Blinky dentro de la casa sin estar adentro (Ghost.cs:90 y Game.cs:243, 258)
  g.Reset() usa los valores por defecto outOfRoom=false y canUseDoor=true para los cuatro fantasmas. Blinky reaparece en
  (11,14) creyendo que está dentro de la casa. Sale bien solo porque al caminar a la izquierda llega a la columna de la
  puerta y LeaveRoom lo da por fuera. Te recomiendo guardar InitialOutRoom e InitialCanUseDoor en el constructor, igual
  que ya haces con InitialDirection, y quitar los parámetros de Reset.

  ⚠️5. Al regresar a la casa, el fantasma igual elige dirección (Ghost.cs:168-180)
  Cuando los ojos llegan a su base, el código revive al fantasma y después llama a ChooseDirection en el mismo frame,
  aunque ya tiene Status normal y OutRoom = false. Mueve ChooseDirection a un else. Además, ese bloque repite casi todo
  lo que hace Reset(), así que lo puedes extraer a un Revive().

  ✅6. Warning CS8602 (Game.cs:181-183)
  En el case 'c' usas clyde, que sale de un FirstOrDefault y puede ser null. Ahí clyde es el mismo objeto que g, así que
  usa g y borra la variable clyde.

  Mejoras

  ✅- SetNormal() debería encargarse de todo. Hoy solo cambia Status, y la velocidad la asignan CheckScaredCounter, Reset
    y el revive por separado. Si también pone Speed = NormalSpeed y TimeScared = 10f, se acaban esas repeticiones.
  ✅- Rnd debería ser private static readonly Random. No hace falta que sea público ni que haya uno por fantasma.
  ✅- Tiempos de muerte: 5 s congelado más 2 s de "Listo!" son 7 s cada vez que mueres. Con 1.5–2 s de TimeInDead alcanza.
  ✅- Inicio del juego: al presionar S pasas directo a Playing. Para ser consistente con el reinicio, entra a Ready.
  ✅- Pendientes de la revisión anterior: los puntos en cadena por fantasma (200/400/800/1600) y el parpadeo cuando
    TimeScared < 2.
  ✅- Detalle opcional: en el original, los fantasmas que siguen dentro de la casa también se ponen azules. Ahora
    setScared los ignora porque solo revisa OutRoom.

  Lo que falta

  1. Victoria y siguiente nivel: un contador PelletsLeft en Maze que, al llegar a 0, reinicie el maze y las posiciones.
  ⚠️2. Alternar scatter/chase por tiempo, con reversa forzada al cambiar de modo. Puedes reusar la reversa del bug 1
     (GetReverse + invalidar LastCell).
  ✅3. Puntos en cadena y parpadeo del modo asustado.

  Te sugiero empezar por el bug 1, porque se nota en el juego y el arreglo son tres líneas. Después sigue el 2, y con
  eso el modo asustado queda completo.

  IA de los fantasmas. En cada intersección, el fantasma elige la casilla vecina más cercana a un objetivo, y no puede dar media vuelta. El objetivo cambia según el fantasma:
     - Blinky: la casilla del jugador.
     - Pinky ('r'): 4 casillas delante del jugador.
     - Inky ('i'): un vector calculado desde Blinky.
     - Clyde ('c'): persigue al jugador, pero si está a menos de 8 casillas se va a su esquina.