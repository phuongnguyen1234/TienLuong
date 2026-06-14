<template>
  <div class="form-input-row">
    <div class="form-label" :title="label">
      {{ label }} <span v-if="required" class="required">&nbsp;*</span>
      <div v-if="tooltip" class="info-icon" :title="tooltip">
        <svg
          width="14"
          height="14"
          viewBox="0 0 14 14"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M6.75 4.75H6.75667M6.08333 6.75H6.75V9.41667H7.41667M0.75 6.75C0.75 7.53793 0.905195 8.31815 1.20672 9.0461C1.50825 9.77405 1.95021 10.4355 2.50736 10.9926C3.06451 11.5498 3.72595 11.9917 4.4539 12.2933C5.18185 12.5948 5.96207 12.75 6.75 12.75C7.53793 12.75 8.31815 12.5948 9.0461 12.2933C9.77405 11.9917 10.4355 11.5498 10.9926 10.9926C11.5498 10.4355 11.9917 9.77405 12.2933 9.0461C12.5948 8.31815 12.75 7.53793 12.75 6.75C12.75 5.1587 12.1179 3.63258 10.9926 2.50736C9.86742 1.38214 8.3413 0.75 6.75 0.75C5.1587 0.75 3.63258 1.38214 2.50736 2.50736C1.38214 3.63258 0.75 5.1587 0.75 6.75Z"
            stroke="currentColor"
            stroke-width="1.5"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </div>
    </div>
    <div class="form-input-wrapper">
      <!-- 
        Logic này sẽ lấy phần tử đầu tiên trong slot làm "control chính"
        và bọc nó trong một container `relative` để định vị thông báo lỗi.
        Các phần tử còn lại trong slot sẽ được render như bình thường.
        Điều này cho phép căn giữa thông báo lỗi với control chính (Input/Select)
        ngay cả khi có các control phụ (Checkbox) đi kèm.
      -->
      <div
        v-if="$slots.default"
        class="main-control-container"
        :class="{ 'is-full-width': $slots.default().length === 1 }"
      >
        <component :is="$slots.default()[0]" />
        <div class="form-error-msg">
          {{ error }}
        </div>
      </div>
      <component v-for="vnode in $slots.default?.().slice(1)" :is="vnode" />
    </div>
  </div>
</template>

<script setup>
defineProps({
  label: {
    type: String,
    default: '',
  },
  /**
   * Hướng hiển thị của label và input
   * Values: 'horizontal' (mặc định), 'vertical' (label trên input)
   */
  direction: {
    type: String,
    default: 'horizontal',
    validator: (value) => ['horizontal', 'vertical'].includes(value),
  },
  required: {
    type: Boolean,
    default: false,
  },
  labelWidth: {
    type: String,
    default: '200px',
  },
  alignItems: {
    type: String,
    default: 'flex-start',
  },
  tooltip: {
    type: String,
    default: '',
  },
  error: {
    type: String,
    default: '',
  },
  fitContent: {
    type: Boolean,
    default: false,
  },
})
</script>

<style scoped>
.form-input-row {
  width: 100%;
  min-height: 32px;
  display: v-bind("direction === 'vertical' ? 'block' : 'flex'");
  align-items: v-bind("direction === 'vertical' ? 'flex-start' : alignItems");
  gap: 8px;
}

@media (max-width: 1024px) {
  .form-input-row {
    display: block !important;
  }
}

.form-label {
  width: v-bind("direction === 'vertical' ? '100%' : labelWidth");
  font-weight: 400;
  font-size: 13px;
  color: #111;
  display: flex;
  align-items: center;
  min-width: v-bind("direction === 'vertical' ? '0' : labelWidth");
  height: v-bind("direction === 'vertical' ? 'auto' : '32px'");
  margin-bottom: v-bind("direction === 'vertical' ? '4px' : '0'");
}

@media (max-width: 1024px) {
  .form-label {
    width: 100% !important;
    min-width: 0 !important;
    margin-bottom: 4px !important;
  }
}

.required {
  color: #e61d1d;
}

.info-icon {
  margin-left: 6px;
  cursor: pointer;
  display: flex;
}

.form-input-wrapper {
  flex: v-bind("fitContent ? '0 0 auto' : '1'");
  display: flex;
  align-items: flex-start;
  gap: v-bind("direction === 'vertical' ? '0px' : '8px'");
  width: 100%;
  position: relative;
}

@media (max-width: 1024px) {
  .form-input-wrapper {
    width: 100% !important;
  }
}

.main-control-container {
  position: relative;
  display: flex;
  flex-direction: column;
}

.main-control-container.is-full-width {
  width: 100%;
}

.form-error-msg {
  margin-top: 4px;
  color: #e61d1d;
  font-size: 12px;
  height: 16px; /* Giữ chỗ cố định */
  line-height: 16px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
