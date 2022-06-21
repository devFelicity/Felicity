# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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