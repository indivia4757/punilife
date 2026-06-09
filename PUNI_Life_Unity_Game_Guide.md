# PUNI Life - Unity 모바일 다마고치형 육성 게임 개발 가이드

## 1. 프로젝트 개요

### 프로젝트명
**PUNI Life**

### 장르
- 캐주얼 육성 시뮬레이션
- 다마고치 스타일 펫 케어 게임
- 방치형 성장 요소 포함
- 수집형 진화 게임

### 플랫폼
- Android 우선 출시
- iOS 추후 대응 가능

### 엔진
- Unity 2D
- C#

### 목표
서버 없이 모바일 단독으로 동작하는 다마고치형 캐릭터 육성 게임을 만든다.  
플레이어는 알에서 태어난 작은 생명체 **푸니(PUNI)** 를 돌보고, 성장시키고, 다양한 형태로 진화시킨다.

---

## 2. 핵심 콘셉트

플레이어는 신비한 알을 발견한다.  
알에서 태어난 작은 생명체 **푸니**는 먹고, 자고, 놀고, 씻고, 훈련하면서 성장한다.

푸니는 플레이어가 어떻게 돌보느냐에 따라 서로 다른 모습으로 진화한다.

이 게임의 핵심은 단순히 배고픔을 채우는 것이 아니라,

- 푸니와 애착 형성
- 상태 관리
- 성장 선택
- 진화 수집
- 도감 완성
- 방 꾸미기

를 반복하는 것이다.

---

## 3. 캐릭터 콘셉트

### 캐릭터 이름
**푸니(PUNI)**

### 기본 설정
- 알 속에서 태어난 작은 생명체
- 호기심이 많고 순수한 성격
- 플레이어가 돌보는 방식에 따라 다양한 모습으로 성장
- 머리 쪽은 깔끔하고 둥근 실루엣
- 작고 귀여운 새싹 같은 머리 장식
- 과한 귀나 혐오스러운 돌기 표현은 피함
- 큰 눈, 짧은 팔다리, 둥근 몸통
- 알껍데기 하의 또는 알껍데기 장식이 초반 정체성

### 디자인 방향
- 파스텔 톤
- 귀엽고 부드러운 느낌
- 모바일 작은 화면에서도 잘 보이는 단순한 실루엣
- 표정 변화가 잘 보이는 얼굴
- 스킨과 진화 확장 가능

---

## 4. 게임 핵심 루프

```text
앱 실행
↓
푸니 상태 확인
↓
먹이 주기 / 놀아주기 / 씻기기 / 재우기 / 훈련하기
↓
상태값 변화
↓
경험치 증가
↓
레벨업
↓
진화 조건 누적
↓
진화 또는 성장 단계 변화
↓
도감 등록
↓
꾸미기/아이템 해금
↓
재방문
```

---

## 5. 장르 정의

이 게임은 단순 방치형 게임이 아니다.

정확한 방향은 다음과 같다.

```text
다마고치식 케어
+
방치형 상태 변화
+
프린세스 메이커식 성장 분기
+
수집형 진화 도감
```

앱을 꺼둔 동안에도 시간이 흐르고 푸니 상태가 변화한다.  
하지만 푸니는 죽지 않는다.

오래 방치하면 상태가 나빠지고, 아프거나 삐질 수는 있지만 처음부터 다시 시작하지 않는다.

---

## 6. 사망/초기화 정책

### 푸니는 죽지 않는다.

오래 방치했을 때 발생 가능한 상태:

- 배고픔 감소
- 행복 감소
- 청결 감소
- 성장 속도 감소
- 미니게임 보상 감소
- 아픔 상태
- 삐짐 상태
- 특정 진화 조건에 반영

### 방치 패널티 예시

```text
Hunger 0 → 배고픔 상태
Cleanliness 0 → 더러움 상태
Happiness 0 → 삐짐 상태
여러 상태가 0 → 아픔 상태
```

