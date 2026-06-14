<template>
  <div :class="['m-radio-group', `m-radio-group--${direction}`]">
    <Radio
      v-for="option in options"
      :key="option.value"
      :model-value="modelValue"
      :value="option.value"
      :label="option.label"
      :name="name"
      :disabled="disabled || option.disabled"
      @update:model-value="$emit('update:modelValue', $event)"
      @change="$emit('change', $event)"
    />
  </div>
</template>

<script setup>
import Radio from './Radio.vue'

defineProps({
  modelValue: [String, Number, Boolean],
  options: {
    type: Array,
    required: true,
  },
  name: {
    type: String,
    required: true,
  },
  direction: {
    type: String,
    default: 'horizontal',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
})

defineEmits(['update:modelValue', 'change'])
</script>

<style scoped>
.m-radio-group {
  display: flex;
  gap: 20px;
}
.m-radio-group--vertical {
  flex-direction: column;
  gap: 12px;
  align-items: flex-start;
}
.m-radio-group--horizontal {
  flex-direction: row;
  flex-wrap: wrap;
  min-height: 32px;
  align-items: center;
}
</style>
