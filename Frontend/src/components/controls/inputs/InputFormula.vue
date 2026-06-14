<script setup>
import { ref, computed, onMounted, onBeforeUnmount, watch, nextTick } from 'vue'
import Prism from 'prismjs'
import 'prismjs/themes/prism.css'

// Cấu hình Prism để nhận diện mã thành phần lương [MA_SO]
Prism.languages.formula = {
  variable: {
    pattern: /\[[a-zA-Z0-9_]+\]/,
    alias: 'property',
  },
  operator: /[+\-*/()]/,
  number: /\b\d+(\.\d+)?\b/,
}

const props = defineProps({
  modelValue: {
    type: String,
    default: '',
  },
  placeholder: {
    type: String,
    default: 'Nhập công thức (Bắt đầu bằng dấu =)',
  },
  // Danh sách gợi ý lấy từ API lookup
  suggestions: {
    type: Array,
    default: () => [],
  },
  width: {
    type: [String, Number],
    default: null,
  },
  disabled: {
    type: Boolean,
    default: false,
  },
  error: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['update:modelValue'])

const containerRef = ref(null)
const textareaRef = ref(null)
const showDropdown = ref(false)
const highlightedCode = ref('')
const selectedIndex = ref(0)
const activeTab = ref('formulas') // 'formulas' or 'params'
const dropdownRef = ref(null)
const dropdownStyle = ref({})
const calculatedDirection = ref('bottom')

const formulas = [
  { name: 'SUM(X1, X2, ...)', insert: 'SUM(' },
  { name: 'IF(Logical_test, [value_if_true], [value_if_false])', insert: 'IF(' },
  { name: 'ROUND(Number, Digits)', insert: 'ROUND(' },
  { name: 'MIN(X1, X2, ...)', insert: 'MIN(' },
  { name: 'MAX(X1, X2, ...)', insert: 'MAX(' },
  { name: 'ABS(Number)', insert: 'ABS(' },
]

/**
 * Danh sách hiển thị dựa trên tab đang chọn
 */
const currentList = computed(() => {
  if (activeTab.value === 'formulas') return formulas
  return props.suggestions.map((p) => ({
    ...p,
    displayName: `${p.ScName} (${p.ScCode})`,
    insert: `[${p.ScCode}]`,
  }))
})

function setTab(tab) {
  activeTab.value = tab
  selectedIndex.value = 0
}

/**
 * Đồng bộ highlight khi text thay đổi
 */
function updateHighlight() {
  highlightedCode.value =
    Prism.highlight(props.modelValue, Prism.languages.formula, 'formula') + '\n' // Thêm newline để khớp với textarea
}

/**
 * Tính toán vị trí và hướng hiển thị của dropdown (Top/Bottom)
 */
const updatePosition = () => {
  if (!showDropdown.value || !containerRef.value) return
  const rect = containerRef.value.getBoundingClientRect()
  const viewportHeight = window.innerHeight
  const dropdownMaxHeight = 400 // Giới hạn chiều cao dropdown
  const threshold = dropdownMaxHeight + 10

  const spaceBelow = viewportHeight - rect.bottom
  const spaceAbove = rect.top

  // Ưu tiên mở xuống dưới, nếu không đủ chỗ và phía trên rộng hơn thì mở lên trên
  if (spaceBelow < threshold && spaceAbove > spaceBelow) {
    calculatedDirection.value = 'top'
  } else {
    calculatedDirection.value = 'bottom'
  }

  const newStyle = {
    position: 'fixed',
    left: `${rect.left}px`,
    width: `${rect.width}px`,
    maxHeight: `${dropdownMaxHeight}px`,
    zIndex: 10000,
  }

  if (calculatedDirection.value === 'top') {
    newStyle.bottom = `${viewportHeight - rect.top + 4}px`
    newStyle.top = 'auto'
  } else {
    newStyle.top = `${rect.bottom + 4}px`
    newStyle.bottom = 'auto'
  }
  dropdownStyle.value = newStyle
}

/**
 * Xử lý khi gõ phím
 */
function handleInput(e) {
  const value = e.target.value
  emit('update:modelValue', value)

  // Hiển thị dropdown nếu bắt đầu bằng dấu =
  showDropdown.value = value.startsWith('=')
  if (showDropdown.value) {
    selectedIndex.value = 0
    nextTick(updatePosition)
  }

  updateHighlight()
}

/**
 * Xử lý khi focus: Nếu đã có công thức thì mở gợi ý
 */
function handleFocus() {
  if (props.modelValue && props.modelValue.startsWith('=')) {
    showDropdown.value = true
  }
}

/**
 * Chọn một tham số từ dropdown
 */
function selectSuggestion(item) {
  const newValue = props.modelValue + item.insert
  emit('update:modelValue', newValue)

  nextTick(() => {
    updateHighlight()
    textareaRef.value?.focus()
  })
}

/**
 * Điều hướng dropdown bằng bàn phím
 */
function handleKeyDown(e) {
  if (!showDropdown.value) return

  if (e.key === 'ArrowDown') {
    e.preventDefault()
    selectedIndex.value = (selectedIndex.value + 1) % currentList.value.length
  } else if (e.key === 'ArrowUp') {
    e.preventDefault()
    selectedIndex.value =
      (selectedIndex.value - 1 + currentList.value.length) % currentList.value.length
  } else if (e.key === 'Tab') {
    // Chuyển tab bằng phím Tab khi dropdown đang mở
    e.preventDefault()
    setTab(activeTab.value === 'formulas' ? 'params' : 'formulas')
  } else if (e.key === 'Enter') {
    e.preventDefault()
    if (currentList.value[selectedIndex.value]) {
      selectSuggestion(currentList.value[selectedIndex.value])
    }
  } else if (e.key === 'Escape') {
    showDropdown.value = false
  }
}

/**
 * Trả về chuỗi HTML đã được định dạng (in đậm) cho tên hàm hoặc mã TPL.
 */
function getFormattedLabel(item) {
  if (activeTab.value === 'formulas') {
    // Ví dụ: SUM(X1, X2, ...) -> <strong>SUM</strong>(X1, X2, ...)
    const funcName = item.name.split('(')[0]
    return `<strong>${funcName}</strong>${item.name.substring(funcName.length)}`
  } else {
    // Ví dụ: Lương cơ bản (LCB) -> Lương cơ bản (<strong>LCB</strong>)
    // item ở đây đã có ScName và ScCode do được map từ props.suggestions
    return `${item.ScName} (<strong>${item.ScCode}</strong>)`
  }
}

const handleClickOutside = (event) => {
  const isClickInsideTrigger = containerRef.value && containerRef.value.contains(event.target)
  // Do dropdown được Teleport ra body nên containerRef.contains sẽ trả về false khi click vào dropdown
  const isClickInsideDropdown = dropdownRef.value && dropdownRef.value.contains(event.target)

  if (!isClickInsideTrigger && !isClickInsideDropdown) {
    showDropdown.value = false
  }
}

watch(() => props.modelValue, updateHighlight)

watch(showDropdown, (val) => {
  if (val) nextTick(updatePosition)
})

onMounted(() => {
  updateHighlight()
  document.addEventListener('click', handleClickOutside, true)
  window.addEventListener('scroll', updatePosition, true)
  window.addEventListener('resize', updatePosition)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside, true)
  window.removeEventListener('scroll', updatePosition, true)
  window.removeEventListener('resize', updatePosition)
})
</script>

