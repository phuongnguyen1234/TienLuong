<template>
  <div
    class="m-btn-wrapper"
    :class="[`m-btn-${variant}`, { 'm-btn-split': split, 'm-btn--disabled': disabled }]"
    :style="containerStyle"
    ref="buttonRef"
  >
    <button
      class="m-btn"
      :style="mainButtonStyle"
      :disabled="disabled"
      @click="
        split
          ? $emit('click', $event)
          : dropdownItems.length > 0
            ? toggleDropdown()
            : $emit('click', $event)
      "
    >
      <!-- Icon bên trái (nếu có) -->
      <div v-if="icon || $slots.icon || $slots['icon-active']" class="m-btn-icon">
        <slot v-if="active && $slots['icon-active']" name="icon-active"></slot>
        <slot v-else name="icon">
          <component :is="icon" v-if="icon" />
        </slot>
      </div>

      <!-- Text (nội dung button) -->
      <div v-if="hasDefaultSlotContent" class="m-btn__text">
        <slot></slot>
      </div>
    </button>

    <template v-if="split">
      <div class="m-btn-divider"></div>
      <button class="m-btn-dropdown" :disabled="disabled" @click.stop="toggleDropdown">
        <svg
          width="20"
          height="20"
          viewBox="0 0 20 20"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M5 7.5L10 12.5L15 7.5"
            stroke="currentColor"
            stroke-width="1.6"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </button>
    </template>

    <!-- Dropdown Menu -->
    <Teleport to="body">
      <div v-if="isDropdownOpen" class="m-dropdown-menu" :style="dropdownStyle" @click.stop>
        <div
          v-for="(item, index) in dropdownItems"
          :key="index"
          class="m-dropdown-item"
          @click="handleItemClick(item)"
        >
          <component :is="item.icon" v-if="item.icon" class="m-dropdown-item-icon" />
          <span class="m-dropdown-item-text">{{ item.label }}</span>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<script setup>
import { computed, useSlots, Comment, Text, ref, onMounted, onBeforeUnmount } from 'vue'

const props = defineProps({
  /**
   * Loại button (màu sắc)
   * Values: 'primary' (xanh), 'secondary' (xám nhạt), 'outline' (trắng viền), 'danger' (đỏ)
   */
  variant: {
    type: String,
    default: 'primary',
    validator: (value) =>
      ['primary', 'secondary', 'outline', 'danger', 'text', 'danger-outline'].includes(value),
  },
  /**
   * Có phải nút split (có dropdown bên phải) không
   */
  split: {
    type: Boolean,
    default: false,
  },
  /**
   * Danh sách menu cho dropdown: [{ label, icon, command }]
   */
  dropdownItems: {
    type: Array,
    default: () => [],
  },
  /**
   * Component icon (import từ file .vue khác)
   */
  icon: {
    type: [Object, String, Function],
    default: null,
  },
  width: {
    type: [String, Number],
    default: null,
  },
  height: {
    type: [String, Number],
    default: 36,
  },
  minWidth: {
    type: [String, Number],
    default: 80,
  },
  textSize: {
    type: [String, Number],
    default: 13,
  },
  padding: {
    type: String,
    default: null,
  },
  /**
   * Màu tùy chỉnh (Hex, RGB, Name)
   * - Với variant outline: Áp dụng cho text và border
   * - Với variant text: Áp dụng cho text
   * - Với variant khác: Áp dụng cho background
   */
  color: {
    type: String,
    default: null,
  },
  /**
   * Trạng thái active/toggled của nút
   */
  active: {
    type: Boolean,
    default: false,
  },
  /**
   * Trạng thái disabled của nút
   */
  disabled: {
    type: Boolean,
    default: false,
  },
  /**
   * Màu nền khi hover
   */
  hoverBgColor: {
    type: String,
    default: null,
  },
})

defineEmits(['click'])

const slots = useSlots()

