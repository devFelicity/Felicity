# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [7.6.1]
### Added
- Added `/pb raids`, for now only uses stat trackers until the PGCR collection is in place.

## [7.6.0]
### Added
- Metrics for usage reports! (finally)

### Changed
- Embed colors to match rebrand.

## [7.5.1]
### Added
- Backend automations.

## [7.5.0]
### Added
- New command `/lookup guardian-ranks`.

### Changed
- New logo.

### Removed
- Marsilion from recipes (not obtainable currently).

## [7.4.2]
### Changed
- Fixed an issue where profiles were falsely being returned as private.

## [7.4.1]
### Changed
- Fixed an issue where private profiles would cause an error in emblem commands.

## [7.4.0]
### Added
- `/emblem rarest` command, check your rarest emblems in collections.

### Changed
- `/lookup account-share` is now renamed to `/emblem shares` to fit with changes.

## [7.3.3]
### Added
- Missing Vexcalibur and Revision Zero from /crafted.

## [7.3.2]
### Added
- Missing Rufus's Fury from /crafted.

## [7.3.1]
### Changed
- Fixed an issue where Bungie profiles with no Destiny account would crash commands.

## [7.3.0]
### Added
- A minor joke.

### Changed
- Updated deps.

## [7.2.1]
### Changed
- Fixed `/lookup guardian` showing the wrong Season rank.

## [7.2.0]
### Added
- Root of Nightmares loot table.
- Root of Nightmares craftables.

## [7.1.0]
### Added
- New `/vendor ada-1` command, find your missing shaders.

### Changed
- Removed caching for gunsmith inventory to fix issues created by Lightfall rework. This will be changed again to reflect the new sales in the future.

## [7.0.3]
### Added
- Missing Dimensional Hypotrochoid from craftables.

## [7.0.2]
### Changed
- Fixed an issue where recipes would crash.

## [7.0.1]
### Added
- Missing craftable weapons.

## [7.0.0]
### Added
- Backend start for new weapon sources.

### Changed
- Lightfall update.
- Updated recipes and craftable weapons.
- Updated dependencies.
- Updated Saint-14 inventory.
- Moved recommended rolls to d2foundry.gg
- Fixed an issue where the `/user remove` command wouldn't output anything despite functioning correctly.

### Removed
- `/byte` command.
- Unnecessary Discord permission requests (no effect to servers).

## [6.19.4]
### Added
- Backend method for updating manifest without needing to restart the bot.

### Changed
- Error handler now shows how to fix "Refresh token has expired" error.
- Epicurean and Fixed Odds no longer show as purchasable from vendors.

## [6.19.3]
### Changed
- Updated NuGets

## [6.19.2]
### Changed
- Fixed checkpoint API returning null causing command to crash.
- Fix clarity changes causing mods command to crash.
- Removed unnecessary loop causing mods command to run for longer than it needed.

## [6.19.1]
### Changed
- Backend logging ugly fix.

## [6.19.0]
### Added
- d2checkpoint.com commands.

### Changed
- Update libs.
- Update Clarity db.

## [6.18.1]
### Added
- Spire of the Watcher loot table.

## [6.18.0]
### Added
- Collectibles check for `/lookup guardian` so you can see if players have acquired popular / meta weapons.
**Bear in mind this will only apply for collections and cannot actually check if a user has said item in their inventory.**
- Also added season rank, raid completions, triumph score and will now link to the related Bungie.Net profile.

### Changed
- Sites full names in guardian lookup.

## [6.17.2]
### Changed
- Fixed an issue where UserCannotResolveCentralAccount would crash the command.
- Fixed an issue where certain commands through DMs would crash.

## [6.17.1]
### Changed
- Fixed accidental deletion of the most important part of the command.

## [6.17.0]
### Added
- Bungie API checks to allow more understandable errors.

## [6.16.0]
### Added
- Season 19 (Worthy 2.0) weapon rolls.

## [6.15.0]
### Changed
- Updates for S19

## [6.14.3]
### Removed
- Removed invisible crafted weapon patterns due to them not being in the game yet, promise I'll respect the visibility next time.

