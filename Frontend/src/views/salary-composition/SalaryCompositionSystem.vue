<template>
  <ThePageContainer
    title="Danh mục thành phần lương của hệ thống"
    :can-go-back="true"
    @back="$router.push('/salary-composition')"
  >
    <!-- Action bar -->
    <div class="content_body_actions">
      <div class="content_body_actions_left">
        <div class="search_box">
          <Searchbar v-model="debouncedSearchQuery" placeholder="Tìm kiếm" width="300px" />
        </div>

        <!-- Khối tác vụ hàng loạt khi có dòng được chọn -->
        <template v-if="selectedIds.length > 0">
          <span class="selection-info">
            Đã chọn
            <strong style="font-weight: 700; margin-left: 4px">{{ selectedIds.length }}</strong>
          </span>
          <Button
            variant="text"
            color="var(--border-control-hover)"
            height="32px"
            @click="selectedIds = []"
          >
            Bỏ chọn
          </Button>
          <Button variant="secondary" height="32px" @click="handleBulkAdd">
            <template #icon>
              <svg
                width="16"
                height="16"
                viewBox="242 2 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <g id="Icon Placeholder_13">
                  <path
                    id="Vector_13"
                    d="M 250,4.16667 V 15.8333 M 244.167,10 h 11.666"
                    stroke="var(--color-icon)"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </svg>
            </template>
            Đưa vào danh sách sử dụng
          </Button>
        </template>

        <!-- Lọc theo loại thành phần, chỉ hiển thị khi không có dòng nào được chọn -->
        <template v-else>
          <SelectLabel
            v-model="selectedType"
            label="Loại thành phần"
            :options="typeOptions"
            option-label="label"
            option-value="value"
          />
        </template>
      </div>

      <div class="content_body_actions_right" v-if="selectedIds.length === 0">
        <ButtonGroup>
          <ButtonIcon
            variant="outline"
            title="Bộ lọc"
            @click="isFilterDrawerOpen = !isFilterDrawerOpen"
            tooltip
          >
            <svg
              width="20"
              height="20"
              viewBox="20 0 20 20"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <g id="Icon Placeholder_2">
                <path
                  id="Vector_2"
                  d="m 23.3333,3.33333 h 13.3334 v 1.81 c -10e-5,0.44199 -0.1758,0.86585 -0.4884,1.17834 L 32.5,10 v 5.8333 L 27.5,17.5 V 10.4167 L 23.7667,6.31 C 23.4879,6.00327 23.3334,5.60366 23.3333,5.18917 Z"
                  stroke="currentColor"
                  stroke-width="1.5"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                />
              </g>
            </svg>
          </ButtonIcon>
          <ButtonIcon
            variant="outline"
            title="Thiết lập"
            @click="handleOpenTableSetting"
            padding="6px"
            tooltip
          >
            <svg
              width="20"
              height="20"
              viewBox="320 80 20 20"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <g id="Icon Placeholder_97">
                <g id="Vector_88">
                  <path
                    d="m 328.604,83.5975 c 0.355,-1.4633 2.437,-1.4633 2.792,0 0.053,0.2198 0.158,0.424 0.305,0.5958 0.147,0.1719 0.332,0.3066 0.541,0.3932 0.209,0.0865 0.436,0.1225 0.661,0.1051 0.226,-0.0175 0.444,-0.088 0.637,-0.2058 1.286,-0.7833 2.758,0.6884 1.975,1.975 -0.118,0.1931 -0.188,0.4111 -0.205,0.6365 -0.018,0.2254 0.018,0.4517 0.105,0.6605 0.086,0.2089 0.221,0.3943 0.392,0.5414 0.172,0.1471 0.376,0.2516 0.596,0.305 1.463,0.355 1.463,2.4366 0,2.7916 -0.22,0.0533 -0.424,0.1577 -0.596,0.3048 -0.172,0.1471 -0.307,0.3326 -0.393,0.5416 -0.087,0.209 -0.123,0.4354 -0.106,0.661 0.018,0.2255 0.088,0.4437 0.206,0.6368 0.784,1.2858 -0.688,2.7583 -1.975,1.975 -0.193,-0.1176 -0.411,-0.188 -0.636,-0.2054 -0.226,-0.0174 -0.452,0.0185 -0.661,0.105 -0.209,0.0865 -0.394,0.221 -0.541,0.3927 -0.147,0.1716 -0.252,0.3756 -0.305,0.5952 -0.355,1.4633 -2.437,1.4633 -2.792,0 -0.053,-0.2198 -0.158,-0.424 -0.305,-0.5958 -0.147,-0.1719 -0.332,-0.3066 -0.541,-0.3932 -0.209,0.0865 -0.436,0.1225 -0.661,0.1051 -0.226,0.0175 -0.444,0.088 -0.637,0.2058 -1.286,0.7833 -2.758,-0.6884 -1.975,-1.975 0.118,-0.1931 0.188,-0.4111 0.205,-0.6365 0.018,-0.2254 -0.018,-0.4517 -0.105,-0.6605 -0.086,-0.2089 -0.221,-0.3943 -0.392,-0.5414 -0.172,0.1471 -0.376,0.2516 -0.595,0.305 -1.464,-0.355 -1.464,-2.4366 0,-2.7916 0.219,-0.0533 0.423,-0.1577 0.595,-0.3048 0.172,-0.1471 0.307,-0.3326 0.393,-0.5416 0.087,-0.209 0.123,-0.4354 0.106,-0.661 -0.018,-0.2255 -0.088,-0.4437 -0.206,-0.6368 -0.784,-1.2858 0.688,-2.7583 1.975,-1.975 0.833,0.5067 1.913,0.0583 2.143,-0.8875 z"
                    stroke="#717680"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                  <path
                    d="m 327.5,90 c 0,0.663 0.263,1.2989 0.732,1.7678 0.469,0.4688 1.105,0.7322 1.768,0.7322 0.663,0 1.299,-0.2634 1.768,-0.7322 0.469,-0.4689 0.732,-1.1048 0.732,-1.7678 0,-0.663 -0.263,-1.2989 -0.732,-1.7678 C 331.299,87.7634 330.663,87.5 330,87.5 c -0.663,0 -1.299,0.2634 -1.768,0.7322 C 327.763,88.7011 327.5,89.337 327.5,90 Z"
                    stroke="#717680"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </g>
            </svg>
          </ButtonIcon>
        </ButtonGroup>
      </div>
    </div>

    <!-- Content table -->
    <div class="table-container">
      <Table
        :headers="visibleColumns"
        :data="systemCompositions"
        :selectable="true"
        :loading="isLoading"
        row-key="ScsId"
        v-model:selection="selectedIds"
        @header-click="handleHeaderClick"
      >
        <template #ScCode="{ data }">
          <span :title="data.ScCode">{{ data.ScCode || '-' }}</span>
        </template>
        <template #ScName="{ data }">
          <span :title="data.ScName">{{ data.ScName || '-' }}</span>
        </template>
        <template #ScType="{ data }">
          <span :title="getTypeName(data.ScType)">{{ getTypeName(data.ScType) }}</span>
        </template>
        <template #ScNature="{ data }">
          <span :title="getNatureText(data.ScNature)">{{ getNatureText(data.ScNature) }}</span>
        </template>
        <template #TaxStatus="{ data }">
          <span :title="getTaxStatusText(data.TaxStatus)">{{
            getTaxStatusText(data.TaxStatus)
          }}</span>
        </template>
        <template #IsTaxDeductible="{ data }">
          <span :title="data.IsTaxDeductible ? 'Có' : 'Không'">{{
            data.IsTaxDeductible ? 'Có' : 'Không'
          }}</span>
        </template>
        <template #LimitExpression="{ data }">
          <span :title="data.LimitExpression">{{ data.LimitExpression || '-' }}</span>
        </template>
        <template #ValueType="{ data }">
          <span :title="getValueTypeText(data.ValueType)">{{
            getValueTypeText(data.ValueType)
          }}</span>
        </template>
        <template #FormulaExpression="{ data }">
          <span v-if="data.CalculationMethod === 0" title="Tự động cộng tổng"
            >Tự động cộng tổng</span
          >
          <span
            v-else
            :title="data.FormulaExpression || '-'"
            class="formula-highlight-cell"
            v-html="highlightFormula(data.FormulaExpression) || '-'"
          ></span>
        </template>
        <template #IsDisplayedOnPayroll="{ data }">
          <span :title="getIsDisplayedOnPayrollText(data.IsDisplayedOnPayroll)">
            {{ getIsDisplayedOnPayrollText(data.IsDisplayedOnPayroll) }}
          </span>
        </template>
        <template #Description="{ data }">
          <span :title="data.Description">{{ data.Description || '-' }}</span>
        </template>

        <!-- Chỉ hiển thị nút Thêm (dấu cộng xanh lá) cho row action -->
        <template #row-actions="{ data }">
          <ButtonGroup gap="12px">
            <ButtonIcon
              variant="outline"
              width="28px"
              height="28px"
              min-width="28px"
              padding="6px"
              icon-size="16"
              color="var(--color-primary)"
              title="Đưa vào danh sách sử dụng"
              @click.stop="handleAddToInUse(data)"
              tooltip
            >
              <svg
                width="16"
                height="16"
                viewBox="242 2 16 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <g id="Icon Placeholder_13">
                  <path
                    id="Vector_13"
                    d="M 250,4.16667 V 15.8333 M 244.167,10 h 11.666"
                    stroke="currentColor"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </g>
              </svg>
            </ButtonIcon>
          </ButtonGroup>
        </template>
      </Table>
    </div>

    <Pagination
      v-model:current-page="page"
      v-model:page-size="pageSize"
      :total-records="totalItems"
    />

    <!-- Modal xác nhận đưa vào sử dụng -->
    <ConfirmationModal
      v-model:visible="isCloneConfirmVisible"
      message="Bạn có chắc chắn muốn đưa các thành phần lương đã chọn vào danh sách sử dụng không?"
      @confirm="confirmClone"
    />

    <!-- Sidebars/Modals -->
    <template #sidebar>
      <FilterDrawer
        v-show="isFilterDrawerOpen"
        :applied-filters="appliedFilters"
        :columns="filterColumns"
        @close="isFilterDrawerOpen = false"
        @apply="appliedFilters = $event"
      />
    </template>

    <TableSetting
      v-if="isTableSettingVisible"
      :visible="isTableSettingVisible"
      :columns="columnsConfig"
      :top="tableSettingPos.top"
      :right="tableSettingPos.right"
      :bottom="tableSettingPos.bottom"
      @save="handleColumnSettingsSave"
      @reset="handleColumnSettingsReset"
      @update:visible="isTableSettingVisible = $event"
    />
  </ThePageContainer>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import ThePageContainer from '@/layout/ThePageContainer.vue'
