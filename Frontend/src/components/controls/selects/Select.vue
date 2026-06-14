<script setup>
import { onMounted, computed, ref, watch, nextTick } from 'vue'
import BaseSelect from './BaseSelect.vue'
import Tooltip from '@/components/base/Tooltip.vue'

const props = defineProps({
  width: {
    type: [String, Number],
    default: null,
  },
  dropdownWidth: {
    type: [String, Number],
    default: null,
  },
  modelValue: [String, Number],
  required: Boolean,
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
  showAddButton: {
    type: Boolean,
    default: false,
  },
  searchable: {
    type: Boolean,
    default: false,
  },
  noAutoSelect: {
    type: Boolean,
    default: false,
  },
  direction: {
    type: String,
    default: 'bottom',
  },
  optionTooltip: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['update:modelValue', 'change', 'add'])

const componentStyle = computed(() => {
  if (!props.width) return {}
  return { width: typeof props.width === 'number' ? `${props.width}px` : props.width }
})

const baseSelectRef = ref(null)
const inputRef = ref(null)
const searchQuery = ref('')

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

const displayValue = computed(() => {
  const selected = props.options.find((opt) => getValue(opt) === props.modelValue)
  return selected ? getLabel(selected) : ''
})

const getTooltip = (option) => {
  // Nếu không khai báo optionTooltip, coi như Select này không dùng tooltip
  if (!props.optionTooltip) return ''

  if (typeof option === 'object') {
    return option[props.optionTooltip] || getLabel(option)
  }
  return getLabel(option)
}

const displayTooltip = computed(() => {
  const selected = props.options.find((opt) => getValue(opt) === props.modelValue)
  return selected ? getTooltip(selected) : ''
})

// Cập nhật searchQuery khi displayValue thay đổi (ví dụ: khi modelValue được set từ bên ngoài)
watch(
  displayValue,
  (newVal) => {
    searchQuery.value = newVal
  },
  { immediate: true },
)

const filteredOptions = computed(() => {
  if (!props.searchable || !searchQuery.value) {
    return props.options
  }

  // Nếu text trong input giống hệt với display value của item đã chọn,
  // tức là người dùng chưa gõ gì mới, thì hiển thị tất cả.
  if (searchQuery.value === displayValue.value) {
    return props.options
  }

  const query = searchQuery.value.toLowerCase()
  return props.options.filter((option) => {
    return String(getLabel(option)).toLowerCase().includes(query)
  })
})

const isSelected = (option) => {
  return props.modelValue === getValue(option)
}

const selectOption = (option, close) => {
  const val = getValue(option)
  emit('update:modelValue', val)
  emit('change', val)
  // Cập nhật lại searchQuery để input hiển thị đúng giá trị vừa chọn
  if (props.searchable) {
    searchQuery.value = getLabel(option)
  }
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

onMounted(() => {
  if (!props.noAutoSelect) {
    autoSelect()
  }
})

const handleOpen = () => {
  if (props.searchable) {
    // Khi mở, xóa trắng query để người dùng tìm kiếm, nhưng chỉ khi nó đang hiển thị giá trị đã chọn
    if (searchQuery.value === displayValue.value) {
      searchQuery.value = ''
    }
    nextTick(() => {
      inputRef.value?.focus()
    })
  }
}

const handleClose = () => {
  // Khi đóng, reset lại query về giá trị đang được chọn
  searchQuery.value = displayValue.value
}
</script>

<template>
  <BaseSelect
    ref="baseSelectRef"
    :style="componentStyle"
    :width="dropdownWidth"
    :direction="direction"
    :error="error"
    :disabled="disabled"
    :required="required"
    :show-add-button="showAddButton"
    @add="$emit('add')"
    @open="handleOpen"
    @close="handleClose"
  >
    <template #tooltip v-if="displayTooltip">
      <Tooltip :text="displayTooltip">
        <svg
          width="14"
          height="14"
          viewBox="129 377 14 14"
          fill="none"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M143 384.001C143 385.385 142.589 386.739 141.82 387.89C141.051 389.041 139.958 389.938 138.679 390.468C137.4 390.998 135.992 391.137 134.634 390.866C133.277 390.596 132.029 389.93 131.05 388.951C130.071 387.972 129.405 386.724 129.135 385.367C128.864 384.009 129.003 382.601 129.533 381.322C130.063 380.043 130.96 378.95 132.111 378.181C133.262 377.412 134.616 377.001 136 377.001C136.919 377 137.83 377.181 138.68 377.532C139.529 377.884 140.301 378.4 140.951 379.05C141.601 379.7 142.117 380.472 142.468 381.321C142.82 382.171 143.001 383.082 143 384.001Z"
            fill="#3A94FF"
            fill-opacity="0.2"
          />
          <path
            d="M134.89 387.862C134.78 387.767 134.693 387.648 134.634 387.515C134.575 387.382 134.547 387.237 134.55 387.092C134.63 386.303 134.808 385.527 135.08 384.782C135.161 384.533 135.208 384.274 135.219 384.012C135.219 383.612 135.06 383.502 134.639 383.502C134.419 383.512 134.202 383.563 134 383.651L134.12 383.181C134.6 382.951 135.119 382.812 135.65 382.771C136.39 382.771 136.93 383.131 136.93 383.841C136.929 384.111 136.892 384.38 136.82 384.641L136.4 386.152C136.309 386.452 136.15 387.121 136.4 387.322C136.575 387.397 136.764 387.432 136.955 387.425C137.145 387.418 137.331 387.369 137.5 387.281L137.379 387.751C136.882 387.987 136.344 388.129 135.795 388.168C135.466 388.182 135.143 388.073 134.89 387.862ZM135.81 380.871C135.81 380.639 135.902 380.416 136.066 380.252C136.23 380.088 136.453 379.996 136.685 379.996C136.917 379.996 137.14 380.088 137.304 380.252C137.468 380.416 137.56 380.639 137.56 380.871C137.561 380.987 137.538 381.101 137.494 381.208C137.449 381.314 137.383 381.411 137.3 381.491C137.179 381.615 137.023 381.7 136.852 381.735C136.682 381.769 136.505 381.752 136.345 381.685C136.185 381.619 136.048 381.505 135.953 381.36C135.857 381.215 135.808 381.045 135.81 380.871Z"
            fill="#3A94FF"
          />
        </svg>
      </Tooltip>
    </template>

    <template #trigger>
      <input
        v-if="!searchable"
        type="text"
        class="select-input-field"
        :value="displayValue"
        :placeholder="placeholder"
        readonly
        :disabled="disabled"
      />
      <input
        v-else
        ref="inputRef"
        type="text"
        class="select-input-field"
        v-model="searchQuery"
        :placeholder="placeholder"
        :disabled="disabled"
        autocomplete="off"
        @click.stop="baseSelectRef?.open()"
      />
    </template>

    <template #options="{ close }">
      <li
        v-for="(option, index) in filteredOptions"
        :key="index"
        @click="selectOption(option, close)"
        :class="{ selected: isSelected(option) }"
      >
        <div class="option-item-content">
          <span>{{ getLabel(option) }}</span>
          <Tooltip v-if="getTooltip(option)" :text="getTooltip(option)">
            <svg
              width="14"
              height="14"
              viewBox="129 377 14 14"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M143 384.001C143 385.385 142.589 386.739 141.82 387.89C141.051 389.041 139.958 389.938 138.679 390.468C137.4 390.998 135.992 391.137 134.634 390.866C133.277 390.596 132.029 389.93 131.05 388.951C130.071 387.972 129.405 386.724 129.135 385.367C128.864 384.009 129.003 382.601 129.533 381.322C130.063 380.043 130.96 378.95 132.111 378.181C133.262 377.412 134.616 377.001 136 377.001C136.919 377 137.83 377.181 138.68 377.532C139.529 377.884 140.301 378.4 140.951 379.05C141.601 379.7 142.117 380.472 142.468 381.321C142.82 382.171 143.001 383.082 143 384.001Z"
                fill="#3A94FF"
                fill-opacity="0.2"
              />
              <path
                d="M134.89 387.862C134.78 387.767 134.693 387.648 134.634 387.515C134.575 387.382 134.547 387.237 134.55 387.092C134.63 386.303 134.808 385.527 135.08 384.782C135.161 384.533 135.208 384.274 135.219 384.012C135.219 383.612 135.06 383.502 134.639 383.502C134.419 383.512 134.202 383.563 134 383.651L134.12 383.181C134.6 382.951 135.119 382.812 135.65 382.771C136.39 382.771 136.93 383.131 136.93 383.841C136.929 384.111 136.892 384.38 136.82 384.641L136.4 386.152C136.309 386.452 136.15 387.121 136.4 387.322C136.575 387.397 136.764 387.432 136.955 387.425C137.145 387.418 137.331 387.369 137.5 387.281L137.379 387.751C136.882 387.987 136.344 388.129 135.795 388.168C135.466 388.182 135.143 388.073 134.89 387.862ZM135.81 380.871C135.81 380.639 135.902 380.416 136.066 380.252C136.23 380.088 136.453 379.996 136.685 379.996C136.917 379.996 137.14 380.088 137.304 380.252C137.468 380.416 137.56 380.639 137.56 380.871C137.561 380.987 137.538 381.101 137.494 381.208C137.449 381.314 137.383 381.411 137.3 381.491C137.179 381.615 137.023 381.7 136.852 381.735C136.682 381.769 136.505 381.752 136.345 381.685C136.185 381.619 136.048 381.505 135.953 381.36C135.857 381.215 135.808 381.045 135.81 380.871Z"
                fill="#3A94FF"
              />
            </svg>
          </Tooltip>
        </div>
      </li>
      <li v-if="filteredOptions.length === 0" class="no-result">Không tìm thấy dữ liệu</li>
    </template>
  </BaseSelect>
</template>

<style scoped>
.select-input-field {
  width: 100%;
  height: 32px;
  border: none;
  outline: none;
  padding: 0 12px;
  font-size: 13px;
  color: var(--color-text-main);
  background: transparent;
  cursor: pointer;
  text-overflow: ellipsis;
  box-sizing: border-box;
}
.select-input-field:disabled {
  cursor: not-allowed;
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

.no-result {
  padding: 8px 12px;
  color: var(--color-text-secondary);
  text-align: center;
  cursor: default;
}

.option-item-content {
  display: flex;
  align-items: center;
  gap: 16px;
  width: 100%;
}
</style>
