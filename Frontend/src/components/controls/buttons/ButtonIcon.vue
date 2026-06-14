<template>
  <Tooltip :text="tooltip ? title : ''" :position="tooltipPosition">
    <button
      class="m-btn-icon"
      :class="[`m-btn-${variant}`, { 'is-active': active }]"
      :style="{
        ...buttonStyle,
      }"
      :disabled="disabled"
      :title="!tooltip ? title : undefined"
      @click="dropdownItems.length > 0 ? toggleDropdown() : $emit('click', $event)"
      ref="buttonRef"
    >
      <!-- Slot mặc định chứa icon (svg, i, component) -->
      <div class="m-btn-icon-inner" :style="iconInnerStyle">
        <slot name="icon-active" v-if="active && $slots['icon-active']">
          <!-- Hiển thị icon active nếu có -->
        </slot>
        <slot name="icon" v-else-if="$slots.icon">
          <!-- Icon mặc định -->
          <component :is="icon" v-if="icon" />
        </slot>
        <slot v-else>
          <component :is="icon" v-if="icon" />
        </slot>
      </div>
    </button>

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
  </Tooltip>
</template>

<script setup>
import { computed, ref, onMounted, onBeforeUnmount } from 'vue'
import Tooltip from '@/components/base/Tooltip.vue'

/**
 * Button Icon Component
 * Chỉ dùng cho các nút chỉ có icon, không có text.
 */
const props = defineProps({
  /**
   * Loại button (màu sắc)
   * Values: 'primary', 'secondary', 'outline', 'danger'
   */
  variant: {
    type: String,
    default: 'outline',
    validator: (value) =>
      ['primary', 'secondary', 'outline', 'danger', 'text', 'danger-outline'].includes(value),
  },
  /**
   * Danh sách menu cho dropdown: [{ label, icon, command }]
   */
  dropdownItems: {
    type: Array,
    default: () => [],
  },
  /**
   * Component icon (nếu không dùng slot)
   */
  icon: {
    type: [Object, String, Function],
    default: null,
  },
  width: {
    type: [String, Number],
    default: 32,
  },
  height: {
    type: [String, Number],
    default: 32,
  },
  minWidth: {
    type: [String, Number],
    default: 32,
  },
  /**
   * Kích thước của icon bên trong
   */
  iconSize: {
    type: [String, Number],
    default: 20,
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
   * Màu tùy chỉnh (Hex, RGB, Name)
   */
  color: {
    type: String,
    default: null,
  },
  /**
   * Padding của nút
   */
  padding: {
    type: [String, Number],
    default: null,
  },
  /**
   * Tiêu đề của nút (dùng cho tooltip)
   */
  title: {
    type: String,
    default: '',
  },
  /**
   * Có hiển thị custom tooltip hay không
   */
  tooltip: {
    type: Boolean,
    default: false,
  },
  /**
   * Vị trí hiển thị của tooltip
   */
  tooltipPosition: {
    type: String,
    default: 'bottom',
  },
})

defineEmits(['click'])

const buttonRef = ref(null)
const isDropdownOpen = ref(false)
const dropdownPosition = ref({ top: 0, left: 0 })

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
      top: rect.bottom + 4,
      left: rect.right - 160,
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
  width: '160px',
}))

const buttonStyle = computed(() => ({
  height: typeof props.height === 'number' ? `${props.height}px` : props.height,
  width: typeof props.width === 'number' ? `${props.width}px` : props.width,
  minWidth: typeof props.minWidth === 'number' ? `${props.minWidth}px` : props.minWidth,
  padding:
    props.padding !== null
      ? typeof props.padding === 'number'
        ? `${props.padding}px`
        : props.padding
      : undefined,
  color:
    (props.variant === 'outline' || props.variant === 'text' || props.variant === 'secondary') &&
    props.color
      ? props.color
      : undefined,
  borderColor: props.color ? props.color : undefined,
  backgroundColor:
    props.variant !== 'outline' &&
    props.variant !== 'text' &&
    props.variant !== 'secondary' &&
    props.color
      ? props.color
      : undefined,
}))

const iconInnerStyle = computed(() => ({
  width: typeof props.iconSize === 'number' ? `${props.iconSize}px` : props.iconSize,
  height: typeof props.iconSize === 'number' ? `${props.iconSize}px` : props.iconSize,
}))
</script>

<style scoped>
.m-btn-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  padding: 6px;
  color: inherit;
  /* Đảm bảo icon không bị bóp méo khi không gian hẹp */
  flex-shrink: 0;
}

/* Dropdown Menu Styling (Shared with Button.vue) */
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

.m-btn-icon-inner {
  display: flex;
  align-items: center;
  justify-content: center;
}

.m-btn-icon-inner :deep(svg) {
  width: 100%;
  height: 100%;
  display: block;
  /* Cho phép các đường stroke ở sát mép viewBox hiển thị đầy đủ, không bị cắt */
  overflow: visible;
}

/* Variants Colors - Đồng bộ với Button.vue */
.m-btn-primary {
  background-color: var(--color-primary);
  color: var(--bg-white);
}

.m-btn-secondary {
  border: 1px solid var(--border-control-normal);
  color: var(--color-text-main);
  background-color: var(--bg-white);
}

.m-btn-secondary .m-btn-divider {
  background-color: #707070;
  opacity: 0.5;
}
.m-btn-secondary .m-btn:hover,
.m-btn-secondary .m-btn-dropdown:hover {
  border-color: var(--button-border);
  color: var(--color-text-main);
  background-color: #f2f2f2; /* Hover nhẹ theo thiết kế Icon */
}
.m-btn-secondary .m-btn:active,
.m-btn-secondary .m-btn-dropdown:active {
  border-color: var(--color-primary-pressed);
  color: var(--color-primary-pressed);
  background-color: #ebebeb; /* Press theo thiết kế Icon */
}
/* Áp dụng trực tiếp cho ButtonIcon */
.m-btn-icon.m-btn-secondary:hover {
  border-color: var(--button-border);
  color: var(--color-text-main);
  background-color: #f2f2f2;
}
.m-btn-icon.m-btn-secondary:active {
  border-color: var(--color-primary-pressed);
  color: var(--color-text-main); /* Đồng nhất màu chữ chính */
  background-color: #ebebeb;
}

.m-btn-outline {
  background-color: #ffffff;
  border: 1px solid var(--border-control-normal);
  color: var(--color-text-secondary);
}
.m-btn-outline:hover {
  background-color: var(--item-hover);
  color: var(--color-text-secondary);
}

.m-btn-danger {
  background-color: #d92d20;
  color: var(--bg-white);
}
.m-btn-danger:hover {
  background-color: #b42318;
}

/* Variant Text - Chỉ icon, không background/border */
.m-btn-text {
  background-color: transparent;
  color: var(--color-text-secondary);
}
.m-btn-text:hover {
  background-color: #f9fafb;
  color: var(--color-primary-hover);
}

/* Variant Danger Outline */
.m-btn-danger-outline {
  background-color: #ffffff;
  border: 1px solid var(--border-control-normal);
  color: #d92d20;
}
.m-btn-danger-outline:hover {
  background-color: #fef3f2;
  border-color: #d92d20;
}

/* Style khi nút active */
.m-btn-outline.is-active {
  background-color: #e6f7ff; /* Màu nền xanh nhạt */
  border-color: var(--color-primary); /* Màu viền chính */
  color: var(--color-primary);
}

.m-btn-icon:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}
</style>
