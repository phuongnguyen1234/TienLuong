<template>
  <div class="content_body_table_wrapper">
    <table class="content_body_table" :class="{ 'has-vertical-borders': showVerticalBorders }">
      <thead>
        <tr>
          <!-- Cột checkbox chọn hàng (Bulk Actions) -->
          <th v-if="selectable" class="sticky-selection-header">
            <div class="cell-content center">
              <Checkbox v-model="isAllSelected" :indeterminate="isIndeterminate" />
            </div>
          </th>
          <th
            v-for="(header, colIndex) in headers"
            :key="colIndex"
            :style="{
              width: typeof header.width === 'number' ? `${header.width}px` : header.width,
              minWidth:
                typeof header.minWidth === 'number'
                  ? `${header.minWidth}px`
                  : header.minWidth || '200px',
              ...pinnedOffset(colIndex, true),
            }"
            :class="['table-header-cell', { 'is-pinned': header.pinned }]"
            @click="onHeaderClick(header, $event)"
          >
            <div class="cell-content" :style="{ justifyContent: getFlexAlign(header.align) }">
              <!-- Icon ghim (hiển thị nếu cột được ghim) -->
              <span v-if="header.pinned" class="pinned-icon-wrapper">
                <svg
                  width="16"
                  height="16"
                  viewBox="0 0 16 16"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M5.99984 2.66675V6.66675L4.6665 9.33341V10.6667H11.3332V9.33341L9.99984 6.66675V2.66675M7.99984 10.6667V14.0001M5.33317 2.66675H10.6665"
                    stroke="currentColor"
                    stroke-width="1.5"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </svg>
              </span>

              <div class="header-label" :style="{ textAlign: header.align }" :title="header.label">
                <!-- Slot cho header nếu cần custom (ví dụ checkbox header) -->
                <slot v-if="header.headerSlot" :name="`header-${header.key}`" :header="header">
                  {{ header.label }}
                </slot>
                <template v-else>
                  {{ header.label }}
                </template>
              </div>
            </div>
          </th>
          <!-- Header cho cột action, không có text, chỉ để giữ chỗ và sticky -->
          <th v-if="$slots['row-actions']" class="sticky-action-header"></th>
        </tr>
      </thead>

      <!-- Standard Body -->
      <tbody>
        <tr
          v-for="(item, index) in data"
          :key="item[rowKey] || index"
          v-show="!rowFilterFn || rowFilterFn(item)"
          @dblclick="$emit('row-dblclick', item)"
          @click="$emit('row-click', item)"
        >
          <td v-if="selectable" class="sticky-selection-cell">
            <div class="cell-content center">
              <Checkbox
                :model-value="selection.includes(item[rowKey])"
                @update:model-value="toggleRow(item)"
              />
            </div>
          </td>
          <td
            v-for="(header, colIndex) in headers"
            :key="colIndex"
            :style="{
              width: typeof header.width === 'number' ? `${header.width}px` : header.width,
              minWidth:
                typeof header.minWidth === 'number'
                  ? `${header.minWidth}px`
                  : header.minWidth || '200px',
              ...pinnedOffset(colIndex, false),
            }"
          >
            <div class="cell-content" :style="{ justifyContent: getCellJustify(header.align) }">
              <!-- Custom slot wrapper to support ellipsis -->
              <span v-if="header.type === 'custom'" class="text-truncate">
                <slot :name="header.key" :data="item" :index="index"></slot>
              </span>

              <!-- Boolean type -->
              <div
                v-else-if="header.type === 'boolean'"
                class="boolean-cell"
                :title="item[header.key] ? 'Có' : 'Không'"
              >
                <div v-if="item[header.key]">
                  <svg
                    width="12"
                    height="9"
                    viewBox="0 0 12 9"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      d="M1 4.33333L4.33333 7.66667L11 1"
                      stroke="var(--color-primary)"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                    />
                  </svg>
                </div>
              </div>

              <!-- Default text -->
              <span v-else class="text-truncate" :title="item[header.key]">
                {{ item[header.key] }}
              </span>

              <!-- Vị trí cũ của row-actions đã được xóa khỏi đây -->
            </div>
          </td>
          <!-- Cột action cố định bên phải -->
          <td v-if="$slots['row-actions']" class="sticky-action-cell">
            <div class="row-actions-wrapper">
              <slot name="row-actions" :data="item" :index="index"></slot>
            </div>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import Checkbox from '@/components/controls/checkboxes/Checkbox.vue'

