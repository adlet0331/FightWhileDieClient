# 수익이 되는 간단한 모바일 게임 개발
## 목적: 수익이 들어오는 모바일 게임을 만들어보자

장착한 칼들의 기본 공격력 + 옵션으로 산출된 공격력으로 스테이지를 뚫으며 보상을 얻으며 무한 성장을 하는 게임

## 장비
획득: 뽑기로만 획득 가능

### 종류
1. 칼 (2개 장비 가능, 메인/서브)
2. 망토 (1개만 장비 가능)
3. 펜던트 (1개만 장비 가능)

### 분류
이름, 레어도, 옵션
1. 이름: 랜덤. Sprite 결정
2. 레어도: 랜덤
3. 옵션: 랜덤 (수치는 레어도에 따라 달라짐)

### 레어도
1. Common
2. Rare
3. Epic
4. Unique
5. Legend
6. God

### 옵션
옵션의 종류
1. 오토
- 자동 훈련 해금, 자동 훈련 시 디버프 감소
2. 더 높은 스테이지 지향
- 공격력 + N / + n%
- 기본 공격력 획득량 + N
- 코인 획득량 + N / + n%
3. 유틸
- 공격 후 딜레이 - n%
- 적 달리기 속도 증가 + n%

## 장비 성장

### 장비 강화
##### 1. 방법
- 경험치, 코인을 소모하여 장비 경험치 증가   
- 실패시 강화도가 떨어짐
- 강화 확률을 100%로 만들어주는 강화석이 존재. 현재 레벨 만큼 소모해야 함

### 강화석
장비 분해시 획득
> 장비의 레어도와 같은 강화석을 레벨 만큼 획득 (감가)

##### 2. 확률

- 모든 장비들이 동일함
- 다만,   

- 초반 (레벨 10까지) 너무 어렵지 않게, 모든 유저들이 재미를 느끼는 구간
- 중반 (레벨 20까지) 챌린징한 구간, 대부분의 유저가 상주 할 구간
- 후반 (레벨 25까지) 비틱의 구간, 소수의 유저만이 도달할 구간
- 뒤의 + 수치는 재료 투입 시 추가되는 확률
----------
> 1. 90  + 10
> 2. 80  + 10
> 3. 70  + 20
> 4. 60  + 30
> 5. 50  + 30
> 6. 45  + 30
> 7. 40  + 30
> 8. 35  + 30
> 9. 35  + 30
----------
> 10. 30 + 20
> 11. 30 + 20
> 12. 30 + 20
> 13. 30 + 20
> 14. 30 + 20
> 15. 25 + 20
> 16. 25 + 20
> 17. 25 + 20
> 18. 20 + 20
> 19. 20 + 20
----------
> 20. 10 + 10
> 21. 10 + 10
> 22. 5  + 5
> 23. 3  + 2
> 24. 1  + 1

##### 3. 효과

레벨에 따라 옵션의 수치 증가 증가   
구간을 3개로 나눔   
- 1 ~ 10  (9번)
- 10 ~ 20 (10번)
- 20 ~ 25 (5번)

엑셀로 정리   

서버에서 매번 클라로 json 형식으로 쏴줌

- 공격력   
n 초기값: 20, 50, 100, 200, 500, 2500   
N 초기값: 200, 500, 1000, 2000, 5000, 10000   

- 코인   
n 초기값: 2, 5, 10, 25, 50, 100   
N 초기값: 10, 25, 50, 100, 250, 1000   

아래는 초기값 곱하기 N의 N 값.

- 1 ~ 10 구간
1. 1
2. 2
3. 3
4. 4
5. 5
6. 6
7. 7
8. 8
9. 9
10. 10
- 10 ~ 20 구간
11. 15
12. 20
13. 25
14. 30
15. 35
16. 40
17. 45
18. 50
19. 60
20. 70 
- 21 ~ 25 구간
21. 100
22. 120
23. 140
24. 180
25. 250

