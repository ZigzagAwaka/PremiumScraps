## 2.0.9
- **Updated**
    - Harry Mason's secret effect chance has been increased from 5% to 8% hehehe
- **Fixed**
    - Compatibility patch with the latest version of [Emergency_Dice_Updated](https://thunderstore.io/c/lethal-company/p/slayer6409/Emergency_Dice_Updated/)
        - Should fix the "Premium Scraps" custom dice effect
        - Now requires version 1.6.5 or newer to work, or else will be disabled

## 2.0.8
- **Updated**
    - Added 2 new rare events for The friendship ender effect if you have [Surfaced](https://thunderstore.io/c/lethal-company/p/SurfacedTeam/Surfaced/) installed (if you don't have it, they will be replaced by vanilla ones instead)
    - Added a new config for selecting unlucky players (use this to troll your friends lol)
    - My ■■■■■■th job application got a *special* upgrade when used by unlucky players
- **Fixed**
    - The friendship ender effect (the new one since 2.0.7) should be a little bit more optimized

## 2.0.7
- **Updated**
    - The friendship ender is now more LETHAL
    - It's highly recommanded to have the mod [StarlancerAIFix](https://thunderstore.io/c/lethal-company/p/AudioKnight/StarlancerAIFix/) installed
    - To avoid problems with The friendship ender, the item now has 2 max usage allowed per moons. And if you try to use it more than 2 times something *a little bit less lethal* will happen
    - The Bomb item got a *special* upgrade when used by unlucky players
    - Improved compatibility with [RuntimeIcons](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/RuntimeIcons/)
        - Every scrap that didn't had an icon by default will now display a custom icon when you have RuntimeIcons 0.2.0 or newer installed
        - Custom icons are better than automatically generated ones from this mod, and they don't create any lag when the item spawns
- **Fixed**
    - Fixed every damage and heal not working as intended if (somehow) you have more than max health

## 2.0.6
- **Fixed**
    - Fixed a `ReflectionTypeLoadException` when loading the mod without soft dependencies installed (Thank you [DiFFoZ](https://thunderstore.io/c/lethal-company/p/DiFFoZ/) for the help!)

## 2.0.5
- **Added**
    - Custom compatible dice rolls have been added for the [Emergency_Dice_Updated](https://thunderstore.io/c/lethal-company/p/slayer6409/Emergency_Dice_Updated/) mod!
    - Requires version 1.6.1 or newer, can be disabled in the config
    - Added events:
        - Premium Scraps [Good]
        - Haunted hallucination [Bad]
        - Death hallucination [Awful]
- **Updated**
    - Improved scrap spawning custom code (used by Basics of architecture and now also by my custom compatible dice effects)
    - Basics of architecture *'spawning effect'* can now also be used when in orbit and when you are at Gordion
- **Fixed**
    - Summoned items of Basics of architecture are now synced to all players (scrap value and item rotation)

## 2.0.4
- **Updated**
    - The Stick special effect is now a little bit stronger
    - When a player is summoning *"employees"* with My ■■■■■■th job application, the item will now prevents other players to also summon *"employees"* until the next moon
    - Changed how control tips are displayed to the local player for Basics of architecture and My ■■■■■■th job application
    - Updated to v68
- **Fixed**
    - Fixed My ■■■■■■th job application sometimes not working when there is already another Job application item in use on the map

## 2.0.3
- **Updated**
    - Rebuilt assets with HDRP's Lit Shader Mode = "both"
    - Reduced the number of summoned *"employees"* with My ■■■■■■th job application
    - Updated to v66
- **Fixed**
    - Fixed compatibility issues with [RuntimeIcons](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/RuntimeIcons/)
    - Fixed Fake airhorn resting position when [Matty_Fixes](https://thunderstore.io/c/lethal-company/p/mattymatty/Matty_Fixes/) is installed (this only works with Matty_Fixes 1.1.30+)
    - Fixed one of the effect of the Bomb item not working when [LethalThings](https://thunderstore.io/c/lethal-company/p/Evaisa/LethalThings/) is installed

## 2.0.2
- **Updated**
    - New minor *secret effect* for Helm of Domination (a simple reference)
    - Changed Bomb grab and drop sound effects
    - The King item now requires battery to be used
    - Updated to v65

## 2.0.1
- **Fixed**
    - Fixed Basics of architecture not working since last update

# 2.0.0 Improvements
- **Added**
    - Added Bomb
- **Updated**
    - New effect for Harry Mason: when you drop it, you have 5% chance of *something* happening hehehe
    - Scroll of Town Portal can now also be used when the ship is leaving
    - Stick can now also be used when in orbit
    - Added info message when you try to use The friendship ender and Scroll of Town Portal at the wrong moment (in orbit for example)
    - Removed LethalNetworkAPI dependency, every network effect is now managed by classic unity netcode, this allows all sort of fixes
- **Fixed**
    - Adjusted position of Rupee, SODA, Basics of architecture and Galvanized square steel when they are droped in the ship's cupboard
    - Fixed The friendship ender spawning a strange entity that can't kill people??
    - Fixed Basics of architecture usage, it can now be used by all players, not just the host
    - Scroll of Town Portal will now be properly destroyed when used by someone
    - My ■■■■■■th job application spawning things effects are now more consistent, and not random
    - My ■■■■■■th job application special effect will now be stopped if the item somehow despawn by a non natural way
    - Fixed Balan Statue glowing rainbow variant having a lower part of the wrong material
    - Fake airhorn audio is now properly assigned to the item for other players (this was already the case for the local player)
    - The friendship ender and Scroll of Town Portal audio are now properly assigned to the item
    - All items will now spawn with isInFactory flag enabled, this should fix metalic items getting struck by lightning when inside the dungeon

## 1.9.1
- **Fixed**
    - Fixed Fake airhorn audio playing twice for other players

# 1.9.0 New scraps
- **Added**
    - Added My ■■■■■■th job application, Moogle, El Gazpacho and The talking orb
- **Updated**
    - New effect for crouton: you can now walk on it to make a *special* sound
    - When Basics of architecture is spawning an item, it will now also play a funny sound
    - Fake airhorn's second effect has a new animation
    - Fake airhorn audio now has some sound variation just like the real airhorn. But I'm not that evil so the variations are *not exactly* the same as the real one... anyways good luck !
    - Reduced Stick spawn chance
    - Updated README and mod icon
- **Fixed**
    - The Stick special effect has been completely fixed and is now re-enabled !
    - Basics of architecture 'turning page' audio is now properly assigned to the item and is synced to all players
    - Fake airhorn audio is now properly assigned to the item and can be heard by monsters
    - Optimization of custom effects code

##

<details><summary>Old versions (click to reveal)</summary>

###

## 1.8.4
- **Updated**
    - Updated Frieren, Ainz Ooal Gown, Mystic Cristal and The friendship ender grab animation
    - All scraps will now drop ahead of players (not directly below them) when not in the ship
    - Added some cause of deaths for certain items and camera shake for The friendship ender
- **Fixed**
    - Fixed Fake airhorn and The friendship ender killing the host player by sending the dead body in the void

## 1.8.3
- **Updated**
    - Added an additional *bad* effect for the Fake airhorn, it will be randomnly triggered when used
    - Increased Fake airhorn explosion non-lethal damage to 50
    - Reduced Basics of architecture scrap value
- **Fixed**
    - Fixed Basics of architecture bug where it could spawn on the ship after beeing used despite vanishing, it will now never disapear even if used
    - Fixed Fake airhorn sometimes not working for other players

## 1.8.2
- **Updated**
    - Changed the Chocobo audio to be the one from FF7
- **Fixed**
    - Fixed the new colors of SODA not beeing metalic

## 1.8.1
- **Updated**
    - Added 3 color variations to SODA and 3 new colors for the Rupee (9 in total)
    - Added a custom grab tooltip to Ea-Nasir Statue
    - Increased Fake airhorn explosion range but it will deal 30 damage instead of killing players if they are a little bit far from the origin
    - Updated some spawn chance
    - Reduced The friendship ender audio
- **Fixed**
    - Temporarily disabled the Stick special effect since it's a little unstable, you can still find the item but it will not have any special effect for now
    - Fixed Galvanized square steel material

# 1.8.0 Improvements
- **Added**
    - Added Galvanized square steel
- **Updated**
    - Updated to v60/v61+
    - New effect for Basics of architecture: something special will happen when you have finished reading the book
    - Increased Fake airhorn spawn chance
    - Updated dependencies
- **Fixed**
    - Fixed issues with LethalNetworkAPI new versions 3.0.0+ (I'm still using the old structure but it's working as intended)

## 1.7.3
- **Updated**
    - Updated to v55/v56
    - Reduced Stick audio
- **Fixed**
    - Reverted Fake airhorn explosion range back to version 1.6.0

## 1.7.2
- **Updated**
    - Updated to v55-beta

## 1.7.1
- **Fixed**
    - Removed items left accidentaly in the shop

# 1.7.0 New scraps
- **Added**
    - Added The friendship ender, Scroll of Town Portal, Stick and Basics of architecture
- **Updated**
    - Fake airhorn will now play the real Airhorn sound when used (but it's a little different)
    - Removed the '?' from the Fake airhorn tooltip, use the sound to tell if it's the real one
    - Improved Fake airhorn explosion to be more dangerous and random
    - Updated some spawn chance
    - Updated dependencies
    - Updated README
- **Fixed**
    - Reduced every audio imported quality
    - Reduced Chocobo and Puppy Shark texture quality
    - Fixed Ainz Ooal Gown grab animation
    - Optimized again Ainz Ooal Gown model (it's now extremely low poly)

## 1.6.2
- **Updated**
    - Added a config file to set every spawn chance as you like
    - Increased Comically Large Spoon audio

## 1.6.1
- **Updated**
    - All scraps can now be grabbed before game start
    - Reduced Balan Statue audio
- **Fixed**
    - Optimized Ea-Nasir Statue model

# 1.6.0 Improvements
- **Updated**
    - New effect for The King and Puppy Shark: they can now be used to make some funny sound
    - Added an emissive texture for Helm of Domination and HearthStone Card
    - Increased Comically Large Spoon damage and weight
    - Reduced Comically Large Spoon spawn chance
    - Increased The King audio
- **Fixed**
    - Fixed Chocobo, The King and Puppy Shark grab animation
    - Reduced Fake airhorn explosion range
    - Reduced Mystic Cristal texture quality
    - Optimized Helm of Domination model

# 1.5.0 New scraps
- **Added**
    - Added crouton, Fake airhorn and Balan Statue
- **Updated**
    - Added a rare glowing color variation to Comically Large Spoon
    - HearthStone Card can now be inspected
    - Reduced Helm of Domination, Ea-Nasir Statue and Ainz Ooal Gown weight
    - Updated dependencies (added LethalNetworkAPI)
    - Updated README (+github repo)
- **Fixed**
    - Fixed Comically Large Spoon damage desync bug
    - Reduced HearthStone Card texture quality

# 1.4.0 Improvements
- **Added**
    - Added Comically Large Spoon
- **Updated**
    - Added 6 color variations to the Rupee
    - Added a tool tip to The King
    - Increased Helm of Domination scrap value
    - Reduced Ea-Nasir Statue weight
- **Fixed**
    - Fixed Ea-Nasir Statue grab animation
    - Fixed Rupee texture beeing too dark
    - Optimized Mystic Cristal transparency and model

## 1.3.1
- **Updated**
    - Reduced HearthStone Card, Rupee and The King scrap value
    - Reduced HearthStone Card and SODA audio

# 1.3.0 New scraps
- **Added**
    - Added Rupee, Ea-Nasir Statue, HearthStone Card and SODA
- **Updated**
    - Updated Helm of Domination grab animation
    - Increased Helm of Domination spawn chance and weight
    - Reduced Harry Mason scrap value
    - Reduced Frieren and Helm of Domination textures quality
    - Reduced Puppy Shark audio
- **Fixed**
    - Optimized, fixed textures and reduced light intensity of Ainz Ooal Gown model

## 1.2.2
- **Updated**
    - Updated README
- **Fixed**
    - Optimized Puppy Shark model

## 1.2.1
- **Updated**
    - Increased Ainz Ooal Gown scrap value
    - Reduced Mystic Cristal spawn chance

# 1.2.0 New scraps
- **Added**
    - Added Harry Mason, Mystic Cristal and Puppy Shark
- **Updated**
    - Increased The King spawn chance
    - Updated mod icon
- **Fixed**
    - Fixed Helm of Domination displayed name
    - Fixed The King audio

## 1.1.1
- **Fixed**
    - Fixed typo in README

# 1.1.0 New scraps
- **Added**
    - Added Chocobo, Ainz Ooal Gown, Helm of Domination and The King
- **Updated**
    - Increased Frieren scrap value
    - Updated mod icon

# 1.0.0 Initial release
- **Added**
    - Added Frieren

</details>