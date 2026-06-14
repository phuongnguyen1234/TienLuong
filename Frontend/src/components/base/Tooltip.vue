<template>
  <div
    ref="containerRef"
    class="tooltip-container"
    @mouseenter="showTooltip"
    @mouseleave="hideTooltip"
  >
    <!-- Phần tử trigger (phần tử được hover) -->
    <slot></slot>

    <Teleport to="body">
      <!-- Bong bóng Tooltip -->
      <Transition name="fade">
        <div
          v-if="visible && (text || $slots.content)"
          ref="bubbleRef"
          class="tooltip-bubble"
          :class="[`tooltip--${calculatedPosition}`]"
          :style="bubbleStyle"
        >
          <div class="tooltip-content">
            <slot name="content">{{ text }}</slot>
          </div>
          <div class="tooltip-arrow"></div>
        </div>
      </Transition>
    </Teleport>
  </div>
</template>

<script setup>
import { ref, nextTick, computed, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  /**
   * Nội dung chữ của tooltip
   */
  text: {
    type: String,
    default: '',
  },
  /**
   * Vị trí hiển thị: 'top', 'right', 'bottom', 'left'
   */
  position: {
    type: String,
    default: 'bottom',
    validator: (value) => ['top', 'right', 'bottom', 'left'].includes(value),
  },
  /**
   * Thời gian delay trước khi hiện (ms)
   */
  delay: {
    type: Number,
    default: 200,
  },
  /**
   * Kích thước chữ của nội dung tooltip
   */
  fontSize: {
    type: String,
    default: '13px',
  },
})

const visible = ref(false)
const containerRef = ref(null)
const bubbleRef = ref(null) // New ref for the teleported bubble
const calculatedPosition = ref(props.position)
const nudgeX = ref(0)
const bubbleTop = ref(0)
const bubbleLeft = ref(0)
let timeout = null

const bubbleStyle = computed(() => {
  const style = {
    top: `${bubbleTop.value}px`,
    left: `${bubbleLeft.value}px`,
    fontSize: props.fontSize,
    '--nudge-x': `${nudgeX.value}px`,
  }

  // Apply transforms for centering based on calculatedPosition
  if (calculatedPosition.value === 'top' || calculatedPosition.value === 'bottom') {
    style.transform = `translateX(calc(-50% + var(--nudge-x)))`
  } else if (calculatedPosition.value === 'left' || calculatedPosition.value === 'right') {
    style.transform = `translateY(-50%)`
  }
  return style
})

/**
 * Hàm tính toán vị trí thực tế của Tooltip so với viewport
 */
async function updatePosition() {
  if (!containerRef.value || !visible.value) return

  await nextTick()
  const bubble = bubbleRef.value
  if (!bubble) return

  const triggerRect = containerRef.value.getBoundingClientRect()
  const bubbleRect = bubble.getBoundingClientRect()
  const viewHeight = window.innerHeight
  const viewWidth = window.innerWidth
  const padding = 8

  let newPos = props.position

  // 1. Flip logic
  if (props.position === 'top' && triggerRect.top < bubbleRect.height + padding) {
    newPos = 'bottom'
  } else if (
    props.position === 'bottom' &&
    viewHeight - triggerRect.bottom < bubbleRect.height + padding
  ) {
    newPos = 'top'
  } else if (props.position === 'left' && triggerRect.left < bubbleRect.width + padding) {
    newPos = 'right'
  } else if (
    props.position === 'right' &&
    viewWidth - triggerRect.right < bubbleRect.width + padding
  ) {
    newPos = 'left'
  }
  calculatedPosition.value = newPos

  // 2. Tính tọa độ cơ bản
  let tempTop, tempLeft
  switch (newPos) {
    case 'top':
      tempTop = triggerRect.top - bubbleRect.height - padding
      tempLeft = triggerRect.left + triggerRect.width / 2
      break
    case 'bottom':
      tempTop = triggerRect.bottom + padding
      tempLeft = triggerRect.left + triggerRect.width / 2
      break
    case 'left':
      tempTop = triggerRect.top + triggerRect.height / 2
      tempLeft = triggerRect.left - bubbleRect.width - padding
      break
    case 'right':
      tempTop = triggerRect.top + triggerRect.height / 2
      tempLeft = triggerRect.right + padding
      break
  }

  // 3. Nudge logic (chống tràn ngang cho top/bottom)
  nudgeX.value = 0
  if (newPos === 'top' || newPos === 'bottom') {
    const halfWidth = bubbleRect.width / 2
    if (tempLeft - halfWidth < 8) {
      nudgeX.value = 8 - (tempLeft - halfWidth)
    } else if (tempLeft + halfWidth > viewWidth - 8) {
      nudgeX.value = viewWidth - 8 - (tempLeft + halfWidth)
    }
  }

  bubbleTop.value = tempTop
  bubbleLeft.value = tempLeft
}

