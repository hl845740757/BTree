/*
 * Copyright 2023 wjybxx(845740757@qq.com)
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

/**
 * Task的特征值，高8位为控制流程相关bit，对外开放
 * (用于减少Task中的方法数量)
 *
 * @author wjybxx
 * date - 2023/12/26
 */
public enum TaskFeature {

    /**
     * 告知模板方法否将{@link Task#enter(int)}和{@link Task#execute()}方法分开执行，
     * 1.默认值由{@link Task#flags}中的信息指定，默认不禁用
     * 2.要覆盖默认值应当在{@link Task#beforeEnter()}方法中调用
     * 3.该属性运行期间不应该调整，调整也无效
     */
    DISABLE_ENTER_EXECUTE(24),

    /**
     * 禁用延迟通知
     * 1.默认值由{@link Task#flags}中的信息指定，默认不禁用（即默认延迟通知）
     * 2.要覆盖默认值应当在{@link Task#beforeEnter()}方法中调用
     * 3.用于解决事件驱动模式下调用栈深度问题 -- 类似尾递归优化，可减少一半栈深度
     * 4.先更新自己为完成状态，再通知父节点，这有时候非常有用 -- 在状态机中，你的State可以感知自己是成功或是失败。
     * 5.该属性运行期间不应该调整
     * <p>
     * 理论基础：99.99% 的情况下，Task在调用 setSuccess 等方法后会立即return，那么在当前方法退出后再通知父节点就不会破坏时序。
     */
    DISABLE_DELAY_NOTIFY(25),

    /**
     * 告知模板方法是否自动检测取消
     * 1.默认值由{@link Task#flags}中的信息指定，默认自动检测
     * 2.自动检测取消信号是一个动态的属性，可随时更改 -- 因此不要轻易缓存。
     */
    DISABLE_AUTO_CHECK_CANCEL(26),

    /**
     * 告知模板方法是否自动监听取消事件
     * 1.默认值由{@link Task#flags}中的信息指定，默认不自动监听！自动监听有较大的开销，绝大多数业务只需要在Entry监听。
     * 2.要覆盖默认值应当在{@link Task#beforeEnter()}方法中调用
     */
    AUTO_LISTEN_CANCEL(27),

    /**
     * 告知模板方法是否在{@link Task#enter(int)}前自动调用{@link Task#resetChildrenForRestart()}
     * 1.默认值由{@link Task#flags}中的信息指定，默认不启用
     * 2.要覆盖默认值应当在{@link Task#beforeEnter()}方法中调用
     * 3.部分任务可能在调用{@link Task#resetForRestart()}之前不会再次运行，因此需要该特性
     */
    AUTO_RESET_CHILDREN(28),
    ;

    public final int mask;
    final int notMask;

    TaskFeature(int offset) {
        mask = 1 << offset;
        notMask = ~mask;
    }

    public final boolean isEnable(int ctl) {
        return (ctl & mask) != 0;
    }

    public final int setEnable(int ctl, boolean enable) {
        return enable ? (ctl & mask) : (ctl & notMask);
    }
}