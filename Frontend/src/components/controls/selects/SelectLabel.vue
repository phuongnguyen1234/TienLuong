<script setup>
import { onMounted, computed, watch } from 'vue'
import BaseSelect from './BaseSelect.vue'

const props = defineProps({
  modelValue: [String, Number],
  label: {
    type: String,
    default: '',
  },
  width: {
    type: [String, Number],
    default: null,
  },
  dropdownWidth: {
    type: [String, Number],
    default: null,
  },
  error: Boolean,
  options: {
    type: Array,
    default: () => [],
  },
  placeholder: {
    type: String,
    default: '',
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  optionLabel: {
    type: String,
    default: '',
  },
  optionValue: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['update:modelValue', 'change'])

const componentStyle = computed(() => {
  if (!props.width) return {}
  return { width: typeof props.width === 'number' ? `${props.width}px` : props.width }
})

const getLabel = (option) => {
  if (props.optionLabel && typeof option === 'object') {
    return option[props.optionLabel]
  }
  return option
}

const getValue = (option) => {
  if (props.optionValue && typeof option === 'object') {
    return option[props.optionValue]
  }
  return option
}

const selectedOptionLabel = computed(() => {
  const selected = props.options.find((opt) => getValue(opt) === props.modelValue)
  return selected ? getLabel(selected) : ''
})

const isSelected = (option) => {
  return props.modelValue === getValue(option)
}

const selectOption = (option, close) => {
  const val = getValue(option)
  emit('update:modelValue', val)
  emit('change', val)
  close()
}

const autoSelect = () => {
  if (
    props.options.length > 0 &&
    (props.modelValue === null || props.modelValue === undefined || props.modelValue === '')
  ) {
    // Tự động chọn phần tử đầu tiên nếu chưa có giá trị
    emit('update:modelValue', getValue(props.options[0]))
  }
}

watch(
  () => props.options,
  (newOptions) => {
    if (newOptions && newOptions.length > 0) autoSelect()
  },
)

onMounted(() => {
  autoSelect()
})
</script>

<template>
  <BaseSelect :error="error" :disabled="disabled" :style="componentStyle" :width="dropdownWidth">
    <template #trigger>
      <div class="select-label-trigger">
        <span v-if="label" class="label-prefix">{{ label }}: </span>
        <span v-if="selectedOptionLabel" class="selected-option">{{ selectedOptionLabel }}</span>
        <span v-else class="placeholder">{{ placeholder }}</span>
      </div>
    </template>

    <template #options="{ close }">
      <li
        v-for="(option, index) in options"
        :key="index"
        @click="selectOption(option, close)"
        :class="{ selected: isSelected(option) }"
      >
        {{ getLabel(option) }}
        <svg
          v-if="isSelected(option)"
          width="16"
          height="16"
          viewBox="0 0 16 16"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M13.3333 4L6 11.3333L2.66667 8"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          />
        </svg>
      </li>
    </template>
  </BaseSelect>
</template>

<style scoped>
.select-label-trigger {
  display: flex;
  align-items: center;
  overflow: hidden;
  white-space: nowrap;
  width: fit-content;
  height: 32px;
  padding: 0 30px 0 12px;
  cursor: pointer;
  font-size: 13px;
  color: var(--color-text-main);
}

.label-prefix {
  color: var(--color-text-secondary);
  margin-right: 4px;
}

.placeholder {
  color: var(--color-placeholder);
}

.select-options li {
  padding: 8px 12px;
  cursor: pointer;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: background 0.2s;
}

.select-options li:hover {
  background-color: var(--item-hover);
  color: var(--color-text-main);
}

.select-options li.selected {
  background-color: var(--bg-table-hover);
  color: var(--color-primary);
  font-weight: 500;
}

.selected-option {
  font-weight: 500;
}
</style>
