<template>
  <div class="form-section">
    <div v-if="title" class="form-section-title">{{ title }}</div>
    <slot></slot>
  </div>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    default: '',
  },
})
</script>

<style scoped>
.form-section {
  background-color: #fff;
  border-radius: 8px;
  padding: 16px;
}

.form-section-title {
  font-weight: 600;
  font-size: 13px;
  margin-bottom: 16px;
}
</style>
