/*
 * Copyright 2024 wjybxx(845740757@qq.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package cn.wjybxx.btree.branch;

import cn.wjybxx.btree.Task;

/**
 * Join的完成策略
 * 1.不要在Policy上缓存Join的child。
 * 2.尽量少的缓存数据
 *
 * @author wjybxx
 * date - 2023/12/2
 */
public interface JoinPolicy<T> {

    /** 重置自身数据 */
    void resetForRestart();

    /** 启动前初始化 */
    void beforeEnter(Join<T> join);

    /** 启动 */
    void enter(Join<T> join);

    /**
     * Join在调用该方法前更新了完成计数和成功计数
     *
     * @param child 进入完成状态的child
     */
    void onChildCompleted(Join<T> join, Task<T> child);

    /**
     * join节点收到外部事件
     *
     * @param event 收到的事件
     */
    void onEvent(Join<T> join, Object event);

}