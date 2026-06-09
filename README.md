# PUNI Life MVP

Unity 6 LTS용 2D 모바일 육성 게임 프로토타입입니다. 빈 씬에서 Play를 눌러도 `PuniLifeBootstrap`이 카메라, UI, 게임 매니저를 생성합니다.

## 실행

1. Unity Hub에서 `/Users/sangjin/Documents/sjworkspace/punilife` 폴더를 프로젝트로 엽니다.
2. Unity가 패키지를 복원할 때까지 기다립니다.
3. 빈 씬에서 Play를 누릅니다.

## 프로젝트 설정

- Product Name: `PUNI Life`
- Bundle ID: `com.sangjin.punilife`
- Version: `0.1.0`
- Android 우선 출시 기준
- Portrait 모바일 화면 기준
- 최소 Android SDK: 23

## 포함된 범위

- 푸니 상태값 5개: Hunger, Happiness, Cleanliness, Energy, Affection
- 숨겨진 성장 스탯: Intelligence, Strength, Sensitivity, Courage, Kindness, Neglect
- 행동 6개: Feed, Play, Clean, Sleep, Study, Train
- JSON 로컬 저장
- 저장 파일 백업 및 임시 파일 기반 저장
- `lastSavedAt` 기준 오프라인 진행
- 4단계 성장: Egg, Baby, Young, Evolved
- 5종 진화: Sunny, Scholar, Brave, Forest, Shadow
- 도감 등록과 가든 복원 단계
- 도감/가든 MVP 패널
- Portrait 기준 런타임 생성 메인 UI
- 상점 확장용 아이템 데이터 모델
- Snack Tap 미니게임 MVP: 10초 타이머, 점수, 콤보, 2배 광고 보상 Mock
- 보상형 광고 연결용 `AdManager` Mock: 무료 간식, 상태 회복, 미니게임 코인 2배, 진화 힌트

## 문서

- `PUNI_Life_GDD.md`: 게임 방향과 핵심 시스템
- `PUNI_Life_Unity_Game_Guide.md`: Unity 구현 가이드
- `Assets/Scenes/UnityValidationChecklist.md`: Unity Editor 검증 체크리스트
- `IMPLEMENTATION_STATUS.md`: 현재 구현 상태와 미완료 항목
- `ANDROID_RELEASE_CHECKLIST.md`: Android 빌드/출시 전 체크리스트
- `BALANCE_AND_TEXT.md`: 케어 액션, 진화 루트, UI 문구 기준
