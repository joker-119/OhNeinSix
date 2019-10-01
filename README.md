# Oh Nein Six
Hecho por Joker119

Traducido por JUST
## Descripción
Esto es una modificación de como SCP-096 es jugado para hacerle mas fiel al lore y martenerlo balanceado. 

SCP-096 ahora tendra "objetivos" cuando se enfade. Estos objetivos son solo los jugadores que han visto la cara de SCP-096. Cualquier jugador que dispare o mire a la cara a SCP-096 mientras este enfadado sera considerado un objetivo más.

El enfado de SCP-096 no terminará hasta que todos los objetivos hayan muerto o se hayan alejado demasiado (mas o menos el doble de distancia a la que SCP-939 puede ver jugadores a traves de las paredes). Mientras esté enfadado, solo podrá ver y interactuar con los jugadores que esttén en su lista de objetivos (los demás jugadores serán invisibles para él), y sufrirá daño que aumentará exponencialmente mientras esté enfadado, para evitar que ignore a los objetivos y se quede en un estado de enfado permanente.
SCP-096 también sufrirá daño reducido por cualquier arma excepto de granadas y la MICRO mientras esté enfadado, para compensar por el daño pasivo recibido durante este estado.

La estrategia mas simple para recontener a SCP-096 sería que un jogador se sacrifique para que el resto de jugadores le ataque mientras está en calmado, o tener a un equipo entero mirandole a la cara y correr en direcciones diferentes, con la intención de prolongar su enfado lo maximo posible para que muera por el daño pasivo.
Las granadas de fragmentación y la MICRO siempre le harán el daño completo, y son tambien un metodo valido de recontenerlo.

### Características
 - Jugabilidad fiel al lore de SCP-096
 - SCP-096 ahora tiene una "barra de distancia" mostrando la distacia hasta el objetivo más cercano, y un contador enseñando cuantos objetivos quedan vivos.
 - La mayoria de cosas, como el rango maximo, daño recivido, resistencia al daño, etc, son configurables.
 - Si un humano está mirando a SCP-096, pero no a su cara, el no se podra enfadar (por ejemplo: si un cientifico está mirando a su espalda, SCP-096 no se enfadará).
 - Los jugadores que no miren a SCP-096 estarán libres de su ira.
 - Saldrá un mesaje en la parte superior de la pantalla a los jugadores que reaparezcan como SCP-096 informandoles de que las mecanicas de juego han cambiado, y habisandoles de que tienen más información en la consola (por defecto en la Ñ para jugadores hispanohablantes).
 - La consola de SCP-096 indica que cambios hay en sus mecanicas.

 ### Configuraciones
 Parámetro | Tipo | Valor predeterminado | Descripción
 :---: | :---: | :---: | :------
 oh96_enabled 				| Bool 		| True 	| Determina si el plugin del rework de SCP-096 es usado o no.
 oh96_max_range 			| Float 	| 80 	| El rango máximo de detección de SCP-096 una vez enfadado. este es el rango máximo en el que un jugador deja de ser un objetivo de SCP-096.
 oh96_damage_resistance 	| Float 	| 0.5 	| La cantidad de daño normal recivida por SCP-096 mientras está enfadado. 2=200%, 1.5=150%, 1=100%, 0.75=75%, 0.5=50%, 0.25=25%, 0.0=ningún daño recivido.
 oh96_blacklisted_roles 	| Int List 	| 14 	| Roles incapaces de ser objetivos de SCP-096. Ideal para el uso conjunto con plugins como el de Serpent's Hand. También requiere [la opción de smod](https://github.com/Grover-c13/Smod2/wiki/Config-additions#class-based) (scp096_ignored_role) para añadir roles que no puedan enfadar a SCP-096.
 oh96_punish_delay 			| Float 	| 5 	| El número de segundos entre cada tick de daño de castigo mientras SCP-096 esta enfadado. (El retraso inicial es siempre exactamente el doble de esta valor).
 oh96_punish_damage 		| Int 		| 45 	| El daño recivido cada tick de daño de castigo.
 oh96_extreme_punishment	| Bool	 	| False | Causes 096 to take exponentially increasing damage each time he is punished, this multiplies the punish_damage value by the sum of punish_multiplier to the power of how many times he's been punished.
 oh96_extreme_punishment	| Bool	 	| False | Hace que SCP-096 sufra daño exponencial cada vez que sufre daño de castigo, esta opción multiplica ``punish_damage`` por la suma de ``punish_multiplier`` a la potencia de cuantas veces ha sido castigado.
 oh96_punish_multiplier 	| Foat 		| 1.45	| El multiplicador usado para el aumento del daño exponencial.
 oh96_enraged_bypass 		| Bool 		| True	| Si SCP-096 puede abrir cualquier puerta cuando esté enfadado, para asegurarse de que sus objetivos no se encierren tras puertas indestructubles.

 ### Instalación
 **Debes poner el archivo 0harmony.dll en la carpeta "dependencies", que a suvez está en la carpeta "sm_plugins" para que este plugin funcione.**