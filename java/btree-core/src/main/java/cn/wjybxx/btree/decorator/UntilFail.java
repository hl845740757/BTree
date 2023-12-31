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
 * 重复运行子节点，直到该任务失败
 * （超类做了死循环避免）
 *
 * @author wjybxx
 * date - 2023/11/26
 */
public class UntilFail<T> extends LoopDecorator<T> {

    public UntilFail() {
    }

    public UntilFail(Task<T> child) {
        super(child);
    }

    @Override
    protected void onChildCompleted(Task<T> child) {
        if (child.isCancelled()) {
            setCancelled();
            return;
        }
        if (child.isFailed()) {
            setSuccess();
        }
    }
}