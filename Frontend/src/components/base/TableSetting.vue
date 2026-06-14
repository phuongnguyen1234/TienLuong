<template>
  <BaseModal
    :visible="visible"
    width="415px"
    position="fixed"
    :has-overlay="false"
    :show-close="false"
    padding="16px"
    :top="top"
    :right="right"
    :bottom="bottom"
    @update:visible="close"
  >
    <template #title>Tùy chỉnh cột</template>

    <template #header-actions>
      <ButtonIcon
        variant="text"
        width="36px"
        height="36px"
        title="Lấy lại mặc định"
        class="refresh-btn"
        @click="resetToDefault"
      >
        <svg width="20" height="20" viewBox="260 60 20 20" fill="none">
          <g id="Icon Placeholder_74">
            <path
              id="Vector_68"
              d="m 276.667,69.1667 c -0.204,-1.4665 -0.884,-2.8253 -1.936,-3.8671 -1.053,-1.0418 -2.418,-1.7088 -3.886,-1.8983 -1.469,-0.1895 -2.959,0.1091 -4.241,0.8497 -1.282,0.7407 -2.285,1.8823 -2.854,3.249 m -0.417,-3.3333 V 67.5 h 3.334 m -3.334,3.3333 c 0.204,1.4665 0.884,2.8253 1.936,3.8671 1.052,1.0418 2.418,1.7088 3.886,1.8983 1.469,0.1895 2.959,-0.1091 4.241,-0.8497 1.282,-0.7407 2.285,-1.8823 2.854,-3.249 m 0.417,3.3333 V 72.5 h -3.334"
              stroke="#717680"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </g>
        </svg>
      </ButtonIcon>
    </template>

    <div class="table-setting-container">
      <Searchbar v-model="searchQuery" placeholder="Tìm kiếm" class="search-bar" />
      <div class="column-list-wrapper">
        <draggable
          v-model="filteredColumns"
          tag="div"
          class="column-list"
          item-key="key"
          handle=".drag-handle"
          ghost-class="sortable-ghost"
        >
          <template #item="{ element: column }">
            <div class="column-item">
              <div class="column-item-left">
                <Checkbox
                  :model-value="column.visible"
                  :disabled="column.hideable === false"
                  @update:model-value="(val) => handleVisibilityChange(column, val)"
                />
                <span class="column-label">{{ column.label }}</span>
              </div>
              <GripIcon class="drag-handle" />
            </div>
          </template>
        </draggable>
      </div>
    </div>

    <template #footer>
      <div class="footer-actions">
        <Button variant="primary" width="80" @click="save">Lưu</Button>
      </div>
    </template>
  </BaseModal>
</template>

<script setup>
import { ref, watch, computed, onMounted } from 'vue'
import draggable from 'vuedraggable'
import BaseModal from '@/components/base/BaseModal.vue'
import Searchbar from '@/components/controls/inputs/Searchbar.vue'
import Checkbox from '@/components/controls/checkboxes/Checkbox.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonIcon from '@/components/controls/buttons/ButtonIcon.vue'
import GripIcon from '@/components/icons/GripIcon.vue'

const props = defineProps({
  visible: Boolean,
  columns: {
    type: Array,
    default: () => [],
  },
  top: String,
  right: String,
  bottom: String,
})

const emit = defineEmits(['update:visible', 'save', 'reset'])

const searchQuery = ref('')
const localColumns = ref([])

// Hàm khởi tạo dữ liệu từ props
function initData() {
  if (props.columns && props.columns.length > 0) {
    localColumns.value = JSON.parse(JSON.stringify(props.columns)).map((col) => ({
      ...col,
    }))
  }
}

// Watch thêm props.columns phòng trường hợp dữ liệu thay đổi từ cha khi modal đang mở
watch(() => props.columns, initData, { deep: true, immediate: true })

const filteredColumns = computed({
  get() {
    const baseList = localColumns.value.filter((col) => col.label)
    const query = searchQuery.value.toLowerCase()
    if (!query) {
      return baseList
    }
    return baseList.filter((col) => (col.label || '').toLowerCase().includes(query))
  },
  set(reorderedList) {
    // Nếu không có filter, danh sách được cập nhật là danh sách đầy đủ đã sắp xếp lại.
    if (!searchQuery.value) {
      localColumns.value = reorderedList
      return
    }

    // Nếu có filter, thực hiện việc hợp nhất phức tạp để duy trì thứ tự.
    const reorderedKeys = new Set(reorderedList.map((i) => i.key))
    const reorderedIterator = reorderedList[Symbol.iterator]()

    const newFullList = localColumns.value.map((originalItem) => {
      if (reorderedKeys.has(originalItem.key)) {
        // Vị trí này trong danh sách đầy đủ được chiếm bởi một mục đã lọc.
        // Thay thế nó bằng mục tiếp theo từ danh sách đã được sắp xếp lại.
        return reorderedIterator.next().value
      } else {
        // Mục này không thuộc bộ lọc, vì vậy nó giữ nguyên vị trí tương đối của mình.
        return originalItem
      }
    })

    localColumns.value = newFullList
  },
})

const currentlyVisibleInUI = computed(() => {
  // Bây giờ chỉ cần trả về danh sách đã lọc
  return filteredColumns.value
})

// Logic cho checkbox ở header để chọn/bỏ chọn tất cả
const areAllVisible = computed({
  get() {
    const list = currentlyVisibleInUI.value
    // Chỉ đúng khi danh sách hiển thị có item và tất cả item đó đều được check
    return list.length > 0 && list.every((c) => c.visible)
  },
  set(value) {
    // Chỉ thay đổi trạng thái của các item đang hiển thị trong UI
    currentlyVisibleInUI.value.forEach((c) => {
      if (c.hideable !== false) {
        c.visible = value
      }
    })
  },
})

function handleVisibilityChange(column, isVisible) {
  column.visible = isVisible
}

function close() {
  emit('update:visible', false)
}

function save() {
  emit('save', localColumns.value)
  close()
}

function resetToDefault() {
  emit('reset')
}
</script>

<style scoped>
.table-setting-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}
.refresh-btn {
  border-radius: 50% !important;
  color: var(--color-text-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
}
.refresh-btn:hover {
  background-color: #0000001a !important;
}

.table-setting-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.column-list-wrapper {
  overflow-y: auto;
  margin: 0 -8px;
  padding: 0 8px;
}

.column-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 36px;
  padding: 0 8px;
  border-radius: 6px;
  transition: background-color 0.2s;
  cursor: pointer;
}

.column-item:hover {
  background-color: #eafbf2;
}

.column-item-left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.column-label {
  font-size: 13px;
  color: var(--color-text-main);
}

.drag-handle {
  opacity: 0;
  cursor: move;
  width: 14px;
  height: 14px;
  color: var(--color-text-secondary);
  transition: opacity 0.2s;
}

.column-item:hover .drag-handle {
  opacity: 1;
}

.footer-actions {
  display: flex;
  justify-content: flex-end;
  width: 100%;
}
.sortable-ghost {
  opacity: 0.5;
  background: var(--color-primary-light);
}
</style>
