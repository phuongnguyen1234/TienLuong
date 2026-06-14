<template>
  <div class="page-container">
    <div class="page-container_header">
      <div class="page-container_header_left">
        <div v-if="canGoBack" class="btn-back" @click="$emit('back')">
          <svg
            width="18"
            height="18"
            viewBox="0 0 24 24"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M19 12H5M12 19L5 12L12 5"
              stroke="#717680"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </div>
        <div class="page-container_header_title">{{ title }}</div>
        <slot name="header-left"></slot>
      </div>
      <div class="page-container_header_right">
        <slot name="header-right"></slot>
      </div>
    </div>
    <div class="page-container_body">
      <div class="page-container_body_main">
        <slot></slot>
      </div>
      <slot name="sidebar"></slot>
    </div>
    <slot name="footer"></slot>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    default: '',
  },
  canGoBack: {
    type: Boolean,
    default: false,
  },
  paddingBottom: {
    type: [String, Number],
    default: 12,
  },
})

defineEmits(['back'])
</script>

<style scoped>
.page-container {
  height: 100%;
  display: flex;
  flex-direction: column;
  width: 100%;
  background-color: #f0f2f5;
}

.page-container_header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  height: 56px;
  box-sizing: border-box;
}

.page-container_header_left {
  display: flex;
  align-items: center;
  gap: 8px;
}

.btn-back {
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: 50%;
  transition: background-color 0.2s;
}

.btn-back:hover {
  background-color: #dadce3;
}

.page-container_header_title {
  font-weight: 700;
  font-size: 20px;
  color: #1f2937;
}

.page-container_header_right {
  display: flex;
  align-items: center;
}

.page-container_body {
  flex: 1;
  display: flex;
  overflow: hidden;
  position: relative;
  padding: 0px 16px 0px 16px;
  padding-bottom: v-bind(
    "typeof paddingBottom === 'number' ? paddingBottom + 'px' : paddingBottom"
  );
  box-sizing: border-box;
  gap: 16px;
}

.page-container_body_main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  background-color: #fff;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.05);
}
</style>
