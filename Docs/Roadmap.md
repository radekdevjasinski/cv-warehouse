# Roadmap

The final stretch of the project. The game works end to end, but a player who buys everything that turns green beats it in about two minutes with no real decisions. The goal is to give the run a target and some replayability without adding new systems, then wrap up.

Each step is done in its own session, on its own branch, and updates `Docs/GameDesign.md` in the same change.

## 1. Drop specializations and project upgrades

Leftovers from the first ideas of the game. They are described in the design but never built.

- Remove the `effect` field from the CV JSON, `CvEntryDto`, `CvEntry`, `CvEntryMapper` and their tests.
- Update `Docs/GameDesign.md`: remove the Specializations bullet, "Upgrades: named after projects", the specializations part of "Actions" and the `effect` field under CV Data.
- `category` stays, but only as the colour of a box. Say so in the design.
- Commit as a `refactor:` on its own, before any feature work.

## 2. Time and medals

The main source of depth once specializations are gone: a target that makes "buy a bot or upgrade one" and the choice of focus matter.

- Show the elapsed time on screen during the run, together with the next medal target, for example `01:42 / gold 01:30`.
- Bronze, silver and gold thresholds are scaled to the size of the CV, like the prices: total letters × seconds per letter × a multiplier per medal. The multipliers live in a ScriptableObject.
- Calibrate on the real CV: a run that just buys everything that turns green should land around silver, and gold should need smart play.
- The end screen shows the time and the medal earned.
- Skip and show CV ends the run without a medal, but still shows the CV.
- Optional: keep the best time per CV file in `PlayerPrefs` (IndexedDB in WebGL, so it survives a reload).
- Tests: threshold scaling and medal selection in EditMode; timer, pause handling and the skip path in PlayMode.

## 3. Section-complete milestones

A small reward that also pulls the reader's eye to the CV content.

- When the last letter of a section is delivered, the section flashes on the waybill, a short sound plays and a points bonus pops up.
- Reuse what exists: `PointsPopup`, the points counter pulse and the audio settings asset.
- No choices or new upgrades at a milestone. It stays cosmetic plus points.
- The bonus size is a share of all the points in the warehouse, like the prices, so it scales with the CV.
- Tests: section completion detection and bonus scaling in EditMode.

## 4. Play again with a new seed

The cheapest replayability. Do it before fictional CVs and check whether it is already enough.

- The end screen gets a "play again" button that starts a new run with the same CV and a new seed.

## 5. Fictional CVs (optional)

Only if step 4 does not feel like enough. The cost is the writing, not the code.

- Start with two fictional CVs, not more, as extra JSON files in `StreamingAssets`.
- A CV picker on the end screen (or on the start screen).
- Medals and best time are kept per CV file.
- Every file must pass the same generator and parsing tests as the real CV.