## [6.14.2]
### Changed
- Fixed an issue where Twitch usernames with underscores would break URLs in message content.

## [6.14.1]
### Changed
- Fixed an issue where War Table deepsight being claimed will crash /recipes.
- Added ticks for better /recipes readability.

## [6.14.0]
### Added
- A little money bag to the /recipes command if you can currently buy a deepsight from the vendors.

## [6.13.3]
### Changed
- Fixed an issue causing Saint-14 command to crash.

## [6.13.2]
### Changed
- Fixed an issue where twitch usernames ending with an underscore would cause Discord to attempt formatting as italics.

## [6.13.1]
### Changed
- Slight tweaks to `/crafted` command that will now show when multiple instances of the weapon are found and only display highest level.

## [6.13.0]
### Added
- New command `/crafted` (shows weapons that you have crafted in your inventory and their levels.)
- If reading this and you have an Osteo Striga with a weapon level over 5000, please message us a screenshot... and then seek help.

### Changed
- Changed "Unknown" to "Quest / Unknown", the Quest category only has 2 items for now so not worth it's own.
- Reverted the change to `/recipes hide-complete` => `/recipes show-complete` as it makes more sense.

## [6.12.0]
### Added
- Backend management commands.

### Removed
- `/checkpoint` commands.

## [6.11.0]
### Added
- King's Fall craftables.
- King's Fall loot table.

### Changed
- Removed optional parameters from `/recipes`.
- Update dependencies.

## [6.10.2]
### Changed
- Added KingsFall and Plunder weapon rolls.

## [6.10.1]
### Changed
- Fixed an issue where weapons with multiple rolls would have multiple options in autocomplete.

## [6.10.0]
### Added
- King's Fall loot table (missing weapons due to classified items)
- New command: `/roll-finder` uses curated weapon rolls to provide recommended rolls for PvE/PvP

## [6.9.0]
### Added
- New craftable weapons.
- Preparations for upcoming release.

### Changed
- Fixed Saint14's inventory change due to new season.
- Updated dependencies.
- Fix `/memento` command relying on the source parameter.

## [6.8.1]
### Changed
- Bug fixes.
- `/lookup account-share` can now be used without arguments to look yourself up. Not sure why you'd want it but it's there.

## [6.8.0]
### Added
- Clarity for DIM: integrated into `/vendor mods`

## [6.7.3]
### Changed
- Emergency fixes.

## [6.7.2]
### Changed
- Rename Leviathan => Opulent

## [6.7.1]
### Changed
- Removed status that was causing it to clear.
- Visual updates for `/lookup account-share`.
- Visual updates for Twitch embeds.
- Move vendor icon back to author to prevent unwanted line wrapping.

## [6.7.0]
### Added
- Status cycling service.

### Changed
- Randomise suggestions for `/metrics` and reformat autocompletes.

## [6.6.0]
### Added
- `/metrics` command to fetch various information from stat trackers / profile metrics.

### Changed
- Updated dependencies.

## [6.5.3]
### Changed
- Fixed an error if an emote was deleted from emote banks.

## [6.5.2]
### Changed
- Fixed an error caused by Saint-14 category change.

## [6.5.1]
### Changed
- Added emotes to vendor items to show their item type.

## [6.5.0]
### Added
- `/vendor gunsmith` shows Banshee-44 weapon inventory.

### Changed
- Updated API libraries.

## [6.4.3]
### Changed
- Added master challenge completion requirement for timelost weapons.

## [6.4.2]
### Changed
- Multiple bug fixes and hopefully auto-reconnect on server disconnect.

## [6.4.1]
### Changed
- Fixed `/vendor saint14` throwing a `key not present` error.

## [6.4.0]
### Added
- Timelost weapons to VoG loot tables.
- Dungeon loot tables.

## [6.3.2]
### Added
- Last Wish loot table.

## [6.3.1]
### Changed
- Updated checkpoint parser to new format.

## [6.3.0]
### Added
- Added `/loot-table` command. Search raid (dungeons coming soon) loot per encounter.

