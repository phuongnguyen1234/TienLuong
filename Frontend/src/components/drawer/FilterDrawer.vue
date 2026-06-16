<template>
  <BaseDrawer title="Bộ lọc" @close="$emit('close')">
    <div class="filter-container">
      <Searchbar v-model="searchText" />

      <div class="filter-list">
        <div
          v-for="item in filteredColumns"
          :key="item.key"
          class="filter-item"
          :class="{ 'is-active': item.checked }"
        >
          <div class="filter-item-row">
            <Checkbox v-model="item.checked" :label="item.label" />
          </div>

          <div v-if="item.checked" class="filter-details">
            <!-- Kiểu Enum/Boolean -->
            <template v-if="['bool', 'boolean', 'enum'].includes(item.type)">
              <Select
                v-model="item.operator"
                :options="enumOperations"
                option-label="label"
                option-value="value"
              />
              <template v-if="!['Empty', 'NotEmpty'].includes(item.operator)">
                <!-- Cột tính chất sử dụng Multi-select -->
                <SelectMultipleTags
                  v-if="item.key === 'ScNature'"
                  v-model="item.value"
                  :options="item.options"
                  option-label="label"
                  option-value="value"
                  placeholder="Chọn tính chất"
                />
                <Select
                  v-else
                  v-model="item.value"
                  :options="item.options"
                  option-label="label"
                  option-value="value"
                  placeholder="Chọn giá trị"
                />
              </template>
            </template>

            <!-- Kiểu String và các kiểu khác -->
            <template v-else>
              <Select
                v-model="item.operator"
                :options="stringOperations"
                option-label="label"
                option-value="value"
              />
              <Input
                v-if="!['Empty', 'NotEmpty'].includes(item.operator)"
                v-model="item.value"
                placeholder="Nhập giá trị"
              />
            </template>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <ButtonGroup class="filter-footer-btn">
        <Button variant="outline" height="32px" @click="resetFilters">Bỏ lọc</Button>
        <Button variant="primary" height="32px" @click="applyFilters">Áp dụng</Button>
      </ButtonGroup>
    </template>
  </BaseDrawer>
</template>

<script setup>
import BaseDrawer from '@/components/base/BaseDrawer.vue'
import Input from '@/components/controls/inputs/Input.vue'
import Searchbar from '@/components/controls/inputs/Searchbar.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonGroup from '@/components/controls/buttons/ButtonGroup.vue'
import Select from '@/components/controls/selects/Select.vue'
import Checkbox from '@/components/controls/checkboxes/Checkbox.vue'
import SelectMultipleTags from '@/components/controls/selects/SelectMultipleTags.vue'
import { FilterOperation } from '@/models/filter-operation.js'
import { ref, computed, watch } from 'vue'

