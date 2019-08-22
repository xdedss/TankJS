turn = true;

function update(){
  info.me.health = 1000;//Ã»ÓÃ
  
  log("I'm at " + info.me.x + "," + info.me.z);
  if(info.tanks.length >= 1){
    var ids = "";
    for(var i = 0; i < info.tanks.length; i++){
      ids += info.tanks[i].id;
      ids += ",";
    }
    log("I can see tanks id=" + ids);
  }
  else{
    log("No enemy");
  }
  turn ^= true;
  return turn ? 3 : 1;
}