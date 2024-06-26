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
package cn.wjybxx.btree.branch.join;

import cn.wjybxx.btree.Task;
import cn.wjybxx.btree.TaskStatus;
import cn.wjybxx.btree.branch.Join;
import cn.wjybxx.btree.branch.JoinPolicy;
import cn.wjybxx.btree.branch.Selector;

/**
 * {@link Selector}
 *
 * @author wjybxx
 * date - 2023/12/2
 */
public class JoinSelector<T> implements JoinPolicy<T> {

    private static final JoinSelector<?> INSTANCE = new JoinSelector<>();

    @SuppressWarnings("unchecked")
    public static <T> JoinSelector<T> getInstance() {
        return (JoinSelector<T>) INSTANCE;
    }

    @Override
    public void resetForRestart() {

    }

    @Override
    public void beforeEnter(Join<T> join) {

    }

    @Override
    public void enter(Join<T> join) {
        if (join.getChildCount() == 0) {
            join.setFailed(TaskStatus.CHILDLESS);
        }
    }

    @Override
    public void onChildCompleted(Join<T> join, Task<T> child) {
        if (child.isSucceeded()) {
            join.setSuccess();
        } else if (join.isAllChildCompleted()) {
            join.setFailed(TaskStatus.ERROR);
        }
    }

    @Override
    public void onEvent(Join<T> join, Object event) {

    }
}