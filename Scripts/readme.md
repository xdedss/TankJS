## 食用方法

每回合会向环境中传入info变量，之后调用update方法，update方法需要返回一个整数来表示这一回合的行动。  

```
0 - 什么都不做
1 - 前进1格
2 - 后退1格
3 - 左转
4 - 右转
5 - 攻击
6 - 前进2格
```

info变量结构如下  

```javascript
//info.me为自己的信息，包含：
info.me.id //id唯一且不变
info.me.faction //暂时没用
info.me.x //坐标为整数，从0开始
info.me.z
info.me.rotation //0表示z轴正方向，1表示x轴正方向，2表示z轴负方向，3表示x轴负方向，即每向右转90度就加一
info.me.health //初始为100
info.me.attack //初始为10

//info.tanks为当前可见的其他坦克信息，数组，每一项结构与info.me相同，如：
info.tanks[0].id
info.tanks[0].faction
info.tanks[0].x
info.tanks[0].z
info.tanks[0].rotation
info.tanks[0].health
info.tanks[0].attack

//info.items为当前可见的道具信息，数组，每一项结构如：
info.items[0].x
info.items[0].z
info.items[0].type //整数， 1=heal 2=attack 3=radar
//heal 加10health
//attack 下一次攻击力变成20
//radar 无论如何都能看到对方，持续5回合

//info.mapData为地图，两层数组，info.mapData[x][z]表示坐标(x,z)的格子的地形
//0表示平地 1表示凸起的障碍物方块 -1表示坑，只有平地可以走
info.mapData[x][z]
```

### 视野

敌方坦克只有在视野内的时候才会出现在info.tanks中，道具也必须在视野内才会出现在info.items中。  

距离小于等于2格的物体必定可见；否则只有目标物体在坦克正前方120°以内且没有被凸起的障碍物遮挡时才可见（由坦克中心到物体中心的线段没有穿过任何方块即认为没有被遮挡）。  

### 攻击

攻击的时候，可以打到自己正前方一条直线上没有被挡住的坦克，伤害会随着距离变远而减弱。紧贴着攻击时造成的伤害等于攻击力，每增加1格，伤害减少2点，因此隔着5格或以上攻击没有效果（如果有攻击加成则能打到更远的距离）。  

### 运行环境

每个坦克在独立的环境里执行，可以在自己的环境里乱搞也不会互相影响；修改info内容只会影响到自己环境里的数据，不会有实际的效果。  

可以定义全局变量来保存数据，回合结束后数据不会消失，可以保留到以后的回合使用。  

为了防止死循环，默认如果2000毫秒还没有返回值就判定为超时，放弃本回合。

  

## 例

只会绕圈的坦克

```javascript
var turn = true;
function update(){
    turn ^= true;
    return turn ? 3 : 1;
}
```
