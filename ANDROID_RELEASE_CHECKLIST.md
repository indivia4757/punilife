# Android Release Checklist

## Unity Hub

- Unity 6.4 `6000.4.10f1` installed.
- Android Build Support installed.
- Android SDK & NDK Tools installed.
- OpenJDK installed.

## Unity Project

- Open `/Users/sangjin/Documents/sjworkspace/punilife`.
- Confirm Console has no C# errors.
- Save a scene at `Assets/Scenes/GameScene.unity`.
- Add `GameScene` to Build Profiles / Build Settings.
- Switch Platform to Android.

## Player Settings

- Product Name: `PUNI Life`
- Package Name: `com.sangjin.punilife`
- Version: `0.1.0`
- Bundle Version Code: `1`
- Default Orientation: Portrait
- Minimum Android SDK: 23 or higher
- Target Android SDK: installed SDK target

## Before First APK/AAB

- Replace placeholder PUNI art.
- Add app icon.
- Add splash/background art if needed.
- Test save/load after closing the app.
- Test offline progress by changing device time or using Debug panel.
- Test Snack Tap reward.
- Test Dex/Garden unlock through Debug panel.

## Google Play Later

- Build as `.aab`.
- Prepare privacy policy URL if ads or analytics are used.
- Replace Mock `AdManager` with AdMob SDK.
- Configure AdMob app ID and ad unit IDs.
- Review content rating.
- Prepare screenshots, short description, full description, feature graphic.
