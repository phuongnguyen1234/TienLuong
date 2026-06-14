<script setup>
import { ref, computed } from 'vue'
import BaseSelect from './BaseSelect.vue'
import TreeView from '@/components/base/TreeView.vue'
import CloseIcon from '@/components/icons/CloseIcon.vue'

/**
 * SelectTree Component - Dropdown chọn nhiều theo cấu trúc cây.
 * @author Gemini Code Assist
 */

const props = defineProps({
  width: {
    type: [String, Number],
    default: null,
  },
  maxTags: {
    type: [Number, String],
    default: null,
  },
  modelValue: {
    type: Array,
    default: () => [],
  },
  options: {
    type: Array,
    default: () => [],
  },
  placeholder: {
    type: String,
    default: 'Chọn đơn vị',
  },
  nodeKey: {
    type: String,
    default: 'id',
  },
  labelKey: {
    type: String,
    default: 'name',
  },
  childrenKey: {
    type: String,
    default: 'children',
  },
  error: Boolean,
  dropdownWidth: {
    type: String,
    default: '400px',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  direction: {
    type: String,
    default: 'bottom',
  },
})

const emit = defineEmits(['update:modelValue', 'change'])
const baseSelectRef = ref(null)

const componentStyle = computed(() => {
  const style = {}
  if (props.width) {
    style.width = typeof props.width === 'number' ? `${props.width}px` : props.width
  }
  style.maxWidth = '840px'
  return style
})

/**
 * Thu gọn danh sách hiển thị: Nếu một node cha được chọn hoàn toàn, chỉ hiển thị cha đó thay vì liệt kê con.
 */
const simplifiedSelection = computed(() => {
  const result = []
  const selectedSet = new Set(props.modelValue)

  function process(nodes, isParentSelected = false) {
    for (const node of nodes) {
      const id = node[props.nodeKey]
      const children = node[props.childrenKey] || []
      const isCurrentSelected = selectedSet.has(id)

      // Nếu node hiện tại được chọn và cha của nó chưa được chọn -> Đây là điểm bắt đầu của nhánh
      if (isCurrentSelected && !isParentSelected) {
        result.push({ id, label: node[props.labelKey], node })
        // Tiếp tục duyệt con nhưng đánh dấu là cha đã được chọn để không thêm con vào list tag
        process(children, true)
      } else {
        // Nếu node hiện tại không chọn, hoặc cha nó đã chọn rồi, chỉ duyệt tiếp xuống dưới
        process(children, isParentSelected || isCurrentSelected)
      }
    }
  }

  process(props.options)
  return result
})

/**
 * Trả về danh sách ID tối giản (chỉ lấy ID các node cha cao nhất được chọn)
 */
function getSimplifiedIds() {
  return simplifiedSelection.value.map((item) => item.id)
}

defineExpose({ getSimplifiedIds })

function removeTag(tag) {
  if (props.disabled) return
  let newValue = [...props.modelValue]
  const getAllIds = (node, ids = []) => {
    ids.push(node[props.nodeKey])
    if (node[props.childrenKey]) {
      node[props.childrenKey].forEach((child) => getAllIds(child, ids))
    }
    return ids
  }
  const idsToRemove = getAllIds(tag.node)
  newValue = newValue.filter((id) => !idsToRemove.includes(id))
  emit('update:modelValue', newValue)
  emit('change', newValue)
}

/**
 * Bỏ chọn tất cả các node
 */
function clearAll() {
  if (props.disabled) return
  emit('update:modelValue', [])
  emit('change', [])
}
</script>

<template>
  <BaseSelect
    ref="baseSelectRef"
    :style="componentStyle"
    :error="error"
    :width="dropdownWidth"
    :max-height="400"
    :direction="direction"
    :disabled="disabled"
  >
    <template #trigger="{ isOpen }">
      <div
        class="m-select-tree-trigger"
        :class="{ 'is-active': isOpen, 'is-disabled': disabled, 'input--error': error }"
      >
        <div class="m-select-tree-content">
          <template v-if="simplifiedSelection.length > 0">
            <!-- Nếu có giới hạn tag và số lượng thực tế vượt quá -->
            <template v-if="maxTags !== null && simplifiedSelection.length > Number(maxTags)">
              <div class="m-tag-counter-wrapper">
                <svg
                  width="8"
                  height="24"
                  viewBox="0 0 8 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M8 0C4.68629 0 2 2.68629 2 6V18C2 21.3137 4.68629 24 8 24H6C2.68629 24 0 21.3137 0 18V6C0 2.68629 2.68629 0 6 0H8Z"
                    fill="currentColor"
                  />
                </svg>
                <svg
                  width="8"
                  height="24"
                  viewBox="0 0 8 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                  class="m-svg-overlap"
                >
                  <path
                    d="M8 0C4.68629 0 2 2.68629 2 6V18C2 21.3137 4.68629 24 8 24H6C2.68629 24 0 21.3137 0 18V6C0 2.68629 2.68629 0 6 0H8Z"
                    fill="currentColor"
                  />
                </svg>
                <div class="m-tag m-tag--count">
                  {{ simplifiedSelection.length - Number(maxTags) }}
                </div>
              </div>
              <div
                v-for="tag in simplifiedSelection.slice(0, Number(maxTags))"
                :key="tag.id"
                class="m-tag"
              >
                <span class="m-tag-text">{{ tag.label }}</span>
                <span v-if="!disabled" class="m-tag-remove" @click.stop="removeTag(tag)">
                  <CloseIcon />
                </span>
              </div>
            </template>
            <!-- Nếu hiển thị tất cả hoặc trong giới hạn -->
            <template v-else>
              <div v-for="tag in simplifiedSelection" :key="tag.id" class="m-tag">
                <span class="m-tag-text">{{ tag.label }}</span>
                <span v-if="!disabled" class="m-tag-remove" @click.stop="removeTag(tag)">
                  <CloseIcon />
                </span>
              </div>
            </template>
          </template>
          <span v-else class="m-placeholder">{{ placeholder }}</span>
        </div>
        <!-- Nút xóa sạch tất cả lựa chọn -->
        <div
          v-if="modelValue.length > 0 && !disabled"
          class="m-select-clear"
          @click.stop="clearAll"
        >
          <CloseIcon />
        </div>
      </div>
    </template>

    <template #options>
      <div class="m-tree-container">
        <TreeView
          :data="options"
          :model-value="modelValue"
          :node-key="nodeKey"
          :label-key="labelKey"
          :children-key="childrenKey"
          @update:model-value="$emit('update:modelValue', $event)"
          :disabled="disabled"
        />
      </div>
    </template>
  </BaseSelect>
</template>

<style scoped>
.m-select-tree-trigger {
  display: flex;
  align-items: center;
  gap: 4px;
  padding: 0 36px 0 12px;
  min-height: 32px;
  width: 100%;
  cursor: pointer;
  overflow: hidden;
  position: relative;
}
.m-select-tree-content {
  display: flex;
  align-items: center;
  gap: 4px;
  flex: 1;
  overflow: hidden;
  height: 100%;
}
.m-select-tree-trigger.is-disabled {
  background-color: var(--bg-disabled);
  cursor: not-allowed;
  opacity: 0.7;
}
.m-placeholder {
  display: flex;
  align-items: center;
  gap: 4px;
  flex: 1;
  overflow: hidden;
  height: 100%;
}
.m-placeholder {
  color: var(--color-placeholder);
  font-size: 13px;
}
.m-tag {
  display: flex;
  align-items: center;
  background-color: #f0f0f0;
  border: 1px solid #d9d9d9;
  border-radius: 6px;
  padding: 0 8px;
  height: 24px;
  font-size: 12px;
  max-width: calc(100% - 32px); /* Đảm bảo tag không chèn lên nút clear */
  flex-shrink: 0;
  min-width: 24px;
  font-weight: 500;
}
.m-tag-text {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.m-tag-remove {
  margin-left: 4px;
  cursor: pointer;
  color: var(--color-text-secondary);
  width: 18px;
  height: 18px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s;
}
.m-tag-remove:hover {
  background-color: #e0e0e0;
}
.m-tag-counter-wrapper {
  display: flex;
  align-items: center;
  color: var(--item-hover); /* Màu icon ngoặc */
}
.m-svg-overlap {
  margin-left: -5px; /* Đẩy icon thứ 2 chèn lên icon thứ nhất */
}
.m-tag--count {
  font-weight: 500;
  color: #212121; /* Đảm bảo số lượng hiển thị rõ ràng */
  margin-left: -4px; /* Tag overlap sang trái chèn lên icon ngoặc */
}
.m-tag-remove :deep(svg) {
  width: 12px;
  height: 12px;
}
.m-select-clear {
  position: absolute;
  right: 32px;
  top: 50%;
  transform: translateY(-50%);
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #9e9e9e;
  border-radius: 50%;
}
.m-tree-container {
  padding: 4px 0;
  height: 350px;
  min-height: 350px;
  overflow-y: auto;
}
</style>
