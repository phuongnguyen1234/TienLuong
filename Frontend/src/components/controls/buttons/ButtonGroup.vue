<template>
  <div class="m-button-group" :style="{ gap: normalizedGap }">
    <slot></slot>
  </div>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  /**
   * Khoảng cách giữa các button
   */
  gap: {
    type: [String, Number],
    default: 8,
  },
})

const normalizedGap = computed(() => (typeof props.gap === 'number' ? `${props.gap}px` : props.gap))
</script>

<style scoped>
.m-button-group {
  display: flex;
  align-items: center; /* Căn giữa các button theo chiều dọc */
  /* Width và Height tự động co giãn (fit) theo nội dung bên trong */
  width: fit-content;
  height: fit-content;
  flex-wrap: wrap; /* Cho phép xuống dòng nếu không gian quá hẹp */
}
</style>
