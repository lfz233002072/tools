C# 工具箱，提供Socket（TCP、UDP协议）、Redis、activemq、数据库访问等技术的封装实现 

###1、缓存 
RedisCacheManager：Redis缓存封装
NetMemoryCacheManager：内存缓存封装
FileCacheManager：文件缓存封装

###2、CQRS(命令查询模式)实现
Lfz.Commands.ICommandBus 命令总线
Lfz.Commands.ICommandHandler 命令处理Handler

###3、配置
在Config目录下实现了JSON、XML两种配置方式

###4、数据仓储
在Data目录下实现了数据仓储基本功能，包括NHibernate、ADO.NET、EF等方式


###5、文件访问

在IO目录下实现文件访问功能

###6、Logging 多中类型日志封装
已实现log4net、Nlog等日志封装

###7、ActiveMQ 封装

###8、socket封装
包括高性能TCP、UDP封装、http协议的socket实现，tcp客户端访问封装等

###9、rest api实现封装

###10、Security 安全加密相关实现

###11、Services目录实现定时器、多线程任务等功能
定时器基类：TimeServiceBase
线程服务基类：ThreadServiceBase

###12、WCF封装
实现WCF宿主工厂等功能

###13、Utitlies工具类
Utils： 辅助工具箱，包括IP获取、字符转换、枚举处理等
TypeParse：类型转化工具
ZipHelper：压缩与解压缩实现
ExportEngine：excel 导入、导出管理
NetMailUtils：发送邮件实现

[github地址：https://github.com/lfz233002072/tools](https://github.com/lfz233002072/tools)

[oschina地址:https://git.oschina.net/lfz/tools](https://git.oschina.net/lfz/tools)
