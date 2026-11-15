## [0.5.0](https://github.com/Eugene8388608/R.E.P.O.-Unremove/tree/v0.5.0) - 2026-11-15
* Promoted `Module - Shop - DE - Barricaded Door` to playable rooms.
* Single Walkie Talkies now spawn with different colors, just like pairs of them.
* Put `[HarmonyWrapSafe]` all over the place. This means that the game will less likely hang in case of exceptions in mod's patches.

`Module - Shop - DE - Barricaded Door` was originally added to debug modules because I thought it was broken. Turns out the door wasn't breaking because [REPOFidelity v1.8.2](https://thunderstore.io/c/repo/p/Vippy/REPOFidelity/v/1.8.2/) has a bug that also prevents breaking the wall in shop diner.

Max amount of single Walkie Talkies is one apparently. I use [MoreShopItems Updated](https://thunderstore.io/c/repo/p/Jettcodey/MoreShopItems_Updated/) mod to increase it.

## [0.4.0](https://github.com/Eugene8388608/R.E.P.O.-Unremove/tree/v0.4.0) - 2026-11-09
* General fixes regarding R.E.P.O. v0.5.0 update.
* Unremoved Arctic's `M - 1 - Cargo Central`, `N - 3 - Sector S` and `P - 2 - Frozen Medbay` modules.
* Config option for unremoving `Enemy - Virus` in singleplayer mode.
* Config option for fixing vanilla debug modules not being able to spawn.
* Made 16 new modules available via debug console.
* Recharge Orb has been released.
* `Module - Manor - N - 2 - Dining Hall 2` has been released as `Module - Manor - N - 2 - Gallery Hall`.
* `Module - Museum - N - 1 - Monika's Room` is gone.

## [0.3.0](https://github.com/Eugene8388608/R.E.P.O.-Unremove/tree/v0.3.0) - 2026-10-25
* Unremoved `Module - Museum - N - 2 - Basement Gallery` and `Module - Museum - N - 2 - Smaller rooms and stairs wip`.
* Unremoved `Module - Manor - N - 2 - Dining Hall 2` and `Module - Museum - N - 1 - Monika's Room` as debug-only modules.
* Config option to enable `Start Room - Manor - TV Room` as it is lacking an extraction point.
* Lowered max amount of Throw Upgrades in shop from 10 to 3.
* Split one big source code file.

## [0.2.0](https://github.com/Eugene8388608/R.E.P.O.-Unremove/tree/v0.2.0) - 2026-10-08
* Rewritten using Harmony postfixes.
* Config option "Add Technically Disabled Items". As of R.E.P.O. v0.4.4 just adds single Walkie Talkies.
* Pair each never paired single Walkie Talkie to itself.
* Unremoved Valuable Manor Marble Table.

## [0.1.0](https://github.com/Eugene8388608/R.E.P.O.-Unremove/tree/v0.1.0) - 2026-10-03
Patching and adding Throw Upgrade Item with a BepInEx config entry to enable/disable it.

## [0.0.0](https://github.com/Eugene8388608/R.E.P.O.-Unremove/tree/v0.0.0) - 2026-09-28
Initial release. Enabling each disabled Item and adding it to `StatsManager`.
