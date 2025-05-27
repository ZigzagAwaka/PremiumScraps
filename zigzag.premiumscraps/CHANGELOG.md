## 2.3.6
- **Updated**
    - Added 1 new event for The friendship ender effect if you have [LethalAnomalies](https://thunderstore.io/c/lethal-company/p/Zeldahu/LethalAnomalies/) installed
    - Modified some effects probabilities for The friendship ender
    - Improved material visuals for some items
    - Improved audio loading
    - Optimized Frieren model
- **Fixed**
    - Fixed compatibility with the latest version of [Emergency_Dice_Updated](https://thunderstore.io/c/lethal-company/p/slayer6409/Emergency_Dice_Updated/)
    - When playing with the french translation active, it is now recommended to install [FontUpdate](https://thunderstore.io/c/lethal-company/p/rectorado/FontUpdate/) to fix certain characters from being displayed weirdly

## 2.3.5
- **Fixed**
    - Fixed Serpent of Ronka not getting back their correct variation name when reloading the lobby

## 2.3.4
- **Fixed**
    - Fixed Helm of Domination unlucky effect being global

## 2.3.3
- **Updated**
    - Added a custom condition for [SelfSortingStorage](https://thunderstore.io/c/lethal-company/p/Zigzag/SelfSortingStorage/) to prevent certain items from being stored
    - Updated ShipInventory soft dependency to now target [ShipInventoryUpdated](https://thunderstore.io/c/lethal-company/p/LethalCompanyModding/ShipInventoryUpdated/), and updated some items conditions hehehe
- **Fixed**
    - Basics of architecture state is now saved when reloading the lobby
    - Serpent of Ronka random variation is now saved when reloading the lobby

## 2.3.2
- **Updated**
    - [StarlancerAIFix](https://thunderstore.io/c/lethal-company/p/AudioKnight/StarlancerAIFix/) is now a thunderstore dependency, but it is optional so you can remove it if you don't like it

# 2.3.1 The 2.3 improvements part 2
- **Serpent of Ronka**
    - A new scrap!
- **Controller**
    - Added compatibility with [OpenBodyCams](https://thunderstore.io/c/lethal-company/p/Zaggy1024/OpenBodyCams/), the item screen will now use a Body Cam instead of the normal one if you have this mod installed (a config has been added to deactivate this compatibility if you want)
- **The King**
    - Audio from noisemaker is now synced
    - New animation when used
    - New rare effect and *special* unlucky effect
    - Adjusted position in hand
- **Galvanized square steel**
    - Can no longer be pocketed because it's two handed
- **Helm of Domination**
    - Special audio is now properly assigned to the item
- **HearthStone Card**
    - New variant drop sfx
    - New *special* unlucky effect
- **Misc**
    - Added a complete french mode that is automatically activated if your system is in french (translate some tool tips, effects messages and change some sounds)
    - The config `Language mode` has been added to manually change the language if you want
    - Re-added the Gazpacho sound config but it's now working under the french mode

# 2.3.0 The 2.3 improvements part 1
- **Controller**
    - Can now be used on players driving the cruiser
    - New *special* unlucky effect
    - Added something to try improving compatibility with mods that change player body layer (to fix player bodies being invisible on the item screen)
    - Fixed antena sometimes not despawning after stoping the control mode
- **Fake airhorn**
    - New *special* unlucky effect (this one is pretty straight forward)
    - New animation when the user is about to die, and also new visuals for the actual effect
- **Harry Mason**
    - Adjusted the unlucky effect to be more dangerous
- **My ■■■■■■th job application**
    - Re-added the effect to manipulate the weather but it's now a combined weather (only works if you have [WeatherRegistry](https://thunderstore.io/c/lethal-company/p/mrov/WeatherRegistry/) installed, if not will have no effect)
    - Summoning *"employees"* now work on modded company-like moons
    - Some effects are a bit more dangerous
    - The unlucky effect is now even more dangerous
- **Stick**
    - The ultimate knockback effect which was 5% chance, is now 40% when used on an unlucky player
- **Helm of Domination**
    - New *special* unlucky effect
- **Galvanized square steel**
    - Unlucky effect is now more frequent
- **Puppy Shark**
    - New animation when used
- **Scroll of Town Portal**
    - Sound effect is now played at position for players not using the item
- **The friendship ender**
    - Can now be used 3 times max per moons
- **Misc**
    - Updated README to explain unlucky players
    - Changed the "Premium Scraps" dice event to include the Controller in the spawn pool 
    - Removed some configs that are not needed anymore

# 2.2.0 The Controller update
- **Added**
    - Added Controller
- **Updated**
    - New *special* unlucky effects for: Harry Mason, El Gazpacho and The talking orb
    - Removed My ■■■■■■th job application's ability to manipulate the weather, but a config has been added to revert this change if you want
    - Galvanized square steel can now be used as a weapon, with a special unlucky effect hehe (configurable with a config)
- **Fixed**
    - Added a custom condition for [ShipInventory](https://thunderstore.io/c/lethal-company/p/WarperSan/ShipInventory/) to prevent certain items from being stored
    - Fixed item tool tips sometimes overlaping with other tool tips
    - Basics of architecture state is now synced for late join players
    - Small fix for Harry Mason secret effect
    - Optimized The talking orb's code

##

<details><summary>Old versions (click to reveal)</summary>

###

## 2.1.1
- **Updated**
    - Bomb
        - New animation, new sound and new particle system
    - Stick
        - Improved knockback effect so you can influence the direction of the knockback by looking at where you want to sent the other player (for example, if you are crouching and look up while hitting the player it's going to make it go very high)
        - Added 5% to use the ultimate knockback effect
    - El Gazpacho
        - Changed grab and drop sounds to be more vanilla friendly
        - Added a config to revert this change if you want
    - Misc
        - Improved the collider of small items so it's easier to grab
        - Christmas assets will now automatically be applied every year during christmas period
- **Fixed**
    - Surviving the Death hallucination dice event will now properly stop the event

# 2.1.0 Christmas update
- **Added**
    - New christmas assets for all items !
- **Updated**
    - Fake airhorn will now play a "warning audio" when the user is about to die (Thank you **Kogi** for the idea!)
    - Updated default spawn chance for some items
- **Fixed**
    - Fixed Fake airhorn "landmine audio" playing incorectly

## 2.0.16
- **Fixed**
    - Fixed an issue with My ■■■■■■th job application

## 2.0.15
- **Fixed**
    - Removed test code of 2.0.14

## 2.0.14
- **Updated**
    - Improved Hazard hallucination both for My ■■■■■■th job application and the custom dice event
- **Fixed**
    - My ■■■■■■th job application is now compatible with [WeatherRegistry](https://thunderstore.io/c/lethal-company/p/mrov/WeatherRegistry/)
    - Fixed an issue with The friendship ender

## 2.0.13
- Update for [Emergency_Dice_Updated](https://thunderstore.io/c/lethal-company/p/slayer6409/Emergency_Dice_Updated/) 1.7.4+, if you have it installed
    - New events:
        - Instrument of legends [Great]
        - Hazard hallucination [Bad]
        - Abibaland [Great]
        - Towers [Awful]
        - Bombs infestation [Bad]
        - Where is everybody? [Mixed]
        - Disturbing noise [Good]
        - Flowerman Academy [Awful]
    - Changed the "Premium Scraps" event to only spawn different items

## 2.0.12
- **Added**
    - Added a config to set custom min,max scrap value for all items (can be left empty for default values)
- **Updated**
    - Better knockback effect for Stick
    - Better teleportation code for Scroll of Town Portal
    - Better audio code for some items
    - Stick can now also be used on players that are performing a special interaction, like when they are on the terminal
    - All Friendship ender events will now be replaced by vanilla ones if you don't have [StarlancerAIFix](https://thunderstore.io/c/lethal-company/p/AudioKnight/StarlancerAIFix/) installed (less fun, please install StarlancerAIFix !)

## 2.0.11
- **Fixed**
    - Fixed The friendship ender breaking if you use it mid-air

## 2.0.10
- **Updated**
    - Summoning *"employees"* with My ■■■■■■th job application will now also change the weather to eclipsed
    - Added 1 new event for The friendship ender effect if you have [LethalAnomalies](https://thunderstore.io/c/lethal-company/p/Zeldahu/LethalAnomalies/) installed
    - The friendship ender got a *special* upgrade when used by unlucky players
    - Nerfed El Gazpacho drunk effect, it's now ending two times faster

## 2.0.9
- **Updated**
    - Harry Mason's secret effect chance has been increased from 5% to 8% hehehe
- **Fixed**
    - Compatibility patch with the latest version of [Emergency_Dice_Updated](https://thunderstore.io/c/lethal-company/p/slayer6409/Emergency_Dice_Updated/) 1.6.5+
        - Should fix the "Premium Scraps" custom dice effect

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
    - Improved scrap spawning custom code (used by Basics of architecture)
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