## [6.2.3]
### Changed
- Fixed `/byte` not working.

## [6.2.2]
### Added
- Add cache busting to twitch live thumbnails.

### Changed
- Mention users on join/leave for easier administration.
- Fixed account-share loading incorrect platforms, leading to "error loading clan" messages from Bungie.net.

## [6.2.1]
### Changed
- Added error handling in `accountshare` command.

## [6.2.0]
### Added
- New `/support` command for finding useful links related to Felicity.

### Changed
- Allowed hiding of certain fields in `/recipes`, this will allow for easier viewing once more categories get added throughout the seasons.
- Fixed a crash when commands were ran in DMs with the bot.
- Fixed an issue where logging was not working as intended.
- Fixed an issue where oauth flow could fail due to visibility issues.

## [6.1.0]
### Added
- Felicity server changelog channel is now an announcement channel, you can now follow the channel for updates in your own server.

### Changed
- Slight change to `/vendor mods` format.
- Eyecandy changes to `/recipes`.

## [6.0.5]
### Changed
- Fixed `/recipes` not showing red frames in inventory properly.

## [6.0.4]
### Changed
- Error handling got a little TOO keen on helping out.

## [6.0.3]
### Added
- Docker runtime. (also using this to test automation lol)

### Changed
- Fixed oauth token refresh causing command to fail.

## [6.0.2]
### Changed
- Fixed Twitch Service crashing on stream going offline.
- Fixed VOD-less stream duration being offset.

## [6.0.1]
### Added
- Docker backend support
- Proper error reporting

### Changed
- Fixed twitch streams not being tracked
- Fixed twitch streams not posting properly due to new config setup

## [6.0.0]
### Added
- Honestly... too much to remember.
- `/server` command revamp. Now groups everything.
- `/server summary`, check on your server settings.

### Changed
- Rebuilt on aspnet... Painful, but worth it.

### Removed
- Many `/server` commands have been grouped into a single new command.

## [5.3.2] - 2022-06-21
### Changed
- Change up Twitch embeds to show the game icon for easier eyeballing.

## [5.3.1] - 2022-06-20
### Changed
- Fixed TWAB search logic to (hopefully) include all TWABs.

## [5.3.0] - 2022-06-17
### Added
- `/lookup wish` command: view wishes from the Last Wish raid for easy reference

## [5.2.1] - 2022-06-16
### Changed
- fixed `/recipes` searching for sunset versions of Austringer and Drang (Baroque)

## [5.2.0] - 2022-06-14
### Added
- `/recipes` now shows red frames from your inventory

### Changed
- `/recipes` now defaults to hiding completed frames to prevent spam, you can still view completed frames by using `/recipes hidecomplete: false`

## [5.1.3] - 2022-06-13
### Changed
- Something very tiny, just to trigger CI deploy.

## [5.1.2] - 2022-06-13
### Added
- Changelog integration with CI

## [5.0.0] - 2022-06-01
### Added
- proper error reporting
- automatic manifest updates, no more crashes on updates

### Fixed
- checkpoints failing to autocomplete
- mementos failing to autocomplete
- hanging commands due to localization errors
- failing to fetch saint-14 inventory
- "Felicity is thinking" instead of returning error message

### Removed
- "member banned" alert setting since this is unreliable and also triggers member left, plus... audit log is a thing...

## [4.3.0] - 2022-05-15
### Added
- /checkpoint command.
> Quickly get join codes for active checkpoints or contact info for saved checkpoints

### Fixed
- Twitch embeds are now Green or Red depending on stream status for easier visibility

## [4.2.0] - 2022-05-10
### Added
- `/vendor saint14` command
> View Saint-14's weekly rotating perks on weapons according to your current rank

### Fixed
- Twitch streams no longer post multiple times

## [4.1.0] - 2022-05-02
### Added
- `/memento` command
> View how a weapon will look with the memento shader applied, useful if you don't want to use your memento to see what it'll look like
- `/vendor xur` command.
> Pulls Xur's inventory from API and shows his location, perks, stats and armor sets

### Fixed
- Twitch streams can now be added to multiple servers.
