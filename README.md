# TPSMiniGameDemo

Demo的演示地址 : https://www.bilibili.com/video/BV1FD4y1o7PD/

开发采用的Unity版本是2019.4.1f1，编译后的exe在Bin文件夹中。  

1. 这是我学习Unity期间做的第三人称射击游戏Demo，从入门到做出Demo大概花了1个月的时间。
  主要完成了下面的功能：    

- 1.Player的蹲，站，走，跑，跳等角色动作实现。  
- 2.Player的射击动作，采用了FinalIK实现。   
- 3.Player的射击机制，采用Raycast实现。    
- 4.围绕着Player的第三人称摄像机轨迹实现，根据玩家射击状态进行姿态改变。   
- 5.敌人死亡的动作实现。   
- 6.枪械后坐力的模拟。   

2. 当前还存在的不足：   
- 1.没有实现相对基本的敌人状态机，比如巡逻，发现敌人，战斗等基本逻辑。  
- 2.整个地图的美术资源是从第三方网站下的，直接导入了制作好的大地图。为了避免在某些地点陷入地下，避免对建筑或者车辆等物体的穿模，对整个地图所有物体添加了碰撞。但是在某些地方依然会存在上述现象。    
- 3.路过某些带坡度的地点时，由于重力，角色会滑行。   
- 4.角色在枪械间以及部分动作之间切换时，动画效果生硬。  
- 5.还有很多就不列了。  
