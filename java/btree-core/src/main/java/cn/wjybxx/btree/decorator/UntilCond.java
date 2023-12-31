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
package cn.wjybxx.btree.decorator;

import cn.wjybxx.btree.Task;

/**
 * 循环子节点直到给定的条件达成
 *
 * @author wjybxx
 * date - 2023/12/1
 */
public class UntilCond<T> extends LoopDecorator<T> {

    /** 循环条件 -- 不能直接使用child的guard，意义不同 */
    private Task<T> cond;

    @Override
    protected void onChildCompleted(Task<T> child) {
        if (template_checkGuard(cond)) {
            setSuccess();
        }
    }

    public Task<T> getCond() {
        return cond;
    }

    public UntilCond<T> setCond(Task<T> cond) {
        this.cond = cond;
        return this;
    }

}
