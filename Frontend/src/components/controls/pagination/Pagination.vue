<template>
  <div class="pagination-container">
    <!--pagination left-->
    <div class="pagination_left">
      Tổng số: <strong>{{ totalRecords }}</strong>
    </div>

    <!--pagination right-->
    <div class="pagination_right">
      <div class="pagination_record_per_page_text">Số dòng/trang</div>

      <Select
        :model-value="pageSize"
        :options="pageSizeOptions"
        width="80px"
        :style="{ flexShrink: 0, boxSizing: 'border-box', minWidth: '80px', maxWidth: '80px' }"
        direction="top"
        @update:modelValue="onPageSizeChange"
      />

      <div class="pagination_record_per_page_text pagination_range-text">
        {{ startRecordIndex }} - {{ endRecordIndex }}
      </div>

      <ButtonGroup :gap="0">
        <!-- First Page -->
        <ButtonIcon variant="text" :disabled="currentPage === 1" @click="firstPage" icon-size="16">
          <svg
            width="9"
            height="10"
            viewBox="0 0 9 10"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M0.75 0.75V8.75M8.08333 0.75L4.08333 4.75L8.08333 8.75"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </ButtonIcon>

        <!-- Previous Page -->
        <ButtonIcon variant="text" :disabled="currentPage === 1" @click="prevPage" icon-size="16">
          <svg
            width="6"
            height="10"
            viewBox="0 0 6 10"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M4.75 0.75L0.75 4.75L4.75 8.75"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </ButtonIcon>

        <!-- Next Page -->
        <ButtonIcon
          variant="text"
          :disabled="currentPage === totalPages || totalPages === 0"
          @click="nextPage"
          icon-size="16"
        >
          <svg
            width="6"
            height="10"
            viewBox="0 0 6 10"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M0.75 0.75L4.75 4.75L0.75 8.75"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </ButtonIcon>

        <!-- Last Page -->
        <ButtonIcon
          variant="text"
          :disabled="currentPage === totalPages || totalPages === 0"
          @click="lastPage"
          icon-size="16"
        >
          <svg
            width="9"
            height="11"
            viewBox="0 0 9 11"
            fill="none"
            xmlns="http://www.w3.org/2000/svg"
          >
            <path
              d="M0.75 1.41667L4.75 5.41667L0.75 9.41667M8.08333 0.75V9.41667"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
          </svg>
        </ButtonIcon>
      </ButtonGroup>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import ButtonIcon from '@/components/controls/buttons/ButtonIcon.vue'
import Select from '@/components/controls/selects/Select.vue'
import ButtonGroup from '../buttons/ButtonGroup.vue'

const props = defineProps({
  totalRecords: { type: Number, required: true },
  pageSize: { type: Number, default: 10 },
  currentPage: { type: Number, default: 1 },
})

const emit = defineEmits(['update:currentPage', 'update:pageSize'])

const pageSizeOptions = [15, 25, 50, 100]

const totalPages = computed(() => Math.ceil(props.totalRecords / props.pageSize))

const startRecordIndex = computed(() => {
  if (props.totalRecords === 0) return 0
  return (props.currentPage - 1) * props.pageSize + 1
})

const endRecordIndex = computed(() => {
  return Math.min(props.currentPage * props.pageSize, props.totalRecords)
})

function onPageSizeChange(newValue) {
  emit('update:pageSize', Number(newValue))
}

function firstPage() {
  if (props.currentPage > 1) {
    emit('update:currentPage', 1)
  }
}

function prevPage() {
  if (props.currentPage > 1) {
    emit('update:currentPage', props.currentPage - 1)
  }
}

function nextPage() {
  if (props.currentPage < totalPages.value) {
    emit('update:currentPage', props.currentPage + 1)
  }
}

function lastPage() {
  if (props.currentPage < totalPages.value) {
    emit('update:currentPage', totalPages.value)
  }
}
</script>

<style scoped>
.pagination-container {
  height: 48px;
  display: flex;
  justify-content: space-between;
  background-color: white;
  border-radius: 0 0 5px 5px;
  align-items: center;
  border-top: 1px solid #e0e6ec;
  padding: 0 16px;
}

.pagination_left {
  font-weight: 500;
  font-size: 13px;
}

.pagination_right {
  display: flex;
  align-items: center;
  gap: 16px;
  font-size: 13px;
}

.pagination_record_per_page_text {
  font-weight: 400;
}

.pagination_range-text {
  width: 48px;
  text-align: center;
  font-weight: 600;
}

.pagination_right :deep(.m-button-group) {
  flex-wrap: nowrap;
}
</style>
