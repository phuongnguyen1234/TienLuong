<script setup>
import { ref, computed, nextTick } from 'vue'
import BaseSelect from './BaseSelect.vue'
import Checkbox from '@/components/controls/checkboxes/Checkbox.vue'

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => [],
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

const searchQuery = ref('')
const inputRef = ref(null)
const baseSelectRef = ref(null)

const getLabel = (option) => {
  if (props.optionLabel && option && typeof option === 'object') {
    return option[props.optionLabel]
  }
  return option
}

const getValue = (option) => {
  if (props.optionValue && option && typeof option === 'object') {
    return option[props.optionValue]
  }
  return option
}

const filteredOptions = computed(() => {
  if (!searchQuery.value) return props.options
  const query = searchQuery.value.toLowerCase()
  return props.options.filter((option) => String(getLabel(option)).toLowerCase().includes(query))
})

const isSelected = (option) => {
  return props.modelValue.includes(getValue(option))
}

const handleOpen = () => {
  nextTick(() => inputRef.value?.focus())
}

const selectOption = (option) => {
  const val = getValue(option)
  const newValue = [...props.modelValue]
  const index = newValue.indexOf(val)

  if (index === -1) {
    newValue.push(val)
  } else {
    newValue.splice(index, 1)
  }
  emit('update:modelValue', newValue)
  emit('change', newValue)

  // Clear search and keep focus
  searchQuery.value = ''
  nextTick(() => inputRef.value?.focus())
}

const removeTag = (index) => {
  const newValue = [...props.modelValue]
  newValue.splice(index, 1)
  emit('update:modelValue', newValue)
  emit('change', newValue)
}

const handleBackspace = () => {
  if (searchQuery.value === '' && props.modelValue.length > 0) {
    removeTag(props.modelValue.length - 1)
  }
}

// Hàm tìm label dựa trên ID đã chọn để hiển thị lên tag
const getTagLabel = (val) => {
  const option = props.options.find((opt) => getValue(opt) === val)
  return option ? getLabel(option) : val
}
</script>

<template>
  <BaseSelect ref="baseSelectRef" :error="error" @open="handleOpen">
    <template #trigger="{ isOpen, open }">
      <div
        class="tags-input-container"
        :class="{ 'input--error': error, 'is-active': isOpen }"
        @click="open"
      >
        <div v-for="(tag, index) in modelValue" :key="index" class="tag">
          {{ getTagLabel(tag) }}
          <span class="remove-tag" @click.stop="removeTag(index)">
            <svg
              width="8"
              height="8"
              viewBox="0 0 10 10"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M8.75 0.75L0.75 8.75M0.75 0.75L8.75 8.75"
                stroke="currentColor"
                stroke-width="1.5"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </span>
        </div>
        <input
          ref="inputRef"
          type="text"
          class="tag-input-field"
          v-model="searchQuery"
          :placeholder="modelValue.length === 0 ? placeholder : ''"
          @keydown.backspace="handleBackspace"
          @focus="open"
          @click.stop="open"
        />
      </div>
    </template>

    <template #options>
      <li
        v-for="(option, index) in filteredOptions"
        :key="index"
        @click.stop="selectOption(option)"
        :class="{ selected: isSelected(option) }"
      >
        <div class="option-content">
          <Checkbox :model-value="isSelected(option)" @update:model-value="selectOption(option)" />
          <span>{{ getLabel(option) }}</span>
        </div>
      </li>
      <li v-if="filteredOptions.length === 0" class="no-result">Không tìm thấy dữ liệu</li>
    </template>
  </BaseSelect>
</template>

<style scoped>
.tags-input-container {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px;
  /* Border is now on BaseSelect wrapper */
  padding: 3px 8px;
  cursor: text;
  width: 100%;
  box-sizing: border-box;
}

.tag {
  background-color: #f0f0f0;
  border-radius: 3px;
  padding: 2px 6px;
  font-size: 12px;
  display: flex;
  align-items: center;
  gap: 6px;
  color: #333;
  border: 1px solid #d9d9d9;
}
.remove-tag {
  cursor: pointer;
  display: flex;
  align-items: center;
  color: #666;
}
.remove-tag:hover {
  color: #e61d1d;
}
.tag-input-field {
  border: none;
  outline: none;
  flex: 1;
  min-width: 60px;
  font-size: 13px;
  padding: 4px 0;
  margin: 0;
  background: transparent;
  color: #111;
}
.select-options li {
  padding: 8px 12px;
  cursor: pointer;
  transition: background 0.2s;
}
.select-options li:hover {
  background-color: var(--item-hover);
}
.select-options li.selected {
  background-color: var(--bg-table-hover);
  color: var(--color-primary);
  font-weight: 500;
}
.option-content {
  display: flex;
  align-items: center;
  gap: 10px;
}
.icon-container {
  width: 16px;
  height: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.no-result {
  padding: 8px 12px;
  color: #999;
  text-align: center;
  cursor: default;
  font-size: 13px;
}
</style>