import Table from '@/components/controls/tables/Table.vue'
import Searchbar from '@/components/controls/inputs/Searchbar.vue'
import SelectLabel from '@/components/controls/selects/SelectLabel.vue'
import Button from '@/components/controls/buttons/Button.vue'
import ButtonIcon from '@/components/controls/buttons/ButtonIcon.vue'
import ButtonGroup from '@/components/controls/buttons/ButtonGroup.vue'
import Pagination from '@/components/controls/pagination/Pagination.vue'
import ConfirmationModal from '@/components/base/ConfirmationModal.vue'
import FilterDrawer from '@/components/drawer/FilterDrawer.vue'
import TableSetting from '@/components/base/TableSetting.vue'
import { highlightFormula } from '@/utils/formula-highlighter'
import { useToast } from '@/utils/use-toast'
import salaryCompositionSystemService from '@/services/salary-composition-system-service'
import {
  getTypeName,
  getNatureText,
  getTaxStatusText,
  getValueTypeText,
  getIsDisplayedOnPayrollText,
} from '@/utils/salary-composition-helpers.js'
import {
  SalaryCompositionType,
  SalaryCompositionNature,
  TaxStatus,
  ValueType,
  DisplayOnPayroll,
} from '@/models/SalaryEnums'
import { FilterOperation } from '@/models/filter-operation'