### 회복 방법
- 밥 주기
- 씻기기
- 놀아주기
- 약 먹이기
- 광고 보고 즉시 회복

---

## 7. MVP 기능 범위

초기 출시 버전에서는 기능을 작게 가져간다.

### 포함 기능
- 푸니 캐릭터 1종
- 성장 단계 3단계
- 진화 타입 5종
- 상태값 5개
- 행동 6개
- 미니게임 1개
- 도감
- 로컬 저장
- 오프라인 상태 변화
- 간단한 상점
- 보상형 광고 구조 준비

### 제외 기능
- Supabase 연동
- 온라인 랭킹
- 친구 시스템
- 실시간 서버
- 복잡한 퀘스트
- 대규모 스토리
- 멀티 캐릭터 동시 육성

---

## 8. 주요 상태값

상태값은 0~100 사이로 관리한다.

```csharp
public class PuniStatus
{
    public int hunger;       // 배고픔
    public int happiness;    // 행복
    public int cleanliness;  // 청결
    public int energy;       // 에너지
    public int affection;    // 애정
    public int level;
    public int exp;
    public int coin;
}
```

### 기본 상태 의미

| 상태값 | 설명 |
|---|---|
| Hunger | 낮으면 배고픔 |
| Happiness | 낮으면 우울/삐짐 |
| Cleanliness | 낮으면 더러움 |
| Energy | 낮으면 행동 제한 |
| Affection | 높을수록 좋은 진화 가능성 증가 |
| Exp | 성장 경험치 |
| Level | 성장 단계 판단 |
| Coin | 아이템 구매 재화 |

---

## 9. 숨겨진 성장 능력치

프린세스 메이커식 성장 분기를 위해 유저에게 직접 노출하지 않는 숨겨진 스탯을 둔다.

```csharp
public class PuniGrowthStats
{
    public int intelligence; // 지능
    public int strength;     // 체력
    public int sensitivity;  // 감성
    public int courage;      // 용기
    public int kindness;     // 친화력
    public int neglect;      // 방치 누적
}
```

이 값들은 진화 타입 결정에 사용한다.

---

## 10. 기본 행동

### Feed
밥 주기

효과:
- Hunger +25
- Affection +3
- Coin -10

### Play
놀아주기

효과:
- Happiness +20
- Energy -10
- Affection +5
- Exp +5

### Clean
씻기기

효과:
- Cleanliness +30
- Happiness +5
- Exp +3

### Sleep
재우기

효과:
- Energy +40
- Hunger -5

### Study
공부하기

효과:
- Intelligence +5
- Energy -10
- Happiness -3
- Exp +8

### Train
훈련하기

효과:
- Strength +5
- Courage +3
- Energy -15
- Exp +8

---

## 11. 오프라인 상태 변화

앱을 꺼둔 시간에 따라 상태가 변화한다.

### 계산 기준
마지막 저장 시간 `lastSavedAt` 과 현재 시간의 차이를 계산한다.

예시:

```text
1시간마다 Hunger -5
1시간마다 Happiness -3
1시간마다 Cleanliness -4
1시간마다 Energy +5
```

단, 너무 하드하면 유저가 이탈하므로 최대 감소량을 제한한다.

### 제한 예시
- 최대 오프라인 계산 시간: 24시간
- 24시간 이상 방치해도 추가 패널티는 제한
- 상태값은 0 미만으로 내려가지 않음
- Energy는 100 초과하지 않음

---

## 12. 성장 단계

### Stage 0: Egg
- 알 상태
- 일정 시간 또는 첫 접속 튜토리얼 후 부화

### Stage 1: Baby Puni
- 알껍데기 안의 아기 푸니
- 기본 케어 중심

### Stage 2: Young Puni
- 알껍데기 일부 제거
- 훈련/공부/미니게임 해금

### Stage 3: Evolved Puni
- 관리 방식에 따른 진화
- 도감 등록

---

## 13. 진화 타입

MVP에서는 5종만 구현한다.

### 1. Sunny Puni
밝고 행복한 푸니

