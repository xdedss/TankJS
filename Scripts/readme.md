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
info.me.id
info.me.faction
info.me.x
info.me.z
info.me.rotation
info.me.health
info.me.attack

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
```

## 注意事项

- 每个坦克在独立的环境里执行，可以在自己的环境里乱搞也不会互相影响
- 修改info内容只会影响到自己环境里的数据，不会有实际的效果

## 例

只会绕圈的坦克

```javascript
var turn = true;
function update(){
    turn ^= true;
    return turn ? 3 : 1;
}
```