// State
const systemCompositions = ref([])
const totalItems = ref(0)
const isLoading = ref(false)
const searchQuery = ref('')
const debouncedSearchQuery = ref('')
const selectedType = ref(null)
const selectedIds = ref([])
const page = ref(1)
const pageSize = ref(15)
const isFilterDrawerOpen = ref(false)
const isTableSettingVisible = ref(false)
const tableSettingPos = ref({ top: '0px', right: '32px', bottom: '70px' })
const appliedFilters = ref([])

const isCloneConfirmVisible = ref(false)
const pendingCloneIds = ref([])

const GRID_KEY = 'pa_salary_composition_system'

const { showToast } = useToast()

// Sử dụng enum SalaryCompositionType để tạo options cho SelectLabel
const typeOptions = computed(() => [
  { label: 'Tất cả', value: null },
  ...Object.values(SalaryCompositionType).map((val) => ({
    label: getTypeName(val),
    value: val,
  })),
])

// Columns Config
const getDefaultColumns = () => [
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
    key: 'TaxStatus',
    label: 'Chịu thuế',
    width: '200px',
    visible: false,
    type: 'custom',
    minWidth: 150,
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
    visible: true,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'IsDisplayedOnPayroll',
    label: 'Hiển thị trên phiếu lương',
    width: '200px',
    visible: true,
    type: 'custom',
    minWidth: 150,
  },
  {
    key: 'empty_spacer',
    label: '',
    width: '64px',
    visible: true,
    minWidth: 64,
    filterable: false,
    hideable: false,
  },
]