조건 예시:
- Happiness 높음
- Affection 높음
- Neglect 낮음

### 2. Scholar Puni
공부를 많이 한 지적인 푸니

조건 예시:
- Intelligence 가장 높음

### 3. Brave Puni
훈련을 많이 한 용감한 푸니

조건 예시:
- Strength 또는 Courage 높음

### 4. Forest Puni
자연 친화적인 푸니

조건 예시:
- Kindness 높음
- Cleanliness 평균 높음

### 5. Shadow Puni
오래 방치되어 외로움을 많이 탄 푸니

조건 예시:
- Neglect 높음
- Happiness 낮은 시간이 많음

중요:
Shadow Puni는 실패가 아니라 수집 가능한 진화 타입으로 설계한다.

---

## 14. 진화 판정 로직

진화 시점:
- Level 10 도달
또는
- 누적 플레이 일수 3일 이상
또는
- 특정 성장 단계 완료

예시 로직:

```csharp
public PuniEvolutionType DecideEvolution(PuniGrowthStats stats, PuniStatus status)
{
    if (stats.neglect >= 50)
        return PuniEvolutionType.Shadow;

    if (stats.intelligence >= stats.strength &&
        stats.intelligence >= stats.courage)
        return PuniEvolutionType.Scholar;

    if (stats.strength >= 40 || stats.courage >= 40)
        return PuniEvolutionType.Brave;

    if (stats.kindness >= 40)
        return PuniEvolutionType.Forest;

    return PuniEvolutionType.Sunny;
}
```

---

## 15. 미니게임

MVP에서는 1개만 구현한다.

### Snack Tap Mini Game

10초 동안 화면에 나타나는 간식을 터치하는 게임.

규칙:
- 제한 시간 10초
- 간식 오브젝트가 랜덤 위치에 생성
- 터치하면 점수 +1
- 연속 터치 성공 시 콤보
- 종료 후 점수에 따라 Coin 지급

보상:
```text
Coin = Score * 2
Happiness +10
Exp +5
```

광고 보상:
- 광고 시청 시 코인 2배

---

## 16. 도감 시스템

진화한 푸니는 도감에 등록된다.

### 도감 데이터
```csharp
public class PuniDexEntry
{
    public PuniEvolutionType type;
    public bool unlocked;
    public DateTime unlockedAt;
}
```

### MVP 도감 항목
- Sunny Puni
- Scholar Puni
- Brave Puni
- Forest Puni
- Shadow Puni

---

## 17. 저장 시스템

서버 없이 동작해야 하므로 로컬 저장을 우선한다.

### 저장 방식
초기에는 JSON 파일 저장 추천.  
간단 구현은 PlayerPrefs도 가능하지만, 상태값이 늘어날 예정이므로 JSON 저장을 권장한다.

### 저장 데이터
```csharp
[Serializable]
public class SaveData
{
    public PuniStatus status;
    public PuniGrowthStats growthStats;
    public PuniStage stage;
    public PuniEvolutionType evolutionType;
    public List<PuniDexEntry> dexEntries;
    public int currentRoomThemeId;
    public List<int> ownedItemIds;
    public string lastSavedAt;
}
```

### 저장 시점
- 앱 종료 시
- 행동 실행 후
- 미니게임 종료 후
- 진화 완료 후
- 일정 시간마다 자동 저장

---

## 18. Unity 씬 구조

### Scene 목록

#### 1. BootScene
- 초기 로딩
- SaveData 로드
- GameScene 이동

#### 2. GameScene
메인 플레이 화면

구성:
- 푸니 캐릭터
- 상태바
- 행동 버튼
- 코인 표시
- 방 배경
- 하단 메뉴

#### 3. MiniGameScene
간식 터치 미니게임

#### 4. DexScene
도감 화면

#### 5. ShopScene
아이템/방 꾸미기 상점

---

## 19. GameScene UI 구성

