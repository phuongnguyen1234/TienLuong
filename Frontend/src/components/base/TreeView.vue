<template>
  <div class="m-tree-view">
    <div v-for="node in data" :key="node[nodeKey]" class="m-tree-node-wrapper">
      <div
        class="m-tree-node"
        :class="{ 'is-selected': isSelected(node) }"
        :style="{ paddingLeft: `${8 + level * indent}px` }"
        @click="toggleExpand(node)"
      >
        <!-- Icon Expand/Collapse (Chỉ hiển thị ở node có con) -->
        <div class="m-tree-node-icon">
          <svg
            v-if="hasChildren(node)"
            :class="{ 'is-expanded': isExpanded(node) }"
            width="16"
            height="16"
            viewBox="0 0 16 16"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M6 12L10 8L6 4"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </div>

        <!-- Checkbox chọn node -->
        <div class="m-tree-node-checkbox" @click.stop>
          <Checkbox
            :model-value="isSelected(node)"
            @update:model-value="(val) => handleSelect(node, val)"
          />
        </div>

        <!-- Nhãn của node -->
        <div class="m-tree-node-label">
          {{ node[labelKey] }}
        </div>
      </div>

      <!-- Hiển thị các node con nếu được mở rộng -->
      <div v-if="hasChildren(node) && isExpanded(node)" class="m-tree-node-children">
        <!-- Đệ quy TreeView -->
        <TreeView
          :data="node[childrenKey]"
          :model-value="modelValue"
          :node-key="nodeKey"
          :label-key="labelKey"
          :children-key="childrenKey"
          :level="level + 1"
          :indent="indent"
          @update:model-value="emitUpdate"
        />
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import Checkbox from '@/components/controls/checkboxes/Checkbox.vue'

/**
 * TreeView Component - Hiển thị cấu trúc cây với Checkbox và khả năng đệ quy.
 * @author Gemini Code Assist
 */

const props = defineProps({
  // Mảng dữ liệu cây
  data: {
    type: Array,
    default: () => [],
  },
  // Mảng các key (ID) đã được chọn
  modelValue: {
    type: Array,
    default: () => [],
  },
  // Tên trường làm Key định danh
  nodeKey: {
    type: String,
    default: 'id',
  },
  // Tên trường hiển thị text
  labelKey: {
    type: String,
    default: 'name',
  },
  // Tên trường chứa mảng con
  childrenKey: {
    type: String,
    default: 'children',
  },
  // Cấp độ hiện tại (dùng nội bộ cho đệ quy)
  level: {
    type: Number,
    default: 0,
  },
  // Độ thụt lề mỗi cấp (px)
  indent: {
    type: Number,
    default: 20,
  },
})

const emit = defineEmits(['update:modelValue'])

// Quản lý các node đang được mở rộng
const expandedKeys = ref(new Set())

const hasChildren = (node) => node[props.childrenKey]?.length > 0
const isExpanded = (node) => expandedKeys.value.has(node[props.nodeKey])
const isSelected = (node) => props.modelValue.includes(node[props.nodeKey])

function toggleExpand(node) {
  if (!hasChildren(node)) return
  const key = node[props.nodeKey]
  if (expandedKeys.value.has(key)) {
    expandedKeys.value.delete(key)
  } else {
    expandedKeys.value.add(key)
  }
}

/**
 * Lấy toàn bộ ID trong một nhánh cây (node hiện tại và các con cháu)
 */
function getAllKeysInBranch(node, keys = []) {
  keys.push(node[props.nodeKey])
  if (hasChildren(node)) {
    node[props.childrenKey].forEach((child) => getAllKeysInBranch(child, keys))
  }
  return keys
}

/**
 * Hàm xử lý tập trung việc phát hành cập nhật và tính toán Cascade Up
 */
function emitUpdate(newSelection) {
  let updatedSelection = [...newSelection]

  // Logic Cascade Up: Kiểm tra các node ở cấp độ hiện tại
  // Nếu tất cả các con của một node cha ở cấp này đều đã được chọn -> Tự động chọn cha
  props.data.forEach((node) => {
    if (hasChildren(node)) {
      const parentId = node[props.nodeKey]
      const children = node[props.childrenKey]
      const allChildrenSelected = children.every((child) =>
        updatedSelection.includes(child[props.nodeKey]),
      )

      if (allChildrenSelected) {
        if (!updatedSelection.includes(parentId)) updatedSelection.push(parentId)
      } else {
        // Nếu không còn đủ con, bỏ chọn cha
        updatedSelection = updatedSelection.filter((id) => id !== parentId)
      }
    }
  })

  emit('update:modelValue', updatedSelection)
}

function handleSelect(node, checked) {
  let currentSelection = [...props.modelValue]
  const branchKeys = getAllKeysInBranch(node)

  if (checked) {
    branchKeys.forEach((key) => {
      if (!currentSelection.includes(key)) currentSelection.push(key)
    })
  } else {
    currentSelection = currentSelection.filter((key) => !branchKeys.includes(key))
  }

  emitUpdate(currentSelection)
}
</script>

<style scoped>
.m-tree-view {
  width: 100%;
  user-select: none;
}
.m-tree-node {
  display: flex;
  align-items: center;
  height: 32px;
  cursor: pointer;
  transition: background-color 0.2s;
  padding: 8px 12px 8px 0px; /* Loại bỏ padding-left cố định, sẽ được xử lý bằng inline style */
}
.m-tree-node.is-selected {
  background-color: #edfcf4;
}
.m-tree-node.is-selected .m-tree-node-label {
  color: var(--color-primary);
}
.m-tree-node:hover {
  background-color: var(--item-hover);
}
.m-tree-node-icon {
  width: 24px;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #666;
}
.m-tree-node-icon svg {
  transition: transform 0.2s;
}
.m-tree-node-icon svg.is-expanded {
  transform: rotate(90deg);
}
.m-tree-node-checkbox {
  margin-right: 8px;
}
.m-tree-node-label {
  font-size: 13px;
  color: var(--color-text-main);
}
</style>
