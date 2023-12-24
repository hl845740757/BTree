# 模块说明

btree-core是从bigcat中分离出来的，为保持最小依赖，核心包只依赖我个人的base包和jsr305注解包；
但行为树是需要能序列化的，这样才能在编辑器中编辑；在bigcat仓库的时候，btree模块依赖了我的dson-codec包，
但dson-codec包的类比较多，依赖也比较大(尤其是fastutil)，因此在该仓库将dson序列化做为可选项。

btree-codec是基于dson-codec的行为树序列化实现；btree-codec模块仅有几个配置类，真正的codec是基于dson-apt注解自动生成的。
如果你需要使用基于dson的行为树序列化实现，可以添加btree-codec到项目。