```text
상단
- Level
- Coin
- Settings

중앙
- PUNI 캐릭터
- 현재 감정 표현
- 방 배경

상태 영역
- Hunger Bar
- Happiness Bar
- Cleanliness Bar
- Energy Bar
- Affection Bar

하단 버튼
- Feed
- Play
- Clean
- Sleep
- Study
- Train

하단 메뉴
- Home
- Mini Game
- Dex
- Shop
```

---

## 20. 스크립트 구조

```text
Assets/Scripts
├── Core
│   ├── GameManager.cs
│   ├── TimeManager.cs
│   └── Constants.cs
│
├── Puni
│   ├── PuniController.cs
│   ├── PuniStatus.cs
│   ├── PuniGrowthStats.cs
│   ├── PuniEvolutionType.cs
│   └── PuniStage.cs
│
├── Systems
│   ├── CareSystem.cs
│   ├── EvolutionSystem.cs
│   ├── SaveManager.cs
│   ├── DexManager.cs
│   ├── EconomyManager.cs
│   └── OfflineProgressSystem.cs
│
├── UI
│   ├── UIManager.cs
│   ├── StatusBarView.cs
│   ├── PuniView.cs
│   └── PopupManager.cs
│
├── MiniGame
│   ├── SnackTapGameManager.cs
│   └── SnackItem.cs
│
└── Ads
    └── AdManager.cs
```

---

## 21. 핵심 클래스 역할

### GameManager
- 게임 전체 상태 관리
- 매니저 초기화
- 저장/로드 흐름 제어

### PuniController
- 현재 푸니 상태 보관
- 행동 실행 요청
- 진화 상태 반영

### CareSystem
- Feed, Play, Clean, Sleep, Study, Train 처리
- 상태값 변화 계산

### EvolutionSystem
- 성장 조건 체크
- 진화 타입 결정
- 도감 등록 요청

### OfflineProgressSystem
- 마지막 접속 이후 경과 시간 계산
- 오프라인 상태 변화 적용

### SaveManager
- JSON 저장/로드
- 저장 파일 경로 관리

### DexManager
- 진화 도감 등록
- 해금 여부 조회

### UIManager
- 상태바 갱신
- 버튼 이벤트 연결
- 팝업 표시

### AdManager
- AdMob 연동 예정
- MVP 초기에는 Mock 형태로 작성 가능

---

## 22. 광고 설계

초기에는 AdManager를 인터페이스처럼 만들어둔다.

### 보상형 광고 사용처
- 무료 간식 받기
- 코인 2배
- 아픔 즉시 회복
- 미니게임 추가 플레이

### 전면 광고
- 과도하게 넣지 않기
- 초반 MVP에서는 제외 가능
- 추후 3~5회 행동 또는 미니게임 후 노출 고려

### 배너 광고
- 게임 몰입을 방해할 수 있으므로 MVP 이후 검토

---

## 23. 수익화 방향

### 1차 수익화
- 보상형 광고

### 2차 수익화
- 광고 제거 인앱결제
- 방 꾸미기 아이템
- 푸니 스킨
- 특별 간식
- 진화 힌트

---

## 24. 상점 시스템

MVP에서는 간단한 코인 상점만 구현한다.

### 아이템 종류
- 기본 간식
- 고급 간식
- 약
- 방 배경
- 장식품

### 아이템 효과
```csharp
public class ItemData
{
    public int id;
    public string name;
    public ItemType type;
    public int price;
    public int hungerValue;
    public int happinessValue;
    public int cleanlinessValue;
    public int energyValue;
    public int affectionValue;
}
```

---

## 25. 밸런스 초기값

### 초기 상태
```text
Hunger: 80
Happiness: 80
Cleanliness: 80
Energy: 80
Affection: 10
Level: 1
Exp: 0
Coin: 100
```

### 레벨업
```text
NextExp = Level * 20
```

### 상태값 제한
```text
Min: 0
Max: 100
```

---

## 26. MVP 개발 순서

### Step 1. 프로젝트 생성
- Unity 2D 프로젝트 생성
- Portrait 모바일 비율 설정
- 기본 GameScene 생성

