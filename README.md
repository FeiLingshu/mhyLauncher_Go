# __关于对米哈游启动器支持度的相关说明__
`出于以下一些原因，截止本程序首版完成时，米哈游启动器所支持的4款游戏（分别为崩坏3、原神、崩坏·星穹铁道、绝区零），仅支持原神和崩坏·星穹铁道两款游戏`
```C
++ 由于本人没有全部游玩4款游戏（甚至截止本程序首版完成时，绝区零还没有公测），对其他游戏进行支持成本开销太大且没有办法保证适配性
++ 目前程序功能所使用的实现方式为调用Windows系统组件，受到Windows系统策略的诸多限制，如果增加游戏支持会导致出现可以预见的系统错误（下方会进行详细说明）
```
<br></br>
# __mhyLauncher_Go__
> ### __程序介绍__
本程基于米哈游启动器开发，用于支持米哈游启动器下的游戏实现窗口化宽屏适配（目前支持存在一定限制，详情见上方[相关说明](#关于对米哈游启动器支持度的相关说明)）
> ### __游戏支持列表__
- [ ] 崩坏3
- [x] 原神
- [x] 崩坏·星穹铁道
- [ ] 绝区零
> ### __运行环境__
- [x] `x64` 平台下的 `Windows` 操作系统
- [x] `.Net Framework 4.0` 运行环境
- [ ] [`Releases`](../../releases) 中将会提供 `.Net Framework 4.0` 的安装包和下载地址
> ### __程序安装位置及相关命令行参数__
`由于米哈游启动器及其启动的任意游戏均会以管理员身份运行，本程序在启动时需申请管理员权限（UAC）`
* 程序需安装在米哈游启动器安装文件夹中，和launcher.exe相同的目录下
* 程序支持两种命令行参数：
```html
参数1：(PHONE)  【执行窗口化宽屏适配模式，在2K分辨率标准下，以手机屏幕分辨率显示游戏画面，显示分辨率为2400×1080】
参数2：(PC)     【执行窗口化宽屏适配模式，在2K分辨率标准下，以21:9显示比例显示游戏画面，显示分辨率为2520×1080】  
_____________
以上显示效果均基于2K分辨率计算，其他屏幕空间大小将以2K分辨率下参数为基准等比例进行适配
命令行参数中不包含空格，且括号均为半角（英文）括号
```
- [x] 本程序未调用任何注入相关的API，不会对启动器进程及游戏进程进行任何修改
> ### __使用说明__
* 将程序文件安装到指定位置后，可直接运行本程序启动米哈游启动器并进行挂载（也可在米哈游启动器运行时运行本程序直接进行挂载）
* 在挂载本程序前，请先进入游戏确认游戏正以窗口化模式运行，否则程序将无法对游戏窗口进行任何处理
* 程序成功挂载后，米哈游启动器窗口右下角会出现提示，启动游戏时按正常流程操作即可
* 程序成功检测到游戏窗口后，会使用窗口标题进行提示，按 `F11` 可使程序强制进行一次窗口调整，按 `F12` 可以在是否锁定窗口位置间切换
- [x] 更多详细使用说明请[前往这里](https://space.bilibili.com/483822869)进行查看
> ### __支持__
* > 如程序在运行中发生崩溃并弹出崩溃提示窗口，请记录其提示信息，并提交给作者  
* > 程序已开源，如需增加任何游戏的支持，可自行下载源代码进行修改并重新编译（代码90%+已注释，请确保自己有足够的编程能力）；修改后的程序可能存在运行不稳定的问题，详情见上方[相关说明](#关于对米哈游启动器支持度的相关说明)
- [x] 请尽量不要在GitHub上进行反馈，作者不会经常访问GitHub查看反馈信息，反馈请[前往这里](https://space.bilibili.com/483822869)的评论区进行反馈




<br></br>
___
> ### **_本程序由作者本人（即 FeiLingshu）原创编写。_**