const hasDefaultSlotContent = computed(() => {
  if (!slots.default) return false
  // Lọc ra các node không phải là comment hoặc text rỗng
  return slots.default().some((node) => {
    if (node.type === Comment) return false // Bỏ qua comment nodes
    if (node.type === Text && !node.children.trim()) return false // Bỏ qua text nodes chỉ có khoảng trắng
    return true // Giữ lại các element nodes hoặc text nodes có nội dung
  })
})
const buttonRef = ref(null)
const isDropdownOpen = ref(false)
const dropdownPosition = ref({ top: 0, left: 0, width: 0 })

const toggleDropdown = () => {
  if (props.disabled) return
  isDropdownOpen.value = !isDropdownOpen.value
  if (isDropdownOpen.value) {
    updatePosition()
  }
}

const updatePosition = () => {
  if (buttonRef.value) {
    const rect = buttonRef.value.getBoundingClientRect()
    dropdownPosition.value = {
      top: rect.bottom + 4, // 4px khoảng cách từ button
      // Tính toán left để dropdown cách cạnh phải của button 16px
      left: rect.right - 230,
      width: 230,
    }
  }
}

const handleItemClick = (item) => {
  if (item.command) item.command()
  isDropdownOpen.value = false
}

const handleClickOutside = (event) => {
  if (buttonRef.value && !buttonRef.value.contains(event.target)) {
    isDropdownOpen.value = false
  }
}

onMounted(() => {
  window.addEventListener('click', handleClickOutside)
  window.addEventListener('scroll', updatePosition, true)
})

onBeforeUnmount(() => {
  window.removeEventListener('click', handleClickOutside)
  window.removeEventListener('scroll', updatePosition, true)
})

const dropdownStyle = computed(() => ({
  top: `${dropdownPosition.value.top}px`,
  left: `${dropdownPosition.value.left}px`,
  width: `${dropdownPosition.value.width}px`, // Đặt width cố định
}))
const containerStyle = computed(() => ({
  width: typeof props.width === 'number' ? `${props.width}px` : props.width,
  height: typeof props.height === 'number' ? `${props.height}px` : props.height,
  minWidth: typeof props.minWidth === 'number' ? `${props.minWidth}px` : props.minWidth,
  borderColor: props.color ? props.color : undefined,
  '--m-btn-hover-bg': props.hoverBgColor,
  '--m-btn-hover-border': props.color ? props.color : 'var(--color-primary)',
  '--m-btn-hover-text': props.color ? props.color : 'var(--color-primary)',
}))

const buttonBackgroundStyle = computed(() => ({
  backgroundColor:
    props.variant !== 'outline' &&
    props.variant !== 'text' &&
    props.variant !== 'secondary' &&
    props.color
      ? props.color
      : undefined,
}))

const mainButtonStyle = computed(() => ({
  fontSize: typeof props.textSize === 'number' ? `${props.textSize}px` : props.textSize,
  color:
    (props.variant === 'outline' || props.variant === 'text' || props.variant === 'secondary') &&
    props.color
      ? props.color
      : undefined,
  padding: props.padding ?? (hasDefaultSlotContent.value ? '0 12px' : '8px'),
  ...buttonBackgroundStyle.value,
}))
</script>

<style scoped>
.m-btn-wrapper {
  display: inline-flex;
  align-items: stretch;
  border-radius: var(--border-radius-base);
  overflow: hidden;
  box-sizing: border-box;
  transition: all 0.2s ease;
  vertical-align: middle;
}

.m-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: transparent;
  cursor: pointer;
  font-weight: 500;
  color: inherit;
  gap: 8px;
  white-space: nowrap;
  transition: background-color 0.2s ease;
}

.m-btn-dropdown {
  width: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: transparent;
  cursor: pointer;
  color: inherit;
  padding: 0;
  transition: background-color 0.2s ease;
}

.m-btn-divider {
  width: 1px;
  background-color: #fff; /* Default for primary */
  height: 20px; /* Chiều cao cố định của divider */
  align-self: center; /* Căn giữa theo chiều dọc, không bị stretch */
}

/* Icon styling */
.m-btn-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 16px;
  height: 16px;
  color: inherit;
}

/* Dropdown Menu Styling */
.m-dropdown-menu {
  position: fixed;
  background-color: var(--bg-white);
  border: 1px solid var(--border-control-normal);
  border-radius: var(--border-radius-base);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: var(--z-index-dropdown);
  padding: 8px 0;
  display: flex;
  flex-direction: column;
}

