# Unity Validation Checklist

Use this after opening the project in Unity 6.4.

## Editor

- Console has no C# compile errors.
- `Assembly-CSharp.dll` is generated under `Library/ScriptAssemblies`.
- Empty scene Play creates camera, UI, and `GameManager`.
- `PUNI Life` title appears.
- No duplicate Canvas appears after entering Play more than once.

## Core Loop

- Feed changes Hunger, Affection, Coin.
- Play changes Happiness, Energy, Affection, EXP.
- Clean and Sleep unlock after Egg becomes Baby.
- Study and Train unlock at Young.
- Level increases when EXP reaches `Level * 20`.

## Save

- Save file is created at `Application.persistentDataPath/puni_save.json`.
- Relaunch loads the same status.
- Backup file is created after the second save.
- Offline progress changes Hunger, Happiness, Cleanliness, Energy, and Neglect.

## Systems

- Dex button opens the PUNI Dex panel.
- Evolved PUNI unlocks one dex entry.
- Garden restoration count follows dex unlock count.
- Snack Tap runs for 10 seconds.
- Snack Tap Claim grants coins, Happiness, and EXP.
- Snack Tap 2x Ad doubles coin reward through Mock AdManager.
- Free Snack and Recover buttons trigger Mock rewarded ad rewards.
- Debug button opens the Debug panel.
- Debug EXP +50 changes level/EXP.
- Debug Offline 12h applies offline progress.
- Debug Force Evolve unlocks one evolution route.
- Debug Reset Save returns data to initial values.

## Android

- Build platform is switched to Android.
- Product Name is `PUNI Life`.
- Package Name is `com.sangjin.punilife`.
- Orientation is Portrait.
- Android Build Support, SDK/NDK Tools, and OpenJDK are installed.
