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
package cn.wjybxx.btree;

import javax.annotation.Nonnull;

/**
 * 条件节点
 * 1. 大多数条件节点都只需要返回bool值，不需要详细的错误码，因此提供该模板实现。
 * 2. 并非所有条件节点都需要继承该类
 *
 * @author wjybxx
 * date - 2023/11/25
 */
public abstract class ConditionTask<T> extends LeafTask<T> {

    @Override
    protected final void execute() {
        if (test()) {
            setSuccess();
        } else {
            setFailed(TaskStatus.ERROR);
        }
    }

    protected abstract boolean test();

    @Override
    public boolean canHandleEvent(@Nonnull Object event) {
        return false;
    }

    /** 条件节点正常情况下不会触发事件 */
    @Override
    protected void onEventImpl(@Nonnull Object event) {

    }

}