function showTooltip() {
  timeout = setTimeout(() => {
    visible.value = true
    updatePosition()
  }, props.delay)
}

function hideTooltip() {
  clearTimeout(timeout)
  visible.value = false
  // Reset về vị trí ban đầu sau khi ẩn để lần sau tính toán lại
  setTimeout(() => {
    calculatedPosition.value = props.position
    nudgeX.value = 0
    bubbleTop.value = 0
    bubbleLeft.value = 0
  }, 200)
}

// Recalculate position on window resize/scroll
function handleResizeOrScroll() {
  updatePosition()
}

onMounted(() => {
  window.addEventListener('resize', handleResizeOrScroll)
  window.addEventListener('scroll', handleResizeOrScroll, true) // Use capture phase for scroll
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResizeOrScroll)
  window.removeEventListener('scroll', handleResizeOrScroll, true)
})
</script>

<style scoped>
.tooltip-container {
  position: relative;
  display: inline-block;
  width: fit-content;
}

.tooltip-bubble {
  position: fixed; /* Changed from absolute to fixed for Teleport */
  z-index: 9999;
  background-color: var(--color-tooltip-bg);
  color: var(--color-tooltip-text);
  padding: 8px 12px;
  border-radius: 8px;
  font-size: 13px;
  width: max-content;
  max-width: 600px;
  white-space: normal;
  pointer-events: none;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  text-align: center;
}

.tooltip-content {
  word-wrap: break-word;
}

/* Mũi tên của bubble speech */
.tooltip-arrow {
  position: absolute;
  width: 0;
  height: 0;
  border-style: solid;
}

/* --- Cấu hình các hướng hiển thị --- */

/* TOP */
.tooltip--top {
  bottom: calc(100% + 8px); /* Cách trigger 8px theo guideline */
  left: 50%;
  /* Positioned by JS, transform only for centering */
}
.tooltip--top .tooltip-arrow {
  top: 100%;
  left: calc(50% - var(--nudge-x, 0px));
  transform: translateX(-50%);
  border-width: 6px 6px 0 6px;
  border-color: var(--color-tooltip-bg) transparent transparent transparent;
}

/* BOTTOM */
.tooltip--bottom {
  top: calc(100% + 8px);
  left: 50%;
  /* Positioned by JS, transform only for centering */
}
.tooltip--bottom .tooltip-arrow {
  bottom: 100%;
  left: calc(50% - var(--nudge-x, 0px));
  transform: translateX(-50%);
  border-width: 0 6px 6px 6px;
  border-color: transparent transparent var(--color-tooltip-bg) transparent;
}

/* LEFT */
.tooltip--left {
  right: calc(100% + 8px);
  top: 50%;
  /* Positioned by JS, transform only for centering */
}
.tooltip--left .tooltip-arrow {
  left: 100%;
  top: 50%;
  transform: translateY(-50%);
  border-width: 6px 0 6px 6px;
  border-color: transparent transparent transparent var(--color-tooltip-bg);
}

/* RIGHT */
.tooltip--right {
  left: calc(100% + 8px);
  top: 50%;
  /* Positioned by JS, transform only for centering */
}
.tooltip--right .tooltip-arrow {
  right: 100%;
  top: 50%;
  transform: translateY(-50%);
  border-width: 6px 6px 6px 0;
  border-color: transparent var(--color-tooltip-bg) transparent transparent;
}

/* Animation */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