const columnsConfig = ref(getDefaultColumns())
const visibleColumns = computed(() => columnsConfig.value.filter((c) => c.visible))

const filterColumns = computed(() => {
  return getDefaultColumns()
    .filter((col) => col.label && col.key !== 'empty_spacer')
    .map((col) => {
      let type = 'text'
      let options = []

      const enumKeys = [
        'ScType',
        'ScNature',
        'TaxStatus',
        'IsTaxDeductible',
        'ValueType',
        'IsDisplayedOnPayroll',
      ]

      if (enumKeys.includes(col.key)) {
        type = 'enum'
        switch (col.key) {
          case 'ScType':
            options = Object.values(SalaryCompositionType).map((v) => ({
              label: getTypeName(v),
              value: v,
            }))
            break
          case 'ScNature':
            options = Object.values(SalaryCompositionNature).map((v) => ({
              label: getNatureText(v),
              value: v,
            }))
            break
          case 'TaxStatus':
            options = Object.values(TaxStatus).map((v) => ({
              label: getTaxStatusText(v),
              value: v,
            }))
            break
          case 'IsTaxDeductible':
            options = [
              { label: 'Có', value: 1 },
              { label: 'Không', value: 0 },
            ]
            break
          case 'ValueType':
            options = Object.values(ValueType).map((v) => ({
              label: getValueTypeText(v),
              value: v,
            }))
            break
          case 'IsDisplayedOnPayroll':
            options = Object.values(DisplayOnPayroll).map((v) => ({
              label: getIsDisplayedOnPayrollText(v),
              value: v,
            }))
            break
        }
      }

      return { key: col.key, label: col.label, type, options }
    })
})

// Logic lọc dữ liệu
async function fetchData() {
  isLoading.value = true
  try {
    const filters = []
    if (selectedType.value !== null) {
      filters.push(`ScType:${FilterOperation.Equals}:${selectedType.value}`)
    }

    // Map thêm filters từ FilterDrawer nếu có
    appliedFilters.value.forEach((f) => {
      if (f.value !== null) {
        filters.push(`${f.key}:${f.operator}:${f.value}`)
      }
    })

    const response = await salaryCompositionSystemService.getPaging(
      debouncedSearchQuery.value,
      filters,
      page.value,
      pageSize.value,
    )

    systemCompositions.value = response.Items || []
    totalItems.value = response.TotalItems || 0
  } catch (error) {
    const serverMsg = error.response?.data?.UserMessage
    showToast(serverMsg || 'Không thể tải dữ liệu hệ thống', 'error')
  } finally {
    isLoading.value = false
  }
}

// Watchers cho phân trang và tìm kiếm
watch([page, pageSize, debouncedSearchQuery, selectedType, appliedFilters], () => fetchData())

let searchTimeout = null
watch(searchQuery, (newVal) => {
  if (searchTimeout) clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    debouncedSearchQuery.value = newVal
  }, 300)
})

// Actions
const handleAddToInUse = async (item) => {
  pendingCloneIds.value = [item.ScsId]
  isCloneConfirmVisible.value = true
}

const handleBulkAdd = async () => {
  pendingCloneIds.value = selectedIds.value
  isCloneConfirmVisible.value = true
}

