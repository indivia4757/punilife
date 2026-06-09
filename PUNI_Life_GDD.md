# PUNI Life - Game Design Document

## 한 줄 소개
알에서 태어난 작은 생명체 '푸니'를 돌보고 성장시켜 다양한 진화형을 수집하며, 잊혀진 푸니 가든을 복원하는 캐주얼 육성 게임.

---

# 핵심 컨셉

- 다마고치식 케어
- 방치형 상태 변화
- 프린세스 메이커식 성장 분기
- 진화 수집
- 가든 복원

푸니는 죽지 않는다.
방치하면 상태가 나빠지지만 그것조차 새로운 진화 루트가 된다.

---

# 세계관

인간과 함께 살던 신비한 생명체 '푸니'는 세상에서 잊혀졌다.

플레이어는 우연히 마지막 푸니 알을 발견하게 되고,
푸니를 성장시키며 사라진 푸니 가든을 복원하게 된다.

---

# 게임 루프

앱 실행
→ 푸니 상태 확인
→ 먹이주기 / 놀아주기 / 씻기기 / 재우기 / 훈련
→ 경험치 획득
→ 성장
→ 진화
→ 도감 등록
→ 가든 복원
→ 새로운 콘텐츠 해금

---

# 상태값

- Hunger
- Happiness
- Cleanliness
- Energy
- Affection

## 숨겨진 성장 스탯

- Intelligence
- Strength
- Sensitivity
- Courage
- Kindness
- Neglect

---

# 성장 단계

## Egg
알 상태

## Baby Puni
기본 케어 가능

## Young Puni
훈련 / 공부 / 미니게임 해금

## Evolved Puni
진화 완료 상태

---

# 진화 타입 (MVP)

## Sunny Puni
행복과 애정이 높은 푸니

## Scholar Puni
공부를 많이 한 푸니

## Brave Puni
훈련을 많이 한 푸니

## Forest Puni
친화력과 청결이 높은 푸니

## Shadow Puni
오래 외로웠던 푸니

---

# 푸니 가든

도감을 채울수록 가든이 복원된다.

- 꽃
- 나무
- 연못
- 별빛 배경
- 푸니 하우스

순차 해금

---

# 미니게임

## Snack Tap

10초 동안 나타나는 간식을 터치

보상
- Coin
- Happiness
- EXP

광고 시청 시 보상 2배

---

# 수익화

## 보상형 광고

- 무료 간식
- 코인 2배
- 상태 회복
- 미니게임 추가 플레이
- 진화 힌트

## 향후

- 광고 제거
- 스킨 판매
- 방 꾸미기
- 특별 간식

---

# MVP 범위

- 푸니 1종
- 성장 단계 4단계
- 진화 5종
- 상태값 5개
- 행동 6개
- 미니게임 1종
- 도감
- 가든 복원
- 로컬 저장
- 오프라인 진행
- 광고 구조

---

# Unity 구조

Scenes

- BootScene
- GameScene
- MiniGameScene
- DexScene
- ShopScene

Scripts

- GameManager
- PuniController
- CareSystem
- EvolutionSystem
- SaveManager
- OfflineProgressSystem
- DexManager
- UIManager
- AdManager

---

# 최종 목표

PUNI Life는 단순한 다마고치가 아니라

'푸니를 성장시키고,
다양한 진화를 발견하고,
푸니 가든을 복원하는 힐링 수집형 육성 게임'

을 목표로 한다.