### Step 2. 데이터 클래스 작성
- PuniStatus
- PuniGrowthStats
- SaveData
- Enum 정의

### Step 3. 저장 시스템 구현
- SaveManager 작성
- JSON 저장/로드 확인
- lastSavedAt 저장

### Step 4. 메인 화면 구현
- 푸니 이미지 배치
- 상태바 UI
- 버튼 UI

### Step 5. 케어 시스템 구현
- Feed
- Play
- Clean
- Sleep
- Study
- Train

### Step 6. 오프라인 진행 구현
- 시간 차이 계산
- 상태값 감소/회복 적용

### Step 7. 성장/진화 구현
- Exp 증가
- Level 증가
- Stage 변화
- Evolution 결정

### Step 8. 도감 구현
- 진화 타입 등록
- 도감 화면 표시

### Step 9. 미니게임 구현
- Snack Tap 게임
- 보상 지급
- 메인 게임 상태 반영

### Step 10. 광고 구조 추가
- AdManager Mock 작성
- 추후 AdMob 실제 연동 가능하게 분리

---

## 27. Codex 작업 요청 문장

아래 문장을 Codex에 그대로 입력해서 개발을 시작한다.

```text
이 문서를 기준으로 Unity 2D 모바일 게임 PUNI Life의 MVP를 개발해줘.

먼저 다음을 구현해줘.

1. Unity C# 스크립트 폴더 구조 생성
2. PuniStatus, PuniGrowthStats, SaveData, PuniStage, PuniEvolutionType 데이터 클래스 작성
3. SaveManager를 JSON 파일 저장 방식으로 구현
4. GameManager와 PuniController 기본 구조 작성
5. CareSystem에서 Feed, Play, Clean, Sleep, Study, Train 행동 처리 구현
6. OfflineProgressSystem에서 lastSavedAt 기준으로 오프라인 상태 변화 적용
7. UIManager가 상태값을 Slider/Text로 갱신할 수 있도록 메서드 작성

초보자가 이어서 수정하기 쉽도록 복잡한 패턴은 피하고, 간결한 C# 코드로 작성해줘.
Unity 2D Portrait 모바일 게임 기준으로 작성해줘.
```

---

## 28. 코드 스타일 가이드

- 복잡한 아키텍처 사용 금지
- 싱글톤은 필요한 매니저에만 제한적으로 사용
- UniRx, Zenject 등 외부 프레임워크 사용하지 않기
- ScriptableObject는 추후 확장 때 사용
- MVP에서는 직관적인 클래스 구조 우선
- 주석은 핵심 로직에만 짧게 작성
- 초보자가 Unity Inspector에서 연결할 수 있는 구조 유지

---

## 29. 향후 업데이트 아이디어

### 콘텐츠 확장
- 진화 타입 30종
- 계절 이벤트
- 한정 스킨
- 방 꾸미기
- 알 종류 추가
- 푸니 친구 추가

### 시스템 확장
- Supabase 클라우드 저장
- Supabase 랭킹
- 출석 보상
- 일일 미션
- 푸시 알림
- IAP 광고 제거
- Google Play Games 업적

### 미니게임 확장
- 색깔 맞추기
- 리듬 터치
- 간식 떨어뜨리기
- 산책하기
- 퍼즐 미니게임

---

## 30. 최종 방향성

PUNI Life는 단순히 밥 주는 게임이 아니라,

```text
귀여운 캐릭터를 돌보고
성장 방향을 선택하고
다양한 푸니로 진화시키고
도감을 채우는 게임
```

으로 개발한다.

초기 목표는 거창한 게임이 아니라,

```text
1마리의 푸니를 돌보고
상태값이 변하고
레벨업하고
5가지 타입 중 하나로 진화하고
도감에 등록되는 것
```

까지 완성하는 것이다.

이 MVP가 완성되면 이후 광고, 상점, 스킨, Supabase, 푸시 알림을 순차적으로 추가한다.
