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
import java.util.stream.Stream;

/**
 * 装饰任务（最多只有一个子节点）
 *
 * @author wjybxx
 * date - 2023/11/25
 */
public abstract class Decorator<T> extends Task<T> {

    protected Task<T> child;

    public Decorator() {
    }

    public Decorator(Task<T> child) {
        this.child = child;
    }

    @Override
    protected void stopRunningChildren() {
        Task.stop(child);
    }

    @Override
    protected void onChildRunning(Task<T> child) {

    }

    @Override
    protected void onEventImpl(@Nonnull Object event) {
        if (child != null) {
            child.onEvent(event);
        }
    }

    // region child

    @Override
    public final int indexChild(Task<?> task) {
        if (task != null && task == this.child) {
            return 0;
        }
        return -1;
    }

    @Override
    public final Stream<Task<T>> childStream() {
        return Stream.ofNullable(child);
    }

    @Override
    public final int getChildCount() {
        return child == null ? 0 : 1;
    }

    @Override
    public final Task<T> getChild(int index) {
        if (index == 0 && child != null) {
            return child;
        }
        throw new IndexOutOfBoundsException(index);
    }

    @Override
    protected final int addChildImpl(Task<T> task) {
        if (child != null) {
            throw new IllegalStateException("A task entry cannot have more than one child");
        }
        child = task;
        return 0;
    }

    @Override
    protected final Task<T> setChildImpl(int index, Task<T> task) {
        if (index == 0 && child != null) {
            Task<T> r = this.child;
            child = task;
            return r;
        }
        throw new IndexOutOfBoundsException(index);
    }

    @Override
    protected final Task<T> removeChildImpl(int index) {
        if (index == 0 && child != null) {
            Task<T> r = this.child;
            child = null;
            return r;
        }
        throw new IndexOutOfBoundsException(index);
    }
    // endregion

    //region 序列化

    public Task<T> getChild() {
        return child;
    }

    public void setChild(Task<T> child) {
        this.child = child;
    }

    // endregion
}