const props = defineProps({
  headers: {
    type: Array,
    required: true,
  },
  data: {
    type: Array,
    required: true,
  },
  showVerticalBorders: {
    type: Boolean,
    default: false,
  },
  rowFilterFn: {
    type: Function,
    default: null,
  },
  rowKey: {
    type: String,
    default: 'id',
  },
  selectable: {
    type: Boolean,
    default: false,
  },
  selection: {
    type: Array,
    default: () => [],
  },
})
const emit = defineEmits([
  'row-dblclick',
  'row-click',
  'update:data',
  'update:selection',
  'header-click',
])

const SELECTION_COL_WIDTH = 50

const isAllSelected = computed({
  get() {
    return props.data.length > 0 && props.selection.length === props.data.length
  },
  set(val) {
    const newSelection = val ? props.data.map((item) => item[props.rowKey]) : []
    emit('update:selection', newSelection)
  },
})

/**
 * Trạng thái lửng (Indeterminate) của checkbox header:
 * Trả về true nếu số lượng item được chọn lớn hơn 0 và nhỏ hơn tổng số item hiện có trong bảng.
 */
const isIndeterminate = computed(() => {
  return props.selection.length > 0 && props.selection.length < props.data.length
})

function toggleRow(item) {
  const key = item[props.rowKey]
  const newSelection = [...props.selection]
  const index = newSelection.indexOf(key)
  if (index > -1) {
    newSelection.splice(index, 1)
  } else {
    newSelection.push(key)
  }
  emit('update:selection', newSelection)
}

function onHeaderClick(header, event) {
  // Không mở menu cho cột checkbox hoặc nếu filterable = false
  if (header.filterable !== false) {
    emit('header-click', header, event)
  }
}

function getWidthNumber(width) {
  if (!width) return 0
  return Number(String(width).replace('px', ''))
}

function pinnedOffset(colIndex, isHeader = false) {
  const col = props.headers[colIndex]

  if (col?.pinned !== 'left') return {}

  // Nếu có cột chọn, các cột pinned left khác phải nhường chỗ (offset)
  let offset = props.selectable ? SELECTION_COL_WIDTH : 0

  for (let i = 0; i < colIndex; i++) {
    if (props.headers[i].pinned === 'left') {
      offset += getWidthNumber(props.headers[i].width)
    }
  }

  return {
    position: 'sticky',
    left: offset + 'px',
    // Header ghim phải có z-index cao hơn cả header thường và cell ghim
    zIndex: isHeader ? 'calc(var(--z-index-table-header) + 5)' : 'var(--z-index-table-pinned)',
    background: isHeader ? 'var(--bg-table-header)' : 'var(--bg-white)',
    borderRight: 'none',
  }
}

function getFlexAlign(align) {
  if (align === 'right') return 'flex-end'
  if (align === 'center') return 'center'
  return 'space-between'
}

function getCellJustify(align) {
  if (align === 'right') return 'flex-end'
  if (align === 'center') return 'center'
  return 'flex-start'
}
</script>

<style scoped>
.content_body_table_wrapper {
  background-color: var(--bg-white);
  flex: 1;
  overflow: auto;
}

.content_body_table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  table-layout: fixed;
}

.content_body_table th {
  position: sticky;
  top: 0;
  background-color: var(--bg-table-header);
  z-index: var(--z-index-table-header);
  height: 36px;
  padding: 0; /* Padding is now on the inner cell-content */
  text-align: left;
  font-weight: 600;
  font-size: 13px;
  line-height: 34px; /* Giúp nội dung căn giữa và chiếm đủ không gian trừ border */
  border-bottom: 1px solid var(--border-control-normal);
  border-top: 1px solid var(--border-control-normal);
  box-sizing: border-box;
}

