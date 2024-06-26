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

import cn.wjybxx.btree.Decorator;
import cn.wjybxx.btree.Task;
import cn.wjybxx.btree.TaskStatus;

/**
 * 反转装饰器，它用于反转子节点的执行结果。
 * 如果被装饰的任务失败，它将返回成功；
 * 如果被装饰的任务成功，它将返回失败；
 * 如果被装饰的任务取消，它将返回取消。
 *
 * @author wjybxx
 * date - 2023/11/26
 */
public class Inverter<T> extends Decorator<T> {

    public Inverter() {
    }

    public Inverter(Task<T> child) {
        super(child);
    }

    @Override
    protected void execute() {
        template_runChild(child);
    }

    @Override
    protected void onChildCompleted(Task<T> child) {
        switch (child.getNormalizedStatus()) {
            case TaskStatus.SUCCESS -> setFailed(TaskStatus.ERROR);
            case TaskStatus.ERROR -> setSuccess();
            case TaskStatus.CANCELLED -> setCancelled(); // 取消是个奇葩情况
            default -> throw new AssertionError();
        }
    }
}