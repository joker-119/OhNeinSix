# Oh Nein Six
==============
Made by Joker119
## Description
This is an almost entier re-work of how SCP-096 is played, to make him more lore-friendly but still keep his gameplay balanced. 

SCP-096 will now be given "targets" when he enrages. These targets are only people who can actively see his face. Anyone who shoots him or looks at his face while he's already enraged will also be added as targets.

SCP-096's enrage will not end until all of his targets are either dead or get too far away (about twice the distance that 939 can see through walls). While enraged, he can only see and interact with humans who are on his target list (all others will be invisible to him), and he will take exponentially increasing damage while he is enraged, to prevent him from simply ignoring his last target to gain permanent-rage.
SCP-096 will also take reduced damage from all sources except grenades and MICRO's while enraged, to compensate for his passive damage taken during this state.

The objective to killing him would simpy have a singe person trigger him and sacrifice themselves so that the rest of the team can gang up on him during his cooldown state, or have an entire team trigger him and run in opposite directions, with the intention of prolonging his enrage as long as possible to make him kill himself.
Frag Grenades and MICROHID's will always deal full damage to 096, and are also a valid method of containing him.

### Features
 - Lore-friendly 096 gameplay mechanics
 - SCP-096 is given a "distance bar" showing how close he is to his nearest target, and how many targets are remaining.
 - Most things, including his max range, damage taken, damage resistance, etc are all configurable.
 - A unique idea that if a human is looking at 096, but not at his face, he cannot be triggered (IE: if a scientist is looking at his back, said scientist is not in danger of being killed by 096).
 - People who do not look at 096's face or shoot him are free from his wrath.
 - A broadcast is shown to players when they spawn as SCP-096 telling them the SCP's mechanics have changed, and directs them to look at their client console (~) for more information.
 - The 096's client console details how his mechanics have been changed.

 ### Config Settings
 Config Option | Config Type | Default Value | Description
 :---: | :---: | :---: | :------
 oh96_enabled | Bool | True | Wether or not the SCP-096 rework plugin is to be used or not.
 oh96_max_range | Float | 80 | The maximum detection range of 096 once he's enraged. This is the range at whiche targets will cease to be targets if exceeded.
 oh96_damage_resistance | Float | 0.5 | The amount of normal damage taken by SCP-096 while enraged. 2=200%, 1.5=150%, 1=100%, 0.75=75%, 0.5=50%, 0.25=25%, 0.0=no damage taken
 oh96_blacklisted_roles | Int List | 14 | Roles that are unable to be added as targets to SCP-096. For use with plugins like Serpent's Hand. Also requires the smod setting for 096 ignored roles to be used.
 oh96_punish_delay | Float | 5 | The number of seconds between each damage tick while 096 is enraged. (The initial delay is always exactly double this number).
 oh96_punish_damage | Int | 45 | The damage taken each punishment tick.
 oh96_extreme_punishment | Bool | False | Causes 096 to take exponentially increasing damage each time he is punished, this multiplies the punish_damage value by the sum of punish_multiplier to the power of how many times he's been punished.
 oh96_punish_multiplier | Foat | 1.45 | The multiplier used for exponential damage increases.
 oh96_enraged_bypass | Bool | True | Wether or not SCP-096 should be given bypass mode while enraged, to make sure his targets can't simply hide from him behind unbreakable doors.

 ### Installation
 **You must put the included 0harmony.dll file inside the "dependencies" folder within "sm_plugins" for this plugin to work.**