<template>
  <div
    class="m-formula-container"
    :style="[
      width ? { width: typeof width === 'number' ? `${width}px` : width } : {},
      $attrs.style,
    ]"
    :class="{ 'm-formula-container--error': error }"
    ref="containerRef"
  >
    <div class="m-formula-editor">
      <!-- Lớp hiển thị highlight (phía sau) -->
      <pre
        class="m-formula-highlight"
        aria-hidden="true"
      ><code v-html="highlightedCode"></code></pre>

      <!-- Lớp nhập liệu (phía trước, trong suốt) -->
      <textarea
        ref="textareaRef"
        class="m-formula-input"
        :value="modelValue"
        :placeholder="placeholder"
        @input="handleInput"
        @focus="handleFocus"
        @keydown="handleKeyDown"
        :disabled="disabled"
      ></textarea>
    </div>

    <!-- Dropdown gợi ý Intellisense -->
    <Teleport to="body">
      <div
        v-if="showDropdown"
        ref="dropdownRef"
        class="m-formula-dropdown"
        :class="{ 'opens-up': calculatedDirection === 'top' }"
        :style="dropdownStyle"
      >
        <div class="m-dropdown-tabs">
          <div
            class="m-tab-item"
            :class="{ 'is-active': activeTab === 'formulas' }"
            @mousedown.prevent="setTab('formulas')"
          >
            Công thức
          </div>
          <div
            class="m-tab-item"
            :class="{ 'is-active': activeTab === 'params' }"
            @mousedown.prevent="setTab('params')"
          >
            Tham số
          </div>
        </div>
        <div class="m-dropdown-list">
          <div
            v-for="(item, index) in currentList"
            :key="index"
            class="dropdown-item"
            :class="{ 'is-active': index === selectedIndex }"
            @mousedown.prevent="selectSuggestion(item)"
          >
            <div class="item-icon">
              <svg
                v-if="activeTab === 'formulas'"
                width="24"
                height="24"
                viewBox="340 100 24 24"
                fill="none"
              >
                <g id="Icon Placeholder_118">
                  <path
                    id="Vector_108"
                    d="m 342.5,115.833 c 0,0.442 0.176,0.866 0.488,1.179 0.313,0.312 0.737,0.488 1.179,0.488 1.666,0 1.666,-3.333 2.5,-7.5 0.833,-4.167 0.833,-7.5 2.5,-7.5 0.442,0 0.866,0.176 1.178,0.488 0.313,0.313 0.488,0.737 0.488,1.179 M 344.167,110 h 5 m 3.333,0 5,5 m -5,0 5,-5"
                    stroke="#717680"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </svg>
              <svg v-else width="24" height="24" viewBox="360 100 24 24" fill="none">
                <g id="Icon Placeholder_119">
                  <path
                    id="Vector_109"
                    d="m 363.333,105 c 0,0.663 0.703,1.299 1.953,1.768 1.25,0.469 2.946,0.732 4.714,0.732 1.768,0 3.464,-0.263 4.714,-0.732 1.25,-0.469 1.953,-1.105 1.953,-1.768 m -13.334,0 c 0,-0.663 0.703,-1.299 1.953,-1.768 1.25,-0.469 2.946,-0.732 4.714,-0.732 1.768,0 3.464,0.263 4.714,0.732 1.25,0.469 1.953,1.105 1.953,1.768 m -13.334,0 v 5 m 13.334,-5 v 5 m -13.334,0 c 0,0.663 0.703,1.299 1.953,1.768 1.25,0.469 2.946,0.732 4.714,0.732 1.768,0 3.464,-0.263 4.714,-0.732 1.25,-0.469 1.953,-1.105 1.953,-1.768 m -13.334,0 v 5 c 0,0.663 0.703,1.299 1.953,1.768 1.25,0.469 2.946,0.732 4.714,0.732 1.768,0 3.464,-0.263 4.714,-0.732 1.25,-0.469 1.953,-1.105 1.953,-1.768 v -5"
                    stroke="#717680"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </svg>
            </div>
            <span class="item-name" v-html="getFormattedLabel(item)"></span>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
