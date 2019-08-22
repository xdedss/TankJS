//碰到墙随机转向
//碰到敌人时射击

var directions = [[0, 1], [1, 0], [0, -1], [-1, 0]];
var actions = [[0, 0], [1, 0], [-1, 0], [0, -1], [0, 1], [0, 0], [2, 0]];

function update(){
  var frontPos = getFrontPos(1);
  if(sampleTank(frontPos) != null){
    return 5;
  }
  if(sampleMap(frontPos) == 0 && Math.random() < 0.92){
    return 6;
  }
  return randomIn([3, 4]);
}

function getActionPos(rotation, actionIndex){
  var forward = directions[rotation];
  var action = actions[actionIndex];
  return [action[0] * forward[0], action[0] * forward[1], action[1]];
}

function addPos(pos1, pos2){//3
  if(pos1.length == 3)
    return [pos1[0] + pos2[0], pos1[1] + pos2[1], norm4(pos1[2] + pos2[2])];
  else
    return [pos1[0] + pos2[0], pos1[1] + pos2[1]];
}

function multiplyPos(pos, n){//3
  if(pos.length == 3)
    return [pos[0] * n, pos[1] * n, norm4(pos[2] * n)];
  else
    return [pos[0] * n, pos[1] * n];
}

function getPos(tank){
  return [tank.x, tank.z, tank.rotation];
}

function getFrontPos(distance){
  var frontDelta = multiplyPos(getActionPos(info.me.rotation, 1), distance);
  var frontPos = addPos(getPos(info.me), frontDelta);
  return frontPos;
}

function sampleMap(pos){//2
  if(inBound(pos)){
    return info.mapData[pos[0]][pos[1]];
  }
  return -1;
}

function sampleTank(pos){//2
  //log("search in tanks " + info.tanks.length);
  for(var i = 0; i < info.tanks.length; i++){
    if(info.tanks[i].x == pos[0] && info.tanks[i].z == pos[1]){
      return info.tanks[i];
    }
  }
  return null;
}

function inBound(pos){
  return pos[0] >= 0 && pos[0] < info.mapData.length && pos[1] >= 0 && pos[1] < info.mapData[0].length;
}

function norm4(num){
  return (num % 4 + 4) % 4;
}

function randomIn(arr){
  var i = Math.floor(Math.random() * arr.length);
  return arr[i];
}