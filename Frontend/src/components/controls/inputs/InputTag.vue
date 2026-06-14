<script setup>
import { ref, onMounted, onBeforeUnmount } from 'vue'

const props = defineProps({
  modelValue: {
    type: Array,
    default: () => [],
  },
  error: Boolean,
  placeholder: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['update:modelValue', 'change'])

const inputValue = ref('')
const inputRef = ref(null)
const containerRef = ref(null)
const isActive = ref(false)

const focusInput = () => {
  isActive.value = true
  inputRef.value?.focus()
}

const addTag = () => {
  const val = inputValue.value.trim()
  if (val) {
    // Kiểm tra trùng lặp (nếu cần thiết, có thể bỏ qua nếu cho phép trùng)
    if (!props.modelValue.includes(val)) {
      const newValue = [...props.modelValue, val]
      emit('update:modelValue', newValue)
      emit('change', newValue)
    }
    inputValue.value = ''
  }
}

const removeTag = (index) => {
  const newValue = [...props.modelValue]
  newValue.splice(index, 1)
  emit('update:modelValue', newValue)
  emit('change', newValue)
}

const handleBackspace = () => {
  if (inputValue.value === '' && props.modelValue.length > 0) {
    removeTag(props.modelValue.length - 1)
  }
}

const handleClickOutside = (event) => {
  if (containerRef.value && !containerRef.value.contains(event.target)) {
    isActive.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside, true)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside, true)
})
</script>

<template>
  <div
    ref="containerRef"
    class="tags-input-container"
    :class="{ 'input--error': error, 'is-active': isActive }"
    @click="focusInput"
  >
    <div v-for="(tag, index) in modelValue" :key="index" class="tag">
      {{ tag }}
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
      v-model="inputValue"
      :placeholder="modelValue.length === 0 ? placeholder : ''"
      @keydown.enter.prevent="addTag"
      @keydown.backspace="handleBackspace"
    />
  </div>
</template>

<style scoped>
.tags-input-container {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 4px;
  min-height: 36px;
  padding: 3px 8px;
  border: 1px solid #e0e6ec;
  border-radius: 8px;
  background-color: #fff;
  cursor: text;
  width: 100%;
  box-sizing: border-box;
  transition: border-color 0.2s;
}
.tags-input-container:hover,
.tags-input-container.is-active {
  border-color: #2680eb;
}
.tags-input-container.input--error {
  border-color: #e61d1d;
}
.tag {
  background-color: #f0f0f0;
  border-radius: 3px;
  padding: 2px 6px;
  font-size: 13px;
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
</style>
