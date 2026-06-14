<template>
  <BaseModal :visible="visible" @update:visible="onClose">
    <template #title>Thông báo</template>
    <div class="confirmation-content">
      <p class="message-text">{{ message }}</p>

      <div class="confirmation-actions">
        <ButtonGroup>
          <Button variant="secondary" @click="onCancel">{{ cancelButtonText }}</Button>
          <Button :variant="confirmButtonVariant" @click="onConfirm">{{
            confirmButtonText
          }}</Button>
        </ButtonGroup>
      </div>
    </div>
  </BaseModal>
</template>

<script setup>
import BaseModal from '@/components/base/BaseModal.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonGroup from '@/components/controls/buttons/ButtonGroup.vue'

defineProps({
  visible: {
    type: Boolean,
    default: false,
  },
  message: {
    type: String,
    required: true,
  },
  cancelButtonText: {
    type: String,
    default: 'Hủy',
  },
  confirmButtonText: {
    type: String,
    default: 'Xác nhận',
  },
  confirmButtonVariant: {
    type: String,
    default: 'primary',
    validator: (value) =>
      ['primary', 'secondary', 'outline', 'danger', 'text', 'danger-outline'].includes(value),
  },
})

const emit = defineEmits(['update:visible', 'confirm', 'cancel', 'close'])

function onConfirm() {
  emit('confirm')
}

function onCancel() {
  emit('cancel')
  emit('update:visible', false)
}

function onClose() {
  emit('update:visible', false)
  emit('close')
}
</script>

<style scoped>
.confirmation-content {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.message-text {
  margin: 0;
  font-size: 14px;
}
.confirmation-actions {
  display: flex;
  justify-content: flex-end;
}
</style>
