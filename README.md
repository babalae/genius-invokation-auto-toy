更多： [🐟 自动钓鱼机](https://github.com/babalae/genshin-fishing-toy) | [🛠️账号切换](https://github.com/babalae/mihoyo-starter)

# 🎲 七圣召唤PVE全自动打牌

PC原神七圣召唤PVE全自动打牌。

<del>[📺视频演示](https://www.bilibili.com/video/BV13h4y1L7PH)</del>

当前只完整支持无缩放 1920x1080 的窗口化游戏，其它分辨率很有可能会没法正常识别

支持角色邀请、每周来客挑战、部分大世界NPC挑战。

部分场景不支持、或者打不过、拿不满奖励。

1.3版本开始完全支持角色被超载、冻结等异常情况（雷电将军相关卡组由于无充能判断，可能在被冻结的情况下无法进行后续步骤）。

其他分辨率、语言想要支持也简单的，只要在对应分辨率下截取一些图片替换软件目录下的文件即可。

## 截图

![image](https://github.com/babalae/genius-invokation-auto-toy/assets/15783049/b41dc84f-6a83-406c-81ef-3eaddeac28c6)

## 下载地址

[📥Github下载（1.3）](https://github.com/babalae/genius-invokation-auto-toy/releases/download/1.3/GeniusInvokationAutoToy.v1.3.4.zip)

[📥蓝奏云下载](https://wwmy.lanzouq.com/b00r9kqwf) 密码:coco


## 使用方法

由于图像识别比较吃性能，低配置电脑可能无法正常使用。

你的系统需要满足以下条件：
  * Windows 7 或更高版本
  * 系统是64位（都已经2023年了，总没有还在用32位系统的人了吧）
  * [.NET Framework 4.7.2](https://support.microsoft.com/zh-cn/topic/%E9%80%82%E7%94%A8%E4%BA%8E-windows-%E7%9A%84-microsoft-net-framework-4-7-2-%E7%A6%BB%E7%BA%BF%E5%AE%89%E8%A3%85%E7%A8%8B%E5%BA%8F-05a72734-2127-a15d-50cf-daf56d5faec2) 或更高版本。**低于此版本在打开程序时可能无反应，或者直接报错**。

**游戏推荐图像设置（图像质量中以上）：**

<img width="600px" src="https://github.com/babalae/genius-invokation-auto-toy/assets/15783049/d1923d04-42ca-4078-839a-1ba38325922f"/>

1、首先你的牌组必须是 **【莫娜、砂糖、琴】** 或者 **【刻晴、雷电将军、甘雨】** （其他牌组可以参考下面的“自定义自动打牌策略”），顺序不能变，带什么牌无所谓。（[颠勺牌组玩法来源](https://www.bilibili.com/video/BV1ZP41197Ws)，雷神牌组来源NGA）

<img width="300px" src="https://raw.githubusercontent.com/babalae/genius-invokation-auto-toy/main/Image/p1.png"/>
<img width="300px" src="https://github.com/babalae/genius-invokation-auto-toy/assets/15783049/26b87618-473c-4a48-b5b3-dab0842118d5"/>

2、**只支持1920x1080分辨率的游戏，游戏整个界面不能被其他窗口遮挡！游戏不能使用任何显卡滤镜！**

3、在游戏内进入七圣召唤对局，到**初始手牌**界面，如下图：

<img width="500px" src="https://raw.githubusercontent.com/babalae/genius-invokation-auto-toy/main/Image/p2.png"/>

4、然后直接点击开始自动打牌，双手离开键盘鼠标（快捷键<kbd>F11</kbd>）。

## 自定义自动打牌策略

在软件当前目录的 `strategy` 的文件夹下，复制一个策略示例txt文件，自行参考格式编辑即可，注意技能1~3是**从右往左数**的。软件会自动根据行动策略和当前对局情况来切换角色和使用技能。

如果你有更好的卡组策略、或者是某种情况下的针对解法，欢迎发[Issue](https://github.com/babalae/genius-invokation-auto-toy/issues)分享~

## FAQ
* 为什么需要管理员权限？
  * 因为游戏以管理员权限启动，软件不以管理员权限启动的话没法模拟鼠标点击。
* 会不会封号？
  * 只能说理论上不会被封，但是mhy是自由的，用户条款上明确说明模拟操作是封号理由之一。当前使用了 mouse_event 模拟鼠标点击，还是存在被检测的可能。只能说请低调使用，请不要跳脸官方。

## 问题反馈

提 [Issue](https://github.com/babalae/genius-invokation-auto-toy/issues) 或 QQ群[894935931](https://qm.qq.com/cgi-bin/qm/qr?k=u9Ij0HrDVQhvcoFvaiQGv38V3R7ZNY6K&jump_from=webapi&authKey=N++f74HhGHDzFje1dDD6E8vzuf45jmSFaPiVbc3Z7x/nTUWGwZ3UdSPqYQqPfOXK)
 
## 投喂

觉的好用的话，可以支持作者哟ヾ(･ω･`｡) 👇
* [⚡爱发电](https://afdian.net/@huiyadanli)
* [🍚微信赞赏](https://github.com/huiyadanli/huiyadanli/blob/master/DONATE.md)
