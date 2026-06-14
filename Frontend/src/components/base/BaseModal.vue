<template>
  <div class="modal" v-show="visible">
    <!--overlay-->
    <Overlay :has-overlay="hasOverlay" @close="close" />

    <!--modal-->
    <div class="form" :style="modalStyle">
      <!--form header-->
      <div class="form_header">
        <div class="form_header_title">
          <div class="title-text">
            <slot name="title"></slot>
          </div>

          <!-- Option 2: Nút close nằm ngay bên phải title -->
          <ButtonIcon
            v-if="showClose && closePosition === 'title'"
            variant="text"
            @click="close"
            title="Đóng"
            class="close-btn-title"
          >
            <CloseIcon />
          </ButtonIcon>
        </div>

        <div class="form_header_actions">
          <ButtonIcon v-if="info" variant="text" :title="info" class="info-icon">
            <svg
              width="20"
              height="20"
              viewBox="0 0 20 20"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M10 14V14.01M10 11C10.45 11.001 10.887 10.851 11.241 10.573C11.594 10.296 11.844 9.907 11.95 9.47C12.056 9.033 12.011 8.573 11.823 8.164C11.635 7.755 11.315 7.42199 10.914 7.21799C10.516 7.01399 10.061 6.951 9.623 7.039C9.185 7.126 8.789 7.35999 8.5 7.70099M1 10C1 11.182 1.233 12.352 1.685 13.444C2.137 14.536 2.8 15.528 3.636 16.364C4.472 17.2 5.464 17.863 6.556 18.315C7.648 18.767 8.818 19 10 19C11.182 19 12.352 18.767 13.444 18.315C14.536 17.863 15.528 17.2 16.364 16.364C17.2 15.528 17.863 14.536 18.315 13.444C18.767 12.352 19 11.182 19 10C19 7.613 18.052 5.32399 16.364 3.63599C14.676 1.94799 12.387 1 10 1C7.613 1 5.324 1.94799 3.636 3.63599C1.948 5.32399 1 7.613 1 10Z"
                stroke="currentColor"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </ButtonIcon>
          <slot name="header-actions"></slot>
          <!-- Option 1 (Default): Nút close nằm sát lề phải -->
          <ButtonIcon
            v-if="showClose && closePosition === 'right'"
            variant="text"
            @click="close"
            title="Đóng"
          >
            <CloseIcon />
          </ButtonIcon>
        </div>
      </div>

      <!--form content-->
      <div class="form_content">
        <slot></slot>
      </div>

      <!--form footer-->
      <div v-if="$slots.footer" class="form_footer">
        <slot name="footer"></slot>
      </div>
    </div>
  </div>
</template>

<script setup>
import CloseIcon from '@/components/icons/CloseIcon.vue'
import { computed } from 'vue'
import Overlay from './Overlay.vue'
import ButtonIcon from '@/components/controls/buttons/ButtonIcon.vue'

const props = defineProps({
  visible: Boolean,
  info: String,
  showClose: {
    type: Boolean,
    default: true,
  },
  width: {
    type: String,
    default: '450px',
  },
  height: {
    type: String,
    default: 'fit-content',
  },
  padding: {
    type: String,
    default: '24px 24px 16px 24px',
  },
  contentPadding: {
    type: String,
    default: '16px 0 0 0',
  },
  hasOverlay: {
    type: Boolean,
    default: true,
  },
  closePosition: {
    type: String,
    default: 'right', // 'right' | 'title'
  },
  position: {
    type: String,
    default: 'fixed', // 'fixed' | 'absolute'
  },
  top: String,
  right: String,
  left: String,
  bottom: String,
})

const emit = defineEmits(['update:visible'])

/**
 * Tính toán margin âm cho footer để tràn viền dựa trên padding của modal
 */
const footerMargin = computed(() => {
  const p = props.padding.split(/\s+/)
  const top = p[0] || '0px'
  const right = p[1] || top
  const bottom = p[2] || top
  const left = p[3] || right

  // Tràn sang trái, phải và xuống dưới
  return `16px -${right} -${bottom} -${left}`
})

function close() {
  emit('update:visible', false)
}

const modalStyle = computed(() => {
  const style = { width: props.width, height: props.height }

  if (props.top || props.left || props.right || props.bottom) {
    if (props.top) style.top = props.top
    if (props.left) style.left = props.left
    if (props.right) style.right = props.right
    if (props.bottom) style.bottom = props.bottom

    // Nếu cả top và bottom đều có, bỏ fit-content để modal co giãn (stretch)
    if (props.top && props.bottom && props.height === 'fit-content') {
      style.height = 'auto'
    }
    style.transform = 'none'
  } else {
    // Mặc định căn giữa màn hình nếu không truyền tọa độ
    style.top = '50%'
    style.left = '50%'
    style.transform = 'translate(-50%, -50%)'
  }

  return style
})
</script>

<style scoped>
.info-icon {
  cursor: pointer;
}

.modal {
  position: v-bind(position);
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: var(--z-index-modal);
}

.form {
  background-color: #fff;
  max-height: 98vh; /* Tăng max-height để hỗ trợ các modal cao như 94% */
  width: 560px;
  min-width: 200px;
  position: absolute;
  z-index: calc(var(--z-index-overlay) + 1);
  display: flex;
  flex-direction: column;
  border-radius: 8px;
  padding: v-bind(padding);
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.16);
}

.form_header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
}

.form_header_title {
  font-weight: 700;
  font-size: 16px;
  display: flex;
  align-items: center;
  flex-grow: 1; /* Cho phép title group chiếm không gian, đẩy actions group sang phải */
  min-width: 0; /* Quan trọng để overflow hoạt động trong flex item */
}
.title-text {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.close-btn-title {
  margin-left: 8px;
}
.form_header_actions {
  display: flex;
  align-items: center;
  gap: 8px; /* Khoảng cách giữa các icon action */
}

.form_content {
  overflow-y: auto;
  overflow-x: hidden;
  flex-grow: 1; /* Cho phép content chiếm hết không gian còn lại */
  gap: 8px;
  padding: v-bind(contentPadding);
}

.form_footer {
  background-color: #fff;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  padding: 12px 16px;
  flex-shrink: 0;
  border-top: 1px solid var(--item-hover);
  /* Sử dụng giá trị đã tính toán từ script */
  margin: v-bind(footerMargin);
  /* Re-apply bottom border radius that parent had */
  border-bottom-left-radius: 8px;
  border-bottom-right-radius: 8px;
}
</style>
