<template>
  <BaseModal
    :visible="visible"
    @update:visible="$emit('update:visible', $event)"
    width="75%"
    height="94%"
  >
    <template #title>Thêm từ danh mục của hệ thống</template>

    <div class="add-from-system-content">
      <!-- Action bar -->
      <div class="modal_actions">
        <div class="modal_actions_left">
          <div class="search_box">
            <Searchbar v-model="searchQuery" placeholder="Tìm kiếm" width="300px" />
          </div>
          <SelectLabel
            v-model="selectedType"
            label="Loại thành phần"
            :options="typeOptions"
            option-label="label"
            option-value="value"
          />
        </div>
      </div>

      <!-- Table -->
      <div class="table-container">
        <Table
          :headers="headers"
          :data="systemCompositions"
          :selectable="true"
          row-key="ScsId"
          :loading="isLoading"
          v-model:selection="selectedIds"
        >
          <template #ScCode="{ data }">
            {{ data.ScCode ?? '-' }}
          </template>
          <template #ScName="{ data }">
            {{ data.ScName ?? '-' }}
          </template>
          <template #ScType="{ data }">
            {{ getTypeName(data.ScType) }}
          </template>
          <template #ScNature="{ data }">
            {{ data.ScNature !== null ? getNatureText(data.ScNature) : '-' }}
          </template>
          <template #IsTaxDeductible="{ data }">
            {{ data.IsTaxDeductible ? 'Có' : 'Không' }}
          </template>
          <template #LimitExpression="{ data }">
            {{ data.LimitExpression ?? '-' }}
          </template>
          <template #ValueType="{ data }">
            {{ data.ValueType !== null ? getValueTypeText(data.ValueType) : '-' }}
          </template>
          <template #FormulaExpression="{ data }">
            <span v-if="data.CalculationMethod === 0"> Tự động cộng tổng </span>
            <span v-else>{{ data.FormulaExpression || '-' }}</span>
          </template>
          <template #Description="{ data }">
            {{ data.Description ?? '-' }}
          </template>
          <template #IsDisplayedOnPayroll="{ data }">
            {{
              data.IsDisplayedOnPayroll !== null
                ? getIsDisplayedOnPayrollText(data.IsDisplayedOnPayroll)
                : '-'
            }}
          </template>
        </Table>
      </div>

      <Pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :total-records="totalRecords"
      />
    </div>

    <template #footer>
      <ButtonGroup>
        <Button variant="secondary" @click="$emit('update:visible', false)">Hủy bỏ</Button>
        <Button variant="primary" :disabled="selectedIds.length === 0" @click="handleConfirm">
          Đồng ý
        </Button>
      </ButtonGroup>
    </template>
  </BaseModal>

  <!-- Modal xác nhận -->
  <ConfirmationModal
    v-model:visible="isConfirmModalVisible"
    message="Bạn có chắc chắn muốn đưa các thành phần lương đã chọn vào danh sách sử dụng không?"
    @confirm="executeClone"
  />
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import BaseModal from '@/components/base/BaseModal.vue'
import Table from '@/components/controls/tables/Table.vue'
import Searchbar from '@/components/controls/inputs/Searchbar.vue'
import SelectLabel from '@/components/controls/selects/SelectLabel.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonGroup from '@/components/controls/buttons/ButtonGroup.vue'
import Pagination from '@/components/controls/pagination/Pagination.vue'
import ConfirmationModal from '@/components/base/ConfirmationModal.vue'
import salaryCompositionSystemService from '@/services/salary-composition-system-service'
import {
  getTypeName,
  getNatureText,
  getValueTypeText,
  getIsDisplayedOnPayrollText,
} from '@/utils/salary-composition-helpers.js'
import { SalaryCompositionType } from '@/models/SalaryEnums'
import { FilterOperation } from '@/models/filter-operation'
import { useToast } from '@/utils/use-toast'

const props = defineProps({
  visible: Boolean,
})

const emit = defineEmits(['update:visible', 'confirm'])
const { showToast } = useToast()