const props = defineProps({
  // Danh sách các cột cho phép lọc, được truyền từ component cha (thường là kết quả map từ columnsConfig)
  columns: {
    type: Array,
    default: () => [],
  },
  // Danh sách các filter đang thực sự được áp dụng (dùng để khôi phục trạng thái khi mở drawer)
  appliedFilters: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits(['close', 'apply'])

// Từ khóa tìm kiếm tên trường lọc trong danh sách
const searchText = ref('')

// Trạng thái nội bộ của bộ lọc, chứa thông tin chi tiết: checked, operator, value cho từng cột
const filterState = ref([])

/**
 * Danh sách toán tử dành cho kiểu chuỗi (Text/String)
 */
const stringOperations = [
  { value: FilterOperation.Contains, label: 'Chứa' },
  { value: FilterOperation.NotContains, label: 'Không chứa' },
  { value: FilterOperation.Equals, label: 'Bằng' },
  { value: FilterOperation.NotEqual, label: 'Khác' },
  { value: FilterOperation.StartsWith, label: 'Bắt đầu bằng' },
  { value: FilterOperation.EndsWith, label: 'Kết thúc bằng' },
  { value: FilterOperation.Empty, label: 'Trống' },
  { value: FilterOperation.NotEmpty, label: 'Không trống' },
]

/**
 * Danh sách toán tử dành cho kiểu Enum hoặc Boolean
 */
const enumOperations = [
  { value: FilterOperation.Equals, label: 'Bằng' },
  { value: FilterOperation.NotEqual, label: 'Khác' },
  { value: FilterOperation.Empty, label: 'Trống' },
  { value: FilterOperation.NotEmpty, label: 'Không trống' },
]

/**
 * Watcher thực hiện đồng bộ hóa dữ liệu từ props vào filterState nội bộ.
 * logic: Kết hợp danh sách 'columns' mặc định với 'appliedFilters' hiện có để đánh dấu 'checked'
 * và điền lại các giá trị operator/value người dùng đã chọn trước đó.
 */
watch(
  () => [props.columns, props.appliedFilters],
  ([newCols, newAppliedFilters]) => {
    // Tạo Map để lookup filter đang áp dụng nhanh hơn
    const appliedMap = new Map(newAppliedFilters.map((f) => [f.key, f]))

    filterState.value = newCols.map((col) => {
      const applied = appliedMap.get(col.key)
      let defaultValue
      let defaultOperator

      // Xác định giá trị mặc định dựa trên loại dữ liệu (DataType)
      if (col.key === 'ScNature') {
        defaultValue = []
        defaultOperator = FilterOperation.Equals
      } else if (['boolean', 'bool', 'enum'].includes(col.type)) {
        defaultValue = null
        defaultOperator = FilterOperation.Equals
      } else {
        // text or string
        defaultValue = ''
        defaultOperator = FilterOperation.Contains
      }

      // Nếu cột này đang được áp dụng bộ lọc từ bên ngoài, khởi tạo state với giá trị đó
      let finalValue = applied ? applied.value : defaultValue

      return {
        ...col,
        checked: !!applied,
        operator: applied ? applied.operator : defaultOperator,
        value: finalValue,
        options:
          col.options && col.options.length > 0
            ? col.options
            : ['bool', 'boolean'].includes(col.type)
              ? [
                  { label: 'Có', value: 1 },
                  { label: 'Không', value: 0 },
                ]
              : [],
      }
    })
  },
  { immediate: true, deep: true },
)

/**
 * Computed property trả về danh sách các trường lọc đã được filter qua thanh tìm kiếm (searchText)
 */
const filteredColumns = computed(() => {
  if (!searchText.value) return filterState.value
  const lower = searchText.value.toLowerCase()
  return filterState.value.filter((col) => col.label.toLowerCase().includes(lower))
})

/**
 * Đưa tất cả các trường lọc về trạng thái chưa chọn (unchecked) và reset giá trị về mặc định.
 * Sau đó tự động gọi applyFilters để cập nhật lại danh sách dữ liệu ngoài màn hình chính.
 */
function resetFilters() {
  filterState.value.forEach((item) => {
    item.checked = false
    if (item.key === 'ScNature') {
      item.value = []
      item.operator = FilterOperation.Equals
    } else if (['boolean', 'bool', 'enum'].includes(item.type)) {
      item.value = null
      item.operator = FilterOperation.Equals
    } else {
      item.value = ''
      item.operator = FilterOperation.Contains
    }
  })
  // Sau khi bỏ lọc, áp dụng ngay thay đổi (danh sách rỗng)
  applyFilters()
}

/**
 * Lọc ra những trường đang được tích chọn (checked) và phát sự kiện 'apply' kèm dữ liệu filter.
 * Component cha sẽ nhận mảng này để build query gọi API.
 */
function applyFilters() {
  const activeFilters = filterState.value.filter((item) => item.checked)
  emit('apply', activeFilters)
}
</script>

<style scoped>
.filter-container {
  display: flex;
  flex-direction: column;
  gap: 12px;
  height: 100%;
  padding: 0 8px;
}

.filter-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.filter-item {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-item.is-active {
  background-color: #eafbf2;
  border-radius: 8px;
  padding: 8px;
  margin: 0 -8px;
}

.filter-item-row {
  display: flex;
  align-items: center;
  gap: 8px;
}

.filter-details {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.filter-footer-btn {
  width: 100%;
  justify-content: space-between;
}
</style>
