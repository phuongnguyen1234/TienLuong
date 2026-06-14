<script setup>
import { computed, useAttrs } from 'vue'

defineOptions({
  inheritAttrs: false,
})

const props = defineProps({
  modelValue: [String, Number],
  error: Boolean,
  width: {
    type: [String, Number],
    default: null,
  },
  rows: {
    type: [String, Number],
    default: 3,
  },
  placeholder: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['update:modelValue'])

const attrs = useAttrs()

const wrapperAttrs = computed(() => ({
  class: attrs.class,
  style: attrs.style,
}))

const textareaAttrs = computed(() => {
  const { class: _c, style: _s, ...rest } = attrs
  return rest
})

function handleInput(event) {
  emit('update:modelValue', event.target.value)
}
</script>

<template>
  <div
    class="m-input-area-wrapper"
    :class="wrapperAttrs.class"
    :style="[
      width ? { width: typeof width === 'number' ? `${width}px` : width } : {},
      wrapperAttrs.style,
    ]"
  >
    <textarea
      v-bind="textareaAttrs"
      :value="modelValue"
      :rows="rows"
      :placeholder="placeholder"
      @input="handleInput"
      class="m-textarea"
      :class="{ 'input--error': error }"
    ></textarea>
  </div>
</template>

<style scoped>
.m-input-area-wrapper {
  position: relative;
  width: 100%;
}

.m-textarea {
  width: 100%;
  padding: 8px 12px;
  border-radius: 8px;
  border: 1px solid var(--border-control-normal);
  outline: none;
  font-family: 'Inter', sans-serif;
  font-size: 13px;
  line-height: 1.5;
  color: var(--color-text-main);
  resize: vertical;
  box-sizing: border-box;
  transition: border-color 0.2s;
  min-height: 86px;
}

.m-textarea:focus {
  border-color: var(--color-primary);
}

.input--error {
  border-color: var(--color-error) !important;
}
</style>