// State
const systemCompositions = ref([])
const totalRecords = ref(0)
const isLoading = ref(false)
const searchQuery = ref('')
const selectedType = ref(null)
const selectedIds = ref([])
const currentPage = ref(1)
const pageSize = ref(15)

const isConfirmModalVisible = ref(false)

const typeOptions = [
  { label: 'Tất cả', value: null },
  ...Object.values(SalaryCompositionType).map((val) => ({
    label: getTypeName(val),
    value: val,
  })),
]

const headers = [
  {
    key: 'ScCode',
    label: 'Mã thành phần',
    width: '200px',
    visible: true,
    pinned: 'left',
    type: 'custom',
    minWidth: 120,
  },
  {
    key: 'ScName',
    label: 'Tên thành phần',
    width: '200px',
    visible: true,
    pinned: false,
    type: 'custom',
    minWidth: 180,
  },
  {
    key: 'ScType',
    label: 'Loại thành phần',
    width: '200px',
    visible: true,
    type: 'custom',
    minWidth: 120,
  },
  {
    key: 'ScNature',
    label: 'Tính chất',
    width: '200px',
    visible: true,
    type: 'custom',
    minWidth: 120,
  },
  {
    key: 'IsTaxDeductible',
    label: 'Giảm trừ khi tính thuế',
    width: '200px',
    visible: false,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'LimitExpression',
    label: 'Định mức',
    width: '200px',
    visible: false,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'ValueType',
    label: 'Kiểu giá trị',
    width: '200px',
    visible: true,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'FormulaExpression',
    label: 'Giá trị',
    width: '200px',
    visible: true,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'Description',
    label: 'Mô tả',
    width: '200px',
    visible: false,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'IsDisplayedOnPayroll',
    label: 'Hiển thị trên phiếu lương',
    width: '200px',
    visible: false,
    type: 'custom',
    minWidth: 150,
  },
]

// Logic lấy dữ liệu từ API
async function fetchData() {
  isLoading.value = true
  try {
    const filters = []
    if (selectedType.value !== null) {
      filters.push(`ScType:${FilterOperation.Equals}:${selectedType.value}`)
    }

    const response = await salaryCompositionSystemService.getPaging(
      searchQuery.value, // Sử dụng searchQuery trực tiếp vì đây là modal, không cần debounce phức tạp
      filters,
      currentPage.value,
      pageSize.value,
    )

    systemCompositions.value = response.Items || []
    totalRecords.value = response.TotalItems || 0
  } catch (error) {
    showToast('Không thể tải dữ liệu hệ thống', 'error')
  } finally {
    isLoading.value = false
  }
}

// Reset paging khi lọc
watch([searchQuery, selectedType], () => {
  currentPage.value = 1
  fetchData() // Gọi lại API khi search/filter thay đổi
})

// Watchers cho phân trang
watch([currentPage, pageSize], () => {
  fetchData()
})

const handleConfirm = () => {
  if (selectedIds.value.length === 0) {
    showToast('Vui lòng chọn ít nhất một thành phần lương để thêm.', 'warning')
    return
  }
  isConfirmModalVisible.value = true
}

const executeClone = async () => {
  try {
    const count = await salaryCompositionSystemService.bulkClone(selectedIds.value)
    showToast(`Đã đưa thành công ${count} thành phần lương vào danh sách sử dụng.`)
    emit('confirm', count) // Emit số lượng đã clone thành công
    emit('update:visible', false)
    selectedIds.value = []
  } catch (error) {
    showToast('Có lỗi xảy ra khi thêm thành phần lương từ hệ thống.', 'error')
  } finally {
    isConfirmModalVisible.value = false
  }
}

onMounted(() => {
  fetchData() // Lần đầu tiên mở modal, fetch data
})
</script>

<style scoped>
.add-from-system-content {
  display: flex;
  flex-direction: column;
  height: 100%;
  gap: 16px;
}

.modal_actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal_actions_left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.table-container {
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
}
</style>
