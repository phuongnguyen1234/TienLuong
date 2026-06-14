<template>
  <div
    class="m-checkbox"
    :class="{
      'm-checkbox--checked': modelValue,
      'm-checkbox--disabled': disabled,
      'm-checkbox--indeterminate': indeterminate,
    }"
    @click.stop="toggle"
  >
    <div class="m-checkbox-inner">
      <!-- Trạng thái Unchecked -->
      <svg
        v-if="!modelValue && !indeterminate"
        width="20"
        height="20"
        viewBox="0 0 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect
          x="3"
          y="3"
          width="14"
          height="14"
          rx="2"
          stroke="var(--border-control-normal)"
          stroke-width="2"
        />
      </svg>
      <!-- Trạng thái Indeterminate (Chọn lửng) -->
      <svg
        v-else-if="indeterminate"
        width="20"
        height="20"
        viewBox="0 0 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect x="2" y="2" width="16" height="16" rx="3" fill="var(--color-primary, #34b057)" />
        <rect x="6" y="9" width="8" height="2" fill="white" />
      </svg>
      <!-- Trạng thái Checked -->
      <svg
        v-else
        width="20"
        height="20"
        viewBox="0 0 20 20"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect x="2" y="2" width="16" height="16" rx="3" fill="var(--color-primary, #34b057)" />
        <path
          d="M6 10L9 13L14 7"
          stroke="white"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        />
      </svg>
    </div>
    <span v-if="label" class="m-checkbox-label">{{ label }}</span>
  </div>
</template>

<script setup>
const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false,
  },
  label: {
    type: String,
    default: '',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  indeterminate: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['update:modelValue', 'change'])

function toggle() {
  if (props.disabled) return
  const newValue = !props.modelValue
  emit('update:modelValue', newValue)
  emit('change', newValue)
}
</script>

<style scoped>
.m-checkbox {
  display: inline-flex;
  align-items: center;
  cursor: pointer;
  user-select: none;
}
.m-checkbox-inner {
  display: flex;
  align-items: center;
  justify-content: center;
}
.m-checkbox--disabled {
  cursor: not-allowed;
  opacity: 0.6;
}
.m-checkbox-label {
  margin-left: 8px;
  font-size: 13px;
  color: var(--color-text-main);
}
</style>
