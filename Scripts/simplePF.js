//A* pathfinding example
//utf-8

var directions = [[0, 1], [1, 0], [0, -1], [-1, 0]];//[x,z]
var actions = [[0, 0], [1, 0], [-1, 0], [0, -1], [0, 1], [0, 0], [2, 0]];//[forward,rotation]

var nodeMap = [];
var openNodes;
var hFactor = 0.75;

var pathCache = [];
var targetPosition = null;
var flag = true;

//在左上角和右下角之间来回走
function update(){
  var myPosition = getPos(info.me);
  if(targetPosition == null || equalPos(myPosition, targetPosition)){
    targetPosition = (flag ^= true) ? [0, 0, 0] : [info.mapData.length - 1, info.mapData[0].length - 1, 0];
  }
  if(!pfValidate(pathCache, myPosition, targetPosition, equalPos)){
    pathCache = pfSearch(myPosition, targetPosition, equalPos, taxiDistance);//从当前位置前往target，严格相等，曼哈顿距离
  }
  log(pathCache.join("-"));
  return pathCache.length == 0 ? 0 : pathCache.shift();
}

//pos1 pos2 初始位置和目标位置
//cond 判断终止条件的函数，function(p1, p2)，返回布尔值，判断p1是否到达p2
//heuristic 启发函数，function(p1, p2)，返回数字
function pfSearch(pos1, pos2, cond, heuristic){//pos v3
  pfResetMap();
  pfUpdateNode(pos1, 0, heuristic(pos1, pos2), 0);//打开初始位置节点
  while(openNodes.length != 0){
    var bestNodePos = pfBestNodePos();//权重最小的节点
    //log(bestNodePos[0] + "," + bestNodePos[1] + ";" + heuristic(bestNodePos, pos2));
    if(cond(bestNodePos, pos2)){//已经到达终点？
      return pfTracePath(bestNodePos);//返回路径
    }
    var bestNode = pfSampleMap(bestNodePos);
    var allPos = [];
    allPos.push(addPos(bestNodePos, getActionPos(bestNodePos[2], 6)));//前进2格 特殊处理
    for(var act = 1; act < 5; act++){
      allPos.push(addPos(bestNodePos, getActionPos(bestNodePos[2], act)));//其余行动
    }
    if(pfSampleMap(allPos[1]) != -1){//前进2格需判断前方1格是否可走
      pfUpdateNode(allPos[0], bestNode.cost + 1, heuristic(allPos[0], pos2), 6);
    }
    for(var act = 1; act < 5; act++){
      pfUpdateNode(allPos[act], bestNode.cost + 1, heuristic(allPos[act], pos2), act);//其余行动的节点
    }
    pfCloseNode(bestNodePos);
  }
  return [];//找不到路径
}

function pfValidate(pathC, pos1, pos2, cond){
  for(var i = 0; i < pathC.length; i++){
    pos1 = addPos(pos1, getActionPos(pos1[2], pathC[i]));
  }
  return cond(pos1, pos2);
}

function pfResetMap(){
  openNodes = [];
  if(nodeMap.length == 0){
    for(var x = 0; x < info.mapData.length; x++){
      var xarr = [];
      for(var y = 0; y < info.mapData[0].length; y++){
        var yarr = [];
        for(var r = 0; r < 4; r++){
          yarr.push(0);
        }
        xarr.push(yarr);
      }
      nodeMap.push(xarr);
    }
  }
  for(var x = 0; x < info.mapData.length; x++){
    for(var y = 0; y < info.mapData[0].length; y++){
      if(sampleMap([x, y]) == 0){
        for(var r = 0; r < 4; r++){
          nodeMap[x][y][r] = 0;
        }
      }
      else{
        for(var r = 0; r < 4; r++){
          nodeMap[x][y][r] = -1;
        }
      }
    }
  }
}

function pfSampleMap(pos){
  if(inBound(pos)){
    return nodeMap[pos[0]][pos[1]][pos[2]];
  }
  return -1;
}

function pfTracePath(pos){
  var node = pfSampleMap(pos);
  if(node.prev == 0){
    return [];
  }
  else{
    var prevMove = getActionPos(pos[2], node.prev);
    var prevPos = addPos(pos, revPos(prevMove));
    var prevPath = pfTracePath(prevPos);
    prevPath.push(node.prev);
    return prevPath;
  }
}

function pfUpdateNode(pos, cost, est, prev){
  var node = pfSampleMap(pos);
  if(node == 0){
    nodeMap[pos[0]][pos[1]][pos[2]] = {
      cost : cost,
      est : est,
      prev : prev//每个节点记录上一个动作，这样最后就能顺着路径找回去
    };
    pfOpenNode(pos);
  }
  else if(typeof(node) == 'object'){
    if(pfNodeSum(node) > cost + est * hFactor){
      node.cost = cost;
      node.est = est;
      node.prev = prev;
      pfOpenNode(pos);
    }
  }
}

function pfBestNodePos(){
  var bestNodePos = null;
  var bestSum = Infinity;
  for(var i = 0; i < openNodes.length; i++){
    var nodePos = openNodes[i];
    var node = pfSampleMap(nodePos);
    if(typeof(node) == 'object'){
      if(pfNodeSum(node) < bestSum){
        bestSum = pfNodeSum(node);
        bestNodePos = nodePos;
      }
    }
  }
  return bestNodePos;
}

function pfNodeSum(node){
  return node.cost + node.est * hFactor;
}

function pfCloseNode(pos){
  for(var i = 0; i < openNodes.length; i++){
    var nodePos = openNodes[i];
    if(equalPos(nodePos, pos)){
      openNodes.splice(i, 1);
      return;
    }
  }
}

function pfOpenNode(pos){
  for(var i = 0; i < openNodes.length; i++){
    var nodePos = openNodes[i];
    if(equalPos(nodePos, pos)){
      return;
    }
  }
  openNodes.push(pos);
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

function revPos(pos){
  if(pos.length == 3)
    return [-pos[0], -pos[1], -pos[2]];
  else
    return [-pos[0], -pos[1]];
}

function multiplyPos(pos, n){//3
  if(pos.length == 3)
    return [pos[0] * n, pos[1] * n, norm4(pos[2] * n)];
  else
    return [pos[0] * n, pos[1] * n];
}

function equalPos(pos1, pos2){
  if(pos1.length != pos2.length) return false;
  if(pos1.length == 3)
    return (pos1[0] == pos2[0] && pos1[1] == pos2[1] && pos1[2] == pos2[2]);
  else
    return (pos1[0] == pos2[0] && pos1[1] == pos2[1]);
}

function taxiDistance(pos1, pos2){
  return Math.abs(pos1[0] - pos2[0]) + Math.abs(pos1[1] - pos2[1]);
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

//lzr 2019/10/03
