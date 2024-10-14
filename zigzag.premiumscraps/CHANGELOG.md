# 2.0.0 Improvements
- **Added**
    - Added Bomb
- **Updated**
    - New effect for Harry Mason: when you drop it, you have 2% chance of *something* happening hehehe
    - Removed LethalNetworkAPI dependency, every network effect is now managed by classic unity netcode, this allows all sort of fixes
- **Fixed**
    - Fixed position of Rupee, SODA, Basics of architecture and Galvanized square steel when they are droped in the ship's cupboard

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