<template>
  <div class="overlay" :class="{ 'is-transparent': !hasOverlay }" @click="handleClick"></div>
</template>

<script setup>
defineProps({
  hasOverlay: {
    type: Boolean,
    default: true,
  },
})

const emit = defineEmits(['close'])

function handleClick() {
  emit('close')
}
</script>

<style scoped>
.overlay {
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.3);
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: var(--z-index-overlay);
  transition: background-color 0.2s ease;
}

.overlay.is-transparent {
  background-color: transparent;
}
</style>
