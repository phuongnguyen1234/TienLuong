<template>
  <div class="form-input-section" :style="{ left: left }">
    <slot></slot>
  </div>
</template>

<script setup>
defineProps({
  left: {
    type: String,
    default: '0',
  },
})
</script>

<style scoped>
.form-input-section {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
</style>