##### 4. 비용
- 레어도별 시작 코인: 1, 2, 4, 10, 20, 50
- 1 ~ 13 : (시작 코인) * level 
- 14 ~ 22: (시작 코인) * (3 * (level - 13) + 13)
- 23 ~ 25: (시작코인) * (100, 250, 1000)

##### 5. 최대 강화
- Epic 이하는 15, Legend는 20, God는 25

### 장비 돌파
- 조건: 돌파하는 장비가 최대 수치까지 강화가 되어 있어야 함
- 효과: 레어도 1단계 증가
- 돌파 비용: 주체 이외 2개. 레어도, 옵션이 같아야 함. 강화도는 상관 없음
- 강화 후: 강화도가 5 내려가서 시작함 (15 -> 10, 20 -> 15)

## 뽑기
- 비용: 10 ^ 하루에 시도한 횟수 
- 광고 보면 하루에 한 번 무료 

### 확률
1. Common: 나머지
2. Rare: 1 / 4^1 = 1/4
3. Epic: 1 / 4^2 = 1/16
4. Unique: 1 / 4^3 = 1/64
5. Legend: 1 / 4^4 = 1/256
6. God: 1 / 4^5 = 1/1024

## 플레이어

### 스테이터스
#### 1. 공격력
1. 기본 공격력: 스테이지를 하나 클리어하면 올라간다
2. 총 공격력: (캐릭터 기본 공격력 + 칼의 기본 공격력) * (장비 옵션 %)
#### 2. HP
1로 고정. 한대 맞으면 죽음
#### 3. 공격 선 딜레이
고정. 변동 X
#### 4. 공격 후 딜레이
장비 스테이터스로 감소 가능

### 재화
1. 코인
강화, 뽑기에 쓰이는 재화
2. 강화석
장비를 분해하여 나오는 강화 재료

### 옵션
1. 난이도   
상, 중, 하로 나뉘어짐.
> 적 AI의 공격 딜레이 조정

##### 보상
1. 상: 보상 x 1.5
2. 중: 보상 x 1.0
3. 하: 보상 x 0.8

## 스테이지
플레이어는 움직이지 않고, 적이 움직이는 타이밍에 맞추어 공격을 해서 적의 HP를 모두 깎으면 승리

#### 적 AI
- 사거리에 들어왔을 때, 랜덤하게 뒤점프 or 공격을 함
- 적 HP는 일정 비율로 증가. 플레이어에게 UI로만 알려줌
> 50 * (1.2) ^ (stage - 1)    
> 1.2 ^ 10 = 6.2   
- **1 :   300**
- **11:   1,857**
- **21:   11,501**
- **31:   71,212**
- **41:   440,931**
- **51:   2,730,131**
- **61:   16,904,254**
- **71:   104,666,687**
- **81:   648,068,538**
- **91:   4,012,669,574**
- **100:  24,845,392,356** 

#### 보상
- Stage에 비례하게, 다중 클리어 상정하고 설계
- 보상: 기본 공격력 상승, 코인
1. 기본 공격력 상승: 경험치, 레벨 개념. 시간으로 살 수 있는 개념
- (1.1 ^ Stage) * 10 만큼 상승
2. 코인: 재화 수급
- (1.1 ^ Stage) * 10 만큼 획득

### 밸런싱
일단 100스테이지가 최종 목표로 구성
- 250억을 어떻게 찍을 수 있을까..
- 1달 정도 잡고 생각해보자
- 이론상 God 칼: 6250배, 공격력 + 250만 = 156억 

## 랭킹
간단한 DB가 있는 서버로 랭킹 만들어 보기   
언제 업데이트 되냐? 일정 시간 (1분?) 마다 되면 좋겠다 

### 랭킹 목록 
1. 공격력
2. 클리어 한 TOP 스테이지
3. 한 번에 가장 길게 클리어한 스테이지 수

## 재미
- 장비 강화
- 뽑기
- 랭킹 경쟁

