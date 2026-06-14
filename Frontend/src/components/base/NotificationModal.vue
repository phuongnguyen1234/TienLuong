<template>
  <BaseModal :visible="visible" @update:visible="close" content-padding="0">
    <template #title>{{ title }}</template>
    <p class="notification-message">{{ message }}</p>
    <div class="notification-actions">
      <Button variant="primary" @click="close">Đóng</Button>
    </div>
  </BaseModal>
</template>

<script setup>
import BaseModal from './BaseModal.vue'
import Button from '../controls/buttons/Button.vue'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  title: {
    type: String,
    default: 'Thông báo',
  },
  message: {
    type: String,
    default: 'Nội dung thông báo.',
  },
})

const emit = defineEmits(['update:visible'])

function close() {
  emit('update:visible', false)
}
</script>

<style scoped>
.notification-message {
  font-size: 13px;
  color: var(--color-text-main);
  margin-bottom: 24px;
}

.notification-actions {
  display: flex;
  justify-content: flex-end;
}
</style>
