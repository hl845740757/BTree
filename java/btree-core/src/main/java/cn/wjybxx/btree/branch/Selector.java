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
import cn.wjybxx.btree.TaskStatus;

import javax.annotation.Nullable;
import java.util.List;

/**
 * @author wjybxx
 * date - 2023/11/26
 */
public class Selector<T> extends SingleRunningChildBranch<T> {

    public Selector() {
    }

    public Selector(List<Task<T>> children) {
        super(children);
    }

    public Selector(Task<T> first, @Nullable Task<T> second) {
        super(first, second);
    }

    @Override
    protected void onChildCompleted(Task<T> child) {
        runningChild = null;
        if (child.isCancelled()) {
            setCancelled();
            return;
        }
        if (child.isSucceeded()) {
            setSuccess();
        } else if (isAllChildCompleted()) {
            setFailed(TaskStatus.ERROR);
        } else if (!isExecuting()) {
            template_execute();
        }
    }
}