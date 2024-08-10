# _该分支为需配合 [`NoWMI版本`](https://github.com/FeiLingshu/mhyLauncher_Go/tree/mhyLauncher_Go_NoWMI) 使用的DLL库_
### _该DLL库能够支持在部分配置的设备上实现更好的能效表现_
### _若要使用该DLL库提供的功能，请将DLL库文件复制到与程序本体相同目录下_
```diff
# 该实现基于 Windows 11 22H2 版本及以上版本中，由 Microsoft 提供的应用程序效能模式开关
- 在低于 Windows 11 22H2 的版本（不包括此版本）中，将无法使用此DLL库提供的功能，即使已经安装此DLL库文件
+ 该功能对程序运行的具体影响（由于使用的设备不尽相同）暂时无法验证，如使用该功能后遇到运行问题，可以选择删除该DLL库文件，删除后程序将不再会执行相关功能的代码
+ 若明确不需要该DLL库提供的功能，则可以直接选择不下载和复制该DLL库文件
```
### _如需下载使用该DLL库的功能，请前往 [`Releases`](https://github.com/FeiLingshu/mhyLauncher_Go/releases)页面，选择带有 `Pre-release` 标记的版本进行下载_
- [x] _由于该DLL库相关代码与 [`NoWMI版本`](https://github.com/FeiLingshu/mhyLauncher_Go/tree/mhyLauncher_Go_NoWMI) 高度绑定，故DLL库文件下载链接仅在带有 `Pre-release` 标记的版本中存在_
- [x] _带有 `Pre-release` 标记的版本不会进行更新迭代，当有新版本上传时，旧版本的相关文件将会被覆盖_




<br></br>
___
> ### **_本程序由作者本人（即 FeiLingshu）原创编写。_**

