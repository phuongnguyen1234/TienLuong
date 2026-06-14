<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

const props = defineProps({
  error: Boolean,
  required: Boolean,
  width: [String, Number], // Cho phép tùy chỉnh độ rộng của dropdown (VD: '600px')
  direction: {
    type: String,
    default: 'bottom', // 'bottom' | 'top'
  },
  showAddButton: { type: Boolean, default: false },
  disabled: { type: Boolean, default: false },
  maxHeight: {
    type: [String, Number],
    default: 200,
  },
})

const emit = defineEmits(['open', 'close', 'add'])

const isOpen = ref(false)
const optionsRef = ref(null)
const containerRef = ref(null)
const optionsStyle = ref({})
const calculatedDirection = ref(props.direction)

const toggle = (event) => {
  if (props.disabled) return
  // Nếu click vào nút thêm mới (quick-add-btn), không thực hiện toggle
  // (Sự kiện sẽ bubble lên document để đóng các select khác, nhưng logic đóng select này đã được xử lý ở handleAdd)
  if (event && event.target.closest('.quick-add-btn')) return

  if (isOpen.value) {
    close()
  } else {
    open()
  }
}

const open = () => {
  isOpen.value = true
  updatePosition()
  emit('open')
}

const close = () => {
  isOpen.value = false
  emit('close')
}

const handleClickOutside = (event) => {
  const isClickInsideTrigger = containerRef.value && containerRef.value.contains(event.target)
  // The options are teleported, so we need to check them separately.
  const isClickInsideOptions = optionsRef.value && optionsRef.value.contains(event.target)

  if (!isClickInsideTrigger && !isClickInsideOptions) {
    close()
  }
}

const updatePosition = () => {
  if (!isOpen.value || !containerRef.value) return
  const rect = containerRef.value.getBoundingClientRect()

  // Tính toán hướng hiển thị dựa trên không gian viewport
  const viewportHeight = window.innerHeight
  const threshold = (typeof props.maxHeight === 'number' ? props.maxHeight : 200) + 10 // Đệm 10px
  const spaceBelow = viewportHeight - rect.bottom
  const spaceAbove = rect.top

  if (props.direction === 'bottom' && spaceBelow < threshold && spaceAbove > spaceBelow) {
    calculatedDirection.value = 'top'
  } else if (props.direction === 'top' && spaceAbove < threshold && spaceBelow > spaceAbove) {
    calculatedDirection.value = 'bottom'
  } else {
    calculatedDirection.value = props.direction
  }

  const customWidth = typeof props.width === 'number' ? `${props.width}px` : props.width

  const newStyle = {
    position: 'fixed',
    left: `${rect.left}px`,
    width: customWidth || `${rect.width}px`, // Nếu có width props thì dùng, không thì bằng input
    minWidth: props.width != null ? 'auto' : `${rect.width}px`, // Chỉ giới hạn min-width nếu không có width cố định
    maxHeight: typeof props.maxHeight === 'number' ? `${props.maxHeight}px` : props.maxHeight,
  }

  if (calculatedDirection.value === 'top') {
    // Vị trí bottom của dropdown cách đáy viewport một khoảng bằng vị trí top của input
    newStyle.bottom = `${window.innerHeight - rect.top}px`
    newStyle.top = 'auto'
  } else {
    newStyle.top = `${rect.bottom}px`
    newStyle.bottom = 'auto'
  }
  optionsStyle.value = newStyle
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside, true)
  // Lắng nghe sự kiện scroll và resize để cập nhật lại vị trí dropdown nếu cần
  window.addEventListener('scroll', updatePosition, true)
  window.addEventListener('resize', updatePosition)
})
onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside, true)
  window.removeEventListener('scroll', updatePosition, true)
  window.removeEventListener('resize', updatePosition)
})

const handleAdd = () => {
  if (props.disabled) return
  close() // Đóng dropdown trước khi thực hiện hành động thêm
  emit('add')
}

defineExpose({ open, close, toggle, isOpen })
</script>

<template>
  <div class="custom-select" ref="containerRef">
    <div
      class="select-input-wrapper"
      :class="{ 'is-active': isOpen, 'input--error': error, 'is-disabled': disabled }"
      @click="toggle"
    >
      <slot name="trigger" :isOpen="isOpen" :toggle="toggle" :open="open" :close="close"></slot>

      <!-- Nút thêm mới -->
      <div
        v-if="showAddButton"
        class="quick-add-btn"
        :class="{ 'is-disabled': disabled }"
        @click="handleAdd"
      >
        <svg
          width="16"
          height="16"
          viewBox="0 0 24 24"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M12 5V19M5 12H19"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </div>

      <div class="select-icon" :class="{ 'has-add-btn': showAddButton }">
        <div v-if="$slots.tooltip" class="select-tooltip-wrapper" @click.stop>
          <slot name="tooltip"></slot>
        </div>
        <svg
          width="12"
          height="7"
          viewBox="0 0 14 8"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M1 1L7 7L13 1"
            stroke="currentColor"
            stroke-width="1.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </div>
    </div>

    <Teleport to="body">
      <div
        ref="optionsRef"
        v-if="isOpen"
        class="select-options"
        :class="{ 'opens-up': direction === 'top' }"
        :style="optionsStyle"
      >
        <slot name="options" :close="close"></slot>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.custom-select {
  position: relative;
  width: 100%;
  min-width: 100px;
}
.select-input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  min-height: 32px;
  border: 1px solid var(--border-control-normal);
  border-radius: var(--border-radius-base);
  background-color: var(--bg-white);
  transition: border-color 0.2s;
  box-sizing: border-box;
}
.select-input-wrapper:not(.is-disabled):hover {
  border-color: var(--border-control-hover);
}
.select-input-wrapper.is-active {
  border-color: var(--border-control-hover);
}
.select-input-wrapper.input--error {
  border-color: var(--color-error);
}
.select-input-wrapper.is-disabled {
  background-color: var(--bg-app);
  cursor: not-allowed;
}
.select-icon {
  position: absolute;
  right: 10px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--color-text-secondary);
  cursor: pointer;
  pointer-events: none;
  display: flex;
  align-items: center;
}
.select-icon.has-add-btn {
  right: 46px;
}
.select-tooltip-wrapper {
  margin-right: 6px;
  display: flex;
  align-items: center;
  pointer-events: auto;
}
.select-options {
  left: 0;
  right: 0;
  background: var(--bg-white);
  border: 1px solid var(--border-control-normal);
  border-radius: var(--border-radius-base);
  margin-top: 4px;
  overflow-y: auto;
  z-index: var(--z-index-dropdown);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  font-size: 13px;
  padding: 4px 0;
}
.select-options.opens-up {
  /* Khi mở lên trên, margin ở dưới */
  margin-top: 0;
  margin-bottom: 4px;
}

.quick-add-btn {
  position: absolute;
  right: 0;
  top: 0;
  bottom: 0;
  width: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-left: 1px solid var(--border-control-normal);
  cursor: pointer;
  color: var(--color-text-secondary);
  background-color: var(--bg-white);
  border-top-right-radius: var(--border-radius-base);
  border-bottom-right-radius: var(--border-radius-base);
  z-index: 2;
}
.select-input-wrapper:not(.is-disabled) .quick-add-btn:hover {
  background-color: var(--bg-sidebar-selected);
  color: var(--color-primary);
}
.quick-add-btn.is-disabled {
  pointer-events: none;
  background-color: var(--bg-app);
  color: var(--color-placeholder);
}
</style>