.content_body_table td {
  height: 36px;
  line-height: 35px; /* Giúp nội dung căn giữa và chiếm đủ không gian trừ border-bottom */
  padding: 0; /* Padding is now on the inner cell-content */
  border-bottom: 1px solid var(--border-control-normal);
  border-right: 1px solid transparent; /* Divider tàng hình để khớp alignment với header */
  font-size: 13px;
  color: var(--color-text-main);
  background-color: var(--bg-white);
  transition: background-color 0.2s ease;
  box-sizing: border-box;
}

.cell-content {
  display: flex;
  align-items: center;
  width: 100%; /* Đảm bảo container chiếm hết chiều rộng cell để justify-content: center có tác dụng */
  padding: 0 12px;
  white-space: nowrap;
  overflow: hidden;
  height: 100%;
  min-width: 0; /* Cực kỳ quan trọng để flex item có thể thu nhỏ và kích hoạt ellipsis */
}

/* Class dùng chung để xử lý cắt chữ có dấu ba chấm */
.text-truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  display: block;
  flex: 1; /* Cho phép span chiếm hết không gian còn lại */
  min-width: 0;
}

.cell-content.center {
  justify-content: center;
  align-items: center;
}

.sticky-selection-header,
.sticky-selection-cell {
  position: sticky;
  left: 0;
  width: 50px !important;
  min-width: 50px !important;
}

.sticky-selection-header {
  background-color: var(--bg-table-header);
}

.sticky-selection-cell {
  background-color: var(--bg-white);
}

.sticky-selection-header .cell-content,
.sticky-selection-cell .cell-content {
  padding: 0;
  justify-content: center;
}

/* Đảm bảo header của cột chọn luôn nằm trên cùng (điểm giao giữa sticky row và sticky col) */
.content_body_table th.sticky-selection-header {
  z-index: calc(var(--z-index-table-header) + 10);
  left: 0;
  top: 0;
  background-color: var(--bg-table-header);
}

.table-header-cell {
  cursor: pointer;
  position: relative;
  border-right: none;
  min-width: 200px;
}

.table-header-cell::after {
  content: '';
  position: absolute;
  right: 0;
  top: 4px;
  bottom: 4px;
  width: 1px;
  background-color: var(--border-control-normal);
  z-index: 1;
}

.pinned-icon-wrapper {
  margin-right: 4px;
  display: flex;
  align-items: center;
  color: #4b5563;
  flex-shrink: 0;
}

.header-label {
  flex-grow: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.filter-indicator {
  margin-left: 4px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
}

.boolean-cell {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
}

.content_body_table.has-vertical-borders th:not(:last-child),
.content_body_table.has-vertical-borders td:not(:last-child) {
  border-right: 1px solid var(--border-control-normal);
}

.content_body_table tr:hover td {
  /* Thêm !important để ghi đè thuộc tính background inline từ hàm pinnedOffset và các class sticky */
  background-color: var(--bg-table-hover) !important;
}

.row-actions-wrapper {
  display: none;
  position: absolute;
  right: 0;
  top: 0;
  align-items: center;
  padding-right: 12px; /* Cách lề phải một chút cho đẹp */
  height: 100%;
  width: max-content; /* Bắt buộc mở rộng theo nội dung, ngăn ButtonGroup bị xuống dòng */
}

.content_body_table tr:hover .row-actions-wrapper {
  display: flex;
}

/* --- Kiểu cho cột action cố định --- */
.sticky-action-header,
.sticky-action-cell {
  position: sticky;
  right: 0;
  width: 0 !important; /* Không chiếm diện tích ngang */
  padding: 0 !important; /* Xóa padding để không bị đẩy ra */
  border: none;
  background: transparent; /* Trong suốt để đúng chất overlay */
  overflow: visible;
}

/* Ghi đè overflow:hidden cho ô action để các nút không bị cắt.
 * Selector này có độ ưu tiên cao hơn (.content_body_table td) nên sẽ được áp dụng. */
.content_body_table .sticky-action-cell {
  overflow: visible;
}

.sticky-action-cell {
  z-index: var(--z-index-table-action); /* Đảm bảo nằm trên các cột khác khi cuộn */
}

/* Khi hover vào row, cell action cũng đổi màu */
.content_body_table tr:hover .sticky-action-cell {
  background-color: transparent;
}

/* Header của cột action phải có z-index cao hơn header thường */
.sticky-action-header {
  z-index: calc(var(--z-index-table-action) + 1);
}
</style>
