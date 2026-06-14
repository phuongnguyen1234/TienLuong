<script setup>
import BaseIcon from '@/components/base/BaseIcon.vue'

defineProps({
  size: [Number, String],
  color: String,
  strokeWidth: [Number, String],
})
</script>

<template>
  <BaseIcon :size="size" :color="color" :stroke-width="strokeWidth" :viewBox="'0 0 14 8'">
    <path d="M1 1L7 7L13 1" />
  </BaseIcon>
</template>
