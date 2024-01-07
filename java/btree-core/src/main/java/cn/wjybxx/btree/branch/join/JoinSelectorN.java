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

import cn.wjybxx.btree.Status;
import cn.wjybxx.btree.Task;
import cn.wjybxx.btree.branch.Join;
import cn.wjybxx.btree.branch.JoinPolicy;
import cn.wjybxx.btree.branch.SelectorN;

/**
 * {@link SelectorN}
 *
 * @author wjybxx
 * date - 2023/12/2
 */
public class JoinSelectorN<T> implements JoinPolicy<T> {

    /** 需要达成的次数 */
    private int required = 1;
    /** 是否快速失败 */
    private boolean failFast;

    public JoinSelectorN() {
    }

    public JoinSelectorN(int required) {
        this.required = required;
    }

    @Override
    public void resetForRestart() {

    }

    @Override
    public void beforeEnter(Join<T> join) {

    }

    @Override
    public void enter(Join<T> join) {
        if (required <= 0) {
            join.setSuccess();
        } else if (join.getChildCount() == 0) {
            join.setFailed(Status.CHILDLESS);
        } else if (checkFailFast(join)) {
            join.setFailed(Status.INSUFFICIENT_CHILD);
        }
    }

    @Override
    public void onChildCompleted(Join<T> join, Task<T> child) {
        if (join.getSucceededCount() >= required) {
            join.setSuccess();
        } else if (join.isAllChildCompleted() || checkFailFast(join)) {
            join.setFailed(Status.ERROR);
        }
    }

    private boolean checkFailFast(Join<T> join) {
        return failFast && (join.getChildCount() - join.getCompletedCount()) < required - join.getSucceededCount();
    }

    @Override
    public void onEvent(Join<T> join, Object event) {

    }

    public int getRequired() {
        return required;
    }

    public void setRequired(int required) {
        this.required = required;
    }

    public boolean isFailFast() {
        return failFast;
    }

    public void setFailFast(boolean failFast) {
        this.failFast = failFast;
    }
}