.m-dropdown-item {
  padding: 8px 12px;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 13px;
  color: var(--color-text-main);
  transition: background-color 0.2s ease;
}

.m-dropdown-item-icon {
  /* Thiết lập màu mặc định cho icon trong dropdown là màu icon của hệ thống */
  color: var(--color-icon);
}

.m-dropdown-item-icon :deep(svg) {
  width: 100%;
  height: 100%;
  display: block;
  /* Cho phép hiển thị đầy đủ stroke, không bị cắt 4 cạnh */
  overflow: visible;
}

.m-dropdown-item:hover {
  background-color: #f2f2f2;
  color: var(--color-text-primary);
}

.m-dropdown-item-icon {
  width: 16px;
  height: 16px;
  flex-shrink: 0;
}

.m-btn-icon :deep(svg) {
  width: 100%;
  height: 100%;
  display: block;
  overflow: visible;
}

/* Inner text wrapper */
.m-btn__text {
  color: inherit;
}

/* Variants Colors */
.m-btn-primary {
  background-color: var(--color-primary);
  color: var(--bg-white);
}
.m-btn-primary .m-btn,
.m-btn-primary .m-btn-dropdown {
  background-color: transparent;
}

.m-btn-primary .m-btn:hover,
.m-btn-primary .m-btn-dropdown:hover {
  background-color: var(--color-primary-hover);
}

.m-btn-primary .m-btn:active,
.m-btn-primary .m-btn-dropdown:active {
  background-color: var(--color-primary-pressed);
}

/* Secondary: chỉ có outline (theo ComponentRedesign.md) */
.m-btn-secondary {
  border: 1px solid var(--border-control-normal);
  color: var(--color-text-main);
  background-color: var(--bg-white);
}
.m-btn-secondary .m-btn,
.m-btn-secondary .m-btn-dropdown {
  background-color: transparent;
}
.m-btn-secondary .m-btn-divider {
  background-color: #707070;
  opacity: 0.5;
}
/* Fix: Đổi border-color của wrapper khi hover/active */
.m-btn-wrapper.m-btn-secondary:hover {
  border-color: var(--button-border);
}
.m-btn-wrapper.m-btn-secondary:active {
  border-color: var(--color-primary-pressed);
}
.m-btn-secondary .m-btn:hover,
.m-btn-secondary .m-btn-dropdown:hover {
  color: var(--color-text-main);
  background-color: #f2f2f2; /* Hover nhẹ theo thiết kế Icon */
}
.m-btn-secondary .m-btn:active,
.m-btn-secondary .m-btn-dropdown:active {
  color: var(--color-text-main); /* Giữ màu chữ chính theo yêu cầu */
  background-color: #ebebeb; /* Press theo thiết kế Icon */
}

.m-btn-outline {
  background-color: #ffffff;
  border: 1px solid var(--border-control-normal);
  color: var(--color-text-secondary);
}
.m-btn-outline:hover {
  background-color: var(--m-btn-hover-bg, #f9fafb);
  border-color: var(--m-btn-hover-border);
  color: var(--m-btn-hover-text);
}

.m-btn-danger {
  color: var(--bg-white);
}
.m-btn-danger .m-btn,
.m-btn-danger .m-btn-dropdown {
  background-color: var(--color-error);
}
.m-btn-danger .m-btn:hover,
.m-btn-danger .m-btn-dropdown:hover {
  filter: brightness(0.9);
}

/* Variant Text */
.m-btn-text {
  background-color: transparent;
  color: var(--color-primary);
  font-weight: 400;
}
.m-btn-text:hover {
  background-color: #f9fafb;
  color: var(--color-primary);
}

.m-btn-danger-outline {
  background-color: var(--bg-white);
  border: 1px solid var(--border-control-normal);
  color: var(--color-error);
}
.m-btn-danger-outline:hover {
  background-color: #fef3f2;
  border-color: var(--color-error);
  color: var(--color-error);
}

.m-btn--disabled {
  opacity: 0.6;
  cursor: not-allowed;
  pointer-events: none;
}
</style>