const confirmClone = async () => {
  try {
    const count = await salaryCompositionSystemService.bulkClone(pendingCloneIds.value)
    if (count > 0) {
      showToast(`Đã đưa thành công ${count} thành phần vào danh sách sử dụng`)
      selectedIds.value = selectedIds.value.filter((id) => !pendingCloneIds.value.includes(id))
      fetchData()
    }
  } catch (error) {
    const serverMsg = error.response?.data?.UserMessage
    showToast(serverMsg || 'Có lỗi xảy ra khi đưa thành phần lương vào sử dụng', 'error')
  } finally {
    isCloneConfirmVisible.value = false
    pendingCloneIds.value = []
  }
}

const handleOpenTableSetting = (event) => {
  const rect = event.currentTarget.getBoundingClientRect()
  tableSettingPos.value = {
    top: `${rect.bottom + 8}px`,
    right: '32px',
    bottom: '70px',
  }
  isTableSettingVisible.value = true
}

const handleColumnSettingsSave = async (newSettings) => {
  try {
    await salaryCompositionSystemService.saveGridConfig(GRID_KEY, newSettings)
    columnsConfig.value = newSettings
    isTableSettingVisible.value = false
    showToast('Lưu thiết lập cột thành công')
  } catch (e) {
    showToast('Lỗi khi lưu thiết lập cột', 'error')
  }
}

const handleColumnSettingsReset = async () => {
  try {
    const defaults = getDefaultColumns()
    await salaryCompositionSystemService.saveGridConfig(GRID_KEY, defaults)
    columnsConfig.value = defaults
    isTableSettingVisible.value = false
    showToast('Đã khôi phục thiết lập mặc định')
  } catch (e) {
    showToast('Lỗi khi khôi phục thiết lập', 'error')
  }
}

/**
 * Fetch cấu hình cột từ DB và áp dụng. Nếu chưa có, lưu cấu hình mặc định vào DB.
 */
async function fetchGridConfig() {
  try {
    const response = await salaryCompositionSystemService.getGridConfig(GRID_KEY)
    if (response?.ConfigData && response.ConfigData !== '[]') {
      const savedConfig = JSON.parse(response.ConfigData)
      const defaultConfig = getDefaultColumns()
      const defaultConfigMap = new Map(defaultConfig.map((c) => [c.key, c]))

      // 1. Ưu tiên thứ tự từ Database
      let finalConfig = savedConfig
        .filter((savedCol) => defaultConfigMap.has(savedCol.key))
        .map((savedCol) => {
          const defaultCol = defaultConfigMap.get(savedCol.key)
          let col = { ...defaultCol, ...savedCol }
          if (col.hideable === false) col.visible = true
          return col
        })

      // 2. Bổ sung cột mới
      const savedKeys = new Set(savedConfig.map((c) => c.key))
      const newCols = defaultConfig.filter((c) => !savedKeys.has(c.key))
      finalConfig = [...finalConfig, ...newCols]

      // 3. Sắp xếp ghim
      finalConfig.sort((a, b) => {
        if (a.key === 'empty_spacer') return 1
        if (b.key === 'empty_spacer') return -1
        const aP = a.pinned === 'left' ? 1 : 0
        const bP = b.pinned === 'left' ? 1 : 0
        return bP - aP
      })

      columnsConfig.value = finalConfig
    } else {
      // Nếu chưa có cấu hình trong DB, thực hiện lưu cấu hình mặc định
      const defaultConfig = getDefaultColumns()
      await salaryCompositionSystemService.saveGridConfig(GRID_KEY, defaultConfig)
      columnsConfig.value = defaultConfig
    }
  } catch (e) {
    console.error('Lỗi khi tải hoặc lưu cấu hình grid hệ thống:', e)
    // Fallback về mặc định
    columnsConfig.value = getDefaultColumns()
  }
}

onMounted(() => {
  fetchGridConfig()
  fetchData()
})
</script>

<style scoped>
.content_body_actions {
  background-color: white;
  padding: 12px 16px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-radius: 8px 8px 0 0;
}

.selection-info {
  font-size: 13px;
  color: var(--color-text-main);
  margin-right: 8px;
}

.content_body_actions_left {
  display: flex;
  align-items: center;
  gap: 12px;
}

.table-container {
  flex: 1;
  background-color: white;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* Style cho highlight công thức trong table */
.formula-highlight-cell :deep(span) {
  font-family: 'Inter', sans-serif !important;
  font-size: 13px;
}
</style>
