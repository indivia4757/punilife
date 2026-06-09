# PUNI Life Scenes

MVP는 빈 씬에서도 `PuniLifeBootstrap`이 런타임에 카메라, UI, `GameManager`를 생성하도록 구성되어 있습니다.

권장 씬 구성:

- `BootScene`: 저장 데이터 로드 및 초기화 전용
- `GameScene`: 메인 케어 화면
- `MiniGameScene`: Snack Tap 확장용
- `DexScene`: 도감 확장용
- `ShopScene`: 상점 확장용

현재 단계에서는 빈 `GameScene` 하나만 만들어 Play를 눌러도 동작하도록 유지합니다.
