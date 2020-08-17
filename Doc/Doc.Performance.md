
# 系统性能策略

## 页面调用统计（含接口）

- [x] 可以在MonitorModule里面实现
- [x] 担心会增加数据库压力，现阶段访问仅做文本日志
- [x] 如果存储在数据库中，需考虑高性能方案，如nosql数据库

## 缓存方案

### 文件缓存

- 页面缓存：依赖Asp.net 的 PageCache 机制
- 图片及缩略图缓存：内存方式 & 物理方式
- 动态页面缓存：物理方式
- Word文档缓存：物理方式

### 数据缓存

- EF 扩展类库提供缓存能力
- DAL层内建实现
    * EntityBase.GetCache().Select
    * EntityBase.RefleshCache();
    * Get()先尝试从缓存层取数据（参考Gentle.net）
    * Save()保存在缓存中，SaveToDb（）保存到数据库
    * Application 级别的 Cache
    * Application 级别始终打开 数据库连接？
- 在HttpApi接口层实现
    [HttpApi("Name", CacheSeconds=x)]
- 分布式缓存考虑
    * 多个网站，一个数据提供出口（http或binary）
    * 研究分布式缓存框架: Memcached, Radis, ...



## 消息队列

利用消息队列进行系统消息解耦
用 SignalR 推送到客户端

