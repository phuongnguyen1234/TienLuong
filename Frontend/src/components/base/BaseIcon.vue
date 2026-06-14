<script setup>
const props = defineProps({
  size: {
    type: [Number, String],
    default: 20,
  },
  color: {
    type: String,
    default: 'currentColor',
  },
  strokeWidth: {
    type: [Number, String],
    default: 2,
  },
})
</script>

<template>
  <svg
    v-bind="$attrs"
    :width="size"
    :height="size"
    :viewBox="$attrs.viewBox || '0 0 24 24'"
    :stroke="color"
    :stroke-width="strokeWidth"
    fill="none"
    stroke-linecap="round"
    stroke-linejoin="round"
  >
    <slot />
  </svg>
</template>
