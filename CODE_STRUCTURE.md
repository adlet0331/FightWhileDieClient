# Fight While Die - 코드 구조 문서

## 프로젝트 개요
수익이 되는 간단한 모바일 게임 개발 프로젝트 (Unity C# 기반)
- 장착한 칼들의 공격력으로 스테이지를 클리어하며 무한 성장하는 모바일 게임
- 뽑기 시스템, 장비 강화, 랭킹 경쟁 등의 요소 포함

---

## 📁 전체 디렉토리 구조

```
Assets/Scripts/
├── Combat/              # 전투 시스템 관련
├── Data/                # 데이터 모델 정의
├── Editor/              # Unity 에디터 확장
├── NonDestroyObject/    # 싱글톤 매니저들 (게임 전역 관리)
│   └── DataManage/      # 데이터 관리 서브시스템
├── UI/                  # 사용자 인터페이스
│   ├── Gatcha/          # 뽑기 시스템 UI
│   ├── Inventory/       # 인벤토리 시스템 UI
│   │   ├── Decompose/   # 장비 분해
│   │   ├── Enhance/     # 장비 강화
│   │   ├── Equip/       # 장비 착용
│   │   └── ItemView/    # 아이템 뷰
│   ├── Ranking Board/   # 랭킹 시스템
│   └── Touch/           # 터치 입력 처리
└── Utils/               # 유틸리티 함수들
```

---

## 🎮 기능별 코드 구조

### 1. 전투 시스템 (Combat)

#### 1.1 핵심 클래스
**위치:** `Assets/Scripts/Combat/`

##### CombatEntityParent.cs (267줄)
- **역할:** 플레이어와 적 엔티티의 공통 부모 클래스
- **주요 기능:**
  - 전투 엔티티의 기본 상태 관리 (Idle, Running, Attack, Damaged, Dying 등)
  - 애니메이션 제어
  - 체력 관리
  - 공격 판정 (히트박스)
  - 코루틴 기반 액션 타이밍 제어
- **핵심 메서드:**
  - `EntityAction()`: 엔티티 행동 실행
  - `Damaged()`: 피해 처리
  - `SwitchStatusAndAnimation()`: 상태 및 애니메이션 전환

##### PlayerEntity.cs (51줄)
- **역할:** 플레이어 캐릭터 전투 로직
- **주요 기능:**
  - 일반 공격, 차지 공격, 퍼펙트 차지 공격 처리
  - 플레이어 히트 판정 (`PlayerHittingEnemy`)
  - 플레이어 사망 처리
- **특징:** 플레이어 HP는 항상 1 (원샷 원킬)

##### EnemyEntity.cs (94줄)
- **역할:** 적 캐릭터 AI 및 전투 로직
- **주요 기능:**
  - AI 기반 자동 행동 업데이트 (`ActionUpdate()`)
  - 거리에 따른 행동 결정 (달리기, 공격, 점프백)
  - HP 비율에 따른 차지 공격 사용 (50% 이하)
  - 적 히트 판정 (`IsEnemyHittingPlayer`)

##### EnemyKnightEntity.cs
- **역할:** 특수 적 타입 (기사)
- **특징:** EnemyEntity를 상속받아 특수 동작 구현

##### AttackRangeObject.cs (51줄)
- **역할:** 공격 범위 충돌 감지
- **주요 기능:**
  - 트리거 영역으로 공격 범위 판정
  - 상대방이 범위 내에 있는지 확인

---

### 2. 게임 매니저 시스템 (NonDestroyObject)

#### 2.1 싱글톤 패턴 기반 매니저들
**위치:** `Assets/Scripts/NonDestroyObject/`

##### Singleton.cs (47줄)
- **역할:** 싱글톤 패턴 베이스 클래스
- **특징:** 
  - DontDestroyOnLoad로 씬 전환 시에도 유지
  - 스레드 세이프 구현
  - 게임 종료 시 자동 정리

##### CombatManager.cs (352줄)
- **역할:** 전투 시스템 전체 관리
- **주요 기능:**
  - 전투 시작/종료 (`StartCombat()`, `EndCombat()`)
  - 플레이어 입력 처리 (터치, 홀드, 공격)
  - 히트 판정 및 피해 계산
  - 적 AI 업데이트
  - 카메라 쉐이크 효과
  - 오토 플레이 모드
- **핵심 시스템:**
  - `Update()`: 플레이어 입력 및 터치 처리
  - `FixedUpdate()`: 히트 판정 및 엔티티 이동
  - 차지 공격 홀드 타이밍 계산
  - 퍼펙트 어택 타이밍 윈도우

##### UIManager.cs (218줄)
- **역할:** UI 전체 관리
- **주요 기능:**
  - 모든 팝업 참조 관리
  - 메인 UI 업데이트 (코인, 스테이지, 공격력, HP)
  - 전투 UI ↔ 메인 UI 전환
  - 공격 게이지 슬라이더
  - 퍼펙트 어택 이펙트
- **관리 대상:**
  - 뽑기 팝업
  - 인벤토리 팝업
  - 랭킹 팝업
  - 일시정지 팝업
  - 로딩 팝업

##### DataManager.cs
- **역할:** 데이터 매니저 통합 관리
- **주요 기능:**
  - PlayerDataManager 참조
  - ItemManager 참조
  - StaticDataManager 참조
  - 데이터 초기화 및 관리

##### NetworkManager.cs (327줄)
- **역할:** 서버 통신 관리
- **주요 기능:**
  - HTTP 통신 (POST 요청)
  - 사용자 생성/삭제/업데이트 (`CreateNewUser()`, `DeleteUser()`, `FetchUser()`)
  - 아이템 뽑기 요청 (`AddRandomEquipItems()`)
  - 정적 데이터 다운로드 (`GetStaticDataJsonList()`)
  - 연결 상태 확인 (`CheckConnection()`)
- **통신 환경:**
  - 실서버: `http://fwt-server.haje.org/playerserver/`
  - 로컬 테스트: `http://localhost:8253/playerserver/`
  - 연결 실패 처리 및 재시도 메커니즘

##### SoundManager.cs (99줄)
- **역할:** 사운드 및 음악 재생 관리
- **주요 기능:**
  - BGM 관리
  - 효과음 재생
  - 사운드 ON/OFF 설정

##### AdsManager.cs (138줄)
- **역할:** 광고 시스템 관리 (Google Mobile Ads)
- **주요 기능:**
  - 배너 광고 표시
  - 보상형 광고 (Rewarded Ads)
  - 광고 로드 및 표시 이벤트 처리
  - 보상 지급 처리

##### AutoManager.cs (38줄)
- **역할:** 자동 플레이 모드 관리
- **주요 기능:**
  - 오토 모드 ON/OFF
  - 오토 속도 조절
  - 오토 디버프 감소 옵션

##### BackgroundManager.cs
- **역할:** 배경 화면 관리
- **주요 기능:** 배경 스프라이트 및 애니메이션

##### ResolutionManager.cs
- **역할:** 화면 해상도 관리
- **주요 기능:** 다양한 디바이스 해상도 대응

##### CustomLogManager.cs (60줄)
- **역할:** 커스텀 로그 시스템
- **주요 기능:** 디버깅용 로그 수집 및 표시

---

### 3. 데이터 관리 시스템 (NonDestroyObject/DataManage)

#### 3.1 데이터 매니저들
**위치:** `Assets/Scripts/NonDestroyObject/DataManage/`

##### PlayerDataManager.cs (354줄)
- **역할:** 플레이어 데이터 전체 관리 (Unity PlayerPrefs 사용)
- **주요 데이터:**
  - 사용자 정보: ID, 이름
  - 게임 진행: 스테이지, 최고 스테이지
  - 스탯: 기본 공격력, 총 공격력
  - 재화: 코인
  - 장비: 장착 아이템 리스트, 강화석 리스트
  - 일일 제한: 뽑기 횟수, 광고 시청 횟수
- **핵심 메서드:**
  - `StageCleared()`: 스테이지 클리어 처리 (보상 지급)
  - `StageReset()`: 스테이지 리셋
  - `SpendCoin()`: 코인 소비
  - `GetOptionValue()`: 장비 옵션 계산
  - `FetchAllStatus()`: 서버 동기화
- **계산 로직:**
  - 적 HP: `enemyStartHp * (enemyHpMultiplier ^ stage)` (50 * 1.2^스테이지)
  - 공격력: 기본 공격력 + 장비 옵션 (% 및 고정값)
  - 코인 획득: 스테이지 기반 + 장비 옵션

##### ItemManager.cs (95줄)
- **역할:** 아이템(장비) 관리
- **주요 기능:**
  - 아이템 추가/삭제
  - 아이템 ID로 검색
  - 장비 아이템 정렬 (레어도, 레벨, 옵션)
  - 장비 강화/분해
  - 아이템 리스트 업데이트
- **관리 대상:**
  - 장착 가능한 장비 (칼)
  - 강화 재료

##### StaticDataManager.cs (174줄)
- **역할:** 서버에서 받아온 정적 데이터 관리
- **주요 기능:**
  - 강화 확률 데이터
  - 강화 비용 데이터
  - 강화 효과 데이터
  - 서버에서 JSON 형식으로 데이터 다운로드
- **데이터 종류:**
  - `EnhancementChanceData`: 강화 성공 확률
  - `EnhancementCostData`: 강화 비용
  - `EnhancementEffectData`: 강화 효과

##### StaticDataClasses.cs (88줄)
- **역할:** 정적 데이터 클래스 정의
- **데이터 구조:**
  - 강화 확률 테이블
  - 강화 비용 테이블
  - 강화 효과 테이블

---

### 4. 데이터 모델 (Data)

#### 4.1 아이템 시스템
**위치:** `Assets/Scripts/Data/`

##### Item.cs (13줄)
- **역할:** 모든 아이템의 추상 베이스 클래스
- **속성:** `id` (아이템 고유 ID)

##### EquipItemObject.cs (62줄)
- **역할:** 장비 아이템 데이터
- **속성:**
  - `itemType`: 아이템 타입 (칼)
  - `rare`: 레어도 (Common ~ God: 6단계)
  - `option`: 옵션 종류
  - `level`: 강화 레벨
  - `sprite`: 아이템 이미지
- **메서드:**
  - `GetOptionValue()`: 현재 강화 레벨에 따른 옵션 수치 계산
  - `Enhance()`: 장비 강화
  - `Decompose()`: 장비 분해 (강화석 획득)

##### ItemTypeEnum.cs (42줄)
- **역할:** 아이템 관련 열거형 정의
- **정의:**
  - `ItemType`: 아이템 종류 (칼)
  - `EquipmentOption`: 장비 옵션 종류
    - 공격력 증가 (고정값/퍼센트)
    - 코인 획득 증가
    - 기본 공격력 획득량 증가
    - 공격 후 딜레이 감소
    - 오토 관련
  - `ItemRare`: 레어도 (Common, Rare, Epic, Unique, Legend, God)

---

### 5. UI 시스템 (UI)

#### 5.1 UI 기본 구조
**위치:** `Assets/Scripts/UI/`

##### Popup.cs
- **역할:** 모든 팝업의 베이스 클래스
- **주요 기능:**
  - `Open()`: 팝업 열기
  - `Close()`: 팝업 닫기
  - 애니메이션 제어

##### View.cs
- **역할:** 뷰 컴포넌트 베이스 클래스
- **주요 기능:** UI 뷰 전환 및 애니메이션

##### LoadingPopup.cs
- **역할:** 로딩 화면 표시

##### PopupPause.cs (42줄)
- **역할:** 일시정지 팝업
- **주요 기능:**
  - 게임 일시정지
  - 사운드 ON/OFF
  - 계정 삭제

##### PopupEnterName.cs
- **역할:** 닉네임 입력 팝업
- **주요 기능:** 최초 실행 시 닉네임 입력

##### SimpleTextPopup.cs
- **역할:** 간단한 텍스트 메시지 팝업

#### 5.2 뽑기 시스템 (UI/Gatcha)
**위치:** `Assets/Scripts/UI/Gatcha/`

##### GatchaPopup.cs (212줄)
- **역할:** 뽑기 시스템 메인 UI
- **주요 기능:**
  - 코인 뽑기 (비용: 10^일일 횟수)
  - 광고 시청 무료 뽑기
  - 뽑기 애니메이션 연출
  - 결과 표시
- **뽑기 확률:**
  - Common: 나머지
  - Rare: 1/4
  - Epic: 1/16
  - Unique: 1/64
  - Legend: 1/256
  - God: 1/1024

##### GatchaTriggerObj.cs (59줄)
- **역할:** 뽑기 상자 트리거 오브젝트
- **주요 기능:** 상자 클릭 시 아이템 공개

##### GatchaResultObj.cs
- **역할:** 뽑기 결과 아이템 표시

#### 5.3 인벤토리 시스템 (UI/Inventory)
**위치:** `Assets/Scripts/UI/Inventory/`

##### InventoryPopup.cs (155줄)
- **역할:** 인벤토리 메인 팝업
- **주요 기능:**
  - 탭 전환 (장착, 강화, 분해)
  - 아이템 리스트 표시
  - 정렬 기능

##### ItemSlot.cs (174줄)
- **역할:** 아이템 슬롯 UI
- **주요 기능:**
  - 아이템 정보 표시 (이미지, 레어도, 레벨)
  - 아이템 선택 처리
  - 빈 슬롯 처리

##### ItemView.cs (290줄)
- **역할:** 아이템 상세 뷰
- **주요 기능:**
  - 아이템 상세 정보 표시
  - 아이템 리스트 스크롤
  - 정렬 옵션 (레어도, 레벨, 옵션)
  - 오름차순/내림차순 전환

##### IngredientBanner.cs (82줄)
- **역할:** 강화석 정보 배너
- **주요 기능:** 보유 강화석 수량 표시

##### IngredientValUI.cs
- **역할:** 강화석 수량 UI

#### 5.4 장비 강화 시스템 (UI/Inventory/Enhance)
**위치:** `Assets/Scripts/UI/Inventory/Enhance/`

##### EnhanceView.cs (284줄)
- **역할:** 장비 강화 메인 뷰
- **주요 기능:**
  - 강화 대상 선택
  - 강화 재료 투입
  - 강화 실행
  - 강화 확률 표시
  - 강화 결과 처리
- **강화 시스템:**
  - 코인 + 강화석 소비
  - 확률 기반 성공/실패
  - 실패 시 강화도 감소
  - 강화석으로 100% 확률 보장

##### EnhanceTriggerObj.cs (108줄)
- **역할:** 강화 실행 트리거
- **주요 기능:** 강화 버튼 및 애니메이션

##### EnhanceResultBoard.cs (35줄)
- **역할:** 강화 결과 표시 보드

##### EnhanceIngredButton.cs (40줄)
- **역할:** 강화석 투입 버튼

#### 5.5 장비 착용 시스템 (UI/Inventory/Equip)
**위치:** `Assets/Scripts/UI/Inventory/Equip/`

##### EquipView.cs (92줄)
- **역할:** 장비 착용 뷰
- **주요 기능:**
  - 장착 슬롯 관리 (칼 2개)
  - 장비 착용/해제
  - 장착된 장비 정보 표시

##### EquipSlot.cs (42줄)
- **역할:** 장착 슬롯 UI
- **주요 기능:** 장착된 아이템 표시

#### 5.6 장비 분해 시스템 (UI/Inventory/Decompose)
**위치:** `Assets/Scripts/UI/Inventory/Decompose/`

##### DecomposeView.cs
- **역할:** 장비 분해 뷰
- **주요 기능:**
  - 분해할 아이템 선택
  - 분해 실행
  - 강화석 획득 (레어도 * 레벨만큼)

#### 5.7 랭킹 시스템 (UI/Ranking Board)
**위치:** `Assets/Scripts/UI/Ranking Board/`

##### PopupRanking.cs
- **역할:** 랭킹 팝업
- **주요 기능:**
  - 랭킹 리스트 표시
  - 랭킹 종류 선택 (공격력, 최고 스테이지 등)

##### RankingScroll.cs
- **역할:** 랭킹 스크롤 뷰
- **주요 기능:** 랭킹 데이터 동적 로드

#### 5.8 터치 입력 시스템 (UI/Touch)
**위치:** `Assets/Scripts/UI/Touch/`

##### ATouch.cs
- **역할:** 터치 입력 추상 베이스 클래스

##### BackGroundTouch.cs
- **역할:** 배경 터치 처리

##### RestartTouch.cs
- **역할:** 재시작 터치 처리

##### AdditionalTouchArea.cs
- **역할:** 추가 터치 영역 확장

##### NoResponseTouch.cs
- **역할:** 터치 무시 영역

##### SimpleTextBackGroundTouch.cs
- **역할:** 텍스트 팝업 배경 터치

#### 5.9 기타 UI 컴포넌트

##### AutoButton.cs (43줄)
- **역할:** 오토 플레이 버튼
- **주요 기능:** 오토 모드 ON/OFF

##### CoinUI.cs
- **역할:** 코인 표시 UI
- **주요 기능:** 코인 수량 업데이트

##### CoinEffect.cs (52줄)
- **역할:** 코인 획득 이펙트
- **주요 기능:** 코인 파티클 애니메이션

##### TouchParticle.cs
- **역할:** 터치 파티클 이펙트

##### ValueText.cs
- **역할:** 값 표시 텍스트
- **주요 기능:** 숫자 값 포맷팅 및 표시

##### SliderWithValue.cs
- **역할:** 값 표시 슬라이더
- **주요 기능:** 슬라이더 + 수치 표시

---

### 6. 유틸리티 시스템 (Utils)

#### 6.1 유틸리티 클래스들
**위치:** `Assets/Scripts/Utils/`

##### CoroutineUtils.cs (40줄)
- **역할:** 코루틴 유틸리티
- **주요 기능:**
  - `WaitAndOperation()`: 대기 후 작업 실행
  - `WaitAndOperationIEnum()`: IEnumerator 반환 버전

##### AnimatorUtil.cs
- **역할:** 애니메이터 유틸리티
- **주요 기능:** 애니메이션 시간 계산

##### ArrayElementTitle.cs (99줄)
- **역할:** Unity Inspector에서 배열 요소에 이름 표시
- **주요 기능:** 에디터 확장 (PropertyAttribute)

##### CustomLog.cs (35줄)
- **역할:** 커스텀 로그 시스템
- **주요 기능:** 로그 레벨 및 포맷팅

##### IntToUnitString.cs
- **역할:** 숫자를 단위 문자열로 변환
- **주요 기능:** 큰 숫자를 K, M, B 등으로 표현

##### JsonSL.cs (60줄)
- **역할:** JSON 직렬화/역직렬화
- **주요 기능:** Unity JsonUtility 래퍼

##### ResourcesLoad.cs
- **역할:** 리소스 로드 유틸리티
- **주요 기능:** Resources 폴더에서 에셋 로드

##### NetworkReqRes.cs (86줄)
- **역할:** 네트워크 요청/응답 데이터 클래스
- **정의:**
  - `CheckConnectionReq/Res`
  - `CreateNewUserReq/Res`
  - `DeleteUserReq/Res`
  - `FetchUserReq/Res`
  - `AddEquipItemsReq/Res`
  - `StaticDataJsonReq/Res`

##### NetworkResults.cs (43줄)
- **역할:** 네트워크 결과 열거형
- **정의:**
  - `CheckConnectionResult`
  - `CreateNewUserResult`
  - `DeleteUserResult`
  - `FetchUserResult`
  - `AddItemsResult`
  - `StaticDataJsonResult`

---

### 7. 에디터 확장 (Editor)

#### 7.1 빌드 시스템
**위치:** `Assets/Scripts/Editor/`

##### BuildCommand.cs (288줄)
- **역할:** 자동 빌드 스크립트 (CI/CD)
- **주요 기능:**
  - Android APK 빌드
  - iOS 빌드
  - 빌드 설정 관리
  - 커맨드 라인 빌드 지원

---

## 🔄 주요 게임 플로우

### 게임 시작 플로우
```
1. Singleton 매니저들 초기화
   ├─ NetworkManager: 서버 연결 체크
   ├─ DataManager: PlayerPrefs 로드
   ├─ UIManager: UI 초기화
   ├─ SoundManager: 오디오 초기화
   └─ AdsManager: 광고 SDK 초기화

2. 사용자 데이터 로드
   ├─ PlayerDataManager.LoadAllPrefs()
   ├─ NetworkManager.FetchUser()
   └─ StaticDataManager.GetStaticDatasFromServer()

3. 메인 화면 표시
   └─ UIManager.UpdateMainUI()
```

### 전투 플로우
```
1. 전투 시작 (CombatManager.StartCombat())
   ├─ 스테이지 리셋
   ├─ 플레이어 & 적 HP 초기화
   ├─ 적 AI 랜덤 선택
   └─ 전투 UI 활성화

2. 전투 진행
   ├─ Update(): 플레이어 입력 처리
   │   ├─ 터치 다운: 차지 시작
   │   ├─ 터치 홀드: 차지 게이지 증가
   │   └─ 터치 업: 공격 실행 (일반/차지/퍼펙트)
   │
   └─ FixedUpdate(): 히트 판정 & AI 업데이트
       ├─ 플레이어 공격 히트 체크
       ├─ 적 공격 히트 체크
       └─ 적 AI 행동 업데이트

3. 전투 종료 (CombatManager.EndCombat())
   ├─ 승리: 스테이지 클리어
   │   ├─ 보상 지급 (코인, 기본 공격력)
   │   ├─ 다음 스테이지 준비
   │   └─ 랜덤 적 선택
   │
   └─ 패배: 스테이지 리셋
       ├─ 메인 UI로 복귀
       └─ 스테이지 10단위로 리셋
```

### 뽑기 플로우
```
1. 뽑기 팝업 열기 (GatchaPopup.Open())
   └─ 현재 코인 및 가격 표시

2. 뽑기 실행
   ├─ 코인 뽑기
   │   ├─ 코인 차감
   │   ├─ NetworkManager.AddRandomEquipItems()
   │   └─ 서버에서 랜덤 아이템 생성
   │
   └─ 광고 뽑기
       ├─ AdsManager.RequestRewardAds()
       ├─ 광고 시청
       └─ 보상 지급

3. 뽑기 결과 표시
   ├─ 뽑기 애니메이션
   ├─ 아이템 공개
   └─ ItemManager에 아이템 추가
```

### 장비 강화 플로우
```
1. 강화 뷰 열기 (EnhanceView)
   ├─ 보유 아이템 리스트 표시
   └─ 강화석 수량 표시

2. 강화 대상 선택
   └─ 아이템 상세 정보 및 다음 레벨 효과 표시

3. 강화 재료 투입
   ├─ 강화석 선택 (0~현재레벨)
   └─ 확률 계산 (기본확률 + 강화석보너스)

4. 강화 실행
   ├─ 코인 및 강화석 차감
   ├─ 확률 판정
   ├─ 성공: 레벨 업, 옵션 수치 증가
   └─ 실패: 레벨 다운

5. 결과 표시
   └─ EnhanceResultBoard에 결과 애니메이션
```

### 데이터 동기화 플로우
```
1. 로컬 변경 발생
   ├─ PlayerPrefs에 즉시 저장
   └─ UIManager.UpdateAllUIInGame()

2. 서버 동기화
   ├─ NetworkManager.FetchUser()
   ├─ 현재 데이터 전송
   └─ 서버에 업데이트

3. 주요 동기화 시점
   ├─ 스테이지 클리어
   ├─ 뽑기 실행
   ├─ 장비 강화
   └─ 앱 시작/종료
```

---

## 🎯 핵심 게임 시스템 설명

### 공격력 계산 시스템
```csharp
// PlayerDataManager.cs - Atk 프로퍼티
총 공격력 = (기본 공격력 + 장비 고정값 옵션) * (1 + 장비 퍼센트 옵션)

예시:
- 기본 공격력: 1000
- 장비1 옵션: 공격력 +200
- 장비2 옵션: 공격력 +50%
- 총 공격력 = (1000 + 200) * (1 + 0.5) = 1800
```

### 적 HP 스케일링
```csharp
// PlayerDataManager.cs - CurrentEnemyHp
적 HP = 50 * (1.2 ^ 스테이지)

스테이지별 HP:
- 스테이지 1: 300
- 스테이지 11: 1,857
- 스테이지 21: 11,501
- 스테이지 100: 24,845,392,356
```

### 강화 시스템
```
강화 레벨 1~25 (레어도별 최대 레벨 제한)
- Epic 이하: 최대 15레벨
- Legend: 최대 20레벨
- God: 최대 25레벨

강화 확률 (레벨별):
- 1~10: 90~30% (초반 구간)
- 11~20: 30~10% (중반 구간)
- 21~25: 10~1% (후반 구간)

강화석 시스템:
- 장비 분해 시 획득
- 레어도별 강화석 존재
- 투입 시 확률 보너스 (+10~30%)
- 현재 레벨만큼 투입 시 100% 성공
```

### 뽑기 확률
```
레어도별 확률:
- Common: 약 57.8% (나머지)
- Rare: 25% (1/4)
- Epic: 6.25% (1/16)
- Unique: 1.56% (1/64)
- Legend: 0.39% (1/256)
- God: 0.098% (1/1024)
```

---

## 📊 데이터 흐름도

### PlayerPrefs 저장 데이터
```
로컬 저장 (영구):
├─ 사용자 정보
│   ├─ Id: 서버 ID
│   ├─ Name: 닉네임
│   └─ LastUpdated: 마지막 업데이트 날짜
│
├─ 게임 진행
│   ├─ TopStage: 최고 스테이지
│   ├─ BaseAtk: 기본 공격력
│   └─ Coin: 보유 코인
│
├─ 강화 재료
│   └─ EnhanceIngredient1~8: 레어도별 강화석 수량
│
├─ 장착 장비
│   ├─ EquipItem1Id: 장착 슬롯 1
│   └─ EquipItem2Id: 장착 슬롯 2
│
└─ 일일 제한
    ├─ DailyGatchaNum: 일일 뽑기 횟수
    └─ DailyLastAdNum: 일일 광고 시청 가능 횟수
```

### 서버 통신 데이터
```
서버 ↔ 클라이언트:
├─ 사용자 CRUD
│   ├─ CreateNewUser: 신규 사용자 생성
│   ├─ FetchUser: 사용자 데이터 동기화
│   └─ DeleteUser: 사용자 삭제
│
├─ 아이템
│   └─ AddRandomEquipItems: 랜덤 장비 생성
│
└─ 정적 데이터
    └─ GetStaticDataJsonList: 강화 테이블 다운로드
```

---

## 🔧 주요 설정 값

### 전투 설정
```
// CombatManager.cs
strongAttackHoldTime: 차지 완성 시간 (예: 0.8초)
perfectAttackTimeInterval: 퍼펙트 타이밍 윈도우 (예: ±0.1초)
playerDamagePerAttackRate: 공격 타입별 데미지 배율
  - Normal: 1.0
  - Charge: 1.5
  - PerfectCharge: 2.0
```

### 보상 설정
```
// PlayerDataManager.cs
스테이지 클리어 보상:
- 기본 공격력 증가: 10 + (장비 옵션)
- 코인 획득: stage * (1 + 장비 코인 옵션)
```

### 뽑기 설정
```
// PlayerDataManager.cs
gatchaStartCoin: 100 (첫 뽑기 가격)
뽑기 가격 공식: 100 * (2 ^ 일일 뽑기 횟수)
  - 1회: 100 코인
  - 2회: 200 코인
  - 3회: 400 코인
  - ...
```

---

## 🚀 빌드 및 배포

### 빌드 환경
```
Unity 버전: (프로젝트 설정 참조)
플랫폼:
  - Android (APK)
  - iOS

외부 패키지:
  - Google Mobile Ads SDK (광고)
  - Newtonsoft.Json (JSON 처리)
  - UniTask (비동기 처리)
  - TextMeshPro (텍스트 렌더링)
```

### CI/CD
```
// BuildCommand.cs
자동 빌드 지원:
- 커맨드 라인 빌드
- 버전 관리
- 빌드 설정 자동화
```

---

## 📝 코드 컨벤션 및 구조

### 네임스페이스 구조
```csharp
namespace Combat { }               // 전투 시스템
namespace Data { }                 // 데이터 모델
namespace NonDestroyObject { }     // 싱글톤 매니저
namespace NonDestroyObject.DataManage { } // 데이터 관리
namespace UI { }                   // UI 컴포넌트
namespace UI.Gatcha { }           // 뽑기 UI
namespace UI.Inventory { }        // 인벤토리 UI
namespace Utils { }               // 유틸리티
```

### 디자인 패턴
```
1. Singleton 패턴
   - 모든 매니저 클래스
   - DontDestroyOnLoad로 영구 유지

2. MVC 패턴 (부분적)
   - Manager: Controller
   - Data: Model
   - UI: View

3. Observer 패턴
   - UIUpdateVoid 델리게이트
   - 이벤트 기반 UI 업데이트

4. Object Pool (부분적)
   - ItemSlot 재사용
   - 파티클 이펙트
```

### 비동기 처리
```csharp
// UniTask 사용
async UniTask<ResultType> MethodName()
{
    await UniTask.SwitchToThreadPool(); // 백그라운드
    // ... 네트워크 통신 등
    await UniTask.SwitchToMainThread(); // 메인 스레드
    // ... UI 업데이트
}
```

---

## 📌 주요 의존성

### 클래스 의존성 다이어그램 (간소화)
```
CombatManager
├─ → PlayerEntity (전투 엔티티)
├─ → EnemyEntity (전투 엔티티)
├─ → DataManager (데이터 참조)
└─ → UIManager (UI 업데이트)

DataManager
├─ → PlayerDataManager (플레이어 데이터)
├─ → ItemManager (아이템 관리)
└─ → StaticDataManager (정적 데이터)

UIManager
├─ → 모든 Popup 참조
├─ → DataManager (데이터 참조)
└─ → NetworkManager (서버 통신)

NetworkManager
└─ → DataManager (데이터 전송)

PlayerDataManager
├─ → ItemManager (장비 옵션 계산)
├─ → NetworkManager (서버 동기화)
└─ → UIManager (UI 업데이트)
```

---

## 🎮 게임 밸런싱 참고

### 레벨 디자인
```
초반 (스테이지 1~10):
- 쉬운 난이도
- 빠른 진행
- 튜토리얼 역할

중반 (스테이지 11~50):
- 챌린징한 구간
- 대부분의 유저 상주 구간
- 장비 강화 필수

후반 (스테이지 51~100):
- 하드코어 구간
- 소수 유저만 도달
- 고레벨 강화 및 God 등급 장비 필요
```

### 수익 모델
```
1. 광고 수익
   - 배너 광고 (상시 표시)
   - 보상형 광고 (무료 뽑기, 일일 3회)

2. (향후) 인앱 결제
   - 코인 구매
   - 강화석 구매
   - VIP 패스
```

---

## 🔍 파일별 상세 목록

### Combat (전투) - 5개 파일
1. **AttackRangeObject.cs** - 공격 범위 충돌 감지
2. **CombatEntityParent.cs** - 전투 엔티티 베이스 클래스
3. **EnemyEntity.cs** - 적 AI 및 전투 로직
4. **EnemyKnightEntity.cs** - 특수 적 타입
5. **PlayerEntity.cs** - 플레이어 전투 로직

### Data (데이터 모델) - 3개 파일
1. **EquipItemObject.cs** - 장비 아이템 데이터 및 로직
2. **Item.cs** - 아이템 베이스 클래스
3. **ItemTypeEnum.cs** - 아이템 관련 열거형

### NonDestroyObject (매니저) - 11개 파일
1. **AdsManager.cs** - 광고 시스템
2. **AutoManager.cs** - 오토 플레이
3. **BackgroundManager.cs** - 배경 관리
4. **CombatManager.cs** - 전투 시스템 총괄
5. **CustomLogManager.cs** - 로그 시스템
6. **DataManager.cs** - 데이터 매니저 통합
7. **NetworkManager.cs** - 서버 통신
8. **ResolutionManager.cs** - 해상도 관리
9. **Singleton.cs** - 싱글톤 베이스 클래스
10. **SoundManager.cs** - 사운드 시스템
11. **UIManager.cs** - UI 시스템 총괄

### NonDestroyObject/DataManage (데이터 관리) - 4개 파일
1. **ItemManager.cs** - 아이템 관리
2. **PlayerDataManager.cs** - 플레이어 데이터 관리
3. **StaticDataClasses.cs** - 정적 데이터 클래스
4. **StaticDataManager.cs** - 정적 데이터 관리

### UI (사용자 인터페이스) - 14개 파일
1. **AutoButton.cs** - 오토 버튼
2. **CoinEffect.cs** - 코인 이펙트
3. **CoinUI.cs** - 코인 UI
4. **LoadingPopup.cs** - 로딩 팝업
5. **Popup.cs** - 팝업 베이스 클래스
6. **PopupEnterName.cs** - 닉네임 입력 팝업
7. **PopupPause.cs** - 일시정지 팝업
8. **SimpleTextPopup.cs** - 간단한 텍스트 팝업
9. **SliderWithValue.cs** - 값 표시 슬라이더
10. **TouchParticle.cs** - 터치 파티클
11. **ValueText.cs** - 값 텍스트
12. **View.cs** - 뷰 베이스 클래스
13. **ItemSlot.cs** - 아이템 슬롯
14. **ItemSlotAdditionalTouchArea.cs** - 아이템 슬롯 터치 확장

### UI/Gatcha (뽑기) - 3개 파일
1. **GatchaPopup.cs** - 뽑기 팝업
2. **GatchaResultObj.cs** - 뽑기 결과 오브젝트
3. **GatchaTriggerObj.cs** - 뽑기 트리거 오브젝트

### UI/Inventory (인벤토리) - 3개 파일
1. **IngredientBanner.cs** - 강화석 배너
2. **IngredientValUI.cs** - 강화석 수량 UI
3. **InventoryPopup.cs** - 인벤토리 팝업

### UI/Inventory/Decompose (분해) - 1개 파일
1. **DecomposeView.cs** - 분해 뷰

### UI/Inventory/Enhance (강화) - 4개 파일
1. **EnhanceIngredButton.cs** - 강화석 버튼
2. **EnhanceResultBoard.cs** - 강화 결과 보드
3. **EnhanceTriggerObj.cs** - 강화 트리거
4. **EnhanceView.cs** - 강화 뷰

### UI/Inventory/Equip (장착) - 2개 파일
1. **EquipSlot.cs** - 장착 슬롯
2. **EquipView.cs** - 장착 뷰

### UI/Inventory/ItemView (아이템 뷰) - 1개 파일
1. **ItemView.cs** - 아이템 상세 뷰

### UI/Ranking Board (랭킹) - 2개 파일
1. **PopupRanking.cs** - 랭킹 팝업
2. **RankingScroll.cs** - 랭킹 스크롤

### UI/Touch (터치) - 6개 파일
1. **ATouch.cs** - 터치 베이스 클래스
2. **AdditionalTouchArea.cs** - 추가 터치 영역
3. **BackGroundTouch.cs** - 배경 터치
4. **NoResponseTouch.cs** - 무반응 터치
5. **RestartTouch.cs** - 재시작 터치
6. **SimpleTextBackGroundTouch.cs** - 텍스트 배경 터치

### Utils (유틸리티) - 9개 파일
1. **AnimatorUtil.cs** - 애니메이터 유틸
2. **ArrayElementTitle.cs** - 배열 요소 이름 표시
3. **CoroutineUtils.cs** - 코루틴 유틸
4. **CustomLog.cs** - 커스텀 로그
5. **IntToUnitString.cs** - 숫자 단위 변환
6. **JsonSL.cs** - JSON 직렬화
7. **NetworkReqRes.cs** - 네트워크 요청/응답 클래스
8. **NetworkResults.cs** - 네트워크 결과 열거형
9. **ResourcesLoad.cs** - 리소스 로드

### Editor (에디터) - 1개 파일
1. **BuildCommand.cs** - 빌드 자동화 스크립트

---

## 📊 통계

### 코드 라인 수
```
전체 C# 파일: 69개

카테고리별:
- Combat: 5개 파일
- Data: 3개 파일
- NonDestroyObject: 15개 파일
- UI: 36개 파일
- Utils: 9개 파일
- Editor: 1개 파일

주요 파일:
- PlayerDataManager.cs: 354줄
- CombatManager.cs: 352줄
- NetworkManager.cs: 327줄
- ItemView.cs: 290줄
- BuildCommand.cs: 288줄
```

---

## 🎯 개발 우선순위 및 핵심 파일

### 핵심 게임 로직 (반드시 이해해야 할 파일)
1. **CombatManager.cs** - 게임의 핵심 전투 루프
2. **PlayerDataManager.cs** - 모든 데이터 및 진행 상황
3. **NetworkManager.cs** - 서버 통신
4. **PlayerEntity.cs / EnemyEntity.cs** - 전투 엔티티 동작

### 게임 콘텐츠 (게임 플레이 관련)
1. **GatchaPopup.cs** - 뽑기 시스템
2. **EnhanceView.cs** - 강화 시스템
3. **EquipView.cs** - 장착 시스템
4. **ItemManager.cs** - 아이템 관리

### UI 시스템 (사용자 경험)
1. **UIManager.cs** - 모든 UI 통합 관리
2. **InventoryPopup.cs** - 인벤토리 메인
3. **ItemView.cs** - 아이템 리스트 및 상세

### 지원 시스템 (부가 기능)
1. **AdsManager.cs** - 광고 수익
2. **SoundManager.cs** - 사운드
3. **StaticDataManager.cs** - 게임 밸런스 데이터

---

## 📝 마무리

이 문서는 **Fight While Die** 프로젝트의 전체 코드 구조를 기능별로 정리한 것입니다.

### 프로젝트 특징
- **Unity 기반 모바일 게임** (Android/iOS)
- **C# 언어** 사용
- **싱글톤 패턴** 기반 매니저 구조
- **서버-클라이언트 아키텍처** (REST API)
- **비동기 처리** (UniTask)
- **PlayerPrefs** 기반 로컬 저장

### 주요 게임 시스템
1. **전투 시스템** - 타이밍 기반 액션
2. **성장 시스템** - 장비 강화 및 뽑기
3. **경제 시스템** - 코인 및 강화석
4. **소셜 시스템** - 랭킹 시스템
5. **수익 시스템** - 광고 (배너 + 보상형)

### 코드 품질
- 명확한 네임스페이스 구조
- 기능별 디렉토리 분리
- 주석 및 한글 설명 포함
- 시리얼라이즈 필드를 통한 Unity Inspector 연동

---

**문서 버전:** 1.0  
**작성일:** 2025-11-24  
**프로젝트:** Fight While Die Client  
**Repository:** https://github.com/adlet0331/FightWhileDieClient