.m-formula-container {
  position: relative;
  width: 100%;
  border: 1px solid var(--border-control-normal);
  border-radius: 8px;
  background: #fff;
}

.m-formula-container--error {
  border-color: var(--color-error);
}

.m-formula-editor {
  position: relative;
  min-height: 80px;
  font-family: 'Inter';
  font-size: 13px;
  line-height: 1.6;
}

.m-formula-input,
.m-formula-highlight {
  position: absolute;
  font-family: 'Inter', sans-serif !important;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  padding: 8px 12px;
  margin: 0;
  border: none;
  box-sizing: border-box;
  overflow-y: auto;
  white-space: pre-wrap;
  word-break: break-all;
  font-size: inherit;
  line-height: inherit;
}

.m-formula-input {
  background: transparent;
  color: transparent; /* Giấu text thật để hiện highlight phía sau */
  caret-color: #212121; /* Giữ lại con trỏ chuột */
  outline: none;
  z-index: 2;
  resize: none;
}

.m-formula-highlight {
  z-index: 1;
  color: #212121;
  pointer-events: none;
}

/* Ghi đè font mặc định của Prism */
.m-formula-highlight :deep(code),
.m-formula-highlight :deep(span) {
  font-family: 'Inter', sans-serif !important;
}

.m-formula-dropdown {
  background: #fff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  z-index: 1000;
  display: flex;
  flex-direction: column;
  padding: 16px; /* Giảm padding cho gọn */
  box-sizing: border-box;
  overflow: hidden;
}

.m-formula-dropdown.opens-up {
  box-shadow: 0 -4px 12px rgba(0, 0, 0, 0.15);
}

.m-dropdown-tabs {
  display: flex;
  border-bottom: 1px solid #e0e0e0;
  background: #fff;
  gap: 16px;
}

.m-tab-item {
  padding: 8px 0;
  text-align: center;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  color: var(--color-text-secondary);
  border-bottom: 2px solid transparent;
}

.m-tab-item.is-active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
  background: #fff;
}

.m-dropdown-list {
  flex: 1;
  overflow-y: auto;
  padding-bottom: 8px;
  margin-top: 4px;
}

.dropdown-item {
  cursor: pointer;
  display: flex;
  align-items: stretch;
  gap: 12px;
  font-size: 13px;
  padding: 0 16px;
}

.dropdown-item.is-active,
.dropdown-item:hover {
  background-color: var(--bg-table-hover);
}

.dropdown-item.is-active .item-name,
.dropdown-item:hover .item-name {
  color: var(--color-primary);
}

.item-icon {
  display: flex;
  align-items: center;
  color: #717680;
}

.item-name {
  color: var(--color-text-main);
  flex: 1;
  display: flex;
  align-items: center;
  border-bottom: 1px solid #e0e0e0;
  padding: 16px 0;
}

.dropdown-item:last-child .item-name {
  border-bottom: none;
}
</style>
